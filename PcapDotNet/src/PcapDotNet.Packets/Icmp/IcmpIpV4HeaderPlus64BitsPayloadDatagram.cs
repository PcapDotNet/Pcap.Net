using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// <pre>
    /// +-----+------+------+-----------+
    /// | Bit | 0-7  | 8-15 | 16-31     |
    /// +-----+------+------+-----------+
    /// | 0   | Type | Code | Checksum  |
    /// +-----+------+------+-----------+
    /// | 32  | unused                  |
    /// +-----+-------------------------+
    /// | 64  | Internet Header         |
    /// |     | + 64 bits of            |
    /// |     | Original Data Datagram  |
    /// +-----+-------------------------+
    /// </pre>
    /// </summary>
    public abstract class IcmpIpV4HeaderPlus64BitsPayloadDatagram : IcmpIpV4PayloadDatagram
    {
        /// <summary>
        /// The number of bytes this payload includes from the original data datagram.
        /// </summary>
        public const int OriginalDatagramPayloadLength = 8;

        internal IcmpIpV4HeaderPlus64BitsPayloadDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        /// <summary>
        /// ICMP with IPv4 payload and 64 bits IPv4 payload is valid if the datagram's length is OK, the checksum is correct, the code is in the expected range,
        /// the IPv4 payload contains at least an IPv4 header and the IPv4's payload is in the expected size.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            if (!base.CalculateIsValid())
                return false;

            return IpV4.Payload.Length == OriginalDatagramPayloadLength;
        }
    }
}