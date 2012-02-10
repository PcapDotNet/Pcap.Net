using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 4025.
    /// <pre>
    /// +-----+--------------+
    /// | bit | 0-7          |
    /// +-----+--------------+
    /// | 0   | precedence   |
    /// +-----+--------------+
    /// | 8   | gateway type |
    /// +-----+--------------+
    /// | 16  | algorithm    |
    /// +-----+--------------+
    /// | 24  | gateway      |
    /// | ... |              |
    /// +-----+--------------+
    /// |     | public key   |
    /// | ... |              |
    /// +-----+--------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.IpSecKey)]
    public sealed class DnsResourceDataIpSecKey : DnsResourceDataNoCompression, IEquatable<DnsResourceDataIpSecKey>
    {
        private static class Offset
        {
            public const int Precedence = 0;
            public const int GatewayType = Precedence + sizeof(byte);
            public const int Algorithm = GatewayType + sizeof(byte);
            public const int Gateway = Algorithm + sizeof(byte);
        }

        public const int ConstPartLength = Offset.Gateway;

        public DnsResourceDataIpSecKey(byte precedence, DnsGateway gateway, DnsPublicKeyAlgorithm algorithm, DataSegment publicKey)
        {
            Precedence = precedence;
            Gateway = gateway;
            Algorithm = algorithm;
            PublicKey = publicKey;
        }

        /// <summary>
        /// Precedence for this record.
        /// Gateways listed in IPSECKEY records with lower precedence are to be attempted first.
        /// Where there is a tie in precedence, the order should be non-deterministic.
        /// </summary>
        public byte Precedence { get; private set; }

        /// <summary>
        /// Indicates the format of the information that is stored in the gateway field.
        /// </summary>
        public DnsGatewayType GatewayType { get { return Gateway.Type; } }

        /// <summary>
        /// Indicates a gateway to which an IPsec tunnel may be created in order to reach the entity named by this resource record.
        /// </summary>
        public DnsGateway Gateway { get; private set;}

        /// <summary>
        /// Identifies the public key's cryptographic algorithm and determines the format of the public key field.
        /// </summary>
        public DnsPublicKeyAlgorithm Algorithm { get; private set;} 

        /// <summary>
        /// Contains the algorithm-specific portion of the KEY RR RDATA.
        /// </summary>
        public DataSegment PublicKey { get; private set; }

        public bool Equals(DnsResourceDataIpSecKey other)
        {
            return other != null &&
                   Precedence.Equals(other.Precedence) &&
                   Gateway.Equals(other.Gateway) &&
                   Algorithm.Equals(other.Algorithm) &&
                   PublicKey.Equals(other.PublicKey);
        }

        public override bool Equals(object other)
        {
            return Equals(other as DnsResourceDataIpSecKey);
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge(Precedence, (byte)Algorithm), Gateway, PublicKey);
        }

        internal DnsResourceDataIpSecKey()
            : this(0, DnsGateway.None, DnsPublicKeyAlgorithm.None, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstPartLength + Gateway.Length + PublicKey.Length;
        }

        internal override int WriteData(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Precedence, Precedence);
            buffer.Write(offset + Offset.GatewayType, (byte)GatewayType);
            buffer.Write(offset + Offset.Algorithm, (byte)Algorithm);
            Gateway.Write(buffer, offset + Offset.Gateway);
            PublicKey.Write(buffer, offset + ConstPartLength + Gateway.Length);

            return GetLength();
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstPartLength)
                return null;

            byte precedence = dns[offsetInDns + Offset.Precedence];
            DnsGatewayType gatewayType = (DnsGatewayType)dns[offsetInDns + Offset.GatewayType];
            DnsPublicKeyAlgorithm algorithm = (DnsPublicKeyAlgorithm)dns[offsetInDns + Offset.Algorithm];
            DnsGateway gateway = DnsGateway.CreateInstance(gatewayType, dns, offsetInDns + Offset.Gateway, length - ConstPartLength);
            if (gateway == null)
                return null;
            DataSegment publicKey = dns.Subsegment(offsetInDns + ConstPartLength + gateway.Length, length - ConstPartLength - gateway.Length);

            return new DnsResourceDataIpSecKey(precedence, gateway, algorithm, publicKey);
        }
    }
}