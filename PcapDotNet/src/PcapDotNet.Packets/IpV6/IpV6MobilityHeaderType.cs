namespace PcapDotNet.Packets.IpV6
{
    public enum IpV6MobilityHeaderType : byte
    {
        /// <summary>
        /// RFC 6275.
        /// </summary>
        BindingRefreshRequest = 0,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        HomeTestInit = 1,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        CareOfTestInit = 2,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        HomeTest = 3,

        /// <summary>
        /// RFC6275
        /// </summary>
        CareOfTest = 4,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        BindingUpdate = 5,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        BindingAcknowledgement = 6,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        BindingError = 7,

        /// <summary>
        /// RFC 5568.
        /// </summary>
        FastBindingUpdate = 8,

        /// <summary>
        /// RFC 5568.
        /// </summary>
        FastBindingAcknowledgement = 9,

        /// <summary>
        /// RFCs 4068, 5568.
        /// Deprecated.
        /// </summary>
        FastNeighborAdvertisement = 10,

        /// <summary>
        /// RFC 5096.
        /// </summary>
        Experimental = 11,

        /// <summary>
        /// RFC 5142.
        /// </summary>
        HomeAgentSwitchMessage = 12,

        /// <summary>
        /// RFC 5847.
        /// </summary>
        HeartbeatMessage = 13,

        /// <summary>
        /// RFC 5568.
        /// </summary>
        HandoverInitiateMessage = 14,

        /// <summary>
        /// RFC 5568.
        /// </summary>
        HandoverAcknowledgeMessage = 15,

        /// <summary>
        /// RFC 5846.
        /// </summary>
        BindingRevocationMessage = 16,

        /// <summary>
        /// RFC-ietf-netext-pmip-lr-10.
        /// </summary>
        LocalizedRoutingInitiation = 17,

        /// <summary>
        /// RFC-ietf-netext-pmip-lr-10.
        /// </summary>
        LocalizedRoutingAcknowledgement = 18,
    }
}