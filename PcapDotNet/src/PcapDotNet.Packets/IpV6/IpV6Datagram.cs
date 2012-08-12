using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ip;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2460.
    /// Represents an IPv6 datagram.
    /// <pre>
    /// +-----+---------+---------------+-------+-------------+-----------+
    /// | Bit | 0-3     | 4-11          | 12-15 | 16-23       | 24-31     |
    /// +-----+---------+---------------+-------+-------------+-----------+
    /// | 0   | Version | Traffic Class | Flow Label                      |
    /// +-----+---------+---------------+-------+-------------+-----------+
    /// | 32  | Payload Length                  | Next Header | Hop Limit |
    /// +-----+---------------------------------+-------------+-----------+
    /// | 64  | Source Address                                            |
    /// |     |                                                           |
    /// |     |                                                           |
    /// |     |                                                           |
    /// +-----+-----------------------------------------------------------+
    /// | 192 | Destination Address                                       |
    /// |     |                                                           |
    /// |     |                                                           |
    /// |     |                                                           |
    /// +-----+-----------------------------------------------------------+
    /// | 320 | Extension Headers (optional)                              |
    /// | ... |                                                           |
    /// +-----+-----------------------------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6Datagram : Datagram
    {
        /// <summary>
        /// The number of bytes the header takes in bytes (not including extension headers).
        /// </summary>
        public const int HeaderLength = 40;

        private static class Offset
        {
            public const int Version = 0;
            public const int TrafficClass = 0;
            public const int FlowLabel = 1;
            public const int PayloadLength = 4;
            public const int NextHeader = 6;
            public const int HopLimit = 7;
            public const int SourceAddress  = 8;
            public const int DestinationAddress = 24;
        }

        private static class Mask
        {
            public const byte Version = 0xF0;
            public const ushort TrafficClass = 0x0FF0;
            public static readonly UInt24 FlowLabel = (UInt24)0x0FFFFF;
        }

        private static class Shift
        {
            public const int Version = 4;
            public const int TrafficClass = 4;
        }
        /// <summary>
        /// The version (6).
        /// </summary>
        public const int DefaultVersion = 0x6;

        /// <summary>
        /// Internet Protocol version number.
        /// </summary>
        public byte Version
        {
            get { return (byte)((this[Offset.Version] & Mask.Version) >> Shift.Version); }
        }

        /// <summary>
        /// Available for use by originating nodes and/or forwarding routers to identify and distinguish between different classes or priorities of 
        /// IPv6 packets.
        /// </summary>
        public byte TrafficClass
        {
            get { return (byte)((ReadUShort(Offset.TrafficClass, Endianity.Big) & Mask.TrafficClass) >> Shift.TrafficClass); }
        }

        /// <summary>
        /// May be used by a source to label sequences of packets for which it requests special handling by the IPv6 routers, 
        /// such as non-default quality of service or "real-time" service.
        /// Hosts or routers that do not support the functions of the Flow Label field are required to set the field to zero when originating a packet,
        /// pass the field on unchanged when forwarding a packet, and ignore the field when receiving a packet.
        /// </summary>
        public int FlowLabel
        {
            get { return ReadUInt24(Offset.FlowLabel, Endianity.Big) & Mask.FlowLabel; }
        }

        /// <summary>
        /// Length of the IPv6 payload, i.e., the rest of the packet following this IPv6 header, in octets.
        /// Note that any extension headers present are considered part of the payload, i.e., included in the length count.
        /// </summary>
        public ushort PayloadLength
        {
            get { return ReadUShort(Offset.PayloadLength, Endianity.Big); }
        }

        public ushort RealPayloadLength
        {
            get { return (ushort)Math.Min(PayloadLength, Length); }
        }

        /// <summary>
        /// Identifies the type of header immediately following the IPv6 header.
        /// Uses the same values as the IPv4 Protocol field.
        /// </summary>
        public IpV4Protocol NextHeader
        {
            get { return (IpV4Protocol)this[Offset.NextHeader]; }
        }

        /// <summary>
        /// Decremented by 1 by each node that forwards the packet.
        /// The packet is discarded if Hop Limit is decremented to zero.
        /// </summary>
        public byte HopLimit
        {
            get { return this[Offset.HopLimit]; }
        }

        /// <summary>
        /// Address of the originator of the packet.
        /// </summary>
        public IpV6Address Source
        {
            get { return ReadIpV6Address(Offset.SourceAddress, Endianity.Big); }
        }

        /// <summary>
        /// Address of the intended recipient of the packet (possibly not the ultimate recipient, if a Routing header is present).
        /// </summary>
        public IpV6Address CurrentDestination
        {
            get { return ReadIpV6Address(Offset.DestinationAddress, Endianity.Big); }
        }

        public ReadOnlyCollection<IpV6ExtensionHeader> ExtensionHeaders
        {
            get
            {
                ParseExtensionHeaders();
                return _extensionHeaders;
            }
        }

        private void ParseExtensionHeaders()
        {
            if (_extensionHeaders != null)
                return;

            List<IpV6ExtensionHeader> extensionHeaders = new List<IpV6ExtensionHeader>();
            if (HeaderLength > RealPayloadLength)
            {
                _isValid = false;
                _extensionHeaders = extensionHeaders.AsReadOnly();
                return;
            }
            int extendedHeaderLength = HeaderLength;
            IpV4Protocol nextHeader = NextHeader;
            while (extendedHeaderLength + 8 <= RealPayloadLength && IsExtensionHeader(nextHeader))
            {
                int numBytesRead;
                IpV6ExtensionHeader extensionHeader = CreateExtensionHeader(nextHeader, Subsegment(extendedHeaderLength, Length - extendedHeaderLength), out numBytesRead);
                if (extensionHeader == null)
                    break;
                nextHeader = extensionHeader.NextHeader;
                extendedHeaderLength += numBytesRead;
            }
            _isValid = !IsExtensionHeader(nextHeader) && (HeaderLength + _extensionHeadersLength == PayloadLength);
            _extensionHeaders = extensionHeaders.AsReadOnly();
            _extensionHeadersLength = extendedHeaderLength - HeaderLength;
        }

        private IpV6ExtensionHeader CreateExtensionHeader(IpV4Protocol nextHeader, DataSegment data, out int numBytesRead)
        {
            switch (nextHeader)
            {
                case IpV4Protocol.IpV6HopByHopOption:           // 0
                    return IpV6ExtensionHeaderHopByHopOptions.Parse(data, out numBytesRead);
                    /*
                case IpV4Protocol.IpV6Route:                    // 43
                    return IpV6ExtensionHeaderRouting.Parse(data);

                case IpV4Protocol.FragmentHeaderForIpV6:        // 44
                    return IpV6ExtensionHeaderFragment.Parse(data);

                case IpV4Protocol.EncapsulatingSecurityPayload: // 50
                    return IpV6ExtensionHeaderEncapsulatingSecurityPayload.Parse(data);

                case IpV4Protocol.AuthenticationHeader:         // 51
                    return IpV6ExtensionHeaderAuthentication.Parse(data);

                case IpV4Protocol.IpV6Opts:                     // 60
                    return IpV6ExtensionHeaderDestinationOptions.Parse(data);

                case IpV4Protocol.MobilityHeader:               // 135
                    return IpV6MobilityExtensionHeader.Parse(data);
                    */
                default:
                    throw new InvalidOperationException(string.Format("Invalid next header value {0}", nextHeader));
            }
        }

        private static bool IsExtensionHeader(IpV4Protocol nextHeader)
        {
            switch (nextHeader)
            {
                case IpV4Protocol.IpV6HopByHopOption:           // 0
                case IpV4Protocol.IpV6Route:                    // 43
                case IpV4Protocol.FragmentHeaderForIpV6:        // 44
                case IpV4Protocol.EncapsulatingSecurityPayload: // 50
                case IpV4Protocol.AuthenticationHeader:         // 51
                case IpV4Protocol.IpV6Opts:                     // 60
                case IpV4Protocol.MobilityHeader:               // 135
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return null;
            // TODO: Implement.
//            return new IpV6Layer
//            {
//                Version = Version,
//                TrafficClass = TrafficClass,
//                FlowLabel = FlowLabel,
//                PayloadLength = PayloadLength,
//                NextHeader = NextHeader,
//                HopLimit = HopLimit,
//                Source = Source,
//                CurrentDestination = CurrentDestination,
//                ExtensionHeaders = ExtensionHeaders,
//            };
        }


        internal IpV6Datagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal static void WriteHeader(byte[] buffer, int offset,
                                         byte version, byte trafficClass, int flowLabel, ushort payloadLength, IpV4Protocol nextHeader, byte hopLimit,
                                         IpV6Address source, IpV6Address currentDestination)
        {
            buffer.Write(offset + Offset.Version, (uint)(((((version << Shift.Version) << 8) | trafficClass) << 16) | flowLabel), Endianity.Big);
            buffer.Write(offset + Offset.PayloadLength, payloadLength, Endianity.Big);
            buffer.Write(offset + Offset.NextHeader, (byte)nextHeader);
            buffer.Write(offset + Offset.HopLimit, hopLimit);
            buffer.Write(offset + Offset.SourceAddress, source, Endianity.Big);
            buffer.Write(offset + Offset.DestinationAddress, currentDestination, Endianity.Big);
        }

        protected override bool CalculateIsValid()
        {
            ParseExtensionHeaders();
            return _isValid;
        }

        private ReadOnlyCollection<IpV6ExtensionHeader> _extensionHeaders;
        private int _extensionHeadersLength;
        private bool _isValid;
    }

    /// <summary>
    /// RFC 2460.
    /// </summary>
    public abstract class IpV6ExtensionHeader
    {
        protected IpV6ExtensionHeader(IpV4Protocol nextHeader)
        {
            NextHeader = nextHeader;
        }

        public IpV4Protocol NextHeader { get; private set; }
    }

    /// <summary>
    /// RFC 2460.
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | Options                               |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </summary>
    public class IpV6ExtensionHeaderHopByHopOptions : IpV6ExtensionHeader
    {
        private static class Offset
        {
            public const int NextHeader = 0;
            public const int HeaderExtensionLength = 1;
            public const int Options = 2;
        }

        public const int MinimumLength = 8;

        private IpV6ExtensionHeaderHopByHopOptions(IpV4Protocol nextHeader, IpV6Options options)
            : base(nextHeader)
        {
            Options = options;
        }

        public IpV6Options Options { get; private set; }

        internal static IpV6ExtensionHeaderHopByHopOptions Parse(DataSegment data, out int numBytesRead)
        {
            numBytesRead = 0;
            if (data.Length < MinimumLength)
                return null;
            IpV4Protocol nextHeader = (IpV4Protocol)data[Offset.NextHeader];
            int length = (data[Offset.HeaderExtensionLength] + 1) * 8;
            if (data.Length < length)
                return null;

            IpV6Options options = new IpV6Options(data.Subsegment(Offset.Options, length - Offset.Options));
            numBytesRead = length;
            return new IpV6ExtensionHeaderHopByHopOptions(nextHeader, options);
        }
    }

    public enum IpV6OptionType :  byte
    {
        /// <summary>
        /// RFC 2460.
        /// </summary>
        Pad1 = 0x00,

        /// <summary>
        /// RFC 2460.
        /// </summary>
        PadN = 0x01,

        /// <summary>
        /// RFC 2675.
        /// </summary>
        JumboPayload = 0xC2,

        /// <summary>
        /// RFC 2473.
        /// </summary>
        TunnelEncapsulationLimit = 0x04,

        /// <summary>
        /// RFC 2711.
        /// </summary>
        RouterAlert = 0x05,

        /// <summary>
        /// RFC 4782, Errata 2034.
        /// </summary>
        QuickStart = 0x26,

        /// <summary>
        /// RFC 5570.
        /// </summary>
        Calipso = 0x07,

        /// <summary>
        /// RFC 6621.
        /// </summary>
        SmfDpd = 0x08,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        HomeAddress = 0xC9,

        /// <summary>
        /// Charles Lynn.
        /// </summary>
        EndpointIdentification = 0x8A,

        /// <summary>
        /// RFC 6553.
        /// </summary>
        RplOption = 0x63,

        /// <summary>
        /// RFC irtf-rrg-ilnp-noncev6-06.
        /// </summary>
        IlnpNonce = 0x8B,
    }

    /// <summary>
    /// RFC 2460.
    /// <pre>
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Option Type | Opt Data Len (optional) |
    /// +-----+-------------+-------------------------+
    /// | 16  | Option Data (optional)                |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6Option : Option
    {
        /// <summary>
        /// The type of the IP option.
        /// </summary>
        public IpV6OptionType OptionType { get; private set; }

        internal abstract IpV6Option CreateInstance(DataSegment data);

        protected IpV6Option(IpV6OptionType type)
        {
            OptionType = type;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)OptionType;
        }

        public override int Length
        {
            get { return sizeof(byte); }
        }
    }

    /// <summary>
    /// RFC 2460.
    /// <pre>
    /// +-----+-------------+
    /// | Bit | 0-7         |
    /// +-----+-------------+
    /// | 0   | Option Type |
    /// +-----+-------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6OptionSimple : IpV6Option
    {
        protected IpV6OptionSimple(IpV6OptionType type) : base(type)
        {
        }

        public override sealed int Length
        {
            get { return base.Length; }
        }

        internal override sealed void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
        }
    }

    /// <summary>
    /// RFC 2460.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Option Data                |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6OptionComplex : IpV6Option
    {
        protected IpV6OptionComplex(IpV6OptionType type) 
            : base(type)
        {
        }

        public override sealed int Length
        {
            get { return base.Length + sizeof (byte) + DataLength; }
        }

        internal abstract int DataLength { get; }

        internal override sealed void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer[offset++] = (byte)DataLength;
            WriteData(buffer, ref offset);
        }

        internal abstract void WriteData(byte[] buffer, ref int offset);
    }

    /// <summary>
    /// RFC 2460.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Option Data                |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    public class IpV6OptionUnknown : IpV6OptionComplex
    {
        public IpV6OptionUnknown(IpV6OptionType type, DataSegment data)
            : base(type)
        {
            Data = data;
        }

        public DataSegment Data { get; private set; }

        internal Option Read(byte[] buffer, ref int offset, int length)
        {
            throw new NotImplementedException();
        }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            throw new InvalidOperationException("IpV6OptionUnknown shouldn't be registered.");
 	    }

        internal override int DataLength
        {
            get { return Data.Length; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Data);
        }
    }

    /// <summary>
    /// +-----+-----+
    /// | Bit | 0-7 |
    /// +-----+-----+
    /// | 0   | 0   |
    /// +-----+-----+
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.Pad1)]
    public class IpV6OptionPad1 : IpV6OptionSimple
    {
        public const int OptionLength = sizeof(byte);

        public IpV6OptionPad1()
            : base(IpV6OptionType.Pad1)
        {
        }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            return new IpV6OptionPad1();
        }
    }

    /// <summary>
    /// RFC 2460.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | N            |
    /// +-----+-------------+--------------+
    /// | 16  | 0                          |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.PadN)]
    public class IpV6OptionPadN : IpV6OptionComplex
    {
        public IpV6OptionPadN(int paddingDataLength) : base(IpV6OptionType.PadN)
        {
            PaddingDataLength = paddingDataLength;
        }

        public int PaddingDataLength { get; private set; }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            return new IpV6OptionPadN(data.Length);
        }

        internal override int DataLength
        {
            get { return PaddingDataLength; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            offset += PaddingDataLength;
        }
    }

    /// <summary>
    /// RFC 2675.
    /// <pre>
    /// +-----+-------------+--------+
    /// | Bit | 0-7         | 8-15   |
    /// +-----+-------------+--------+
    /// | 0   | Option Type | 4      |
    /// +-----+-------------+--------+
    /// | 16  | Jumbo Payload Length |
    /// |     |                      |
    /// +-----+----------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.JumboPayload)]
    public class IpV6OptionJumboPayload : IpV6OptionComplex
    {
        public const int OptionDataLength = sizeof(uint);

        public IpV6OptionJumboPayload(uint jumboPayloadLength) : base(IpV6OptionType.JumboPayload)
        {
            JumboPayloadLength = jumboPayloadLength;
        }

        /// <summary>
        /// Length of the IPv6 packet in octets, excluding the IPv6 header but including the Hop-by-Hop Options header and any other extension headers present.
        /// Must be greater than 65,535.
        /// </summary>
        public uint JumboPayloadLength { get; private set; }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6OptionJumboPayload(data.ReadUInt(0, Endianity.Big));
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, JumboPayloadLength, Endianity.Big);
        }
    }

    /// <summary>
    /// RFC 2473.
    /// <pre>
    /// +-----+----------------------------+--------+
    /// | Bit | 0-7                        | 8-15   |
    /// +-----+----------------------------+--------+
    /// | 0   | Option Type                | 1      |
    /// +-----+----------------------------+--------+
    /// | 16  | Tunnel Encapsulation Limit |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.TunnelEncapsulationLimit)]
    public class IpV6OptionTunnelEncapsulationLimit : IpV6OptionComplex
    {
        public const int OptionDataLength = sizeof(byte);

        public IpV6OptionTunnelEncapsulationLimit(byte tunnelEncapsulationLimit)
            : base(IpV6OptionType.JumboPayload)
        {
            TunnelEncapsulationLimit = tunnelEncapsulationLimit;
        }

        /// <summary>
        /// How many further levels of encapsulation are permitted for the packet.
        /// </summary>
        public byte TunnelEncapsulationLimit { get; private set; }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6OptionTunnelEncapsulationLimit(data[0]);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, TunnelEncapsulationLimit);
        }
    }

    /// <summary>
    /// RFC 2711.
    /// </summary>
    public enum IpV6RouterAlertType : ushort
    {
        /// <summary>
        /// RFC 2710.
        /// Datagram contains a Multicast Listener Discovery message.
        /// </summary>
        MulticastListenerDiscovery = 0x0000,

        /// <summary>
        /// RFC 2711.
        /// Datagram contains RSVP message.
        /// </summary>
        Rsvp = 0x0001,

        /// <summary>
        /// RFC 2711.
        /// Datagram contains an Active Networks message.
        /// </summary>
        ActiveNetwork = 0x0002,

        /// <summary>
        /// RFC 3175.
        /// 1 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth1 = 0x0004,

        /// <summary>
        /// RFC 3175.
        /// 2 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth2 = 0x0005,

        /// <summary>
        /// RFC 3175.
        /// 3 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth3 = 0x0006,

        /// <summary>
        /// RFC 3175.
        /// 4 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth4 = 0x0007,

        /// <summary>
        /// RFC 3175.
        /// 5 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth5 = 0x0008,

        /// <summary>
        /// RFC 3175.
        /// 6 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth6 = 0x0009,

        /// <summary>
        /// RFC 3175.
        /// 7 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth7 = 0x000A,

        /// <summary>
        /// RFC 3175.
        /// 8 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth8 = 0x000B,

        /// <summary>
        /// RFC 3175.
        /// 9 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth9 = 0x000C,

        /// <summary>
        /// RFC 3175.
        /// 10 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth10 = 0x000D,

        /// <summary>
        /// RFC 3175.
        /// 11 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth11 = 0x000E,

        /// <summary>
        /// RFC 3175.
        /// 12 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth12 = 0x000F,

        /// <summary>
        /// RFC 3175.
        /// 13 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth13 = 0x0010,


        /// <summary>
        /// RFC 3175.
        /// 14 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth14 = 0x0011,

        /// <summary>
        /// RFC 3175.
        /// 15 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth15 = 0x0012,

        /// <summary>
        /// RFC 3175.
        /// 16 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth16 = 0x0013,

        /// <summary>
        /// RFC 3175.
        /// 17 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth17 = 0x0014,

        /// <summary>
        /// RFC 3175.
        /// 18 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth18 = 0x0015,

        /// <summary>
        /// RFC 3175.
        /// 19 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth19 = 0x0016,

        /// <summary>
        /// RFC 3175.
        /// 20 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth20 = 0x0017,

        /// <summary>
        /// RFC 3175.
        /// 21 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth21 = 0x0018,

        /// <summary>
        /// RFC 3175.
        /// 22 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth22 = 0x0019,

        /// <summary>
        /// RFC 3175.
        /// 23 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth23 = 0x001A,

        /// <summary>
        /// RFC 3175.
        /// 24 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth24 = 0x001B,

        /// <summary>
        /// RFC 3175.
        /// 25 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth25 = 0x001C,

        /// <summary>
        /// RFC 3175.
        /// 26 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth26 = 0x001D,

        /// <summary>
        /// RFC 3175.
        /// 27 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth27 = 0x001E,

        /// <summary>
        /// RFC 3175.
        /// 28 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth28 = 0x001F,

        /// <summary>
        /// RFC 3175.
        /// 29 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth29 = 0x0020,

        /// <summary>
        /// RFC 3175.
        /// 30 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth30 = 0x0021,

        /// <summary>
        /// RFC 3175.
        /// 31 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth31 = 0x0022,

        /// <summary>
        /// RFC 3175.
        /// 32 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth32 = 0x0023,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 0.
        /// </summary>
        QoSNslpAggregationLevel0 = 0x0024,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 1.
        /// </summary>
        QoSNslpAggregationLevel1 = 0x0025,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 2.
        /// </summary>
        QoSNslpAggregationLevel2 = 0x0026,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 3.
        /// </summary>
        QoSNslpAggregationLevel3 = 0x0027,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 4.
        /// </summary>
        QoSNslpAggregationLevel4 = 0x0028,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 5.
        /// </summary>
        QoSNslpAggregationLevel5 = 0x0029,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 6.
        /// </summary>
        QoSNslpAggregationLevel6 = 0x002A,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 7.
        /// </summary>
        QoSNslpAggregationLevel7 = 0x002B,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 8.
        /// </summary>
        QoSNslpAggregationLevel8 = 0x002C,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 9.
        /// </summary>
        QoSNslpAggregationLevel9 = 0x002D,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 10.
        /// </summary>
        QoSNslpAggregationLevel10 = 0x002E,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 11.
        /// </summary>
        QoSNslpAggregationLevel11 = 0x002F,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 12.
        /// </summary>
        QoSNslpAggregationLevel12 = 0x0030,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 13.
        /// </summary>
        QoSNslpAggregationLevel13 = 0x0031,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 14.
        /// </summary>
        QoSNslpAggregationLevel14 = 0x0032,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 15.
        /// </summary>
        QoSNslpAggregationLevel15 = 0x0033,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 16.
        /// </summary>
        QoSNslpAggregationLevel16 = 0x0034,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 17.
        /// </summary>
        QoSNslpAggregationLevel17 = 0x0035,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 18.
        /// </summary>
        QoSNslpAggregationLevel18 = 0x0036,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 19.
        /// </summary>
        QoSNslpAggregationLevel19 = 0x0037,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 20.
        /// </summary>
        QoSNslpAggregationLevel20 = 0x0038,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 21.
        /// </summary>
        QoSNslpAggregationLevel21 = 0x0039,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 22.
        /// </summary>
        QoSNslpAggregationLevel22 = 0x003A,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 23.
        /// </summary>
        QoSNslpAggregationLevel23 = 0x003B,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 24.
        /// </summary>
        QoSNslpAggregationLevel24 = 0x003C,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 25.
        /// </summary>
        QoSNslpAggregationLevel25 = 0x003D,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 26.
        /// </summary>
        QoSNslpAggregationLevel26 = 0x003E,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 27.
        /// </summary>
        QoSNslpAggregationLevel27 = 0x003F,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 28.
        /// </summary>
        QoSNslpAggregationLevel28 = 0x0040,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 29.
        /// </summary>
        QoSNslpAggregationLevel29 = 0x0041,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 30.
        /// </summary>
        QoSNslpAggregationLevel30 = 0x0042,

        /// <summary>
        /// RFC 5974.
        /// QoS NSLP Aggregation Level 31.
        /// </summary>
        QoSNslpAggregationLevel31 = 0x0043,

        /// <summary>
        /// RFC 5973.
        /// NSIS NATFW NSLP.
        /// </summary>
        NsisNatfwNslp = 0x0044,
    }

    /// <summary>
    /// RFC 2711.
    /// <pre>
    /// +-----+-------------+--------+
    /// | Bit | 0-7         | 8-15   |
    /// +-----+-------------+--------+
    /// | 0   | Option Type | 2      |
    /// +-----+-------------+--------+
    /// | 16  | Router Alert Type    |
    /// +-----+----------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.RouterAlert)]
    public class IpV6OptionRouterAlert : IpV6OptionComplex
    {
        public const int OptionDataLength = sizeof(ushort);

        public IpV6OptionRouterAlert(IpV6RouterAlertType routerAlertType)
            : base(IpV6OptionType.RouterAlert)
        {
            RouterAlertType = routerAlertType;
        }

        /// <summary>
        /// Type of router alert.
        /// </summary>
        public IpV6RouterAlertType RouterAlertType { get; private set; }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6OptionRouterAlert((IpV6RouterAlertType)data.ReadUShort(0, Endianity.Big));
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, (ushort)RouterAlertType, Endianity.Big);
        }
    }

    /// <summary>
    /// RFC 4782.
    /// <pre>
    /// +-----+-------------+------+----------+--------------+-------+-------+
    /// | Bit | 0-7         | 8-15 | 16-19    | 20-23        | 24-29 | 30-31 |
    /// +-----+-------------+------+----------+--------------+-------+-------+
    /// | 0   | Option Type | 6    | Function | Rate Request | QS TTL        |
    /// +-----+-------------+------+----------+--------------+-------+-------+
    /// | 32  | QS Nonce                                             | R     |
    /// +-----+------------------------------------------------------+-------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.QuickStart)]
    public class IpV6OptionQuickStart : IpV6OptionComplex, IIpOptionQuickStart
    {
        public const int OptionDataLength = IpOptionQuickStartCommon.DataLength;

        public IpV6OptionQuickStart(IpV4OptionQuickStartFunction function, byte rate, byte ttl, uint nonce)
            : base(IpV6OptionType.QuickStart)
        {
            IpOptionQuickStartCommon.AssertValidParameters(function, rate, ttl, nonce);

            Function = function;
            Rate = rate;
            Ttl = ttl;
            Nonce = nonce;
        }

        public IpV4OptionQuickStartFunction Function { get; private set; }
        public byte Rate { get; private set; }

        public int RateKbps
        {
            get { return IpOptionQuickStartCommon.CalcRateKbps(Rate); }
        }

        public byte Ttl { get; private set; }
        public uint Nonce { get; private set; }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            IpV4OptionQuickStartFunction function;
            byte rate;
            byte ttl;
            uint nonce;
            IpOptionQuickStartCommon.ReadData(data, out function, out rate, out ttl, out nonce);

            return new IpV6OptionQuickStart(function, rate, ttl, nonce);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            IpOptionQuickStartCommon.WriteData(buffer, ref offset, Function, Rate, Ttl, Nonce);
        }
    }
//        Calipso = 0x07,
//        SmfDpd = 0x08,
//        HomeAddress = 0xC9,
//        EndpointIdentification = 0x8A,
//        RplOption = 0x63,
//        IlnpNonce = 0x8B,

    public class IpV6Options : Options<IpV6Option>
    {
        public IpV6Options(DataSegment data) : this(Read(data))
        {
        }

        private IpV6Options(Tuple<IList<IpV6Option>, bool> optionsAndIsValid) : base(optionsAndIsValid.Item1, optionsAndIsValid.Item2, null)
        {
        }

        public static Tuple<IList<IpV6Option>, bool> Read(DataSegment data) 
        {
            int offset = 0;
            List<IpV6Option> options = new List<IpV6Option>();
            bool isValid = true;
            while (offset < data.Length)
            {
                IpV6OptionType optionType = (IpV6OptionType)data[offset++];
                if (optionType == IpV6OptionType.Pad1)
                {
                    options.Add(new IpV6OptionPad1());
                    continue;
                }

                if (offset >= data.Length)
                {
                    isValid = false;
                    break;
                }

                byte optionDataLength = data[offset++];
                if (offset + optionDataLength > data.Length)
                {
                    isValid = false;
                    break;
                }

                IpV6Option option = CreateOption(optionType, data.Subsegment(ref offset, optionDataLength));
                if (option == null)
                {
                    isValid = false;
                    break;
                }

                options.Add(option);
            }

            return new Tuple<IList<IpV6Option>, bool>(options, isValid);
        }

        private static IpV6Option CreateOption(IpV6OptionType optionType, DataSegment data)
        {
            IpV6Option prototype;
            if (!_prototypes.TryGetValue(optionType, out prototype))
                return new IpV6OptionUnknown(optionType, data);
            return prototype.CreateInstance(data);
        }

        private static readonly Dictionary<IpV6OptionType, IpV6Option> _prototypes = InitializePrototypes();

        private static Dictionary<IpV6OptionType, IpV6Option> InitializePrototypes()
        {
            var prototypes =
                from type in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(IpV6Option).IsAssignableFrom(type) &&
                      GetRegistrationAttribute(type) != null
                select new
                       {
                           GetRegistrationAttribute(type).OptionType,
                           Option = (IpV6Option)Activator.CreateInstance(type)
                       };

            return prototypes.ToDictionary(option => option.OptionType, option => option.Option);
        }

        private static IpV6OptionTypeRegistrationAttribute GetRegistrationAttribute(Type type)
        {
            var registraionAttributes = type.GetCustomAttributes<IpV6OptionTypeRegistrationAttribute>(false);
            if (!registraionAttributes.Any())
                return null;

            return registraionAttributes.First();
        }
    }

    internal sealed class IpV6OptionTypeRegistrationAttribute : Attribute
    {
        public IpV6OptionTypeRegistrationAttribute(IpV6OptionType optionType)
        {
            OptionType = optionType;
        }

        public IpV6OptionType OptionType { get; private set; }
    }
    /*
    /// <summary>
    /// RFC 2460.
    /// +-----+-------------+-------------------------+--------------+---------------+
    /// | Bit | 0-7         | 8-15                    | 16-23        | 24-31         |
    /// +-----+-------------+-------------------------+--------------+---------------+
    /// | 0   | Next Header | Header Extension Length | Routing Type | Segments Left |
    /// +-----+-------------+-------------------------+--------------+---------------+
    /// | 32  | type-specific data                                                   |
    /// | ... |                                                                      |
    /// +-----+----------------------------------------------------------------------+
    /// </summary>
    public class IpV6ExtensionHeaderRouting : IpV6ExtensionHeader
    {
    }

    /// <summary>
    /// RFC 2460.
    /// +-----+-------------+----------+-----------------+----------+----+
    /// | Bit | 0-7         | 8-15     | 16-28           | 29-30    | 31 |
    /// +-----+-------------+----------+-----------------+----------+----+
    /// | 0   | Next Header | Reserved | Fragment Offset | Reserved | M  |
    /// +-----+-------------+----------+-----------------+----------+----+
    /// | 32  | Identification                                           |
    /// +-----+----------------------------------------------------------+
    /// </summary>
    public class IpV6ExtensionHeaderFragment : IpV6ExtensionHeader
    {
    }

    /// <summary>
    /// RFC 2460.
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | Options                               |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </summary>
    public class IpV6ExtensionHeaderDestinationOptions : IpV6ExtensionHeader
    {
    }

    /// <summary>
    /// RFC 2402.
    /// </summary>
    public class IpV6ExtensionHeaderAuthentication : IpV6ExtensionHeader
    {
    }

    /// <summary>
    /// RFC 2406.
    /// </summary>
    public class IpV6ExtensionHeaderEncapsulatingSecurityPayload : IpV6ExtensionHeader
    {
    }
    */
}
