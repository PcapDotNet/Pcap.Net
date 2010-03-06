using System;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 1475.
    /// <pre>
    /// +-----+------+------+-----------+
    /// | Bit | 0-7  | 8-15 | 16-31     |
    /// +-----+------+------+-----------+
    /// | 0   | Type | Code | Checksum  |
    /// +-----+------+------+-----------+
    /// | 32  | pointer to problem area |
    /// +-----+-------------------------+
    /// | 64  | copy of datagram that   |
    /// |     | could not be converted  |
    /// |     | ...                     |
    /// +-----+-------------------------+
    /// </pre>
    /// </summary>
    public class IcmpConversionFailedDatagram : IcmpIpV4PayloadDatagram
    {
        /// <summary>
        /// The number of bytes that should be taken from the original datagram for an unsupported transport protocol ICMP code.
        /// </summary>
        public const int OriginalDatagramLengthForUnsupportedTransportProtocol = 256;

        private static class Offset
        {
            public const int Pointer = 4;
        }

        /// <summary>
        /// An offset from the start of the original datagram to the beginning of the offending field.
        /// </summary>
        public uint Pointer
        {
            get { return ReadUInt(Offset.Pointer, Endianity.Big); }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IcmpConversionFailedLayer
                       {
                           Checksum = Checksum,
                           Code = (IcmpCodeConversionFailed)Code,
                           Pointer = Pointer
                       };
        }

        /// <summary>
        /// Valid if the datagram's length is OK, the checksum is correct, the code is in the expected range
        /// and the IPv4 payload contains at least an IPv4 header and the transport header.
        /// If the code is for unsupported transport protocol, the IPv4 payload should contain 256 bytes of the original datagram.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            if (!base.CalculateIsValid())
                return false;

            IpV4Datagram ip = IpV4;

            if ((IcmpCodeConversionFailed)Code == IcmpCodeConversionFailed.UnsupportedTransportProtocol)
                return ip.Length == OriginalDatagramLengthForUnsupportedTransportProtocol;

            switch (ip.Protocol)
            {
                case IpV4Protocol.Udp:
                    return ip.Udp.Length >= UdpDatagram.HeaderLength;

                case IpV4Protocol.Tcp:
                    TcpDatagram tcp = ip.Tcp;
                    return tcp.Length >= TcpDatagram.HeaderMinimumLength && tcp.Length >= tcp.HeaderLength;

                default: // Todo: support more protocols
                    return true;
            }
        }

        /// <summary>
        /// The minimum valid ICMP code for this type of ICMP datagram.
        /// </summary>
        protected override byte MinCodeValue
        {
            get { return _minCode; }
        }

        /// <summary>
        /// The maximum valid ICMP code for this type of ICMP datagram.
        /// </summary>
        protected override byte MaxCodeValue
        {
            get { return _maxCode; }
        }

        internal IcmpConversionFailedDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        private static readonly byte _minCode = (byte)typeof(IcmpCodeConversionFailed).GetEnumValues<IcmpCodeConversionFailed>().Min();
        private static readonly byte _maxCode = (byte)typeof(IcmpCodeConversionFailed).GetEnumValues<IcmpCodeConversionFailed>().Max();
    }
}