namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5845.
    /// </summary>
    public enum IpV6MobilityIpV6AddressPrefixCode : byte
    {
        /// <summary>
        /// Old Care-of Address.
        /// </summary>
        OldCareOfAddress = 1,

        /// <summary>
        /// New Care-of Address.
        /// </summary>
        NewCareOfAddress = 2,

        /// <summary>
        /// NAR's IP address.
        /// </summary>
        NewAccessRouterIpAddress = 3,

        /// <summary>
        /// NAR's Prefix, sent in PrRtAdv.
        /// The Prefix Length field contains the number of valid leading bits in the prefix.
        /// The bits in the prefix after the prefix length are reserved and must be initialized to zero by the sender and ignored by the receiver.
        /// </summary>
        NewAccessRouterPrefix = 4,
    }
}