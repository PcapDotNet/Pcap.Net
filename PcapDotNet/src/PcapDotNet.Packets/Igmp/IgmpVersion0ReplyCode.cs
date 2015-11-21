namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 988.
    /// In an IGMP version 0 Reply message, specifies the outcome of the request.
    /// </summary>
    public enum IgmpVersion0ReplyCode : byte
    {
        /// <summary>
        /// Request granted.
        /// </summary>
        RequestGranted = 0,

        /// <summary>
        /// Request denied, no resources.
        /// </summary>
        RequestDeniedNoResources = 1,

        /// <summary>
        /// Request denied, invalid code.
        /// </summary>
        RequestDeniedInvalidCode = 2,

        /// <summary>
        /// Request denied, invalid group address.
        /// </summary>
        RequestDeniedInvalidGroupAddress = 3,

        /// <summary>
        /// Request denied, invalid access key.
        /// </summary>
        RequestDeniedInvalidAccessKey = 4,

        /// <summary>
        /// Request pending, retry in this many seconds.
        /// </summary>
        RequestPendingRetryInThisManySeconds = 5,
    }
}