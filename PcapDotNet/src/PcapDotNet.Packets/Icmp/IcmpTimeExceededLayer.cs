namespace PcapDotNet.Packets.Icmp
{
    public class IcmpTimeExceededLayer : IcmpLayer
    {
        public IcmpCodeTimeExceeded Code { get; set; }

        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.TimeExceeded; }
        }

        public override byte CodeValue
        {
            get { return (byte)Code; }
        }
    }
}