namespace PcapDotNet.Packets.Dns
{
    public enum DnsDigestType : byte
    {
        /// <summary>
        /// RFC 3658.
        /// SHA-1.
        /// </summary>
        Sha1 = 1,

        /// <summary>
        /// RFC 4509.
        /// SHA-256.
        /// </summary>
        Sha256 = 2,

        /// <summary>
        /// RFC 5933.
        /// GOST R 34.11-94.
        /// </summary>
        GostR341194 = 3,
    }
}