namespace PcapDotNet.Packets.Icmp
{
    public class IcmpSourceQuenchLayer : IcmpLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.SourceQuench; }
        }
    }
}