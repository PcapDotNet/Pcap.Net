using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// <pre>
    /// +-----+-------------------------+
    /// | Bit | 0-31                    |
    /// +-----+-------------------------+
    /// | 0   | unused                  |
    /// +-----+-------------------------+
    /// | 32  | Internet Header         |
    /// |     | + 64 bits of            |
    /// |     | Original Data Datagram  |
    /// +-----+-------------------------+
    /// </pre>
    /// </summary>
    public class IcmpIpV4HeaderPlus64BitsPayloadDatagram : IcmpIpV4PayloadDatagram
    {
        internal IcmpIpV4HeaderPlus64BitsPayloadDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        /// <summary>
        /// The internet header plus the first 64 bits of the original datagram's data.  
        /// This data is used by the host to match the message to the appropriate process.  
        /// If a higher level protocol uses port numbers, they are assumed to be in the first 64 data bits of the original datagram's data.
        /// </summary>
        public IpV4Datagram IpV4
        {
            get { return IpV4Payload; }
        }
    }
}