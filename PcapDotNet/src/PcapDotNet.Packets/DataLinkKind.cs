namespace PcapDotNet.Packets
{
    /// <summary>
    /// Represents the different data links kinds.
    /// </summary>
    public enum DataLinkKind
    {
        /// <summary>
        /// Ethernet data link kind.
        /// </summary>
        Ethernet,

        /// <summary>
        /// IPv4 data link kind.
        /// </summary>
        IpV4,

        /// <summary>
        /// Data Over Cable Service Interface Specification.
        /// </summary>
        Docsis,
    }
}