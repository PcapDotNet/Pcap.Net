namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2711.
    /// <pre>
    /// +-----+-------------+--------+
    /// | Bit | 0-7         | 8-15   |
    /// +-----+-------------+--------+
    /// | 0   | Option Type | 2      |
    /// +-----+-------------+--------+
    /// | 16  | Router Alert Type    |
    /// +-----+----------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.RouterAlert)]
    public sealed class IpV6OptionRouterAlert : IpV6OptionComplex, IIpV6OptionComplexFactory
    {
        /// <summary>
        /// The number of bytes the option data takes.
        /// </summary>
        public const int OptionDataLength = sizeof(ushort);

        /// <summary>
        /// Creates an instance according to the given router alert type.
        /// </summary>
        /// <param name="routerAlertType">Type of router alert.</param>
        public IpV6OptionRouterAlert(IpV6RouterAlertType routerAlertType)
            : base(IpV6OptionType.RouterAlert)
        {
            RouterAlertType = routerAlertType;
        }

        /// <summary>
        /// Type of router alert.
        /// </summary>
        public IpV6RouterAlertType RouterAlertType { get; private set; }

        /// <summary>
        /// Parses the given data to create a router alert IPv6 option.
        /// </summary>
        /// <param name="data">The data to parse to create the option.</param>
        /// <returns>Router alert IPv6 option according to the data parsed.</returns>
        public IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6OptionRouterAlert((IpV6RouterAlertType)data.ReadUShort(0, Endianity.Big));
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6Option other)
        {
            return EqualsData(other as IpV6OptionRouterAlert);
        }

        internal override int GetDataHashCode()
        {
            return RouterAlertType.GetHashCode();
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, (ushort)RouterAlertType, Endianity.Big);
        }

        private IpV6OptionRouterAlert()
            : this(IpV6RouterAlertType.MulticastListenerDiscovery)
        {
        }

        private bool EqualsData(IpV6OptionRouterAlert other)
        {
            return other != null &&
                   RouterAlertType == other.RouterAlertType;
        }
    }
}