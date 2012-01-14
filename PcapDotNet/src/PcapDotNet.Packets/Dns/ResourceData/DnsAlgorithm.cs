namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFCs 2535, 2536, 2537, 2539, 3110, 3755, 4034, 5155, 5702, 5933.
    /// The key algorithm.
    /// </summary>
    public enum DnsAlgorithm : byte
    {
        /// <summary>
        /// RFC 4034.
        /// Field is not used or indicates that the algorithm is unknown to a secure DNS, 
        /// which may simply be the result of the algorithm not having been standardized for DNSSEC.
        /// </summary>
        None = 0,

        /// <summary>
        /// RFCs 2537, 4034.
        /// RSA/MD5.
        /// Deprecated.
        /// </summary>
        RsaMd5 = 1,

        /// <summary>
        /// RFC 2539.
        /// Diffie-Hellman.
        /// Implementation is optional, key only.
        /// </summary>
        DiffieHellman = 2,

        /// <summary>
        /// RFCs 2536, 3755.
        /// DSA - Digital Signature Algorithm.
        /// Implementation is mandatory.
        /// </summary>
        Dsa = 3,

        /// <summary>
        /// Reserved for elliptic curve crypto.
        /// </summary>
        Ecc = 4,

        /// <summary>
        /// RFCs 3110, 3755.
        /// RSA/SHA-1.
        /// </summary>
        RsaSha1 = 5,

        /// <summary>
        /// RFC 5155.
        /// DSA-NSEC3-SHA1.
        /// </summary>
        DsaNsec3Sha1 = 6,

        /// <summary>
        /// RFC 5155.
        /// RSASHA1-NSEC3-SHA1.
        /// </summary>
        RsaSha1Nsec3Sha1 = 7,

        /// <summary>
        /// RFC 5702.
        /// RSA/SHA-256.
        /// </summary>
        RsaSha256 = 8,

        /// <summary>
        /// RFC 5702.
        /// RSA/SHA-512.
        /// </summary>
        RsaSha512 = 10,

        /// <summary>
        /// RFC 5933.
        /// GOST R 34.10-2001.
        /// </summary>
        EccGost = 12,

        /// <summary>
        /// RFC 4034.
        /// Reserved for Indirect Keys.
        /// </summary>
        Indirect = 252,

        /// <summary>
        /// RFCs 2535, 3755.
        /// Private algorithms - domain name.
        /// </summary>
        PrivateDns = 253,

        /// <summary>
        /// RFCs 2535, 3755.
        /// Private algorithms - OID.
        /// </summary>
        PrivateOid = 254,
    }
}