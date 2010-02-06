namespace PcapDotNet.Packets.Icmp
{
    public class IcmpRouterSolicitationLayer : IcmpLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.RouterSolicitation; }
        }
    }
}