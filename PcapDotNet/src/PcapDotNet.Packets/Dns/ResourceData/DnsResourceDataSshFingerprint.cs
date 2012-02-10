using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 4255.
    /// <pre>
    /// +-----+-----------+-----------+
    /// | bit | 0-7       | 8-15      |
    /// +-----+-----------+-----------+
    /// | 0   | algorithm | fp type   |
    /// +-----+-----------+-----------+
    /// | 16  | fingerprint           |
    /// | ... |                       |
    /// +-----+-----------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.SshFingerprint)]
    public sealed class DnsResourceDataSshFingerprint : DnsResourceDataSimple, IEquatable<DnsResourceDataSshFingerprint>
    {
        private static class Offset
        {
            public const int Algorithm = 0;
            public const int FingerprintType = Algorithm + sizeof(byte);
            public const int Fingerprint = FingerprintType + sizeof(byte);
        }

        public const int ConstPartLength = Offset.Fingerprint;

        public DnsResourceDataSshFingerprint(DnsFingerprintPublicKeyAlgorithm algorithm, DnsFingerprintType fingerprintType, DataSegment fingerprint)
        {
            Algorithm = algorithm;
            FingerprintType = fingerprintType;
            Fingerprint = fingerprint;
        }

        /// <summary>
        /// Describes the algorithm of the public key.
        /// </summary>
        public DnsFingerprintPublicKeyAlgorithm Algorithm { get; private set; }

        /// <summary>
        /// Describes the message-digest algorithm used to calculate the fingerprint of the public key.
        /// </summary>
        public DnsFingerprintType FingerprintType { get; private set; }

        /// <summary>
        /// The fingerprint is calculated over the public key blob.
        /// The message-digest algorithm is presumed to produce an opaque octet string output, which is placed as-is in the RDATA fingerprint field.
        /// </summary>
        public DataSegment Fingerprint { get; private set; }

        public bool Equals(DnsResourceDataSshFingerprint other)
        {
            return other != null &&
                   Algorithm.Equals(other.Algorithm) &&
                   FingerprintType.Equals(other.FingerprintType) &&
                   Fingerprint.Equals(other.Fingerprint);
        }

        public override bool Equals(object other)
        {
            return Equals(other as DnsResourceDataSshFingerprint);
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge((byte)Algorithm, (byte)FingerprintType), Fingerprint);
        }

        internal DnsResourceDataSshFingerprint()
            : this(DnsFingerprintPublicKeyAlgorithm.Rsa, DnsFingerprintType.Sha1, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstPartLength + Fingerprint.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Algorithm, (byte)Algorithm);
            buffer.Write(offset + Offset.FingerprintType, (byte)FingerprintType);
            Fingerprint.Write(buffer, offset + Offset.Fingerprint);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            DnsFingerprintPublicKeyAlgorithm algorithm = (DnsFingerprintPublicKeyAlgorithm)data[Offset.Algorithm];
            DnsFingerprintType fingerprintType = (DnsFingerprintType)data[Offset.FingerprintType];
            DataSegment fingerprint = data.Subsegment(Offset.Fingerprint, data.Length - ConstPartLength);

            return new DnsResourceDataSshFingerprint(algorithm, fingerprintType, fingerprint);
        }
    }
}