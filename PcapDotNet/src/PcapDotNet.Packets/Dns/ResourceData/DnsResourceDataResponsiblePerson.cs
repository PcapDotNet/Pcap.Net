namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1183.
    /// <pre>
    /// +------------+
    /// | mbox-dname |
    /// +------------+
    /// | txt-dname  |
    /// +------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Rp)]
    public sealed class DnsResourceDataResponsiblePerson : DnsResourceData2DomainNames
    {
        public DnsResourceDataResponsiblePerson(DnsDomainName mailBox, DnsDomainName textDomain)
            : base(mailBox, textDomain)
        {
        }

        /// <summary>
        /// A domain name that specifies the mailbox for the responsible person.
        /// Its format in master files uses the DNS convention for mailbox encoding, identical to that used for the RNAME mailbox field in the SOA RR.
        /// The root domain name (just ".") may be specified for MailBox to indicate that no mailbox is available.
        /// </summary>
        public DnsDomainName MailBox { get { return First; } }

        /// <summary>
        /// A domain name for which TXT RR's exist. 
        /// A subsequent query can be performed to retrieve the associated TXT resource records at TextDomain.
        /// This provides a level of indirection so that the entity can be referred to from multiple places in the DNS.
        /// The root domain name (just ".") may be specified for TextDomain to indicate that the TXT_DNAME is absent, and no associated TXT RR exists.
        /// </summary>
        public DnsDomainName TextDomain { get { return Second; } }

        internal DnsResourceDataResponsiblePerson()
            : this(DnsDomainName.Root, DnsDomainName.Root)
        {
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            DnsDomainName mailBox;
            DnsDomainName textDomain;
            if (!TryRead(out mailBox, out textDomain, dns, offsetInDns, length))
                return null;

            return new DnsResourceDataResponsiblePerson(mailBox, textDomain);
        }
    }
}