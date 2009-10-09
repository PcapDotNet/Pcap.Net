namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// Echo or Echo Reply
    /// RFC 792.
    /// <pre>
    /// +-----+------------+-----------------+
    /// | Bit | 0-15       | 16-31           |
    /// +-----+------------+-----------------+
    /// | 0   | Identifier | Sequence Number |
    /// +-----+------------+-----------------+
    /// | 32  | Data...                      |
    /// +-----+------------------------------+
    /// </pre>
    /// </summary>
    public class IcmpEchoDatagram : IcmpIdentifiedDatagram
    {
        private class Offset
        {
            public const int Data = 4;
        }

        /// <summary>
        /// The data received in the echo message must be returned in the echo reply message.
        /// </summary>
        public Datagram Data
        {
            get { return new Datagram(Buffer, StartOffset + Offset.Data, Length - Offset.Data); }
        }

        internal IcmpEchoDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }
    }
}