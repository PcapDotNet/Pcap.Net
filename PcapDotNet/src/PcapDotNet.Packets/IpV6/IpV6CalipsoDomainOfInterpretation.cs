namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5570.
    /// CALIPSO Domain of Interpretation (DOI)
    /// </summary>
    public enum IpV6CalipsoDomainOfInterpretation : uint
    {
        /// <summary>
        /// RFC 5570.
        /// Must not appear in any IPv6 packet on any network.
        /// </summary>
        Null = 0x0000,
    }
}