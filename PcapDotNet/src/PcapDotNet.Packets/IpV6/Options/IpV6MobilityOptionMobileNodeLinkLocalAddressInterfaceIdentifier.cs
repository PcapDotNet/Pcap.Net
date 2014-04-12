namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5949.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved                   |
    /// +-----+----------------------------+
    /// | 32  | Interface Identifier       |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.MobileNodeLinkLocalAddressInterfaceIdentifier)]
    public sealed class IpV6MobilityOptionMobileNodeLinkLocalAddressInterfaceIdentifier : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int InterfaceIdentifier = sizeof(ushort);
        }

        public const int OptionDataLength = Offset.InterfaceIdentifier + sizeof(ulong);

        public IpV6MobilityOptionMobileNodeLinkLocalAddressInterfaceIdentifier(ulong interfaceIdentifier)
            : base(IpV6MobilityOptionType.MobileNodeLinkLocalAddressInterfaceIdentifier)
        {
            InterfaceIdentifier = interfaceIdentifier;
        }

        /// <summary>
        /// The Interface Identifier value used for the mobile node's IPv6 Link-local address in the P-AN.
        /// </summary>
        public ulong InterfaceIdentifier { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            ulong interfaceIdentifier = data.ReadULong(Offset.InterfaceIdentifier, Endianity.Big);
            return new IpV6MobilityOptionMobileNodeLinkLocalAddressInterfaceIdentifier(interfaceIdentifier);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionMobileNodeLinkLocalAddressInterfaceIdentifier);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.InterfaceIdentifier, InterfaceIdentifier, Endianity.Big);
            offset += DataLength;
        }

        private IpV6MobilityOptionMobileNodeLinkLocalAddressInterfaceIdentifier()
            : this(0)
        {
        }

        private bool EqualsData(IpV6MobilityOptionMobileNodeLinkLocalAddressInterfaceIdentifier other)
        {
            return other != null &&
                   InterfaceIdentifier == other.InterfaceIdentifier;
        }
    }
}