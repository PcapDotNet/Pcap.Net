namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// Handover Acknowledge Code for IPv6 Mobility Handover Acknowledge Message Extension Header.
    /// </summary>
    public enum IpV6MobilityHandoverAcknowledgeCode : byte
    {
        /// <summary>
        /// Handover Accepted, NCoA valid.
        /// </summary>
        HandoverAcceptedMobilityNodeCareOfAddressValid = 0,
        
        /// <summary>
        /// Handover Accepted, NCoA not valid or in use.
        /// </summary>
        HandoverAcceptedMobilityNodeCareOfAddressNotValidOrInUse = 1,

        /// <summary>
        /// Handover Accepted, NCoA assigned (used in Assigned Addressing).
        /// </summary>
        HandoverAcceptedMobilityNodeCareOfAddressAssigned = 2,

        /// <summary>
        /// Handover Accepted, use PCoA.
        /// </summary>
        HandoverAcceptedUsePreviousCareOfAddress = 3,

        /// <summary>
        /// Message sent unsolicited, usually to trigger an HI message.
        /// </summary>
        MessageSentUnsolicited = 4,

        /// <summary>
        /// Handover Not Accepted, reason unspecified.
        /// </summary>
        HandoverNotAccepted = 128,

        /// <summary>
        /// Administratively prohibited.
        /// </summary>
        AdministrativelyProhibited = 129,

        /// <summary>
        /// Insufficient resources.
        /// </summary>
        InsufficientResources = 130,
    }
}