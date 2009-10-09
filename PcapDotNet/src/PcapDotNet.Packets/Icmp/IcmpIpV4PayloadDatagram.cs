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
    public class IcmpIpV4PayloadDatagram : IcmpTypedDatagram
    {
        internal IcmpIpV4PayloadDatagram(byte[] buffer, int offset, int length)
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
            get
            {
                if (_ipV4 == null && Length >= HeaderLength)
                    _ipV4 = new IpV4Datagram(Buffer, StartOffset + HeaderLength, Length - HeaderLength);
                return _ipV4;
            }
        }

        private IpV4Datagram _ipV4;
    }
}