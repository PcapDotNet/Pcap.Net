namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// Used to represent an ICMP datagram with an unknown message type.
    /// </summary>
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

        protected override bool CalculateIsValid()
        {
            return false;
        }
    }
}