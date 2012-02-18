namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// The option code for a DNS option.
    /// </summary>
    public enum DnsOptionCode : ushort
    {
        /// <summary>
        /// No code defined.
        /// Should not be used.
        /// </summary>
        None = 0,

        /// <summary>
        /// http://files.dns-sd.org/draft-sekar-dns-llq.txt.
        /// LLQ.
        /// </summary>
        LongLivedQuery = 1,

        /// <summary>
        /// http://files.dns-sd.org/draft-sekar-dns-ul.txt.
        /// UL.
        /// </summary>
        UpdateLease = 2,

        /// <summary>
        /// RFC 5001.
        /// NSID.
        /// </summary>
        NameServerIdentifier = 3,
    }
}