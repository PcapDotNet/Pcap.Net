using System;
using System.Globalization;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 5155.
    /// <pre>
    /// +-----+-------------+----------+--------+------------+
    /// | bit | 0-7         | 8-14     | 15     | 16-31      |
    /// +-----+-------------+----------+--------+------------+
    /// | 0   | Hash Alg    | Reserved | OptOut | Iterations |
    /// +-----+-------------+----------+--------+------------+
    /// | 32  | Salt Length | Salt                           |
    /// +-----+-------------+                                |
    /// | ... |                                              |
    /// +-----+----------------------------------------------+
    /// | ... | ...                                          |
    /// +-----+----------------------------------------------+
    /// </pre>
    /// </summary>
    public abstract class DnsResourceDataNextDomainSecure3Base : DnsResourceDataSimple
    {
        private static class Offset
        {
            public const int HashAlgorithm = 0;
            public const int Flags = HashAlgorithm + sizeof(byte);
            public const int Iterations = Flags + sizeof(byte);
            public const int SaltLength = Iterations + sizeof(ushort);
            public const int Salt = SaltLength + sizeof(byte);
        }

        private const int ConstantPartLength = Offset.Salt;

        /// <summary>
        /// Identifies the cryptographic hash algorithm used to construct the hash-value.
        /// </summary>
        public DnsSecNSec3HashAlgorithm HashAlgorithm { get; private set; }

        /// <summary>
        /// Can be used to indicate different processing.
        /// All undefined flags must be zero.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
        public DnsSecNSec3Flags Flags { get; private set; }

        /// <summary>
        /// Defines the number of additional times the hash function has been performed.
        /// More iterations result in greater resiliency of the hash value against dictionary attacks, 
        /// but at a higher computational cost for both the server and resolver.
        /// </summary>
        public ushort Iterations { get; private set; }

        /// <summary>
        /// Appended to the original owner name before hashing in order to defend against pre-calculated dictionary attacks.
        /// </summary>
        public DataSegment Salt { get; private set; }

        internal bool EqualsParameters(DnsResourceDataNextDomainSecure3Base other)
        {
            return other != null &&
                   HashAlgorithm.Equals(other.HashAlgorithm) &&
                   Flags.Equals(other.Flags) &&
                   Iterations.Equals(other.Iterations) &&
                   Salt.Equals(other.Salt);
        }

        internal int GetHashCodeParameters()
        {
            return Sequence.GetHashCode(BitSequence.Merge(Iterations, (byte)HashAlgorithm, (byte)Flags), Salt);
        }

        internal DnsResourceDataNextDomainSecure3Base(DnsSecNSec3HashAlgorithm hashAlgorithm, DnsSecNSec3Flags flags, ushort iterations, DataSegment salt)
        {
            if (salt.Length > byte.MaxValue)
                throw new ArgumentOutOfRangeException("salt", salt.Length, string.Format(CultureInfo.InvariantCulture, "Cannot bigger than {0}.", byte.MaxValue));

            HashAlgorithm = hashAlgorithm;
            Flags = flags;
            Iterations = iterations;
            Salt = salt;
        }

        internal int ParametersLength { get { return GetParametersLength(Salt.Length); } }

        internal static int GetParametersLength(int saltLength)
        {
            return ConstantPartLength + saltLength;
        }

        internal void WriteParameters(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.HashAlgorithm, (byte)HashAlgorithm);
            buffer.Write(offset + Offset.Flags, (byte)Flags);
            buffer.Write(offset + Offset.Iterations, Iterations, Endianity.Big);
            buffer.Write(offset + Offset.SaltLength, (byte)Salt.Length);
            Salt.Write(buffer, offset + Offset.Salt);
        }

        internal static bool TryReadParameters(DataSegment data, out DnsSecNSec3HashAlgorithm hashAlgorithm, out DnsSecNSec3Flags flags, out ushort iterations, out DataSegment salt)
        {
            if (data.Length < ConstantPartLength)
            {
                hashAlgorithm = DnsSecNSec3HashAlgorithm.Sha1;
                flags = DnsSecNSec3Flags.None;
                iterations = 0;
                salt = null;
                return false;
            }

            hashAlgorithm = (DnsSecNSec3HashAlgorithm)data[Offset.HashAlgorithm];
            flags = (DnsSecNSec3Flags)data[Offset.Flags];
            iterations = data.ReadUShort(Offset.Iterations, Endianity.Big);
            
            int saltLength = data[Offset.SaltLength];
            if (data.Length - Offset.Salt < saltLength)
            {
                salt = null;
                return false;
            }
            salt = data.Subsegment(Offset.Salt, saltLength);
            return true;
        }
    }
}