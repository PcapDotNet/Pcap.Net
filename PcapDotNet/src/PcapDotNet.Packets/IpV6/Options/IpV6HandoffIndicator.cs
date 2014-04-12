namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5213.
    /// </summary>
    public enum IpV6HandoffIndicator : byte
    {
        /// <summary>
        /// Attachment over a new interface.
        /// </summary>
        AttachmentOverNewInterface = 1,

        /// <summary>
        /// Handoff between two different interfaces of the mobile node.
        /// </summary>
        HandoffBetweenTwoDifferentInterfacesOfTheMobileNode = 2,

        /// <summary>
        /// Handoff between mobile access gateways for the same interface.
        /// </summary>
        HandoffBetweenMobileAccessGatewaysForTheSameInterface = 3,

        /// <summary>
        /// Handoff state unknown.
        /// </summary>
        HandoffStateUnknown = 4,

        /// <summary>
        /// Handoff state not changed (Re-registration).
        /// </summary>
        HandoffStateNotChanged = 5,
    }
}