namespace PcapDotNet.Packets.Icmp
{
    public class IcmpAddressMaskReplyLayer : IcmpAddressMaskRequestLayer
    {
        public override IcmpMessageType MessageType
        {
            get
            {
                return IcmpMessageType.AddressMaskReply;
            }
        }
    }
}