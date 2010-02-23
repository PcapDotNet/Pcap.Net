namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// The ICMP code values for Traceroute ICMP type.
    /// </summary>
    public enum IcmpCodeTraceRoute : byte
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