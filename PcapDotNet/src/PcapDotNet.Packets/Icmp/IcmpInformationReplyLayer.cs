namespace PcapDotNet.Packets.Icmp
{
    public class IcmpInformationReplyLayer : IcmpIdentifiedLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.InformationReply; }
        }
    }
}