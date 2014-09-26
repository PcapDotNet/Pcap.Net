namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// IPv6 Routing Type.
    /// Used in Routing Extension Header.
    /// </summary>
    public enum IpV6RoutingType : byte
    {
        /// <summary>
        /// RFCs 2460, 5095.
        /// Deprecated.
        /// </summary>
        SourceRoute = 0,

        /// <summary>
        /// Deprecated 06-May-2009.
        /// </summary>
        Nimrod = 1,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        Type2RoutingHeader = 2,

        /// <summary>
        /// RFC 6554.
        /// </summary>
        RoutingProtocolLowPowerAndLossyNetworksSourceRouteHeader = 3,
    }
}