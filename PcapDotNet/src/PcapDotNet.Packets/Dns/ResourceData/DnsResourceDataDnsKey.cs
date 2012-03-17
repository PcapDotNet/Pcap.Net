using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFCs 3757, 4034, 5011.
    /// <pre>
    /// +-----+----------+----------+--------+----------+--------------------+
    /// | bit | 0-6      | 7        | 8      | 9-14     | 15                 |
    /// +-----+----------+----------+--------+----------+--------------------+
    /// | 0   | Reserved | Zone Key | Revoke | Reserved | Secure Entry Point |
    /// +-----+----------+----------+--------+----------+--------------------+
    /// | 16  | Protocol            | Algorithm                              |
    /// +-----+---------------------+----------------------------------------+
    /// | 32  | Public Key                                                   |
    /// | ... |                                                              |
    /// +-----+--------------------------------------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.DnsKey)]
    public sealed class DnsResourceDataDnsKey : DnsResourceDataSimple, IEquatable<DnsResourceDataDnsKey>
    {
        /// <summary>
        /// The expected value for the protocol field.
        /// </summary>
        public const byte ProtocolValue = 3;

        private static class Offset
        {
            public const int ZoneKey = 0;
            public const int Revoke = 1;
            public const int SecureEntryPoint = 1;
            public const int Protocol = sizeof(ushort);
            public const int Algorithm = Protocol + sizeof(byte);
            public const int PublicKey = Algorithm + sizeof(byte);
        }

        private static class Mask
        {
            public const byte ZoneKey = 0x01;
            public const byte Revoke = 0x80;
            public const byte SecureEntryPoint = 0x01;
        }

        private const int ConstantPartLength = Offset.PublicKey;

        /// <summary>
        /// Constructs an instance from the zone key, revoke, secure entry point, protocol, algorithm and public key fields.
        /// </summary>
        /// <param name="zoneKey">
        /// If true, the DNSKEY record holds a DNS zone key, and the DNSKEY RR's owner name must be the name of a zone.
        /// If false, then the DNSKEY record holds some other type of DNS public key and must not be used to verify RRSIGs that cover RRsets.
        /// </param>
        /// <param name="revoke">
        /// If true, and the resolver sees an RRSIG(DNSKEY) signed by the associated key,
        /// then the resolver must consider this key permanently invalid for all purposes except for validating the revocation.
        /// </param>
        /// <param name="secureEntryPoint">
        /// RFC 3757.
        /// If true, then the DNSKEY record holds a key intended for use as a secure entry point.
        /// This flag is only intended to be a hint to zone signing or debugging software as to the intended use of this DNSKEY record;
        /// validators must not alter their behavior during the signature validation process in any way based on the setting of this bit.
        /// This also means that a DNSKEY RR with the SEP bit set would also need the Zone Key flag set in order to be able to generate signatures legally.
        /// A DNSKEY RR with the SEP set and the Zone Key flag not set MUST NOT be used to verify RRSIGs that cover RRsets.
        /// </param>
        /// <param name="protocol">
        /// Must have value 3, and the DNSKEY RR MUST be treated as invalid during signature verification if it is found to be some value other than 3.
        /// </param>
        /// <param name="algorithm">Identifies the public key's cryptographic algorithm and determines the format of the Public Key field.</param>
        /// <param name="publicKey">The public key material. The format depends on the algorithm of the key being stored.</param>
        public DnsResourceDataDnsKey(bool zoneKey, bool revoke, bool secureEntryPoint, byte protocol,  DnsAlgorithm algorithm, DataSegment publicKey)
        {
            ZoneKey = zoneKey;
            Revoke = revoke;
            SecureEntryPoint = secureEntryPoint;
            Protocol = protocol;
            Algorithm = algorithm;
            PublicKey = publicKey;
        }

        /// <summary>
        /// If true, the DNSKEY record holds a DNS zone key, and the DNSKEY RR's owner name must be the name of a zone.
        /// If false, then the DNSKEY record holds some other type of DNS public key and must not be used to verify RRSIGs that cover RRsets.
        /// </summary>
        public bool ZoneKey { get; private set; }

        /// <summary>
        /// If true, and the resolver sees an RRSIG(DNSKEY) signed by the associated key,
        /// then the resolver must consider this key permanently invalid for all purposes except for validating the revocation.
        /// </summary>
        public bool Revoke { get; private set; }

        /// <summary>
        /// RFC 3757.
        /// If true, then the DNSKEY record holds a key intended for use as a secure entry point.
        /// This flag is only intended to be a hint to zone signing or debugging software as to the intended use of this DNSKEY record;
        /// validators must not alter their behavior during the signature validation process in any way based on the setting of this bit.
        /// This also means that a DNSKEY RR with the SEP bit set would also need the Zone Key flag set in order to be able to generate signatures legally.
        /// A DNSKEY RR with the SEP set and the Zone Key flag not set MUST NOT be used to verify RRSIGs that cover RRsets.
        /// </summary>
        public bool SecureEntryPoint { get; private set; }

        /// <summary>
        /// Must have value 3, and the DNSKEY RR MUST be treated as invalid during signature verification if it is found to be some value other than 3.
        /// </summary>
        public byte Protocol { get; private set; }

        /// <summary>
        /// Identifies the public key's cryptographic algorithm and determines the format of the Public Key field.
        /// </summary>
        public DnsAlgorithm Algorithm { get; private set; }

        /// <summary>
        /// The public key material.
        /// The format depends on the algorithm of the key being stored.
        /// </summary>
        public DataSegment PublicKey { get; private set; }

        /// <summary>
        /// Two DnsResourceDataDnsKey are equal iff their zone key, revoke, secure entry point, protocol, algorithm and public key fields are equal.
        /// </summary>
        public bool Equals(DnsResourceDataDnsKey other)
        {
            return other != null &&
                   ZoneKey.Equals(other.ZoneKey) &&
                   Revoke.Equals(other.Revoke) &&
                   SecureEntryPoint.Equals(other.SecureEntryPoint) &&
                   Protocol.Equals(other.Protocol) &&
                   Algorithm.Equals(other.Algorithm) &&
                   PublicKey.Equals(other.PublicKey);
        }

        /// <summary>
        /// Two DnsResourceDataDnsKey are equal iff their zone key, revoke, secure entry point, protocol, algorithm and public key fields are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataDnsKey);
        }

        /// <summary>
        /// The hash code based on the zone key, revoke, secure entry point, protocol, algorithm and public key fields.
        /// </summary>
        public override int GetHashCode()
        {
            return BitSequence.Merge(BitSequence.Merge(ZoneKey, Revoke, SecureEntryPoint), Protocol, (byte)Algorithm).GetHashCode();
        }

        internal DnsResourceDataDnsKey()
            : this(false, false, false, ProtocolValue, DnsAlgorithm.None, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + PublicKey.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            byte flagsByte0 = 0;
            if (ZoneKey)
                flagsByte0 |= Mask.ZoneKey;
            buffer.Write(offset + Offset.ZoneKey, flagsByte0);

            byte flagsByte1 = 0;
            if (Revoke)
                flagsByte1 |= Mask.Revoke;
            if (SecureEntryPoint)
                flagsByte1 |= Mask.SecureEntryPoint;
            buffer.Write(offset + Offset.SecureEntryPoint, flagsByte1);

            buffer.Write(offset + Offset.Protocol, Protocol);
            buffer.Write(offset + Offset.Algorithm, (byte)Algorithm);
            PublicKey.Write(buffer, offset + Offset.PublicKey);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            bool zoneKey = data.ReadBool(Offset.ZoneKey, Mask.ZoneKey);
            bool revoke = data.ReadBool(Offset.Revoke, Mask.Revoke);
            bool secureEntryPoint = data.ReadBool(Offset.SecureEntryPoint, Mask.SecureEntryPoint);
            byte protocol = data[Offset.Protocol];
            DnsAlgorithm algorithm = (DnsAlgorithm)data[Offset.Algorithm];
            DataSegment publicKey = data.Subsegment(Offset.PublicKey, data.Length - ConstantPartLength);

            return new DnsResourceDataDnsKey(zoneKey, revoke, secureEntryPoint, protocol, algorithm, publicKey);
        }
    }
}