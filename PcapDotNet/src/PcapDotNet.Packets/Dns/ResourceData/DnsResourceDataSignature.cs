using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFCs 2535, 4034.
    /// <pre>
    /// +-----+--------------+-----------+--------+
    /// | bit | 0-15         | 16-23     | 24-31  |
    /// +-----+--------------+-----------+--------+
    /// | 0   | type covered | algorithm | labels |
    /// +-----+--------------+-----------+--------+
    /// | 32  | original TTL                      |
    /// +-----+-----------------------------------+
    /// | 64  | signature expiration              |
    /// +-----+-----------------------------------+
    /// | 96  | signature inception               |
    /// +-----+--------------+--------------------+
    /// | 128 | key tag      |                    |
    /// +-----+--------------+ signer's name      |
    /// | ... |                                   |
    /// +-----+-----------------------------------+
    /// |     | signature                         |
    /// | ... |                                   |
    /// +-----+-----------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Signature)]
    [DnsTypeRegistration(Type = DnsType.ResourceRecordSignature)]
    public sealed class DnsResourceDataSignature : DnsResourceData, IEquatable<DnsResourceDataSignature>
    {
        private static class Offset
        {
            public const int TypeCovered = 0;
            public const int Algorithm = TypeCovered + sizeof(ushort);
            public const int Labels = Algorithm + sizeof(byte);
            public const int OriginalTtl = Labels + sizeof(byte);
            public const int SignatureExpiration = OriginalTtl + sizeof(uint);
            public const int SignatureInception = SignatureExpiration + SerialNumber32.SizeOf;
            public const int KeyTag = SignatureInception + SerialNumber32.SizeOf;
            public const int SignersName = KeyTag + sizeof(ushort);
        }

        private const int ConstantPartLength = Offset.SignersName;

        public DnsResourceDataSignature(DnsType typeCovered, DnsAlgorithm algorithm, byte labels, uint originalTtl, SerialNumber32 signatureExpiration,
                                        SerialNumber32 signatureInception, ushort keyTag, DnsDomainName signersName, DataSegment signature)
        {
            TypeCovered = typeCovered;
            Algorithm = algorithm;
            Labels = labels;
            OriginalTtl = originalTtl;
            SignatureExpiration = signatureExpiration;
            SignatureInception = signatureInception;
            KeyTag = keyTag;
            SignersName = signersName;
            Signature = signature;
        }

        /// <summary>
        /// The type of the other RRs covered by this SIG.
        /// </summary>
        public DnsType TypeCovered { get; private set; }

        /// <summary>
        /// The key algorithm.
        /// </summary>
        public DnsAlgorithm Algorithm { get; private set; }

        /// <summary>
        /// An unsigned count of how many labels there are in the original SIG RR owner name not counting the null label for root and not counting any initial "*" for a wildcard.  
        /// If a secured retrieval is the result of wild card substitution, it is necessary for the resolver to use the original form of the name in verifying the digital signature.
        /// This field makes it easy to determine the original form.
        /// 
        /// If, on retrieval, the RR appears to have a longer name than indicated by "labels", the resolver can tell it is the result of wildcard substitution.
        /// If the RR owner name appears to be shorter than the labels count, the SIG RR must be considered corrupt and ignored.
        /// The maximum number of labels allowed in the current DNS is 127 but the entire octet is reserved and would be required should DNS names ever be expanded to 255 labels.
        /// </summary>
        public byte Labels { get; private set; }

        /// <summary>
        /// The "original TTL" field is included in the RDATA portion to avoid
        /// (1) authentication problems that caching servers would otherwise cause by decrementing the real TTL field and
        /// (2) security problems that unscrupulous servers could otherwise cause by manipulating the real TTL field.
        /// This original TTL is protected by the signature while the current TTL field is not.
        /// 
        /// NOTE:  The "original TTL" must be restored into the covered RRs when the signature is verified.
        /// This generaly implies that all RRs for a particular type, name, and class, that is, all the RRs in any particular RRset, must have the same TTL to start with.
        ///  </summary>
        public uint OriginalTtl { get; private set; }

        /// <summary>
        /// The last time the signature is valid.
        /// Numbers of seconds since the start of 1 January 1970, GMT, ignoring leap seconds.
        /// Ring arithmetic is used.
        /// This time can never be more than about 68 years after the inception.
        /// </summary>
        public SerialNumber32 SignatureExpiration { get; private set; }

        /// <summary>
        /// The first time the signature is valid.
        /// Numbers of seconds since the start of 1 January 1970, GMT, ignoring leap seconds.
        /// Ring arithmetic is used.
        /// This time can never be more than about 68 years before the expiration.
        /// </summary>
        public SerialNumber32 SignatureInception { get; private set; }

        /// <summary>
        /// Used to efficiently select between multiple keys which may be applicable and thus check that a public key about to be used for the computationally expensive effort to check the signature is possibly valid.  
        /// For algorithm 1 (MD5/RSA) as defined in RFC 2537, it is the next to the bottom two octets of the public key modulus needed to decode the signature field.
        /// That is to say, the most significant 16 of the least significant 24 bits of the modulus in network (big endian) order. 
        /// For all other algorithms, including private algorithms, it is calculated as a simple checksum of the KEY RR.
        /// </summary>
        public ushort KeyTag { get; private set; }

        /// <summary>
        /// The domain name of the signer generating the SIG RR.
        /// This is the owner name of the public KEY RR that can be used to verify the signature.  
        /// It is frequently the zone which contained the RRset being authenticated.
        /// Which signers should be authorized to sign what is a significant resolver policy question.
        /// The signer's name may be compressed with standard DNS name compression when being transmitted over the network.
        /// </summary>
        public DnsDomainName SignersName { get; private set; }

        /// <summary>
        /// The actual signature portion of the SIG RR binds the other RDATA fields to the RRset of the "type covered" RRs with that owner name and class.
        /// This covered RRset is thereby authenticated. 
        /// To accomplish this, a data sequence is constructed as follows: 
        /// 
        /// data = RDATA | RR(s)...
        /// 
        /// where "|" is concatenation,
        /// 
        /// RDATA is the wire format of all the RDATA fields in the SIG RR itself (including the canonical form of the signer's name) before but not including the signature,
        /// and RR(s) is the RRset of the RR(s) of the type covered with the same owner name and class as the SIG RR in canonical form and order.
        /// 
        /// How this data sequence is processed into the signature is algorithm dependent.
        /// </summary>
        public DataSegment Signature { get; private set; }

        public bool Equals(DnsResourceDataSignature other)
        {
            return other != null &&
                   TypeCovered.Equals(other.TypeCovered) &&
                   Algorithm.Equals(other.Algorithm) &&
                   Labels.Equals(other.Labels) &&
                   OriginalTtl.Equals(other.OriginalTtl) &&
                   SignatureExpiration.Equals(other.SignatureExpiration) &&
                   SignatureInception.Equals(other.SignatureInception) &&
                   KeyTag.Equals(other.KeyTag) &&
                   SignersName.Equals(other.SignersName) &&
                   Signature.Equals(other.Signature);
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge((ushort)TypeCovered, (byte)Algorithm, Labels), OriginalTtl, SignatureExpiration, SignatureInception,
                                        KeyTag, SignersName, Signature);
        }

        public override bool Equals(object other)
        {
            return Equals(other as DnsResourceDataSignature);
        }

        internal DnsResourceDataSignature()
            : this(DnsType.A, DnsAlgorithm.None, 0, 0, 0, 0, 0, DnsDomainName.Root, DataSegment.Empty)
        {
        }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return ConstantPartLength + SignersName.GetLength(compressionData, offsetInDns) + Signature.Length;
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            buffer.Write(dnsOffset + offsetInDns + Offset.TypeCovered, (ushort)TypeCovered, Endianity.Big);
            buffer.Write(dnsOffset + offsetInDns + Offset.Algorithm, (byte)Algorithm);
            buffer.Write(dnsOffset + offsetInDns + Offset.Labels, Labels);
            buffer.Write(dnsOffset + offsetInDns + Offset.OriginalTtl, OriginalTtl, Endianity.Big);
            buffer.Write(dnsOffset + offsetInDns + Offset.SignatureExpiration, SignatureExpiration.Value, Endianity.Big);
            buffer.Write(dnsOffset + offsetInDns + Offset.SignatureInception, SignatureInception.Value, Endianity.Big);
            buffer.Write(dnsOffset + offsetInDns + Offset.KeyTag, KeyTag, Endianity.Big);

            int numBytesWritten = ConstantPartLength;
            numBytesWritten += SignersName.Write(buffer, dnsOffset, compressionData, offsetInDns + numBytesWritten);

            Signature.Write(buffer, dnsOffset + offsetInDns + numBytesWritten);
            return numBytesWritten + Signature.Length;
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstantPartLength)
                return null;

            DnsType typeCovered = (DnsType)dns.ReadUShort(offsetInDns + Offset.TypeCovered, Endianity.Big);
            DnsAlgorithm algorithm = (DnsAlgorithm)dns[offsetInDns + Offset.Algorithm];
            byte labels = dns[offsetInDns + Offset.Labels];
            uint originalTtl = dns.ReadUInt(offsetInDns + Offset.OriginalTtl, Endianity.Big);
            uint signatureExpiration = dns.ReadUInt(offsetInDns + Offset.SignatureExpiration, Endianity.Big);
            uint signatureInception = dns.ReadUInt(offsetInDns + Offset.SignatureInception, Endianity.Big);
            ushort keyTag = dns.ReadUShort(offsetInDns + Offset.KeyTag, Endianity.Big);

            offsetInDns += ConstantPartLength;
            length -= ConstantPartLength;

            DnsDomainName signersName;
            int signersNameLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out signersName, out signersNameLength))
                return null;
            offsetInDns += signersNameLength;
            length -= signersNameLength;

            DataSegment signature = dns.Subsegment(offsetInDns, length);

            return new DnsResourceDataSignature(typeCovered, algorithm, labels, originalTtl, signatureExpiration, signatureInception, keyTag, signersName, signature);
        }
    }
}