namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 2671.
    /// The implementation level of whoever sets it for the OPT resource record.
    /// </summary>
    public enum DnsOptVersion : byte
    {
        /// <summary>
        /// Full conformance with the specification.
        /// </summary>
        Version0 = 0,
    }
}