namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
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
    [IcmpDatagramRegistration(IcmpMessageType.InformationReply)]
    public class IcmpInformationReplyDatagram : IcmpIdentifiedDatagram
    {
        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IcmpInformationReplyLayer
                       {
                           Checksum = Checksum,
                           Identifier = Identifier,
                           SequenceNumber = SequenceNumber
                       };
        }

        internal override IcmpDatagram CreateInstance(byte[] buffer, int offset, int length)
        {
            return new IcmpInformationReplyDatagram(buffer, offset, length);
        }

        private IcmpInformationReplyDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }
    }
}