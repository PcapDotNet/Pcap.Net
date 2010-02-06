namespace PcapDotNet.Packets.Icmp
{
    public class IcmpTimeExceededDatagram : IcmpIpV4HeaderPlus64BitsPayloadDatagram
    {
        public IcmpTimeExceededDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        public override ILayer ExtractLayer()
        {
            return new IcmpTimeExceededLayer
                       {
                           Code = (IcmpCodeTimeExceeded)Code,
                           Checksum = Checksum
                       };
        }
    }
}