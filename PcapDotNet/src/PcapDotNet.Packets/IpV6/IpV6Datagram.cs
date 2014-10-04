using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    public sealed class IpV6Datagram : IpDatagram
    {
        /// <summary>
        /// The number of bytes the header takes in bytes (not including extension headers).
        /// </summary>
        public const int HeaderLength = 40;

        /// <summary>
        /// Maximum flow label value.
        /// </summary>
        public const int MaxFlowLabel = 0xFFFFF;

        private static class Offset
        {
            public const int Version = 0;
            public const int TrafficClass = Version;
            public const int FlowLabel = TrafficClass + 1;
            public const int PayloadLength = FlowLabel + 3;
            public const int NextHeader = PayloadLength + sizeof(ushort);
            public const int HopLimit = NextHeader + sizeof(byte);
            public const int SourceAddress = HopLimit + sizeof(byte);
            public const int DestinationAddress = SourceAddress + IpV6Address.SizeOf;
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

        /// <summary>
        /// The actual payload length 
        /// </summary>
        public ushort RealPayloadLength
        {
            get
            {
                if (Length < HeaderLength)
                    return 0;
                return (ushort)Math.Min(PayloadLength, Length - HeaderLength);
            }
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

        /// <summary>
        /// The IPv6 extension headers.
        /// </summary>
        public IpV6ExtensionHeaders ExtensionHeaders
        {
            get
            {
                ParseExtensionHeaders();
                return _extensionHeaders;
            }
        }

        /// <summary>
        /// The total length - header and payload according to the IP header.
        /// </summary>
        public override int TotalLength
        {
            get { return HeaderLength + PayloadLength; }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IpV6Layer
            {
                TrafficClass = TrafficClass,
                FlowLabel = FlowLabel,
                NextHeader = NextHeader,
                HopLimit = HopLimit,
                Source = Source,
                CurrentDestination = CurrentDestination,
                ExtensionHeaders = ExtensionHeaders,
            };
        }

        /// <summary>
        /// Valid if all extension headers are valid and payload is valid.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            ParseExtensionHeaders();
            if (!_isValidExtensionHeaders)
                return false;

            return IsPayloadValid;
        }

        /// <summary>
        /// Calculates the Transport checksum field value.
        /// </summary>
        /// <returns>The calculated checksum value.</returns>
        protected override ushort CalculateTransportChecksum()
        {
            return CalculateTransportChecksum(Buffer, StartOffset, HeaderLength + ExtensionHeaders.BytesLength, (uint)Transport.Length, Transport.ChecksumOffset,
                                              Transport.IsChecksumOptional, CurrentDestination);
        }

        internal IpV6Datagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal override IpV4Protocol PayloadProtocol
        {
            get
            {
                IpV4Protocol? extensionHeadersNextHeader = ExtensionHeaders.NextHeader;
                if (extensionHeadersNextHeader != null)
                    return extensionHeadersNextHeader.Value;
                return NextHeader;
            }
        }

        internal static int GetTotalLength(Datagram payload)
        {
            if (payload.Length <= HeaderLength)
                return payload.Length;

            return Math.Min(payload.Length, HeaderLength + payload.ReadUShort(Offset.PayloadLength, Endianity.Big));
        }

        internal override DataSegment GetPayload()
        {
            if (Length < HeaderLength)
                return null;
            return Subsegment(HeaderLength + ExtensionHeaders.BytesLength, Length - HeaderLength - ExtensionHeaders.BytesLength);
        }

        internal static void WriteHeader(byte[] buffer, int offset,
                                         byte trafficClass, int flowLabel, ushort payloadLength, IpV4Protocol? nextHeader, IpV4Protocol? nextLayerProtocol,
                                         byte hopLimit, IpV6Address source, IpV6Address currentDestination, IpV6ExtensionHeaders extensionHeaders)
        {
            buffer.Write(offset + Offset.Version, (uint)((((DefaultVersion << 8) | trafficClass) << 20) | flowLabel), Endianity.Big);
            buffer.Write(offset + Offset.PayloadLength, payloadLength, Endianity.Big);
            IpV4Protocol actualNextHeader;
            if (nextHeader.HasValue)
                actualNextHeader = nextHeader.Value;
            else if (extensionHeaders.Any())
                actualNextHeader = extensionHeaders.FirstHeader.Value;
            else if (nextLayerProtocol.HasValue)
                actualNextHeader = nextLayerProtocol.Value;
            else
                throw new InvalidOperationException("Can't determine next header. No extension headers and no known next layer protocol.");
            buffer.Write(offset + Offset.NextHeader, (byte)actualNextHeader);
            buffer.Write(offset + Offset.HopLimit, hopLimit);
            buffer.Write(offset + Offset.SourceAddress, source, Endianity.Big);
            buffer.Write(offset + Offset.DestinationAddress, currentDestination, Endianity.Big);
            extensionHeaders.Write(buffer, offset + HeaderLength, nextLayerProtocol);
        }

        internal static void WriteTransportChecksum(byte[] buffer, int offset, int headerLength, uint transportLength, int transportChecksumOffset,
                                                    bool isChecksumOptional, ushort? checksum, IpV6Address destination)
        {
            ushort checksumValue =
                checksum ?? CalculateTransportChecksum(buffer, offset, headerLength, transportLength, transportChecksumOffset, isChecksumOptional, destination);
            buffer.Write(offset + headerLength + transportChecksumOffset, checksumValue, Endianity.Big);
        }

        private void ParseExtensionHeaders()
        {
            if (_extensionHeaders != null)
                return;

            if (Length < HeaderLength)
            {
                _isValidExtensionHeaders = false;
                _extensionHeaders = IpV6ExtensionHeaders.Empty;
                return;
            }
            _extensionHeaders = new IpV6ExtensionHeaders(Subsegment(HeaderLength, RealPayloadLength), NextHeader);
            _isValidExtensionHeaders = _isValidExtensionHeaders && _extensionHeaders.IsValid;
        }

        private static ushort CalculateTransportChecksum(byte[] buffer, int offset, int fullHeaderLength, uint transportLength, int transportChecksumOffset, bool isChecksumOptional, IpV6Address destination)
        {
            int offsetAfterChecksum = offset + fullHeaderLength + transportChecksumOffset + sizeof(ushort);
            uint sum = Sum16Bits(buffer, offset + Offset.SourceAddress, IpV6Address.SizeOf) +
                       Sum16Bits(destination) +
                       Sum16Bits(transportLength) + buffer[offset + Offset.NextHeader] +
                       Sum16Bits(buffer, offset + fullHeaderLength, transportChecksumOffset) +
                       Sum16Bits(buffer, offsetAfterChecksum, (int)(transportLength - transportChecksumOffset - sizeof(ushort)));

            ushort checksumResult = Sum16BitsToChecksum(sum);
            if (checksumResult == 0 && isChecksumOptional)
                return 0xFFFF;
            return checksumResult;
        }

        private IpV6ExtensionHeaders _extensionHeaders;
        private bool _isValidExtensionHeaders = true;
    }
}
