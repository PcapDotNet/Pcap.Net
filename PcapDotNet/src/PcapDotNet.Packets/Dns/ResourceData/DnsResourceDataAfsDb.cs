namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1183.
    /// <pre>
    /// +-----+----------+
    /// | bit | 0-15     |
    /// +-----+----------+
    /// | 0   | subtype  |
    /// +-----+----------+
    /// | 16  | hostname |
    /// | ... |          |
    /// +-----+----------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.AfsDb)]
    public sealed class DnsResourceDataAfsDb : DnsResourceDataUShortDomainName
    {
        public DnsResourceDataAfsDb(ushort subType, DnsDomainName hostname)
            : base(subType, hostname)
        {
        }

        public ushort SubType { get { return Value; } }

        public DnsDomainName Hostname { get { return DomainName; } }

        internal DnsResourceDataAfsDb()
            : this(0, DnsDomainName.Root)
        {
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            ushort subType;
            DnsDomainName hostName;
            if (!TryRead(out subType, out hostName, dns, offsetInDns, length))
                return null;

            return new DnsResourceDataAfsDb(subType, hostName);
        }
    }
}