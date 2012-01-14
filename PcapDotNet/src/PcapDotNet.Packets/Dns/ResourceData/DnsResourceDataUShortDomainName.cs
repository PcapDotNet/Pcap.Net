using System;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// <pre>
    /// +-----+--------+
    /// | bit | 0-15   |
    /// +-----+--------+
    /// | 0   | Value  |
    /// +-----+--------+
    /// | 16  | Domain |
    /// | ... |        |
    /// +-----+--------+
    /// </pre>
    /// </summary>
    public abstract class DnsResourceDataUShortDomainName : DnsResourceData, IEquatable<DnsResourceDataUShortDomainName>
    {
        private static class Offset
        {
            public const int Value = 0;
            public const int DomainName = Value + sizeof(ushort);
        }

        private const int ConstantPartLength = Offset.DomainName;

        public bool Equals(DnsResourceDataUShortDomainName other)
        {
            return other != null &&
                   GetType().Equals(other.GetType()) &&
                   Value.Equals(other.Value) &&
                   DomainName.Equals(other.DomainName);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataUShortDomainName);
        }

        internal DnsResourceDataUShortDomainName(ushort value, DnsDomainName domainName)
        {
            Value = value;
            DomainName = domainName;
        }

        internal ushort Value { get; private set; }

        internal DnsDomainName DomainName { get; private set; }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return ConstantPartLength +
                   DomainName.GetLength(compressionData, offsetInDns);
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            buffer.Write(dnsOffset + offsetInDns + Offset.Value, Value, Endianity.Big);
            int numBytesWritten = DomainName.Write(buffer, dnsOffset, compressionData, offsetInDns + Offset.DomainName);

            return ConstantPartLength + numBytesWritten;
        }

        internal static bool TryRead(out ushort value, out DnsDomainName domainName,
                                     DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstantPartLength)
            {
                value = 0;
                domainName = null;
                return false;
            }
            value = dns.ReadUShort(offsetInDns + Offset.Value, Endianity.Big);
            length -= ConstantPartLength;

            int domainNameLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns + Offset.DomainName, length, out domainName, out domainNameLength))
                return false;
            length -= domainNameLength;

            if (length != 0)
                return false;

            return true;
        }
    }
}