namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1035.
    /// Type fields are used in resource records.
    /// </summary>
    public enum DnsType : ushort
    {
        /// <summary>
        /// A host address.
        /// </summary>
        A = 1,

        /// <summary>
        /// An authoritative name server.
        /// </summary>
        Ns = 2,

        /// <summary>
        /// A mail destination (Obsolete - use MX).
        /// </summary>
        Md = 3,

        /// <summary>
        /// A mail forwarder (Obsolete - use MX).
        /// </summary>
        Mf = 4,

        /// <summary>
        /// The canonical name for an alias.
        /// </summary>
        CName = 5,
        
        /// <summary>
        /// Marks the start of a zone of authority.
        /// </summary>
        Soa = 6,

        /// <summary>
        /// A mailbox domain name (EXPERIMENTAL).
        /// </summary>
        Mb = 7,

        /// <summary>
        /// A mail group member (EXPERIMENTAL).
        /// </summary>
        Mg = 8,

        /// <summary>
        /// A mail rename domain name (EXPERIMENTAL).
        /// </summary>
        Mr = 9,

        /// <summary>
        /// A null RR (EXPERIMENTAL).
        /// </summary>
        Null = 10,

        /// <summary>
        /// A well known service description..
        /// </summary>
        Wks = 11,

        /// <summary>
        /// A domain name pointer.
        /// </summary>
        Ptr = 12,

        /// <summary>
        /// Host information.
        /// </summary>
        HInfo = 13,

        /// <summary>
        /// mailbox or mail list information.
        /// </summary>
        MInfo = 14,

        /// <summary>
        /// Mail exchange.
        /// </summary>
        Mx = 15,

        /// <summary>
        /// Text strings.
        /// </summary>
        Txt = 16,

        /// <summary>
        /// A request for a transfer of an entire zone.
        /// Query Type.
        /// </summary>
        Axfr = 252,

        /// <summary>
        /// A request for mailbox-related records (MB, MG or MR).
        /// Query Type.
        /// </summary>
        MailB = 253, 

        /// <summary>
        /// A request for mail agent RRs (Obsolete - see MX).
        /// Query Type.
        /// </summary>
        MailA = 254,

        /// <summary>
        /// *.
        /// A request for all records
        /// Query Type.
        /// </summary>
        All = 255,
    }
}