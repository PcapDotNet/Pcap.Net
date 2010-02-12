namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 950.
    /// <pre>
    /// +-----+------+------+-----------------+
    /// | Bit | 0-7  | 8-15 | 16-31           |
    /// +-----+------+------+-----------------+
    /// | 0   | Type | Code | Checksum        |
    /// +-----+------+------+-----------------+
    /// | 32  | Identifier  | Sequence Number |
    /// +-----+-------------+-----------------+
    /// | 64  | Address Mask                  |
    /// +-----+-------------------------------+
    /// </pre>
    /// </summary>
    public class IcmpAddressMaskReplyDatagram : IcmpAddressMaskRequestDatagram
    {
        internal IcmpAddressMaskReplyDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        public override ILayer ExtractLayer()
        {
            return new IcmpAddressMaskReplyLayer
                       {
                           Checksum = Checksum,
                           Identifier = Identifier,
                           SequenceNumber = SequenceNumber,
                           AddressMask = AddressMask
                       };
        }
    }
}