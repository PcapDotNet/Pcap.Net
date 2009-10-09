namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// <pre>
    /// +-----+------------+-----------------+
    /// | Bit | 0-15       | 16-31           |
    /// +-----+------------+-----------------+
    /// | 0   | Identifier | Sequence Number |
    /// +-----+------------+-----------------+
    /// </pre>
    /// </summary>
    public class IcmpIdentifiedDatagram : IcmpTypedDatagram
    {
        private class Offset
        {
            public const int Identifier = 0;
            public const int SequenceNumber = 2;
        }

        /// <summary>
        /// If code = 0, an identifier to aid in matching requests and replies, may be zero.
        /// </summary>
        public ushort Identifier
        {
            get { return ReadUShort(Offset.Identifier, Endianity.Big); }
        }

        /// <summary>
        /// If code = 0, a sequence number to aid in matching requests and replies, may be zero.
        /// </summary>
        public ushort SequenceNumber
        {
            get { return ReadUShort(Offset.SequenceNumber, Endianity.Big); }
        }

        internal IcmpIdentifiedDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }
    }
}