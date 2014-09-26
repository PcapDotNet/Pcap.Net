namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6602.
    /// </summary>
    public enum IpV6MobileNodeGroupIdentifierSubtype : byte
    {
        /// <summary>
        /// Invalid value.
        /// </summary>
        None = 0,

        /// <summary>
        /// RFC 6602.
        /// </summary>
        BulkBindingUpdateGroup = 1,
    }
}