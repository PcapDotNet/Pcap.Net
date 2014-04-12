namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5844.
    /// <pre>
    /// +-----+-------------+------+-------+
    /// | Bit | 0-7         | 8-14 | 15    |
    /// +-----+-------------+------+-------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+------+-------+
    /// | 16  | Reserved           | S     |
    /// +-----+--------------------+-------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.IpV4DhcpSupportMode)]
    public sealed class IpV6MobilityOptionIpV4DhcpSupportMode : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int IsServer = sizeof(byte);
        }

        private static class Mask
        {
            public const byte IsServer = 0x01;
        }

        public const int OptionDataLength = Offset.IsServer + sizeof(byte);

        public IpV6MobilityOptionIpV4DhcpSupportMode(bool isServer)
            : base(IpV6MobilityOptionType.IpV4DhcpSupportMode)
        {
            IsServer = isServer;
        }

        /// <summary>
        /// Specifies the DHCP support mode.
        /// This flag indicates whether the mobile access gateway should function as a DHCP Server or a DHCP Relay for the attached mobile node.
        /// If false, the mobile access gateway should act as a DHCP Relay and if true, it should act as a DHCP Server.
        /// </summary>
        public bool IsServer { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            bool isServer = data.ReadBool(Offset.IsServer, Mask.IsServer);
            return new IpV6MobilityOptionIpV4DhcpSupportMode(isServer);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionIpV4DhcpSupportMode);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            if (IsServer)
                buffer.Write(offset + Offset.IsServer, Mask.IsServer);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionIpV4DhcpSupportMode() 
            : this(false)
        {
        }

        private bool EqualsData(IpV6MobilityOptionIpV4DhcpSupportMode other)
        {
            return other != null &&
                   IsServer == other.IsServer;
        }
    }
}