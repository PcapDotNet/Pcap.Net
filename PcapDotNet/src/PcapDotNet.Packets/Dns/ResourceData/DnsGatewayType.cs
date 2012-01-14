namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// Indicates the format of the information that is stored in the gateway field.
    /// </summary>
    public enum DnsGatewayType : byte
    {
        /// <summary>
        /// No gateway is present.
        /// </summary>
        None = 0,

        /// <summary>
        /// A 4-byte IPv4 address is present.
        /// </summary>
        IpV4 = 1,

        /// <summary>
        /// A 16-byte IPv6 address is present.
        /// </summary>
        IpV6 = 2,
   
        /// <summary>
        /// A wire-encoded domain name is present.
        /// The wire-encoded format is self-describing, so the length is implicit.
        /// The domain name must not be compressed.
        /// </summary>
        DomainName = 3,
    }
}