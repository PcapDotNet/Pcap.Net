namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// </summary>
    public class IcmpSourceQuenchLayer : IcmpLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.SourceQuench; }
        }
    }
}