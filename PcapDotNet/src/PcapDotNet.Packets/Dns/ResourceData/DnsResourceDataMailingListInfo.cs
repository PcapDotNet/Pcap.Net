namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1035.
    /// <pre>
    /// +---------+
    /// | RMAILBX |
    /// +---------+
    /// | EMAILBX |
    /// +---------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.MInfo)]
    public sealed class DnsResourceDataMailingListInfo : DnsResourceData2DomainNames
    {
        public DnsResourceDataMailingListInfo(DnsDomainName mailingList, DnsDomainName errorMailbox)
            : base(mailingList, errorMailbox)
        {
        }

        /// <summary>
        /// Specifies a mailbox which is responsible for the mailing list or mailbox.
        /// If this domain name names the root, the owner of the MINFO RR is responsible for itself.
        /// Note that many existing mailing lists use a mailbox X-request for the RMAILBX field of mailing list X, e.g., Msgroup-request for Msgroup.
        /// This field provides a more general mechanism.
        /// </summary>
        public DnsDomainName MailingList { get { return First; } }

        /// <summary>
        /// Specifies a mailbox which is to receive error messages related to the mailing list or mailbox specified by the owner of the MINFO RR
        /// (similar to the ERRORS-TO: field which has been proposed).
        /// If this domain name names the root, errors should be returned to the sender of the message.
        /// </summary>
        public DnsDomainName ErrorMailbox { get { return Second; } }

        internal DnsResourceDataMailingListInfo()
            : this(DnsDomainName.Root, DnsDomainName.Root)
        {
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            DnsDomainName mailingList;
            DnsDomainName errorMailbox;
            if (!TryRead(out mailingList, out errorMailbox, dns, offsetInDns, length))
                return null;

            return new DnsResourceDataMailingListInfo(mailingList, errorMailbox);
        }
    }
}