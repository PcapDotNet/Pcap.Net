using System;
using System.Globalization;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5555.
    /// <pre>
    /// +-----+------------+---+---+--------------+
    /// | Bit | 0-5        | 6 | 7 | 8-15         |
    /// +-----+------------+---+---+--------------+
    /// | 0   | Option Type        | Opt Data Len |
    /// +-----+------------+---+---+--------------+
    /// | 16  | Prefix-len | P | Reserved         |
    /// +-----+------------+---+------------------+
    /// | 32  | IPv4 home address                 |
    /// |     |                                   |
    /// +-----+-----------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.IpV4HomeAddress)]
    public sealed class IpV6MobilityOptionIpV4HomeAddress : IpV6MobilityOptionComplex, IIpV6MobilityOptionIpV4HomeAddress
    {
        /// <summary>
        /// The maximum value for Prefix Length.
        /// </summary>
        public const byte MaxPrefixLength = 0x3F;

        private static class Offset
        {
            public const int PrefixLength = 0;
            public const int RequestPrefix = PrefixLength;
            public const int HomeAddress = RequestPrefix + sizeof(byte) + sizeof(byte);
        }

        private static class Mask
        {
            public const byte PrefixLength = 0xFC;
            public const byte RequestPrefix = 0x02;
        }

        private static class Shift
        {
            public const int PrefixLength = 2;
        }

        /// <summary>
        /// The number of bytes this option data takes.
        /// </summary>
        public const int OptionDataLength = Offset.HomeAddress + IpV4Address.SizeOf;

        /// <summary>
        /// Creates an instance from Prefix Length, Request Prefix and Home Address.
        /// </summary>
        /// <param name="prefixLength">
        /// The length of the prefix allocated to the mobile node.
        /// If only a single address is allocated, this field must be set to 32.
        /// In the first binding update requesting a prefix, the field contains the prefix length requested.
        /// However, in the following binding updates, this field must contain the length of the prefix allocated.
        /// A value of zero is invalid and must be considered an error.
        /// </param>
        /// <param name="requestPrefix">
        /// When true, indicates that the mobile node requests a mobile network prefix.
        /// This flag is only relevant for new requests, and must be ignored for binding refreshes.
        /// </param>
        /// <param name="homeAddress">
        /// The mobile node's IPv4 home address that should be defended by the home agent.
        /// This field could contain any unicast IPv4 address (public or private) that was assigned to the mobile node.
        /// The value 0.0.0.0 is used to request an IPv4 home address from the home agent.
        /// A mobile node may choose to use this option to request a prefix by setting the address to All Zeroes and setting the RequestPrefix flag.
        /// The mobile node could then form an IPv4 home address based on the allocated prefix.
        /// Alternatively, the mobile node may use two different options, one for requesting an address (static or dynamic) and another for requesting a 
        /// </param>
        public IpV6MobilityOptionIpV4HomeAddress(byte prefixLength, bool requestPrefix, IpV4Address homeAddress)
            : base(IpV6MobilityOptionType.IpV4HomeAddress)
        {
            if (prefixLength > MaxPrefixLength)
                throw new ArgumentOutOfRangeException("prefixLength", prefixLength,
                                                      string.Format(CultureInfo.InvariantCulture, "Exceeded maximum value {0}", MaxPrefixLength));

            PrefixLength = prefixLength;
            RequestPrefix = requestPrefix;
            HomeAddress = homeAddress;
        }

        /// <summary>
        /// The length of the prefix allocated to the mobile node.
        /// If only a single address is allocated, this field must be set to 32.
        /// In the first binding update requesting a prefix, the field contains the prefix length requested.
        /// However, in the following binding updates, this field must contain the length of the prefix allocated.
        /// A value of zero is invalid and must be considered an error.
        /// </summary>
        public byte PrefixLength { get; private set; }

        /// <summary>
        /// When true, indicates that the mobile node requests a mobile network prefix.
        /// This flag is only relevant for new requests, and must be ignored for binding refreshes.
        /// </summary>
        public bool RequestPrefix { get; private set; }

        /// <summary>
        /// The mobile node's IPv4 home address that should be defended by the home agent.
        /// This field could contain any unicast IPv4 address (public or private) that was assigned to the mobile node.
        /// The value 0.0.0.0 is used to request an IPv4 home address from the home agent.
        /// A mobile node may choose to use this option to request a prefix by setting the address to All Zeroes and setting the RequestPrefix flag.
        /// The mobile node could then form an IPv4 home address based on the allocated prefix.
        /// Alternatively, the mobile node may use two different options, one for requesting an address (static or dynamic) and another for requesting a 
        /// prefix.
        /// </summary>
        public IpV4Address HomeAddress { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            byte prefixLength = (byte)((data[Offset.PrefixLength] & Mask.PrefixLength) >> Shift.PrefixLength);
            bool requestPrefix = data.ReadBool(Offset.RequestPrefix, Mask.RequestPrefix);
            IpV4Address homeAddress = data.ReadIpV4Address(Offset.HomeAddress, Endianity.Big);
            return new IpV6MobilityOptionIpV4HomeAddress(prefixLength, requestPrefix, homeAddress);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionIpV4HomeAddress);
        }

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge(PrefixLength, RequestPrefix.ToByte()), HomeAddress);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            byte prefixLengthAndRequestPrefix = (byte)(PrefixLength << Shift.PrefixLength);
            if (RequestPrefix)
                prefixLengthAndRequestPrefix |= Mask.RequestPrefix;

            buffer.Write(offset + Offset.PrefixLength, prefixLengthAndRequestPrefix);
            buffer.Write(offset + Offset.HomeAddress, HomeAddress, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionIpV4HomeAddress()
            : this(0, false, IpV4Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionIpV4HomeAddress other)
        {
            return other != null &&
                   PrefixLength == other.PrefixLength && RequestPrefix == other.RequestPrefix && HomeAddress.Equals(other.HomeAddress);
        }
    }
}