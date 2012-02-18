namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// The type of digest that is used to create a digest value.
    /// </summary>
    public enum DnsDigestType : byte
    {
        /// <summary>
        /// No definition for digest type.
        /// Should not be used.
        /// </summary>
        None = 0,

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