namespace PcapDotNet.Packets.Icmp
{
    public class IcmpEchoLayer : IcmpIdentifiedLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.Echo; }
        }
    }
}