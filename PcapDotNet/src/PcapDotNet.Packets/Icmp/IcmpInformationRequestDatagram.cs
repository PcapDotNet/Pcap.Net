namespace PcapDotNet.Packets.Icmp
{
    public class IcmpInformationRequestDatagram : IcmpIdentifiedDatagram
    {
        internal IcmpInformationRequestDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        public override ILayer ExtractLayer()
        {
            return new IcmpInformationRequestLayer
                       {
                           Checksum = Checksum,
                           Identifier = Identifier,
                           SequenceNumber = SequenceNumber
                       };
        }
    }
}