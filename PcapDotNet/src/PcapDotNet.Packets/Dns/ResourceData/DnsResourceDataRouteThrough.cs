namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1183.
    /// <pre>
    /// +-----+-------------------+
    /// | bit | 0-15              |
    /// +-----+-------------------+
    /// | 0   | preference        |
    /// +-----+-------------------+
    /// | 16  | intermediate-host |
    /// | ... |                   |
    /// +-----+-------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.RouteThrough)]
    public sealed class DnsResourceDataRouteThrough : DnsResourceDataUShortDomainName
    {
        public DnsResourceDataRouteThrough(ushort preference, DnsDomainName intermediateHost)
            : base(preference, intermediateHost)
        {
        }

        /// <summary>
        /// Representing the preference of the route.
        /// Smaller numbers indicate more preferred routes.
        /// </summary>
        public ushort Preference { get { return Value; } }

        /// <summary>
        /// The domain name of a host which will serve as an intermediate in reaching the host specified by the owner.
        /// The DNS RRs associated with IntermediateHost are expected to include at least one A, X25, or ISDN record.
        /// </summary>
        public DnsDomainName IntermediateHost { get { return DomainName; } }

        internal DnsResourceDataRouteThrough()
            : this(0, DnsDomainName.Root)
        {
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            ushort preference;
            DnsDomainName intermediateHost;
            if (!TryRead(out preference, out intermediateHost, dns, offsetInDns, length))
                return null;

            return new DnsResourceDataRouteThrough(preference, intermediateHost);
        }
    }
}