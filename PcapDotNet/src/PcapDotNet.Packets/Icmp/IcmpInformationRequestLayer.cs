namespace PcapDotNet.Packets.Icmp
{
    public class IcmpInformationRequestLayer : IcmpIdentifiedLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.InformationRequest; }
        }
    }
}