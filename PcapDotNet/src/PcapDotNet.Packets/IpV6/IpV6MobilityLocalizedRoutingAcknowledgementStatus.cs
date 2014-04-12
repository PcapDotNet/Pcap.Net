namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC-ietf-netext-pmip-lr-10.
    /// </summary>
    public enum IpV6MobilityLocalizedRoutingAcknowledgementStatus : byte
    {
        /// <summary>
        /// Success.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Localized Routing Not Allowed.
        /// </summary>
        LocalizedRoutingNotAllowed = 128,

        /// <summary>
        /// MN not attached.
        /// </summary>
        MobileNodeNotAttached = 129,
    }
}