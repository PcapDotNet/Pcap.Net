namespace PcapDotNet.Packets.Icmp
{
    public class IcmpSourceQuenchDatagram : IcmpIpV4HeaderPlus64BitsPayloadDatagram
    {
        public IcmpSourceQuenchDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        public override ILayer ExtractLayer()
        {
            return new IcmpSourceQuenchLayer
                       {
                           Checksum = Checksum
                       };
        }

    }
}