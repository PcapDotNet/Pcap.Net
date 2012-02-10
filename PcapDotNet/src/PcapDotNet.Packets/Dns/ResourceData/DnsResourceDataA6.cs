using System;
using System.Globalization;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV6;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 2874.
    /// <pre>
    /// +-------------+----------------+-----------------+
    /// | Prefix len. | Address suffix | Prefix name     |
    /// | (1 octet)   | (0..16 octets) | (0..255 octets) |
    /// +-------------+----------------+-----------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.A6)]
    public sealed class DnsResourceDataA6 : DnsResourceDataNoCompression, IEquatable<DnsResourceDataA6>
    {
        public const byte MaxPrefixLength = 8 * IpV6Address.SizeOf;

        private static class Offset
        {
            public const int PrefixLength = 0;
            public const int AddressSuffix = PrefixLength + sizeof(byte);
        }

        public const int ConstantPartLength = Offset.AddressSuffix;
        public const int MinimumLength = ConstantPartLength + DnsDomainName.RootLength;

        private static bool IsAddressSuffixTooBig(byte prefixLength, IpV6Address addressSuffix)
        {
            return (prefixLength < 128 && (addressSuffix.ToValue() < (UInt128.One << (127 - prefixLength))));
        }

        private static bool IsAddressSuffixTooSmall(byte prefixLength, IpV6Address addressSuffix)
        {
            return (prefixLength > 0 && (addressSuffix.ToValue() >= (UInt128.One << (128 - prefixLength))));
        }

        public DnsResourceDataA6(byte prefixLength, IpV6Address addressSuffix, DnsDomainName prefixName)
        {
            if (IsAddressSuffixTooBig(prefixLength, addressSuffix))
                throw new ArgumentOutOfRangeException("addressSuffix",
                                                      string.Format(CultureInfo.InvariantCulture, "Value is too small for prefix length {0}", prefixLength));
            if (IsAddressSuffixTooSmall(prefixLength, addressSuffix))
                throw new ArgumentOutOfRangeException("addressSuffix",
                                                      string.Format(CultureInfo.InvariantCulture, "Value is too big for prefix length {0}", prefixLength));

            PrefixLength = prefixLength;
            AddressSuffix = addressSuffix;
            PrefixName = prefixName;
        }

        /// <summary>
        /// Encoded as an eight-bit unsigned integer with value between 0 and 128 inclusive.
        /// </summary>
        public byte PrefixLength { get; private set; }

        /// <summary>
        /// An IPv6 address suffix, encoded in network order (high-order octet first).
        /// There must be exactly enough octets in this field to contain a number of bits equal to 128 minus prefix length, with 0 to 7 leading pad bits to make this field an integral number of octets.
        /// Pad bits, if present, must be set to zero when loading a zone file and ignored (other than for SIG verification) on reception.
        /// </summary>
        public IpV6Address AddressSuffix { get; private set; }

        /// <summary>
        /// The number of bytes the address suffix takes.
        /// </summary>
        public int AddressSuffixLength { get { return CalculateAddressSuffixLength(PrefixLength); } }

        /// <summary>
        /// The name of the prefix, encoded as a domain name.
        /// This name must not be compressed.
        /// </summary>
        public DnsDomainName PrefixName { get; private set; }

        public bool Equals(DnsResourceDataA6 other)
        {
            return other != null &&
                   PrefixLength.Equals(other.PrefixLength) &&
                   AddressSuffix.Equals(other.AddressSuffix) &&
                   PrefixName.Equals(other.PrefixName);
        }

        public override bool Equals(object other)
        {
            return Equals(other as DnsResourceDataA6);
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode(PrefixLength, AddressSuffix, PrefixName);
        }

        internal DnsResourceDataA6()
            : this(0, IpV6Address.MaxValue, DnsDomainName.Root)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + AddressSuffixLength + PrefixName.NonCompressedLength;
        }

        internal override int WriteData(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.PrefixLength, PrefixLength);
            buffer.WriteUnsigned(offset + Offset.AddressSuffix, AddressSuffix.ToValue(), AddressSuffixLength, Endianity.Big);
            PrefixName.WriteUncompressed(buffer, offset + ConstantPartLength + AddressSuffixLength);

            return GetLength();
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < MinimumLength)
                return null;

            byte prefixLength = dns[offsetInDns + Offset.PrefixLength];
            if (prefixLength > MaxPrefixLength)
                return null;
            offsetInDns += ConstantPartLength;
            length -= ConstantPartLength;

            int addressSuffixLength = CalculateAddressSuffixLength(prefixLength);
            if (length < addressSuffixLength)
                return null;
            IpV6Address addressSuffix = new IpV6Address((UInt128)dns.ReadUnsignedBigInteger(offsetInDns, addressSuffixLength, Endianity.Big));
            offsetInDns += addressSuffixLength;
            length -= addressSuffixLength;

            if (IsAddressSuffixTooBig(prefixLength, addressSuffix) || IsAddressSuffixTooSmall(prefixLength, addressSuffix))
                return null;

            DnsDomainName prefixName;
            int prefixNameLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out prefixName, out prefixNameLength))
                return null;
            if (prefixNameLength != length)
                return null;

            return new DnsResourceDataA6(prefixLength, addressSuffix, prefixName);
        }

        private static int CalculateAddressSuffixLength(byte prefixLength)
        {
            return (MaxPrefixLength - prefixLength + 7) / 8;
        }
    }
}