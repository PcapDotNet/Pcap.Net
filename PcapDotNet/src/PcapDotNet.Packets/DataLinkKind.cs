namespace PcapDotNet.Packets
{
    /// <summary>
    /// Represents the different data links kinds.
    /// See http://www.tcpdump.org/linktypes.html for more data link kinds.
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

        /// <summary>
        /// PPP With Directional Info.
        /// PPP, as per RFC 1661 and RFC 1662, preceded with a one-byte pseudo-header with a zero value meaning "received by this host" 
        /// and a non-zero value meaning "sent by this host".
        /// </summary>
        PppWithDirection,

        /// <summary>
        /// Linux cooked-mode capture.
        /// </summary>
        LinuxSll,
    }
}