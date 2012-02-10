namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 5155.
    /// </summary>
    public enum DnsSecNSec3HashAlgorithm : byte
    {
        None = 0x00,

        /// <summary>
        /// RFC 5155.
        /// </summary>
        Sha1 = 0x01,
    }
}