namespace PcapDotNet.Packets.Icmp
{
    public enum IcmpCodeTraceroute : byte
    {
        /// <summary>
        /// RFC 1393.
        /// </summary>
        OutboundPacketSuccessfullyForwarded = 0x00,

        /// <summary>
        /// RFC 1393.
        /// </summary>
        NoRouteForOutboundPacketDiscarded = 0x01,
    }
}