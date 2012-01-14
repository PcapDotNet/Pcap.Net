namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// Identifies the public key's cryptographic algorithm and determines the format of the public key field.
    /// </summary>
    public enum DnsPublicKeyAlgorithm : byte
    {
        /// <summary>
        /// Indicates that no key is present.
        /// </summary>
        None = 0,

        /// <summary>
        /// A DSA key is present, in the format defined in RFC 2536.
        /// </summary>
        Dsa = 1,

        /// <summary>
        /// A RSA key is present, in the format defined in RFC 3110 with the following changes:
        /// The earlier definition of RSA/MD5 in RFC 2065 limited the exponent and modulus to 2552 bits in length.
        /// RFC 3110 extended that limit to 4096 bits for RSA/SHA1 keys
        /// The IPSECKEY RR imposes no length limit on RSA public keys, other than the 65535 octet limit imposed by the two-octet length encoding.
        /// This length extension is applicable only to IPSECKEY; it is not applicable to KEY RRs. 
        /// </summary>
        Rsa = 2,
    }
}