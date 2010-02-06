namespace PcapDotNet.Packets.Icmp
{
    public class IcmpDomainNameRequestLayer : IcmpIdentifiedLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.DomainNameRequest; }
        }
    }
}