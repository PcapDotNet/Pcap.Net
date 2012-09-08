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

    /// <summary>
    /// RFC 5570.
    /// CALIPSO Domain of Interpretation (DOI)
    /// </summary>
    public enum IpV6CalipsoDomainOfInterpretation : uint
    {
        /// <summary>
        /// RFC 5570.
        /// Must not appear in any IPv6 packet on any network.
        /// </summary>
        Null = 0x0000,
    }

    /// <summary>
    /// RFC 1662.
    /// </summary>
    public static class PppFrameCheckSequenceCalculator
    {
        private const ushort InitialValue = 0xffff;

        /// <summary>
        /// Calculate FCS16.
        /// </summary>
        public static ushort CalculateFcs16(IEnumerable<byte> values)
        {
            return CalculateFcs16(InitialValue, values);
        }

        public static ushort CalculateFcs16(ushort fcs, IEnumerable<byte> values)
        {
            foreach (byte value in values)
            {
                ushort tableValue = _fcsTable[(fcs ^ value) & 0xff];
                fcs = (ushort)((fcs >> 8) ^ tableValue);
            }
            return fcs;
        }

        private static readonly ushort[] _fcsTable =
            new ushort[]
                {
                    0x0000, 0x1189, 0x2312, 0x329b, 0x4624, 0x57ad, 0x6536, 0x74bf,
                    0x8c48, 0x9dc1, 0xaf5a, 0xbed3, 0xca6c, 0xdbe5, 0xe97e, 0xf8f7,
                    0x1081, 0x0108, 0x3393, 0x221a, 0x56a5, 0x472c, 0x75b7, 0x643e,
                    0x9cc9, 0x8d40, 0xbfdb, 0xae52, 0xdaed, 0xcb64, 0xf9ff, 0xe876,
                    0x2102, 0x308b, 0x0210, 0x1399, 0x6726, 0x76af, 0x4434, 0x55bd,
                    0xad4a, 0xbcc3, 0x8e58, 0x9fd1, 0xeb6e, 0xfae7, 0xc87c, 0xd9f5,
                    0x3183, 0x200a, 0x1291, 0x0318, 0x77a7, 0x662e, 0x54b5, 0x453c,
                    0xbdcb, 0xac42, 0x9ed9, 0x8f50, 0xfbef, 0xea66, 0xd8fd, 0xc974,
                    0x4204, 0x538d, 0x6116, 0x709f, 0x0420, 0x15a9, 0x2732, 0x36bb,
                    0xce4c, 0xdfc5, 0xed5e, 0xfcd7, 0x8868, 0x99e1, 0xab7a, 0xbaf3,
                    0x5285, 0x430c, 0x7197, 0x601e, 0x14a1, 0x0528, 0x37b3, 0x263a,
                    0xdecd, 0xcf44, 0xfddf, 0xec56, 0x98e9, 0x8960, 0xbbfb, 0xaa72,
                    0x6306, 0x728f, 0x4014, 0x519d, 0x2522, 0x34ab, 0x0630, 0x17b9,
                    0xef4e, 0xfec7, 0xcc5c, 0xddd5, 0xa96a, 0xb8e3, 0x8a78, 0x9bf1,
                    0x7387, 0x620e, 0x5095, 0x411c, 0x35a3, 0x242a, 0x16b1, 0x0738,
                    0xffcf, 0xee46, 0xdcdd, 0xcd54, 0xb9eb, 0xa862, 0x9af9, 0x8b70,
                    0x8408, 0x9581, 0xa71a, 0xb693, 0xc22c, 0xd3a5, 0xe13e, 0xf0b7,
                    0x0840, 0x19c9, 0x2b52, 0x3adb, 0x4e64, 0x5fed, 0x6d76, 0x7cff,
                    0x9489, 0x8500, 0xb79b, 0xa612, 0xd2ad, 0xc324, 0xf1bf, 0xe036,
                    0x18c1, 0x0948, 0x3bd3, 0x2a5a, 0x5ee5, 0x4f6c, 0x7df7, 0x6c7e,
                    0xa50a, 0xb483, 0x8618, 0x9791, 0xe32e, 0xf2a7, 0xc03c, 0xd1b5,
                    0x2942, 0x38cb, 0x0a50, 0x1bd9, 0x6f66, 0x7eef, 0x4c74, 0x5dfd,
                    0xb58b, 0xa402, 0x9699, 0x8710, 0xf3af, 0xe226, 0xd0bd, 0xc134,
                    0x39c3, 0x284a, 0x1ad1, 0x0b58, 0x7fe7, 0x6e6e, 0x5cf5, 0x4d7c,
                    0xc60c, 0xd785, 0xe51e, 0xf497, 0x8028, 0x91a1, 0xa33a, 0xb2b3,
                    0x4a44, 0x5bcd, 0x6956, 0x78df, 0x0c60, 0x1de9, 0x2f72, 0x3efb,
                    0xd68d, 0xc704, 0xf59f, 0xe416, 0x90a9, 0x8120, 0xb3bb, 0xa232,
                    0x5ac5, 0x4b4c, 0x79d7, 0x685e, 0x1ce1, 0x0d68, 0x3ff3, 0x2e7a,
                    0xe70e, 0xf687, 0xc41c, 0xd595, 0xa12a, 0xb0a3, 0x8238, 0x93b1,
                    0x6b46, 0x7acf, 0x4854, 0x59dd, 0x2d62, 0x3ceb, 0x0e70, 0x1ff9,
                    0xf78f, 0xe606, 0xd49d, 0xc514, 0xb1ab, 0xa022, 0x92b9, 0x8330,
                    0x7bc7, 0x6a4e, 0x58d5, 0x495c, 0x3de3, 0x2c6a, 0x1ef1, 0x0f78
                };
    }

    /// <summary>
    /// RFC 5570.
    /// <pre>
    /// +-----+-------------+-----------------+
    /// | Bit | 0-7         | 8-15            |
    /// +-----+-------------+-----------------+
    /// | 0   | Option Type | Opt Data Len    |
    /// +-----+-------------+-----------------+
    /// | 16  | Domain of Interpretation      |
    /// |     |                               |
    /// +-----+-------------+-----------------+
    /// | 48  | Cmpt Lengt  | Sens Level      |
    /// +-----+-------------+-----------------+
    /// | 64  | Checksum (CRC-16)             |
    /// +-----+-------------------------------+
    /// | 80  | Compartment Bitmap (Optional) |
    /// | ... |                               |
    /// +-----+-------------------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.Calipso)]
    public class IpV6OptionCalipso : IpV6OptionComplex
    {
        private static class Offset
        {
            public const int DomainOfInterpretation = 0;
            public const int CompartmentLength = DomainOfInterpretation + sizeof(uint);
            public const int SensitivityLevel = CompartmentLength + sizeof(byte);
            public const int Checksum = SensitivityLevel + sizeof(byte);
            public const int CompartmentBitmap = Checksum + sizeof(ushort);
        }

        public const int OptionDataMinimumLength = Offset.CompartmentBitmap;

        public IpV6OptionCalipso(IpV6CalipsoDomainOfInterpretation domainOfInterpretation, byte sensitivityLevel, ushort checksum, DataSegment compartmentBitmap)
            : base(IpV6OptionType.Calipso)
        {
            if (compartmentBitmap.Length % sizeof(int) != 0)
                throw new ArgumentException(string.Format("Compartment Bitmap length must divide by {0}.", sizeof(int)), "compartmentBitmap");
            if (compartmentBitmap.Length / sizeof(int) > byte.MaxValue)
            {
                throw new ArgumentOutOfRangeException(string.Format("Compartment Bitmap length must not be bigger than {0}.", byte.MaxValue * sizeof(int)),
                                                      "compartmentBitmap");
            }

            DomainOfInterpretation = domainOfInterpretation;
            SensitivityLevel = sensitivityLevel;
            Checksum = checksum;
            CompartmentBitmap = compartmentBitmap;
        }

        /// <summary>
        /// The DOI identifies the rules under which this datagram must be handled and protected.
        /// </summary>
        public IpV6CalipsoDomainOfInterpretation DomainOfInterpretation { get; private set; }

        /// <summary>
        /// Specifies the size of the Compartment Bitmap field in 32-bit words.
        /// The minimum value is zero, which is used only when the information in this packet is not in any compartment.
        /// (In that situation, the CALIPSO Sensitivity Label has no need for a Compartment Bitmap).
        /// </summary>
        public byte CompartmentLength { get { return (byte)(CompartmentLengthInBytes / sizeof(int)); } }

        /// <summary>
        /// Specifies the size of the Compartment Bitmap field in bytes.
        /// The minimum value is zero, which is used only when the information in this packet is not in any compartment.
        /// (In that situation, the CALIPSO Sensitivity Label has no need for a Compartment Bitmap).
        /// </summary>
        public int CompartmentLengthInBytes
        {
            get { return CompartmentBitmap.Length; }
        }

        /// <summary>
        /// Contains an opaque octet whose value indicates the relative sensitivity of the data contained in this datagram in the context of the indicated DOI.
        /// The values of this field must be ordered, with 00000000 being the lowest Sensitivity Level and 11111111 being the highest Sensitivity Level.
        /// However, in a typical deployment, not all 256 Sensitivity Levels will be in use.
        /// So the set of valid Sensitivity Level values depends upon the CALIPSO DOI in use.
        /// This sensitivity ordering rule is necessary so that Intermediate Systems (e.g., routers or MLS guards) will be able to apply MAC policy
        /// with minimal per-packet computation and minimal configuration.
        /// </summary>
        public byte SensitivityLevel { get; private set; }

        /// <summary>
        /// Contains the a CRC-16 checksum as defined in RFC 1662. 
        /// The checksum is calculated over the entire CALIPSO option in this packet, including option header, zeroed-out checksum field, option contents,
        /// and any required padding zero bits.
        /// The checksum must always be computed on transmission and must always be verified on reception.
        /// This checksum only provides protection against accidental corruption of the CALIPSO option in cases where neither the underlying medium 
        /// nor other mechanisms, such as the IP Authentication Header (AH), are available to protect the integrity of this option.
        /// Note that the checksum field is always required, even when other integrity protection mechanisms (e.g., AH) are used.
        /// </summary>
        public ushort Checksum { get; private set; }

        public bool IsChecksumCorrect
        {
            get
            {
                if (_isChecksumCorrect == null)
                {
                    byte[] domainOfInterpretationBytes = new byte[sizeof(uint)];
                    domainOfInterpretationBytes.Write(0, (uint)DomainOfInterpretation, Endianity.Big);
                    ushort expectedValue =
                        PppFrameCheckSequenceCalculator.CalculateFcs16(
                            new byte[0].Concat((byte)OptionType, (byte)DataLength).Concat(domainOfInterpretationBytes)
                                .Concat<byte>(CompartmentLength, SensitivityLevel, 0, 0).Concat(CompartmentBitmap));
                    _isChecksumCorrect = (Checksum == expectedValue);
                }

                return _isChecksumCorrect.Value;
            }
        }

        /// <summary>
        /// Each bit represents one compartment within the DOI.
        /// Each "1" bit within an octet in the Compartment Bitmap field represents a separate compartment under whose rules the data in this packet
        /// must be protected.
        /// Hence, each "0" bit indicates that the compartment corresponding with that bit is not applicable to the data in this packet.
        /// The assignment of identity to individual bits within a Compartment Bitmap for a given DOI is left to the owner of that DOI.
        /// This specification represents a Releasability on the wire as if it were an inverted Compartment.
        /// So the Compartment Bitmap holds the sum of both logical Releasabilities and also logical Compartments for a given DOI value.
        /// The encoding of the Releasabilities in this field is described elsewhere in this document.
        /// The Releasability encoding is designed to permit the Compartment Bitmap evaluation to occur without the evaluator necessarily knowing
        /// the human semantic associated with each bit in the Compartment Bitmap.
        /// In turn, this facilitates the implementation and configuration of Mandatory Access Controls based on the Compartment Bitmap 
        /// within IPv6 routers or guard devices.
        /// </summary>
        public DataSegment CompartmentBitmap { get; private set; }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + CompartmentLengthInBytes; }
        }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            IpV6CalipsoDomainOfInterpretation domainOfInterpretation = (IpV6CalipsoDomainOfInterpretation)data.ReadUInt(Offset.DomainOfInterpretation,
                                                                                                                        Endianity.Big);
            byte compartmentLength = data[Offset.CompartmentLength];
            int compartmentLengthInBytes = compartmentLength * sizeof(int);
            if (OptionDataMinimumLength + compartmentLengthInBytes > data.Length)
                return null;
            byte sensitivityLevel = data[Offset.SensitivityLevel];
            ushort checksum = data.ReadUShort(Offset.Checksum, Endianity.Big);
            DataSegment compartmentBitmap = data.Subsegment(Offset.CompartmentBitmap, compartmentLengthInBytes);

            return new IpV6OptionCalipso(domainOfInterpretation, sensitivityLevel, checksum, compartmentBitmap);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, (uint)DomainOfInterpretation, Endianity.Big);
            buffer.Write(ref offset, CompartmentLength);
            buffer.Write(ref offset, SensitivityLevel);
            buffer.Write(ref offset, Checksum, Endianity.Big);
            buffer.Write(ref offset, CompartmentBitmap);
        }

        private bool? _isChecksumCorrect;
    }

    /// <summary>
    /// The type of the Tagger ID for <see cref="IpV6OptionSmfDpdSequenceBased"/>
    /// </summary>
    public enum IpV6TaggerIdType : byte
    {
        /// <summary>
        /// No TaggerId field is present.
        /// </summary>
        Null = 0,

        /// <summary>
        /// A TaggerId of non-specific context is present.
        /// </summary>
        Default = 1,

        /// <summary>
        /// A TaggerId representing an IPv4 address is present.
        /// </summary>
        IpV4 = 2,

        /// <summary>
        /// A TaggerId representing an IPv6 address is present.
        /// </summary>
        IpV6 = 3,
    }

    /// <summary>
    /// RFC 6621.
    /// Simplified Multicast Forwarding Duplicate Packet Detection.
    /// Sequence-based approach.
    /// <pre>
    /// +-----+---+-------+--------+
    /// | Bit | 0 | 1-3   | 4-7    |
    /// +-----+---+-------+--------+
    /// | 0   | Option Type        |
    /// +-----+--------------------+
    /// | 8   | Opt Data Len       |
    /// +-----+---+-------+--------+
    /// | 16  | 0 | TidTy | TidLen |
    /// +-----+---+-------+--------+
    /// | 24  | TaggerId           |
    /// | ... |                    |
    /// +-----+--------------------+
    /// |     | Identifier         |
    /// | ... |                    |
    /// +-----+--------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6OptionSmfDpdSequenceBased : IpV6OptionSmfDpd
    {
        private static class Offset
        {
            public const int TaggerIdType = 0;
            public const int TaggerIdLength = TaggerIdType;
            public const int TaggerId = TaggerIdLength + sizeof(byte);
        }

        private static class Mask
        {
            public const byte TaggerIdType = 0x70;
            public const byte TaggerIdLength = 0x0F;
        }

        private static class Shift
        {
            public const int TaggerIdType = 4;
        }

        /// <summary>
        /// The length of the Tagger Id.
        /// </summary>
        public abstract int TaggerIdLength { get; }

        /// <summary>
        /// DPD packet Identifier.
        /// When the TaggerId field is present, the Identifier can be considered a unique packet identifier 
        /// in the context of the TaggerId:srcAddr:dstAddr tuple.
        /// When the TaggerId field is not present, then it is assumed that the source applied the SMF_DPD option 
        /// and the Identifier can be considered unique in the context of the IPv6 packet header srcAddr:dstAddr tuple.
        /// </summary>
        public DataSegment Identifier { get; private set; }

        /// <summary>
        /// Identifying DPD marking type.
        /// 0 == sequence-based approach with optional TaggerId and a tuple-based sequence number. See <see cref="IpV6OptionSmfDpdSequenceBased"/>.
        /// 1 == indicates a hash assist value (HAV) field follows to aid in avoiding hash-based DPD collisions.
        /// </summary>
        public override bool HashIndicator
        {
            get { return false; }
        }

        /// <summary>
        /// Indicating the presence and type of the optional TaggerId field.
        /// </summary>
        public abstract IpV6TaggerIdType TaggerIdType { get; }

        protected IpV6OptionSmfDpdSequenceBased(DataSegment identifier)
        {
            Identifier = identifier;
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + TaggerIdLength + Identifier.Length; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            byte taggerIdInfo = (byte)(((byte)TaggerIdType << Shift.TaggerIdType) & Mask.TaggerIdType);
            if (TaggerIdType != IpV6TaggerIdType.Null)
                taggerIdInfo |= (byte)((TaggerIdLength - 1) & Mask.TaggerIdLength);
            buffer.Write(ref offset, taggerIdInfo);
            WriteTaggerId(buffer, ref offset);
            buffer.Write(ref offset, Identifier);
        }

        internal abstract void WriteTaggerId(byte[] buffer, ref int offset);

        internal static IpV6OptionSmfDpdSequenceBased CreateSpecificInstance(DataSegment data)
        {
            IpV6TaggerIdType taggerIdType = (IpV6TaggerIdType)((data[Offset.TaggerIdType] & Mask.TaggerIdType) >> Shift.TaggerIdType);
            int taggerIdLength = (taggerIdType == IpV6TaggerIdType.Null ? 0 : (data[Offset.TaggerIdLength] & Mask.TaggerIdLength) + 1);
            if (data.Length < Offset.TaggerId + taggerIdLength)
                return null;
            DataSegment identifier = data.Subsegment(Offset.TaggerId + taggerIdLength, data.Length - Offset.TaggerId - taggerIdLength);
            switch (taggerIdType)
            {
                case IpV6TaggerIdType.Null:
                    return new IpV6OptionSmfDpdNull(identifier);
                    
                case IpV6TaggerIdType.Default:
                    return new IpV6OptionSmfDpdDefault(data.Subsegment(Offset.TaggerId, taggerIdLength), identifier);

                case IpV6TaggerIdType.IpV4:
                    if (taggerIdLength != IpV4Address.SizeOf)
                        return null;
                    IpV4Address ipV4Address = data.ReadIpV4Address(0, Endianity.Big);
                    return new IpV6OptionSmfDpdIpV4(ipV4Address, identifier);

                case IpV6TaggerIdType.IpV6:
                    if (taggerIdLength != IpV6Address.SizeOf)
                        return null;
                    IpV6Address ipV6Address = data.ReadIpV6Address(0, Endianity.Big);
                    return new IpV6OptionSmfDpdIpV6(ipV6Address, identifier);

                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// RFC 6621.
    /// Simplified Multicast Forwarding Duplicate Packet Detection.
    /// Sequence-based approach.
    /// IPv4 tagger ID.
    /// <pre>
    /// +-----+---+-------+--------+
    /// | Bit | 0 | 1-3   | 4-7    |
    /// +-----+---+-------+--------+
    /// | 0   | Option Type        |
    /// +-----+--------------------+
    /// | 8   | Opt Data Len       |
    /// +-----+---+-------+--------+
    /// | 16  | 0 | TidTy | TidLen |
    /// +-----+---+-------+--------+
    /// | 24  | TaggerId           |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// +-----+--------------------+
    /// | 56  | Identifier         |
    /// | ... |                    |
    /// +-----+--------------------+
    /// </pre>
    /// </summary>
    public class IpV6OptionSmfDpdIpV4 : IpV6OptionSmfDpdSequenceBased
    {
        public IpV6OptionSmfDpdIpV4(IpV4Address taggerId, DataSegment identifier)
            :base(identifier)
        {
            TaggerId = taggerId;
        }

        public IpV4Address TaggerId { get; private set; }

        /// <summary>
        /// The length of the Tagger Id.
        /// </summary>
        public override int TaggerIdLength
        {
            get { return IpV4Address.SizeOf; }
        }

        /// <summary>
        /// Indicating the presence and type of the optional TaggerId field.
        /// </summary>
        public override IpV6TaggerIdType TaggerIdType
        {
            get { return IpV6TaggerIdType.IpV4; }
        }

        internal override void WriteTaggerId(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, TaggerId, Endianity.Big);
        }
    }

    /// <summary>
    /// RFC 6621.
    /// Simplified Multicast Forwarding Duplicate Packet Detection.
    /// Sequence-based approach.
    /// IPv6 tagger ID.
    /// <pre>
    /// +-----+---+-------+--------+
    /// | Bit | 0 | 1-3   | 4-7    |
    /// +-----+---+-------+--------+
    /// | 0   | Option Type        |
    /// +-----+--------------------+
    /// | 8   | Opt Data Len       |
    /// +-----+---+-------+--------+
    /// | 16  | 0 | TidTy | TidLen |
    /// +-----+---+-------+--------+
    /// | 24  | TaggerId           |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// |     |                    |
    /// +-----+--------------------+
    /// | 152 | Identifier         |
    /// | ... |                    |
    /// +-----+--------------------+
    /// </pre>
    /// </summary>
    public class IpV6OptionSmfDpdIpV6 : IpV6OptionSmfDpdSequenceBased
    {
        public IpV6OptionSmfDpdIpV6(IpV6Address taggerId, DataSegment identifier)
            : base(identifier)
        {
            TaggerId = taggerId;
        }

        public IpV6Address TaggerId { get; private set; }

        /// <summary>
        /// The length of the Tagger Id.
        /// </summary>
        public override int TaggerIdLength
        {
            get { return IpV6Address.SizeOf; }
        }

        /// <summary>
        /// Indicating the presence and type of the optional TaggerId field.
        /// </summary>
        public override IpV6TaggerIdType TaggerIdType
        {
            get { return IpV6TaggerIdType.IpV6; }
        }

        internal override void WriteTaggerId(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, TaggerId, Endianity.Big);
        }
    }

    /// <summary>
    /// RFC 6621.
    /// Simplified Multicast Forwarding Duplicate Packet Detection.
    /// Sequence-based approach.
    /// Default Tagger ID.
    /// <pre>
    /// +-----+---+-------+--------+
    /// | Bit | 0 | 1-3   | 4-7    |
    /// +-----+---+-------+--------+
    /// | 0   | Option Type        |
    /// +-----+--------------------+
    /// | 8   | Opt Data Len       |
    /// +-----+---+-------+--------+
    /// | 16  | 0 | TidTy | TidLen |
    /// +-----+---+-------+--------+
    /// | 24  | TaggerId           |
    /// | ... |                    |
    /// +-----+--------------------+
    /// |     | Identifier         |
    /// | ... |                    |
    /// +-----+--------------------+
    /// </pre>
    /// </summary>
    public class IpV6OptionSmfDpdDefault : IpV6OptionSmfDpdSequenceBased
    {
        public IpV6OptionSmfDpdDefault(DataSegment taggerId, DataSegment identifier)
            : base(identifier)
        {
            TaggerId = taggerId;
        }

        public DataSegment TaggerId { get; private set; }

        /// <summary>
        /// The length of the Tagger Id.
        /// </summary>
        public override int TaggerIdLength
        {
            get { return TaggerId.Length; }
        }

        /// <summary>
        /// Indicating the presence and type of the optional TaggerId field.
        /// </summary>
        public override IpV6TaggerIdType TaggerIdType
        {
            get { return IpV6TaggerIdType.Default; }
        }

        internal override void WriteTaggerId(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, TaggerId);
        }
    }

    /// <summary>
    /// RFC 6621.
    /// Simplified Multicast Forwarding Duplicate Packet Detection.
    /// Sequence-based approach.
    /// Null Tagger ID.
    /// <pre>
    /// +-----+---+-------+--------+
    /// | Bit | 0 | 1-3   | 4-7    |
    /// +-----+---+-------+--------+
    /// | 0   | Option Type        |
    /// +-----+--------------------+
    /// | 8   | Opt Data Len       |
    /// +-----+---+-------+--------+
    /// | 16  | 0 | TidTy | TidLen |
    /// +-----+---+-------+--------+
    /// | 24  | Identifier         |
    /// | ... |                    |
    /// +-----+--------------------+
    /// </pre>
    /// </summary>
    public class IpV6OptionSmfDpdNull : IpV6OptionSmfDpdSequenceBased
    {
        public IpV6OptionSmfDpdNull(DataSegment identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// The length of the Tagger Id.
        /// </summary>
        public override int TaggerIdLength
        {
            get { return 0; }
        }

        /// <summary>
        /// Indicating the presence and type of the optional TaggerId field.
        /// </summary>
        public override IpV6TaggerIdType TaggerIdType
        {
            get { return IpV6TaggerIdType.Null; }
        }

        internal override void WriteTaggerId(byte[] buffer, ref int offset)
        {
        }
    }

    /// <summary>
    /// RFC 6621.
    /// Simplified Multicast Forwarding Duplicate Packet Detection.
    /// Hash assist value.
    /// <pre>
    /// +-----+---+----------+
    /// | Bit | 0 | 1-7      |
    /// +-----+---+----------+
    /// | 0   | Option Type  |
    /// +-----+--------------+
    /// | 8   | Opt Data Len |
    /// +-----+---+----------+
    /// | 16  | 1 | Hash     |
    /// +-----+---+ Assist   |
    /// | ... | Value (HAV)  |
    /// +-----+--------------+
    /// </pre>
    /// </summary>
    public class IpV6OptionSmfDpdSequenceHashAssistValue : IpV6OptionSmfDpd
    {
        private static class Offset
        {
            public const int HashAssistValue = 0;
        }

        public IpV6OptionSmfDpdSequenceHashAssistValue(DataSegment data)
        {
            byte[] hashAssistValueBuffer = new byte[data.Length - Offset.HashAssistValue];
            data.Buffer.BlockCopy(data.StartOffset + Offset.HashAssistValue, hashAssistValueBuffer, 0, hashAssistValueBuffer.Length);
            hashAssistValueBuffer[0] &= 0x7F;
            HashAssistValue = new DataSegment(hashAssistValueBuffer);
        }

        /// <summary>
        /// Hash assist value (HAV) used to facilitate H-DPD operation.
        /// </summary>
        public DataSegment HashAssistValue { get; private set; }

        internal override int DataLength
        {
            get { return HashAssistValue.Length; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, (byte)(HashAssistValue[0] | 0x80));
            buffer.Write(ref offset, HashAssistValue.Subsegment(1, HashAssistValue.Length - 1));
        }

        public override bool HashIndicator
        {
            get { return true; }
        }
    }

    /// <summary>
    /// RFC 6621.
    /// Simplified Multicast Forwarding Duplicate Packet Detection.
    /// <pre>
    /// +-----+---+------------------+
    /// | Bit | 0 | 1-7              |
    /// +-----+---+------------------+
    /// | 0   | Option Type          |
    /// +-----+----------------------+
    /// | 8   | Opt Data Len         |
    /// +-----+---+------------------+
    /// | 16  | H | DPD Identifier   |
    /// +-----+---+ Option Fields    |
    /// | ... | or Hash Assist Value |
    /// +-----+----------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.SmfDpd)]
    public abstract class IpV6OptionSmfDpd : IpV6OptionComplex
    {
        private static class Offset
        {
            public const int HashIndicator = 0;
        }

        private static class Mask
        {
            public const int HashIndicator = 0x80;
        }

        public const int OptionDataMinimumLength = Offset.HashIndicator + sizeof(byte);

        protected IpV6OptionSmfDpd()
            : base(IpV6OptionType.SmfDpd)
        {
        }

        /// <summary>
        /// Identifying DPD marking type.
        /// 0 == sequence-based approach with optional TaggerId and a tuple-based sequence number. See <see cref="IpV6OptionSmfDpdSequenceBased"/>.
        /// 1 == indicates a hash assist value (HAV) field follows to aid in avoiding hash-based DPD collisions.
        /// </summary>
        public abstract bool HashIndicator { get; }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            bool hashIndicator = data.ReadBool(Offset.HashIndicator, Mask.HashIndicator);
            if (hashIndicator)
                return new IpV6OptionSmfDpdSequenceHashAssistValue(data);
            return IpV6OptionSmfDpdSequenceBased.CreateSpecificInstance(data);
        }
    }

    /// <summary>
    /// RFC 6275.
    /// Home Address Option.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Home Address               |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.HomeAddress)]
    public class IpV6OptionHomeAddress : IpV6OptionComplex
    {
        public const int OptionDataLength = IpV6Address.SizeOf;

        public IpV6OptionHomeAddress(IpV6Address homeAddress)
            : base(IpV6OptionType.HomeAddress)
        {
            HomeAddress = homeAddress;
        }

        /// <summary>
        /// The home address of the mobile node sending the packet.  
        /// This address must be a unicast routable address.
        /// </summary>
        public IpV6Address HomeAddress { get; private set; }

        internal IpV6OptionHomeAddress() 
            : this(IpV6Address.Zero)
        {
        }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;
            IpV6Address homeAddress = data.ReadIpV6Address(0, Endianity.Big);
            return new IpV6OptionHomeAddress(homeAddress);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }


        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, HomeAddress, Endianity.Big);
        }
    }

    /// <summary>
    /// Charles Lynn..
    /// http://ana-3.lcs.mit.edu/~jnc/nimrod/eidoption.txt
    /// Endpoint Identifier Option.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Src Len     | Dst Len      |
    /// +-----+-------------+--------------+
    /// | 32  | Source EID                 |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// |     | Destination EID            |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.EndpointIdentification)]
    public class IpV6OptionEndpointIdentification : IpV6OptionComplex
    {
        private static class Offset
        {
            public const int SourceEndpointIdentifierLength = 0;
            public const int DestinationEndpointIdentifierLength = SourceEndpointIdentifierLength + sizeof(byte);
            public const int SourceEndpointIdentifier = DestinationEndpointIdentifierLength + sizeof(byte);
        }

        public const int OptionDataMinimumLength = Offset.SourceEndpointIdentifier;

        public IpV6OptionEndpointIdentification(DataSegment sourceEndpointIdentifier, DataSegment destinationEndpointIdentifier)
            : base(IpV6OptionType.EndpointIdentification)
        {
            SourceEndpointIdentifier = sourceEndpointIdentifier;
            DestinationEndpointIdentifier = destinationEndpointIdentifier;
        }

        public DataSegment SourceEndpointIdentifier { get; private set; }

        public DataSegment DestinationEndpointIdentifier { get; private set; }

        internal IpV6OptionEndpointIdentification()
            : this(DataSegment.Empty, DataSegment.Empty)
        {
        }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;
            
            int sourceEndpointIdentifierLength = data[Offset.SourceEndpointIdentifierLength];
            int destinationEndpointIdentifierLength = data[Offset.DestinationEndpointIdentifierLength];
            if (data.Length != OptionDataMinimumLength + sourceEndpointIdentifierLength + destinationEndpointIdentifierLength)
                return null;

            DataSegment sourceEndpointIdentifier = data.Subsegment(Offset.SourceEndpointIdentifier, sourceEndpointIdentifierLength);
            int destinationEndpointIdentifierOffset = Offset.SourceEndpointIdentifier + sourceEndpointIdentifierLength;
            DataSegment destinationEndpointIdentifier = data.Subsegment(destinationEndpointIdentifierOffset, destinationEndpointIdentifierLength);
            return new IpV6OptionEndpointIdentification(sourceEndpointIdentifier, destinationEndpointIdentifier);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + SourceEndpointIdentifier.Length + DestinationEndpointIdentifier.Length; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, (byte)SourceEndpointIdentifier.Length);
            buffer.Write(ref offset, (byte)DestinationEndpointIdentifier.Length);
            buffer.Write(ref offset, SourceEndpointIdentifier);
            buffer.Write(ref offset, DestinationEndpointIdentifier);
        }
    }
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
