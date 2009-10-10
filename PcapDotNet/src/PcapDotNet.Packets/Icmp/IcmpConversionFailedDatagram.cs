using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 1475.
    /// <pre>
    /// +-----+-------------------------+
    /// | Bit | 0-31                    |
    /// +-----+-------------------------+
    /// | 0   | pointer to problem area |
    /// +-----+-------------------------+
    /// | 32  | copy of datagram that   |
    /// |     | could not be converted  |
    /// |     | ...                     |
    /// +-----+-------------------------+
    /// </pre>
    /// </summary>
    public class IcmpConversionFailedDatagram : IcmpIpV4PayloadDatagram
    {
        private class Offset
        {
            public const int Pointer = 0;
        }

        /// <summary>
        /// An offset from the start of the original datagram to the beginning of the offending field.
        /// </summary>
        public uint Pointer
        {
            get { return ReadUInt(Offset.Pointer, Endianity.Big); }
        }

        /// <summary>
        /// The data is part of the datagram that could not be converted.  
        /// It must be at least the IP and transport headers, and must include the field pointed to by the previous parameter.  
        /// For code 4, the transport header is probably not identifiable; the data should include 256 bytes of the original datagram.
        /// </summary>
        public IpV4Datagram IpV4
        {
            get { return IpV4Payload; }
        }

        internal IcmpConversionFailedDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }
    }
}