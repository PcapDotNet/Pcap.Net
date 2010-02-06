namespace PcapDotNet.Packets.Icmp
{
    public class IcmpDomainNameRequestDatagram : IcmpIdentifiedDatagram
    {
        internal IcmpDomainNameRequestDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        public override ILayer ExtractLayer()
        {
            return new IcmpDomainNameRequestLayer
                       {
                           Checksum = Checksum,
                           Identifier = Identifier,
                           SequenceNumber = SequenceNumber
                       };
        }
    }
}