namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFCs 6088, 6089.
    /// </summary>
    public enum IpV6FlowIdentificationTrafficSelectorFormat : byte
    {
        /// <summary>
        /// Invalid value.
        /// </summary>
        None = 0,

        /// <summary>
        /// IPv4 binary traffic selector.
        /// </summary>
        IpV4Binary = 1,

        /// <summary>
        /// IPv6 binary traffic selector.
        /// </summary>
        IpV6Binary = 2,
    }
}