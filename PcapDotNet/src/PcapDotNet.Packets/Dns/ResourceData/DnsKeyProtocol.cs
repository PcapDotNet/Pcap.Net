namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 2535.
    /// </summary>
    public enum DnsKeyProtocol : byte
    {
        None = 0,

        /// <summary>
        /// Connection with TLS.
        /// </summary>
        Tls = 1,

        /// <summary>
        /// Connection with email.
        /// </summary>
        Email = 2,

        /// <summary>
        /// DNS security.
        /// The protocol field should be set to this value for zone keys and other keys used in DNS security.
        /// Implementations that can determine that a key is a DNS security key by the fact that flags label it a zone key or the signatory flag field is non-zero are not required to check the protocol field.
        /// </summary>
        DnsSec = 3,

        /// <summary>
        /// Oakley/IPSEC [RFC 2401] protocol.
        /// Indicates that this key is valid for use in conjunction with that security standard.
        /// This key could be used in connection with secured communication on behalf of an end entity or user whose name is the owner name of the KEY RR if the entity or user flag bits are set.
        /// The presence of a KEY resource with this protocol value is an assertion that the host speaks Oakley/IPSEC.
        /// </summary>
        IpSec = 4,

        /// <summary>
        /// The key can be used in connection with any protocol for which KEY RR protocol octet values have been defined.
        /// The use of this value is discouraged and the use of different keys for different protocols is encouraged.
        /// </summary>
        All = 255,
    }
}