namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 1788.
    /// <pre>
    /// +-----+------+------+-----------------+
    /// | Bit | 0-7  | 8-15 | 16-31           |
    /// +-----+------+------+-----------------+
    /// | 0   | Type | Code | Checksum        |
    /// +-----+------+------+-----------------+
    /// | 32  | Identifier  | Sequence Number |
    /// +-----+-------------+-----------------+
    /// </pre>
    /// </summary>
    [IcmpDatagramRegistration(IcmpMessageType.DomainNameRequest)]
    public class IcmpDomainNameRequestDatagram : IcmpIdentifiedDatagram
    {
        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IcmpDomainNameRequestLayer
                       {
                           Checksum = Checksum,
                           Identifier = Identifier,
                           SequenceNumber = SequenceNumber
                       };
        }

        internal override IcmpDatagram CreateInstance(byte[] buffer, int offset, int length)
        {
            return new IcmpDomainNameRequestDatagram(buffer, offset, length);
        }

        private IcmpDomainNameRequestDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }
    }
}