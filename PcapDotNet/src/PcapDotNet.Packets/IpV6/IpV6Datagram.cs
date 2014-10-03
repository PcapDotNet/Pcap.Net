using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public override int TotalLength
        {
            get { return HeaderLength + PayloadLength; }
        }

        protected override ushort CalculateTransportChecksum()
        {
            return CalculateTransportChecksum(Buffer, StartOffset, (ushort)Transport.Length, Transport.ChecksumOffset, Transport.IsChecksumOptional, CurrentDestination);
        }

        private static ushort CalculateTransportChecksum(byte[] buffer, int offset, ushort transportLength, int transportChecksumOffset, bool isChecksumOptional, IpV6Address destination)
        {
            int offsetAfterChecksum = offset + HeaderLength + transportChecksumOffset + 2;
            uint sum = Sum16Bits(buffer, offset + Offset.SourceAddress, IpV6Address.SizeOf) +
                       Sum16Bits(destination) +
                       transportLength + buffer[offset + Offset.NextHeader] +
                       Sum16Bits(buffer, offset + HeaderLength, transportChecksumOffset) +
                       Sum16Bits(buffer, offsetAfterChecksum, transportLength - transportChecksumOffset - 2);

            ushort checksumResult = Sum16BitsToChecksum(sum);
            if (checksumResult == 0 && isChecksumOptional)
                return 0xFFFF;
            return checksumResult;
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

        internal override DataSegment GetPayload()
        {
            if (Length < HeaderLength)
                return null;
            return Subsegment(HeaderLength + ExtensionHeaders.BytesLength, Length - HeaderLength - ExtensionHeaders.BytesLength);
        }

        private void ParseExtensionHeaders()
        {
            if (_extensionHeaders != null)
                return;

            if (Length < HeaderLength)
            {
                _isValid = false;
                _extensionHeaders = IpV6ExtensionHeaders.Empty;
                return;
            }
            _extensionHeaders = new IpV6ExtensionHeaders(Subsegment(HeaderLength, RealPayloadLength), NextHeader);
            _isValid = _isValid && _extensionHeaders.IsValid;
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
        /// The default validity check always returns true.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            ParseExtensionHeaders();
            return _isValid;
        }

        internal IpV6Datagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal static int GetTotalLength(Datagram payload)
        {
            if (payload.Length <= HeaderLength)
                return payload.Length;

            return Math.Min(payload.Length, HeaderLength + payload.ReadUShort(Offset.PayloadLength, Endianity.Big));
        }

        internal static void WriteHeader(byte[] buffer, int offset,
                                         byte trafficClass, int flowLabel, ushort payloadLength, IpV4Protocol nextHeader, byte hopLimit,
                                         IpV6Address source, IpV6Address currentDestination, IpV6ExtensionHeaders extensionHeaders)
        {
            buffer.Write(offset + Offset.Version, (uint)((((DefaultVersion << 8) | trafficClass) << 20) | flowLabel), Endianity.Big);
            buffer.Write(offset + Offset.PayloadLength, payloadLength, Endianity.Big);
            buffer.Write(offset + Offset.NextHeader, (byte)nextHeader);
            buffer.Write(offset + Offset.HopLimit, hopLimit);
            buffer.Write(offset + Offset.SourceAddress, source, Endianity.Big);
            buffer.Write(offset + Offset.DestinationAddress, currentDestination, Endianity.Big);
            extensionHeaders.Write(buffer, offset + HeaderLength);
        }

        private IpV6ExtensionHeaders _extensionHeaders;
        private bool _isValid = true;
    }
}
