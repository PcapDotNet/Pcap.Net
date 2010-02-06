namespace PcapDotNet.Packets.Icmp
{
    public class IcmpTimestampReplyLayer : IcmpTimestampLayer
    {
        public override IcmpMessageType MessageType
        {
            get
            {
                return IcmpMessageType.TimestampReply;
            }
        }
    }
}