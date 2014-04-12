namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6089.
    /// </summary>
    public enum IpV6FlowIdentificationStatus : byte
    {
        /// <summary>
        /// Flow binding successful.
        /// </summary>
        FlowBindingSuccessful = 0,

        /// <summary>
        /// Administratively prohibited
        /// </summary>
        AdministrativelyProhibited = 128,

        /// <summary>
        /// Flow binding rejected, reason unspecified
        /// </summary>
        FlowBindingRejectedReasonUnspecified = 129,

        /// <summary>
        /// Flow identification mobility option malformed.
        /// </summary>
        FlowIdentificationMobilityOptionMalformed = 130,

        /// <summary>
        /// BID not found.
        /// </summary>
        BindingIdNotFound = 131,

        /// <summary>
        /// FID not found.
        /// </summary>
        FlowIdentifierNotFound = 132,

        /// <summary>
        /// Traffic selector format not supported.
        /// </summary>
        TrafficSelectorFormatNotSupported = 133,
    }
}