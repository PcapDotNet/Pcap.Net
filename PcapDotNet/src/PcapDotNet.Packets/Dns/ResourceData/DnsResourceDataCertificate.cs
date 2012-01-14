using System;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 4398.
    /// <pre>
    /// +-----+-----------+------+------------+
    /// | bit | 0-7       | 8-15 | 16-31      |
    /// +-----+-----------+------+------------+
    /// | 0   | type             | key tag    |
    /// +-----+-----------+------+------------+
    /// | 32  | algorithm | certificate or CRL|
    /// +-----+-----------+                   |
    /// |     |                               |
    /// | ... |                               |
    /// +-----+-------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Cert)]
    public sealed class DnsResourceDataCertificate : DnsResourceDataSimple, IEquatable<DnsResourceDataCertificate>
    {
        private static class Offset
        {
            public const int Type = 0;
            public const int KeyTag = Type + sizeof(ushort);
            public const int Algorithm = KeyTag + sizeof(ushort);
            public const int Certificate = Algorithm + sizeof(byte);
        }

        public const int ConstantPartLength = Offset.Certificate;

        public DnsResourceDataCertificate(DnsCertificateType certificateType, ushort keyTag, DnsAlgorithm algorithm, DataSegment certificate)
        {
            CertificateType = certificateType;
            KeyTag = keyTag;
            Algorithm = algorithm;
            Certificate = certificate;
        }

        /// <summary>
        /// The certificate type.
        /// </summary>
        public DnsCertificateType CertificateType { get; private set; }

        /// <summary>
        /// Value computed for the key embedded in the certificate, using the RRSIG Key Tag algorithm.
        /// This field is used as an efficiency measure to pick which CERT RRs may be applicable to a particular key.
        /// The key tag can be calculated for the key in question, and then only CERT RRs with the same key tag need to be examined.
        /// Note that two different keys can have the same key tag.
        /// However, the key must be transformed to the format it would have as the public key portion of a DNSKEY RR before the key tag is computed.
        /// This is only possible if the key is applicable to an algorithm and complies to limits (such as key size) defined for DNS security.
        /// If it is not, the algorithm field must be zero and the tag field is meaningless and should be zero.
        /// </summary>
        public ushort KeyTag { get; private set; }

        /// <summary>
        /// Has the same meaning as the algorithm field in DNSKEY and RRSIG RRs,
        /// except that a zero algorithm field indicates that the algorithm is unknown to a secure DNS, 
        /// which may simply be the result of the algorithm not having been standardized for DNSSEC.
        /// </summary>
        public DnsAlgorithm Algorithm { get; private set; }

        /// <summary>
        /// The certificate data according to the type.
        /// </summary>
        public DataSegment Certificate { get; private set; }

        public bool Equals(DnsResourceDataCertificate other)
        {
            return other != null &&
                   CertificateType.Equals(other.CertificateType) &&
                   KeyTag.Equals(other.KeyTag) &&
                   Algorithm.Equals(other.Algorithm) &&
                   Certificate.Equals(other.Certificate);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataCertificate);
        }

        internal DnsResourceDataCertificate()
            : this(DnsCertificateType.Pkix, 0, DnsAlgorithm.None, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + Certificate.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Type, (ushort)CertificateType, Endianity.Big);
            buffer.Write(offset + Offset.KeyTag, KeyTag, Endianity.Big);
            buffer.Write(offset + Offset.Algorithm, (byte)Algorithm);
            Certificate.Write(buffer, offset + Offset.Certificate);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            DnsCertificateType type = (DnsCertificateType)data.ReadUShort(Offset.Type, Endianity.Big);
            ushort keyTag = data.ReadUShort(Offset.KeyTag, Endianity.Big);
            DnsAlgorithm algorithm = (DnsAlgorithm)data[Offset.Algorithm];
            DataSegment certificate = data.SubSegment(Offset.Certificate, data.Length - ConstantPartLength);

            return new DnsResourceDataCertificate(type, keyTag, algorithm, certificate);
        }
    }
}