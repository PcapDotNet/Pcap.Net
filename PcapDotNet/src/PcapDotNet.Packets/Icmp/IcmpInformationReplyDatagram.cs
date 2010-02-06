namespace PcapDotNet.Packets.Icmp
{
    public class IcmpInformationReplyDatagram : IcmpIdentifiedDatagram
    {
        internal IcmpInformationReplyDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        public override ILayer ExtractLayer()
        {
            return new IcmpInformationReplyLayer
                       {
                           Checksum = Checksum,
                           Identifier = Identifier,
                           SequenceNumber = SequenceNumber
                       };
        }
    }
}