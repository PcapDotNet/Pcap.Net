namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6705.
    /// <pre>
    /// +-----+-------------+----------------+
    /// | Bit | 0-7         | 8-15           |
    /// +-----+-------------+----------------+
    /// | 0   | Option Type | Opt Data Len   |
    /// +-----+-------------+----------------+
    /// | 16  | Reserved    | Address Length |
    /// +-----+-------------+----------------+
    /// | 32  | MAG IPv6 Address             |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// +-----+------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.MobileAccessGatewayIpV6Address)]
    public sealed class IpV6MobilityOptionMobileAccessGatewayIpV6Address : IpV6MobilityOptionComplex
    {
        public const byte AddressLength = 128;

        private static class Offset
        {
            public const int AddressLength = sizeof(byte);
            public const int Address = AddressLength + sizeof(byte);
        }

        public const int OptionDataLength = Offset.Address + IpV6Address.SizeOf;

        public IpV6MobilityOptionMobileAccessGatewayIpV6Address(IpV6Address address)
            : base(IpV6MobilityOptionType.MobileAccessGatewayIpV6Address)
        {
            Address = address;
        }

        /// <summary>
        /// Contains the MAG's IPv6 address.
        /// </summary>
        public IpV6Address Address { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            if (data[Offset.AddressLength] != AddressLength)
                return null;

            return new IpV6MobilityOptionMobileAccessGatewayIpV6Address(data.ReadIpV6Address(Offset.Address, Endianity.Big));
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionMobileAccessGatewayIpV6Address);
        }

        internal override int GetDataHashCode()
        {
            return Address.GetHashCode();
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.AddressLength, AddressLength);
            buffer.Write(offset + Offset.Address, Address, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionMobileAccessGatewayIpV6Address()
            : this(IpV6Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionMobileAccessGatewayIpV6Address other)
        {
            return other != null &&
                   Address.Equals(other.Address);
        }
    }
}