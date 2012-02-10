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
    [DnsTypeRegistration(Type = DnsType.AfsDatabase)]
    public sealed class DnsResourceDataAfsDatabase : DnsResourceDataUShortDomainName
    {
        public DnsResourceDataAfsDatabase(ushort subtype, DnsDomainName hostName)
            : base(subtype, hostName)
        {
        }

        public ushort Subtype { get { return Value; } }

        public DnsDomainName HostName { get { return DomainName; } }

        internal DnsResourceDataAfsDatabase()
            : this(0, DnsDomainName.Root)
        {
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            ushort subtype;
            DnsDomainName hostName;
            if (!TryRead(out subtype, out hostName, dns, offsetInDns, length))
                return null;

            return new DnsResourceDataAfsDatabase(subtype, hostName);
        }
    }
}