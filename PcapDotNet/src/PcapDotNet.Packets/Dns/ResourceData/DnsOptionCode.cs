namespace PcapDotNet.Packets.Dns
{
    public enum DnsOptionCode : ushort
    {
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