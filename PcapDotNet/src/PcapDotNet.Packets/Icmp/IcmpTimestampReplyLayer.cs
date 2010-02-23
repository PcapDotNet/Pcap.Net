namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// </summary>
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