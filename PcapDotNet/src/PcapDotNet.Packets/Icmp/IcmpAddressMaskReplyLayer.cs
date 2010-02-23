namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 950.
    /// </summary>
    public class IcmpAddressMaskReplyLayer : IcmpAddressMaskRequestLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.AddressMaskReply; }
        }
    }
}