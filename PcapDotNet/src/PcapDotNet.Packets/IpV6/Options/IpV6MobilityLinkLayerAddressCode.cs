namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5568.
    /// A code for a Link Layer Address IPv6 option.
    /// </summary>
    public enum IpV6MobilityLinkLayerAddressCode : byte
    {
        /// <summary>
        /// Wildcard requesting resolution for all nearby access points.
        /// </summary>
        Wildcard = 0,

        /// <summary>
        /// Link-Layer Address of the New Access Point.
        /// The address contains the link-layer address of the access point for which handover is about to be attempted.
        /// This is used in the Router Solicitation for Proxy Advertisement message.
        /// </summary>
        NewAccessPoint = 1,

        /// <summary>
        /// Link-Layer Address of the MN (Mobility Node).
        /// The address contains the link-layer address of an MN.
        /// It is used in the Handover Initiate message.
        /// </summary>
        MobilityNode = 2,

        /// <summary>
        /// Link-Layer Address of the NAR (New Access Router) (i.e., Proxied Originator).
        /// The address contains the link-layer address of the access router to which the Proxy Router Solicitation message refers.
        /// </summary>
        NewAccessRouter = 3,

        /// <summary>
        /// Link-Layer Address of the source of RtSolPr (Router Solicitation for Proxy Advertisement) or PrRtAdv (Proxy Router Advertisement) message.
        /// </summary>
        SourceRouterSolicitationForProxyAdvertisementOrProxyRouterAdvertisement = 4,

        /// <summary>
        /// The access point identified by the LLA belongs to the current interface of the router.
        /// </summary>
        AccessPoint = 5,

        /// <summary>
        /// No prefix information available for the access point identified by the LLA
        /// </summary>
        NoPrefixForAccessPoint = 6,

        /// <summary>
        /// No fast handover support available for the access point identified by the LLA
        /// </summary>
        NoFastHandoverSupportForAccessPoint = 7,
    }
}