namespace PcapDotNet.Packets.Icmp
{
    public class IcmpDestinationUnreachableLayer : IcmpLayer
    {
        public IcmpCodeDestinationUnrechable Code { get; set; }

        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.DestinationUnreachable; }
        }

        public override byte CodeValue
        {
            get { return (byte)Code; }
        }
    }
}