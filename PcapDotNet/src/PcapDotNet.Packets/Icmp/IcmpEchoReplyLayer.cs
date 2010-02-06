namespace PcapDotNet.Packets.Icmp
{
    public class IcmpEchoReplyLayer : IcmpIdentifiedLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.EchoReply; }
        }
    }
}