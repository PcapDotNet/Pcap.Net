namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1035.
    /// <pre>
    /// +-----+------------+
    /// | bit | 0-15       |
    /// +-----+------------+
    /// | 0   | PREFERENCE |
    /// +-----+------------+
    /// | 16  | EXCHANGE   |
    /// | ... |            |
    /// +-----+------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.MailExchange)]
    public sealed class DnsResourceDataMailExchange : DnsResourceDataUShortDomainName
    {
        public DnsResourceDataMailExchange(ushort preference, DnsDomainName mailExchangeHost)
            : base(preference, mailExchangeHost)
        {
        }

        /// <summary>
        /// Specifies the preference given to this RR among others at the same owner.
        /// Lower values are preferred.
        /// </summary>
        public ushort Preference { get { return Value; } }

        /// <summary>
        /// Specifies a host willing to act as a mail exchange for the owner name.
        /// </summary>
        public DnsDomainName MailExchangeHost { get { return DomainName; } }

        internal DnsResourceDataMailExchange()
            : this(0, DnsDomainName.Root)
        {
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            ushort preference;
            DnsDomainName mailExchangeHost;
            if (!TryRead(out preference, out mailExchangeHost, dns, offsetInDns, length))
                return null;

            return new DnsResourceDataMailExchange(preference, mailExchangeHost);
        }
    }
}