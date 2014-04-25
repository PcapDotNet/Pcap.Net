using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5844.
    /// </summary>
    public interface IIpV6MobilityOptionIpV4HomeAddress
    {
        byte PrefixLength { get; }
        IpV4Address HomeAddress { get; }
    }

    /// <summary>
    /// RFC 5844.
    /// <pre>
    /// +-----+------------+-----+--------------+
    /// | Bit | 0-5        | 6-7 | 8-15         |
    /// +-----+------------+-----+--------------+
    /// | 0   | Option Type      | Opt Data Len |
    /// +-----+------------+-----+--------------+
    /// | 16  | Prefix-len | Reserved           |
    /// +-----+------------+--------------------+
    /// | 32  | IPv4 home address               |
    /// |     |                                 |
    /// +-----+---------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.IpV4HomeAddressRequest)]
    public sealed class IpV6MobilityOptionIpV4HomeAddressRequest : IpV6MobilityOptionComplex, IIpV6MobilityOptionIpV4HomeAddress
    {
        public const byte MaxPrefixLength = 0x3F;

        private static class Offset
        {
            public const int PrefixLength = 0;
            public const int HomeAddress = PrefixLength + sizeof(ushort);
        }

        public const int OptionDataLength = Offset.HomeAddress + IpV4Address.SizeOf;

        private static class Mask
        {
            public const byte PrefixLength = 0xFC;
        }

        private static class Shift
        {
            public const int PrefixLength = 2;
        }

        public IpV6MobilityOptionIpV4HomeAddressRequest(byte prefixLength, IpV4Address homeAddress)
            : base(IpV6MobilityOptionType.IpV4HomeAddressRequest)
        {
            if (prefixLength > MaxPrefixLength)
                throw new ArgumentOutOfRangeException("prefixLength", prefixLength, string.Format("Max prefix length is {0}", MaxPrefixLength));

            PrefixLength = prefixLength;
            HomeAddress = homeAddress;
        }

        /// <summary>
        /// Indicates the prefix length of the mobile node's IPv4 home network corresponding to the IPv4 home address contained in the option.
        /// </summary>
        public byte PrefixLength { get; private set; }

        /// <summary>
        /// Containing the IPv4 home address that is being requested.
        /// The value of 0.0.0.0 is used to request that the local mobility anchor perform the address allocation.
        /// </summary>
        public IpV4Address HomeAddress { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            byte prefixLength = (byte)((data[Offset.PrefixLength] & Mask.PrefixLength) >> Shift.PrefixLength);
            IpV4Address homeAddress = data.ReadIpV4Address(Offset.HomeAddress, Endianity.Big);
            return new IpV6MobilityOptionIpV4HomeAddressRequest(prefixLength, homeAddress);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionIpV4HomeAddressRequest);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.PrefixLength, (byte)(PrefixLength << Shift.PrefixLength));
            buffer.Write(offset + Offset.HomeAddress, HomeAddress, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionIpV4HomeAddressRequest()
            : this(0, IpV4Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionIpV4HomeAddressRequest other)
        {
            return other != null &&
                   PrefixLength == other.PrefixLength && HomeAddress.Equals(other.HomeAddress);
        }
    }
}