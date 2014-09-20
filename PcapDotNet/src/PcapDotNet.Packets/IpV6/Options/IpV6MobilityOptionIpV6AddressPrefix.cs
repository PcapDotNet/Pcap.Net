using System;
using System.Globalization;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5845.
    /// <pre>
    /// +-----+-------------+---------------+
    /// | Bit | 0-7         | 8-15          |
    /// +-----+-------------+---------------+
    /// | 0   | Option Type | Opt Data Len  |
    /// +-----+-------------+---------------+
    /// | 16  | Option-Code | Prefix Length |
    /// +-----+-------------+---------------+
    /// | 32  | IPv6 Address/Prefix         |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// +-----+-----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.IpV6AddressPrefix)]
    public sealed class IpV6MobilityOptionIpV6AddressPrefix : IpV6MobilityOptionComplex
    {
        /// <summary>
        /// The maximum value for prefix length.
        /// </summary>
        public const byte MaxPrefixLength = 128;

        private static class Offset
        {
            public const int Code = 0;
            public const int PrefixLength = Code + sizeof(byte);
            public const int AddressOrPrefix = PrefixLength + sizeof(byte);
        }

        /// <summary>
        /// The number of bytes this option data takes.
        /// </summary>
        public const int OptionDataLength = Offset.AddressOrPrefix + IpV6Address.SizeOf;

        /// <summary>
        /// Creates an instance from code, prefix length and address or prefix.
        /// </summary>
        /// <param name="code">Describes the kind of the address or the prefix.</param>
        /// <param name="prefixLength">
        /// Indicates the length of the IPv6 Address Prefix.
        /// The value ranges from 0 to 128.
        /// </param>
        /// <param name="addressOrPrefix">The IP address/prefix defined by the Option-Code field.</param>
        public IpV6MobilityOptionIpV6AddressPrefix(IpV6MobilityIpV6AddressPrefixCode code, byte prefixLength, IpV6Address addressOrPrefix)
            : base(IpV6MobilityOptionType.IpV6AddressPrefix)
        {
            if (prefixLength > MaxPrefixLength)
                throw new ArgumentOutOfRangeException("prefixLength", prefixLength,
                                                      string.Format(CultureInfo.InvariantCulture, "Max value is {0}", MaxPrefixLength));

            Code = code;
            PrefixLength = prefixLength;
            AddressOrPrefix = addressOrPrefix;
        }

        /// <summary>
        /// Describes the kind of the address or the prefix.
        /// </summary>
        public IpV6MobilityIpV6AddressPrefixCode Code { get; private set; }

        /// <summary>
        /// Indicates the length of the IPv6 Address Prefix.
        /// The value ranges from 0 to 128.
        /// </summary>
        public byte PrefixLength { get; private set; }

        /// <summary>
        /// The IP address/prefix defined by the Option-Code field.
        /// </summary>
        public IpV6Address AddressOrPrefix { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            IpV6MobilityIpV6AddressPrefixCode code = (IpV6MobilityIpV6AddressPrefixCode)data[Offset.Code];
            byte prefixLength = data[Offset.PrefixLength];
            IpV6Address addressOrPrefix = data.ReadIpV6Address(Offset.AddressOrPrefix, Endianity.Big);
            return new IpV6MobilityOptionIpV6AddressPrefix(code, prefixLength, addressOrPrefix);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionIpV6AddressPrefix);
        }

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge((byte)Code, PrefixLength), AddressOrPrefix);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Code, (byte)Code);
            buffer.Write(offset + Offset.PrefixLength, PrefixLength);
            buffer.Write(offset + Offset.AddressOrPrefix, AddressOrPrefix, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionIpV6AddressPrefix()
            : this(IpV6MobilityIpV6AddressPrefixCode.NewCareOfAddress, 0, IpV6Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionIpV6AddressPrefix other)
        {
            return other != null &&
                   Code == other.Code && PrefixLength == other.PrefixLength && AddressOrPrefix.Equals(other.AddressOrPrefix);
        }
    }
}