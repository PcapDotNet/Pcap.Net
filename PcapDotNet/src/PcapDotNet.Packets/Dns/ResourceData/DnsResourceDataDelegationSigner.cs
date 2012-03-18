using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 3658.
    /// <pre>
    /// 0 Or more of:
    /// +-----+---------+-----------+-------------+
    /// | bit | 0-15    | 16-23     | 24-31       |
    /// +-----+---------+-----------+-------------+
    /// | 0   | key tag | algorithm | Digest type |
    /// +-----+---------+-----------+-------------+
    /// | 32  | digest                            |
    /// | ... |                                   |
    /// +-----+-----------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.DelegationSigner)]
    [DnsTypeRegistration(Type = DnsType.Cds)]
    [DnsTypeRegistration(Type = DnsType.TrustAnchor)]
    [DnsTypeRegistration(Type = DnsType.DnsSecLookAsideValidation)]
    public sealed class DnsResourceDataDelegationSigner : DnsResourceDataSimple, IEquatable<DnsResourceDataDelegationSigner>
    {
        private static class Offset
        {
            public const int KeyTag = 0;
            public const int Algorithm = KeyTag + sizeof(ushort);
            public const int DigestType = Algorithm + sizeof(byte);
            public const int Digest = DigestType + sizeof(byte);
        }

        private const int ConstPartLength = Offset.Digest;

        /// <summary>
        /// Constructs an instance out of the key tag, algorithm, digest type and digest fields.
        /// </summary>
        /// <param name="keyTag">
        /// Lists the key tag of the DNSKEY RR referred to by the DS record.
        /// The Key Tag used by the DS RR is identical to the Key Tag used by RRSIG RRs.
        /// Calculated as specified in RFC 2535.
        /// </param>
        /// <param name="algorithm">Algorithm must be allowed to sign DNS data.</param>
        /// <param name="digestType">An identifier for the digest algorithm used.</param>
        /// <param name="digest">
        /// Calculated over the canonical name of the delegated domain name followed by the whole RDATA of the KEY record (all four fields).
        /// digest = hash(canonical FQDN on KEY RR | KEY_RR_rdata)
        /// KEY_RR_rdata = Flags | Protocol | Algorithm | Public Key
        /// The size of the digest may vary depending on the digest type.
        /// </param>
        public DnsResourceDataDelegationSigner(ushort keyTag, DnsAlgorithm algorithm, DnsDigestType digestType, DataSegment digest)
        {
            if (digest == null)
                throw new ArgumentNullException("digest");

            KeyTag = keyTag;
            Algorithm = algorithm;
            DigestType = digestType;
            int maxDigestLength;
            switch (DigestType)
            {
                case DnsDigestType.Sha1:
                    maxDigestLength = 20;
                    break;

                case DnsDigestType.Sha256:
                    maxDigestLength = 32;
                    break;

                default:
                    maxDigestLength = int.MaxValue;
                    break;
            }
            Digest = digest.Subsegment(0, Math.Min(digest.Length, maxDigestLength));
            ExtraDigest = digest.Subsegment(Digest.Length, digest.Length - Digest.Length);
        }

        /// <summary>
        /// Lists the key tag of the DNSKEY RR referred to by the DS record.
        /// The Key Tag used by the DS RR is identical to the Key Tag used by RRSIG RRs.
        /// Calculated as specified in RFC 2535.
        /// </summary>
        public ushort KeyTag { get; private set; }

        /// <summary>
        /// Algorithm must be allowed to sign DNS data.
        /// </summary>
        public DnsAlgorithm Algorithm { get; private set; }

        /// <summary>
        /// An identifier for the digest algorithm used.
        /// </summary>
        public DnsDigestType DigestType { get; private set; }

        /// <summary>
        /// Calculated over the canonical name of the delegated domain name followed by the whole RDATA of the KEY record (all four fields).
        /// digest = hash(canonical FQDN on KEY RR | KEY_RR_rdata)
        /// KEY_RR_rdata = Flags | Protocol | Algorithm | Public Key
        /// The size of the digest may vary depending on the digest type.
        /// </summary>
        public DataSegment Digest { get; private set; }

        /// <summary>
        /// The extra digest bytes after number of bytes according to the digest type.
        /// </summary>
        public DataSegment ExtraDigest { get; private set; }

        /// <summary>
        /// Two DnsResourceDataDelegationSigner are equal iff their key tag, algorithm, digest type and digest fields are equal.
        /// </summary>
        public bool Equals(DnsResourceDataDelegationSigner other)
        {
            return other != null &&
                   KeyTag.Equals(other.KeyTag) &&
                   Algorithm.Equals(other.Algorithm) &&
                   DigestType.Equals(other.DigestType) &&
                   Digest.Equals(other.Digest);
        }

        /// <summary>
        /// Two DnsResourceDataDelegationSigner are equal iff their key tag, algorithm, digest type and digest fields are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataDelegationSigner);
        }

        /// <summary>
        /// A hash code of the combination of the key tag, algorithm, digest type and digest fields.
        /// </summary>
        public override int GetHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge(KeyTag, (byte)Algorithm, (byte)DigestType), Digest);
        }

        internal DnsResourceDataDelegationSigner()
            : this(0, DnsAlgorithm.None, DnsDigestType.Sha1, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstPartLength + Digest.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.KeyTag, KeyTag, Endianity.Big);
            buffer.Write(offset + Offset.Algorithm, (byte)Algorithm);
            buffer.Write(offset + Offset.DigestType, (byte)DigestType);
            Digest.Write(buffer, offset + Offset.Digest);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            ushort keyTag = data.ReadUShort(Offset.KeyTag, Endianity.Big);
            DnsAlgorithm algorithm = (DnsAlgorithm)data[Offset.Algorithm];
            DnsDigestType digestType = (DnsDigestType)data[Offset.DigestType];
            DataSegment digest = data.Subsegment(Offset.Digest, data.Length - ConstPartLength);

            return new DnsResourceDataDelegationSigner(keyTag, algorithm, digestType, digest);
        }
    }
}