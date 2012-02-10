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
        public DnsResourceDataNextDomainSecure3Parameters(DnsSecNSec3HashAlgorithm hashAlgorithm, DnsSecNSec3Flags flags, ushort iterations, DataSegment salt)
            : base(hashAlgorithm, flags, iterations, salt)
        {
        }

        public bool Equals(DnsResourceDataNextDomainSecure3Parameters other)
        {
            return EqualsParameters(other);
        }

        public override bool Equals(object other)
        {
            return Equals(other as DnsResourceDataNextDomainSecure3Parameters);
        }

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