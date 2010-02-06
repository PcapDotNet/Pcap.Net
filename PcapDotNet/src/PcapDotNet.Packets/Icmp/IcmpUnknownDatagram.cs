namespace PcapDotNet.Packets.Icmp
{
    public class IcmpUnknownDatagram : IcmpDatagram
    {
        public IcmpUnknownDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        public override ILayer ExtractLayer()
        {
            return new IcmpUnknownLayer
                       {
                           LayerMessageType = (byte)MessageType,
                           LayerCode = Code,
                           Checksum = Checksum,
                           LayerValue = Variable,
                           Payload = Payload
                       };
        }
    }
}