namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 1256.
    /// </summary>
    public class IcmpRouterSolicitationLayer : IcmpLayer
    {
        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.RouterSolicitation; }
        }
    }
}