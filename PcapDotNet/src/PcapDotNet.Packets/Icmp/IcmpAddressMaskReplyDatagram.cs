namespace PcapDotNet.Packets.Icmp
{
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