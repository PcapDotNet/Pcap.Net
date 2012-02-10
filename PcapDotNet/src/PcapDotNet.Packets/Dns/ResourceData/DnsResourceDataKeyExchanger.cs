namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 2230.
    /// <pre>
    /// +-----+-------------------+
    /// | bit | 0-15              |
    /// +-----+-------------------+
    /// | 0   | PREFERENCE        |
    /// +-----+-------------------+
    /// | 16  | EXCHANGER         |
    /// | ... |                   |
    /// +-----+-------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.KeyExchanger)]
    public sealed class DnsResourceDataKeyExchanger : DnsResourceDataUShortDomainName
    {
        public DnsResourceDataKeyExchanger(ushort preference, DnsDomainName keyExchanger)
            : base(preference, keyExchanger)
        {
        }

        /// <summary>
        /// Specifies the preference given to this RR among other KX records at the same owner.
        /// Lower values are preferred.
        /// </summary>
        public ushort Preference { get { return Value; } }

        /// <summary>
        /// Specifies a host willing to act as a key exchange for the owner name.
        /// </summary>
        public DnsDomainName KeyExchangeHost { get { return DomainName; } }

        internal DnsResourceDataKeyExchanger()
            : this(0, DnsDomainName.Root)
        {
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            ushort preference;
            DnsDomainName keyExchangeHost;
            if (!TryRead(out preference, out keyExchangeHost, dns, offsetInDns, length))
                return null;

            return new DnsResourceDataKeyExchanger(preference, keyExchangeHost);
        }
    }
}