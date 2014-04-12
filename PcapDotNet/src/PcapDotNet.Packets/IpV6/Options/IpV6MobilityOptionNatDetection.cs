namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5555.
    /// <pre>
    /// +-----+---+---------+--------------+
    /// | Bit | 0 | 1-7     | 8-15         |
    /// +-----+---+---------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+---+---------+--------------+
    /// | 16  | F | Reserved               |
    /// +-----+---+------------------------+
    /// | 32  | Refresh time               |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.NatDetection)]
    public sealed class IpV6MobilityOptionNatDetection : IpV6MobilityOptionComplex
    {
        public const uint RecommendedRefreshTime = 110;

        private static class Offset
        {
            public const int UdpEncapsulationRequired = 0;
            public const int RefreshTime = UdpEncapsulationRequired + sizeof(ushort);
        }

        public const int OptionDataLength = Offset.RefreshTime + sizeof(uint);

        private static class Mask
        {
            public const byte UdpEncapsulationRequired = 0x80;
        }

        public IpV6MobilityOptionNatDetection(bool udpEncapsulationRequired, uint refreshTime)
            : base(IpV6MobilityOptionType.NatDetection)
        {
            UdpEncapsulationRequired = udpEncapsulationRequired;
            RefreshTime = refreshTime;
        }

        /// <summary>
        /// Indicates to the mobile node that UDP encapsulation is required.
        /// When set, this flag indicates that the mobile node must use UDP encapsulation even if a NAT is not located between the mobile node and home agent.
        /// This flag should not be set when the mobile node is assigned an IPv6 care-of address with some exceptions.
        /// </summary>
        public bool UdpEncapsulationRequired { get; private set; }

        /// <summary>
        /// A suggested time (in seconds) for the mobile node to refresh the NAT binding.
        /// If set to zero, it is ignored.
        /// If this field is set to uint.MaxValue, it means that keepalives are not needed, i.e., no NAT was detected.
        /// The home agent must be configured with a default value for the refresh time.
        /// The recommended value is RecommendedRefreshTime.
        /// </summary>
        public uint RefreshTime { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            bool udpEncapsulationRequired = data.ReadBool(Offset.UdpEncapsulationRequired, Mask.UdpEncapsulationRequired);
            uint refreshTime = data.ReadUInt(Offset.RefreshTime, Endianity.Big);
            return new IpV6MobilityOptionNatDetection(udpEncapsulationRequired, refreshTime);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionNatDetection);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            byte udpEncapsulationRequired = 0;
            if (UdpEncapsulationRequired)
                udpEncapsulationRequired |= Mask.UdpEncapsulationRequired;

            buffer.Write(offset + Offset.UdpEncapsulationRequired, udpEncapsulationRequired);
            buffer.Write(offset + Offset.RefreshTime, RefreshTime, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionNatDetection()
            : this(false, 0)
        {
        }

        private bool EqualsData(IpV6MobilityOptionNatDetection other)
        {
            return other != null &&
                   UdpEncapsulationRequired == other.UdpEncapsulationRequired && RefreshTime == other.RefreshTime;
        }
    }
}