using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PcapDotNet.Base;
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
