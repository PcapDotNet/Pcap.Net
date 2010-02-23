namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 1256.
    /// </summary>
    public class IcmpRouterSolicitationLayer : IcmpLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.RouterSolicitation; }
        }
    }
}