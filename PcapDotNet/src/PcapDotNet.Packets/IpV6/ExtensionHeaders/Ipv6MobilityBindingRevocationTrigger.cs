namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// The Per-MN Revocation Trigger values are less than 128.
    /// The Per-MN Revocation Trigger is used when the BRI message intends to revoke one or more bindings for the same mobile node.
    /// The Global Revocation Trigger values are greater than 128 and less than 250 and used in the BRI message 
    /// when the Global (G) bit is set for global revocation.
    /// The values 250-255 are reserved for testing purposes only.
    /// </summary>
    public enum Ipv6MobilityBindingRevocationTrigger : byte
    {
        /// <summary>
        /// Unspecified.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// Administrative Reason.
        /// </summary>
        AdministrativeReason = 1,

        /// <summary>
        /// Inter-MAG Handover - same Access Type.
        /// </summary>
        InterMagHandoverSameAccessType = 2,

        /// <summary>
        /// Inter-MAG Handover - different Access Type.
        /// </summary>
        InterMagHandoverDifferentAccessType = 3,

        /// <summary>
        /// Inter-MAG Handover - Unknown.
        /// </summary>
        InterMagHandoverUnknown = 4,

        /// <summary>
        /// User-Initiated Session(s) Termination
        /// </summary>
        UserInitiatedSessionsTermination = 5,

        /// <summary>
        /// Access Network Session(s) Termination
        /// </summary>
        AccessNetworkSessionsTermination = 6,
        
        /// <summary>
        /// Possible Out-of-Sync BCE State.
        /// </summary>
        PossibleOutOfSyncBceState = 7,

        /// <summary>
        /// Global Revocation Trigger Value.
        /// Per-Peer Policy.
        /// </summary>
        GlobalPerPeerPolicy = 128,

        /// <summary>
        /// Global Revocation Trigger Value.
        /// Revoking Mobility Node Local Policy.
        /// </summary>
        GlobalRevokingMobilityNodeLocalPolicy = 129,
    }
}