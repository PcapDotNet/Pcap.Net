namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// The different ICMP code values for Redirect ICMP type.
    /// </summary>
    public enum IcmpCodeRedirect : byte
    {
        /// <summary>
        /// RFC 792.
        /// </summary>
        ForTheNetwork = 0x00,

        /// <summary>
        /// RFC 792.
        /// </summary>
        ForTheHost = 0x01,

        /// <summary>
        /// RFC 792.
        /// </summary>
        ForTheTypeOfServiceAndNetwork = 0x02,

        /// <summary>
        /// RFC 792.
        /// </summary>
        ForTheTypeOfServiceAndHost = 0x03,
    }
}