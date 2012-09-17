namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// The type of the Tagger ID for <see cref="IpV6OptionSmfDpdSequenceBased"/>
    /// </summary>
    public enum IpV6TaggerIdType : byte
    {
        /// <summary>
        /// No TaggerId field is present.
        /// </summary>
        Null = 0,

        /// <summary>
        /// A TaggerId of non-specific context is present.
        /// </summary>
        Default = 1,

        /// <summary>
        /// A TaggerId representing an IPv4 address is present.
        /// </summary>
        IpV4 = 2,

        /// <summary>
        /// A TaggerId representing an IPv6 address is present.
        /// </summary>
        IpV6 = 3,
    }
}