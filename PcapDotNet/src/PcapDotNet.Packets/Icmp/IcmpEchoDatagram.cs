namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// Echo
    /// RFC 792.
    /// <pre>
    /// +-----+------+------+-----------------+
    /// | Bit | 0-7  | 8-15 | 16-31           |
    /// +-----+------+------+-----------------+
    /// | 0   | Type | Code | Checksum        |
    /// +-----+------+------+-----------------+
    /// | 0   | Identifier  | Sequence Number |
    /// +-----+-------------+-----------------+
    /// | 32  | Data...                       |
    /// +-----+-------------------------------+
    /// </pre>
    /// </summary>
    [IcmpDatagramRegistration(IcmpMessageType.Echo)]
    public sealed class IcmpEchoDatagram : IcmpIdentifiedDatagram
    {
        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IcmpEchoLayer
                       {
                           Checksum = Checksum,
                           Identifier = Identifier,
                           SequenceNumber = SequenceNumber
                       };
        }

        internal override IcmpDatagram CreateInstance(byte[] buffer, int offset, int length)
        {
            return new IcmpEchoDatagram(buffer, offset, length);
        }

        private IcmpEchoDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }
    }
}