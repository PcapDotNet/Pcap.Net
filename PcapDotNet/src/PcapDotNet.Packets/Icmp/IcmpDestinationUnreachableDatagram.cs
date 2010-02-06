namespace PcapDotNet.Packets.Icmp
{
    public class IcmpDestinationUnreachableDatagram : IcmpIpV4HeaderPlus64BitsPayloadDatagram
    {
        public IcmpDestinationUnreachableDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        public override ILayer ExtractLayer()
        {
            return new IcmpDestinationUnreachableLayer
                       {
                           Code = (IcmpCodeDestinationUnrechable)Code,
                           Checksum = Checksum
                       };
        }
    }
}