using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 2163.
    /// <pre>
    /// +-----+------------+
    /// | bit | 0-15       |
    /// +-----+------------+
    /// | 0   | Preference |
    /// +-----+------------+
    /// | 16  | MAP822     |
    /// | ... |            |
    /// +-----+------------+
    /// |     | MAPX400    |
    /// | ... |            |
    /// +-----+------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.PointerX400)]
    public sealed class DnsResourceDataX400Pointer : DnsResourceData, IEquatable<DnsResourceDataX400Pointer>
    {
        private static class Offset
        {
            public const int Preference = 0;
            public const int Map822 = Preference + sizeof(ushort);
        }

        private const int ConstantPartLength = Offset.Map822;

        public DnsResourceDataX400Pointer(ushort preference, DnsDomainName map822, DnsDomainName mapX400)
        {
            Preference = preference;
            Map822 = map822;
            MapX400 = mapX400;
        }

        /// <summary>
        /// The preference given to this RR among others at the same owner.
        /// Lower values are preferred.
        /// </summary>
        public ushort Preference { get; private set; }

        /// <summary>
        /// RFC 822 domain.
        /// The RFC 822 part of the MCGAM.
        /// </summary>
        public DnsDomainName Map822 { get; private set; }

        /// <summary>
        /// The value of x400-in-domain-syntax derived from the X.400 part of the MCGAM.
        /// </summary>
        public DnsDomainName MapX400 { get; private set; }

        public bool Equals(DnsResourceDataX400Pointer other)
        {
            return other != null &&
                   Preference.Equals(other.Preference) &&
                   Map822.Equals(other.Map822) &&
                   MapX400.Equals(other.MapX400);
        }

        public override bool Equals(object other)
        {
            return Equals(other as DnsResourceDataX400Pointer);
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode(Preference, Map822, MapX400);
        }

        internal DnsResourceDataX400Pointer()
            : this(0, DnsDomainName.Root, DnsDomainName.Root)
        {
        }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return ConstantPartLength + Map822.GetLength(compressionData, offsetInDns) + MapX400.GetLength(compressionData, offsetInDns);
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            buffer.Write(dnsOffset + offsetInDns + Offset.Preference, Preference, Endianity.Big);
            int numBytesWritten = Map822.Write(buffer, dnsOffset, compressionData, offsetInDns + Offset.Map822);
            numBytesWritten += MapX400.Write(buffer, dnsOffset, compressionData, offsetInDns + ConstantPartLength + numBytesWritten);

            return ConstantPartLength + numBytesWritten;
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstantPartLength)
                return null;

            ushort preference = dns.ReadUShort(offsetInDns + Offset.Preference, Endianity.Big);

            offsetInDns += ConstantPartLength;
            length -= ConstantPartLength;

            DnsDomainName map822;
            int map822Length;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out map822, out map822Length))
                return null;
            offsetInDns += map822Length;
            length -= map822Length;

            DnsDomainName mapX400;
            int mapX400Length;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out mapX400, out mapX400Length))
                return null;
            length -= mapX400Length;

            if (length != 0)
                return null;

            return new DnsResourceDataX400Pointer(preference, map822, mapX400);
        }
    }
}