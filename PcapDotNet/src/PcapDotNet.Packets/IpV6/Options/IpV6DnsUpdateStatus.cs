namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5026.
    /// </summary>
    public enum IpV6DnsUpdateStatus : byte
    {
        /// <summary>
        /// DNS update performed.
        /// </summary>
        DnsUpdatePerformed = 0,

        /// <summary>
        /// Reason unspecified.
        /// </summary>
        ReasonUnspecified = 128,

        /// <summary>
        /// Administratively prohibited.
        /// </summary>
        AdministrativelyProhibited = 129,

        /// <summary>
        /// DNS update failed.
        /// </summary>
        DnsUpdateFailed = 130,
    }
}