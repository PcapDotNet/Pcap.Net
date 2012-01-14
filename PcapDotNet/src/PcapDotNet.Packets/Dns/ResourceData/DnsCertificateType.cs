namespace PcapDotNet.Packets.Dns
{
    public enum DnsCertificateType : ushort
    {
        /// <summary>
        /// RFC 4398.
        /// Indicates an X.509 certificate conforming to the profile defined by the IETF PKIX working group. 
        /// The certificate section will start with a one-octet unsigned OID length and then an X.500 OID indicating the nature of the remainder of the certificate section.
        /// Note: X.509 certificates do not include their X.500 directory-type-designating OID as a prefix.
        /// </summary>
        Pkix = 1,

        /// <summary>
        /// RFC 4398.
        /// SPKI certificate.
        /// </summary>
        Spki = 2,

        /// <summary>
        /// RFC 4398.
        /// Indicates an OpenPGP packet.
        /// This is used to transfer public key material and revocation signatures.
        /// The data is binary and must not be encoded into an ASCII armor.
        /// An implementation should process transferable public keys, but it may handle additional OpenPGP packets.
        /// </summary>
        Pgp = 3,

        /// <summary>
        /// RFC 4398.
        /// The URL of an X.509 data object.
        /// Must be used when the content is too large to fit in the CERT RR and may be used at the implementer's discretion.
        /// Should not be used where the DNS message is 512 octets or smaller and could thus be expected to fit a UDP packet.
        /// </summary>
        IPkix = 4,

        /// <summary>
        /// RFC 4398.
        /// The URL of an SPKI certificate.
        /// Must be used when the content is too large to fit in the CERT RR and may be used at the implementer's discretion.
        /// Should not be used where the DNS message is 512 octets or smaller and could thus be expected to fit a UDP packet.
        /// </summary>
        ISpki = 5,

        /// <summary>
        /// RFC 4398.
        /// Contains both an OpenPGP fingerprint for the key in question, as well as a URL.
        /// The certificate portion of the IPgp CERT RR is defined as a one-octet fingerprint length, followed by the OpenPGP fingerprint, followed by the URL.
        /// The OpenPGP fingerprint is calculated as defined in RFC 2440.
        /// A zero-length fingerprint or a zero-length URL are legal, and indicate URL-only IPGP data or fingerprint-only IPGP data, respectively.
        /// A zero-length fingerprint and a zero-length URL are meaningless and invalid.
        /// Must be used when the content is too large to fit in the CERT RR and may be used at the implementer's discretion.
        /// Should not be used where the DNS message is 512 octets or smaller and could thus be expected to fit a UDP packet.
        /// </summary>
        Ipgp = 6,

        /// <summary>
        /// RFC 4398.
        /// Attribute Certificate.
        /// </summary>
        AcPkix = 7,

        /// <summary>
        /// RFC 4398.
        /// The URL of an Attribute Certificate.
        /// Must be used when the content is too large to fit in the CERT RR and may be used at the implementer's discretion.
        /// Should not be used where the DNS message is 512 octets or smaller and could thus be expected to fit a UDP packet.
        /// </summary>
        IAcPkix = 8,

        /// <summary>
        /// RFC 4398.
        /// Indicates a certificate format defined by an absolute URI.
        /// The certificate portion of the CERT RR must begin with a null-terminated URI, and the data after the null is the private format certificate itself.
        /// The URI should be such that a retrieval from it will lead to documentation on the format of the certificate.
        /// Recognition of private certificate types need not be based on URI equality but can use various forms of pattern matching so that, for example, subtype or version information can also be encoded into the URI.
        /// </summary>
        Uri = 253,

        /// <summary>
        /// RFC 4398.
        /// Indicates a private format certificate specified by an ISO OID prefix.
        /// The certificate section will start with a one-octet unsigned OID length and then a BER-encoded OID indicating the nature of the remainder of the certificate section.
        /// This can be an X.509 certificate format or some other format.
        /// X.509 certificates that conform to the IETF PKIX profile should be indicated by the PKIX type, not the OID private type.
        /// Recognition of private certificate types need not be based on OID equality but can use various forms of pattern matching such as OID prefix.
        /// </summary>
        Oid = 254,
    }
}