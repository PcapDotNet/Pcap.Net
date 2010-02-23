namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 1788.
    /// </summary>
    public class IcmpDomainNameRequestLayer : IcmpIdentifiedLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.DomainNameRequest; }
        }
    }
}