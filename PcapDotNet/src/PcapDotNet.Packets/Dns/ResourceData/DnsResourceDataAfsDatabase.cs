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
        /// <summary>
        /// Constructs an AFS database resource data from the given subtype and host name.
        /// </summary>
        /// <param name="subtype">The subtype of the resource data.</param>
        /// <param name="hostName">A host that has a server for the cell named by the owner name of the RR.</param>
        public DnsResourceDataAfsDatabase(DnsAfsDatabaseSubtype subtype, DnsDomainName hostName)
            : base((ushort)subtype, hostName)
        {
        }

        /// <summary>
        /// The subtype of the resource data.
        /// </summary>
        public DnsAfsDatabaseSubtype Subtype { get { return (DnsAfsDatabaseSubtype)Value; } }

        /// <summary>
        /// A host that has a server for the cell named by the owner name of the RR.
        /// </summary>
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

            return new DnsResourceDataAfsDatabase((DnsAfsDatabaseSubtype)subtype, hostName);
        }
    }
}