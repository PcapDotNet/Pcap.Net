using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 2930.
    /// <pre>
    /// +------+------------+------------+
    /// | bit  | 0-15       | 16-31      |
    /// +------+------------+------------+
    /// | 0    | Algorithm               |
    /// | ...  |                         |
    /// +------+-------------------------+
    /// | X    | Inception               |
    /// +------+-------------------------+
    /// | X+32 | Expiration              |
    /// +------+------------+------------+
    /// | X+64 | Mode       | Error      |
    /// +------+------------+------------+
    /// | X+96 | Key Size   |            |
    /// +------+------------+ Key Data   |
    /// | ...  |                         |
    /// +------+------------+------------+
    /// |      | Other Size |            |
    /// +------+------------+ Other Data |
    /// | ...  |                         |
    /// +------+-------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.TKey)]
    public sealed class DnsResourceDataTransactionKey : DnsResourceData, IEquatable<DnsResourceDataTransactionKey>
    {
        private static class OffsetAfterAlgorithm
        {
            public const int Inception = 0;
            public const int Expiration = Inception + SerialNumber32.SizeOf;
            public const int Mode = Expiration + SerialNumber32.SizeOf;
            public const int Error = Mode + sizeof(ushort);
            public const int KeySize = Error + sizeof(ushort);
            public const int KeyData = KeySize + sizeof(ushort);
        }

        private const int ConstantPartLength = OffsetAfterAlgorithm.KeyData + sizeof(ushort);

        public DnsResourceDataTransactionKey(DnsDomainName algorithm, SerialNumber32 inception, SerialNumber32 expiration, DnsTransactionKeyMode mode,
                                             DnsResponseCode error, DataSegment key, DataSegment other)
        {
            if (key.Length > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("key", key.Length, string.Format("Cannot be longer than {0}", ushort.MaxValue));
            if (other.Length > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("other", other.Length, string.Format("Cannot be longer than {0}", ushort.MaxValue));

            Algorithm = algorithm;
            Inception = inception;
            Expiration = expiration;
            Mode = mode;
            Error = error;
            Key = key;
            Other = other;
        }

        /// <summary>
        /// Name of the algorithm in domain name syntax.
        /// The algorithm determines how the secret keying material agreed to using the TKEY RR is actually used to derive the algorithm specific key.
        /// </summary>
        public DnsDomainName Algorithm { get; private set; }

        /// <summary>
        /// Number of seconds since the beginning of 1 January 1970 GMT ignoring leap seconds treated as modulo 2**32 using ring arithmetic.
        /// In messages between a DNS resolver and a DNS server where this field is meaningful,
        /// it is either the requested validity interval start for the keying material asked for or
        /// specify the validity interval start of keying material provided.
        /// 
        /// To avoid different interpretations of the inception time in TKEY RRs,
        /// resolvers and servers exchanging them must have the same idea of what time it is.
        /// One way of doing this is with the NTP protocol [RFC 2030] but that or any other time synchronization used for this purpose must be done securely.
        /// </summary>
        public SerialNumber32 Inception { get; private set; }

        /// <summary>
        /// Number of seconds since the beginning of 1 January 1970 GMT ignoring leap seconds treated as modulo 2**32 using ring arithmetic.
        /// In messages between a DNS resolver and a DNS server where this field is meaningful,
        /// it is either the requested validity interval end for the keying material asked for or
        /// specify the validity interval end of keying material provided.
        /// 
        /// To avoid different interpretations of the expiration time in TKEY RRs,
        /// resolvers and servers exchanging them must have the same idea of what time it is.
        /// One way of doing this is with the NTP protocol [RFC 2030] but that or any other time synchronization used for this purpose must be done securely.
        /// </summary>
        public SerialNumber32 Expiration { get; private set; }

        /// <summary>
        /// Specifies the general scheme for key agreement or the purpose of the TKEY DNS message.
        /// Servers and resolvers supporting this specification must implement the Diffie-Hellman key agreement mode and the key deletion mode for queries.
        /// All other modes are optional.
        /// A server supporting TKEY that receives a TKEY request with a mode it does not support returns the BADMODE error.
        /// </summary>
        public DnsTransactionKeyMode Mode { get; private set; }

        /// <summary>
        /// When the TKEY Error Field is non-zero in a response to a TKEY query, the DNS header RCODE field indicates no error.
        /// However, it is possible if a TKEY is spontaneously included in a response the TKEY RR and DNS header error field could have unrelated non-zero error codes.
        /// </summary>
        public DnsResponseCode Error { get; private set; }

        /// <summary>
        /// The key exchange data.
        /// The meaning of this data depends on the mode.
        /// </summary>
        public DataSegment Key { get; private set; }

        /// <summary>
        /// May be used in future extensions.
        /// </summary>
        public DataSegment Other { get; private set; }

        public bool Equals(DnsResourceDataTransactionKey other)
        {
            return other != null &&
                   Algorithm.Equals(other.Algorithm) &&
                   Inception.Equals(other.Inception) &&
                   Expiration.Equals(other.Expiration) &&
                   Mode.Equals(other.Mode) &&
                   Error.Equals(other.Error) &&
                   Key.Equals(other.Key) &&
                   Other.Equals(other.Other);
        }

        public override bool  Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataTransactionKey);
        }

        internal DnsResourceDataTransactionKey()
            : this(DnsDomainName.Root, 0, 0, DnsTransactionKeyMode.DiffieHellmanExchange, DnsResponseCode.NoError, DataSegment.Empty, DataSegment.Empty)
        {
        }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return Algorithm.GetLength(compressionData, offsetInDns) + ConstantPartLength + Key.Length + Other.Length;
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            int algorithmLength = Algorithm.Write(buffer, dnsOffset, compressionData, offsetInDns);
            int offset = dnsOffset + offsetInDns + algorithmLength;
            buffer.Write(offset + OffsetAfterAlgorithm.Inception, Inception.Value, Endianity.Big);
            buffer.Write(offset + OffsetAfterAlgorithm.Expiration, Expiration.Value, Endianity.Big);
            buffer.Write(offset + OffsetAfterAlgorithm.Mode, (ushort)Mode, Endianity.Big);
            buffer.Write(offset + OffsetAfterAlgorithm.Error, (ushort)Error, Endianity.Big);
            buffer.Write(offset + OffsetAfterAlgorithm.KeySize, (ushort)Key.Length, Endianity.Big);
            Key.Write(buffer, offset + OffsetAfterAlgorithm.KeyData);

            int otherSizeOffset = offset + OffsetAfterAlgorithm.KeyData + Key.Length;
            buffer.Write(otherSizeOffset, (ushort)Other.Length, Endianity.Big);
            Other.Write(buffer, otherSizeOffset + sizeof(ushort));

            return algorithmLength + ConstantPartLength + Key.Length + Other.Length;
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstantPartLength + DnsDomainName.RootLength)
                return null;

            DnsDomainName algorithm;
            int algorithmLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length - ConstantPartLength, out algorithm, out algorithmLength))
                return null;
            offsetInDns += algorithmLength;
            length -= algorithmLength;

            if (length < ConstantPartLength)
                return null;

            uint inception = dns.ReadUInt(offsetInDns + OffsetAfterAlgorithm.Inception, Endianity.Big);
            uint expiration = dns.ReadUInt(offsetInDns + OffsetAfterAlgorithm.Expiration, Endianity.Big);
            DnsTransactionKeyMode mode = (DnsTransactionKeyMode)dns.ReadUShort(offsetInDns + OffsetAfterAlgorithm.Mode, Endianity.Big);
            DnsResponseCode error = (DnsResponseCode)dns.ReadUShort(offsetInDns + OffsetAfterAlgorithm.Error, Endianity.Big);

            int keySize = dns.ReadUShort(offsetInDns + OffsetAfterAlgorithm.KeySize, Endianity.Big);
            if (length < ConstantPartLength + keySize)
                return null;
            DataSegment key = dns.SubSegment(offsetInDns + OffsetAfterAlgorithm.KeyData, keySize);

            int totalReadAfterAlgorithm = OffsetAfterAlgorithm.KeyData + keySize;
            offsetInDns += totalReadAfterAlgorithm;
            length -= totalReadAfterAlgorithm;
            int otherSize = dns.ReadUShort(offsetInDns, Endianity.Big);
            if (length != sizeof(ushort) + otherSize)
                return null;
            DataSegment other = dns.SubSegment(offsetInDns + sizeof(ushort), otherSize);

            return new DnsResourceDataTransactionKey(algorithm, inception, expiration, mode, error, key, other);
        }
    }
}