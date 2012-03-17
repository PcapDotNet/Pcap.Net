using System;

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
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.NSec3Parameters)]
    public sealed class DnsResourceDataNextDomainSecure3Parameters : DnsResourceDataNextDomainSecure3Base, IEquatable<DnsResourceDataNextDomainSecure3Parameters>
    {
        /// <summary>
        /// Constructs a next domain secure 3 parameters resource data from the hash algorithm, flags, iterations and salt fields.
        /// </summary>
        /// <param name="hashAlgorithm">Identifies the cryptographic hash algorithm used to construct the hash-value.</param>
        /// <param name="flags">Can be used to indicate different processing. All undefined flags must be zero.</param>
        /// <param name="iterations">
        /// Defines the number of additional times the hash function has been performed.
        /// More iterations result in greater resiliency of the hash value against dictionary attacks, 
        /// but at a higher computational cost for both the server and resolver.
        /// </param>
        /// <param name="salt">Appended to the original owner name before hashing in order to defend against pre-calculated dictionary attacks.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags")]
        public DnsResourceDataNextDomainSecure3Parameters(DnsSecNSec3HashAlgorithm hashAlgorithm, DnsSecNSec3Flags flags, ushort iterations, DataSegment salt)
            : base(hashAlgorithm, flags, iterations, salt)
        {
        }

        /// <summary>
        /// Two DnsResourceDataNextDomainSecure3Parameters are equal if they have the hash algorithm, flags, iterations and salt fields.
        /// </summary>
        public bool Equals(DnsResourceDataNextDomainSecure3Parameters other)
        {
            return EqualsParameters(other);
        }

        /// <summary>
        /// Two DnsResourceDataNextDomainSecure3Parameters are equal if they have the hash algorithm, flags, iterations and salt fields.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataNextDomainSecure3Parameters);
        }

        /// <summary>
        /// A hash code made up from the combination of the hash algorithm, flags, iterations and salt fields.
        /// </summary>
        public override int GetHashCode()
        {
            return GetHashCodeParameters();
        }

        internal DnsResourceDataNextDomainSecure3Parameters()
            : this(DnsSecNSec3HashAlgorithm.Sha1, DnsSecNSec3Flags.None, 0, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ParametersLength;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            WriteParameters(buffer, offset);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            DnsSecNSec3HashAlgorithm hashAlgorithm;
            DnsSecNSec3Flags flags;
            ushort iterations;
            DataSegment salt;
            if (!TryReadParameters(data, out hashAlgorithm, out flags, out iterations, out salt))
                return null;

            if (data.Length != GetParametersLength(salt.Length))
                return null;

            return new DnsResourceDataNextDomainSecure3Parameters(hashAlgorithm, flags, iterations, salt);
        }
    }
}