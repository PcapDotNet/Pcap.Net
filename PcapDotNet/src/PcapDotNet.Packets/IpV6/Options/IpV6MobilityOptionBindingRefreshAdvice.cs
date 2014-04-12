namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+--------------+--------------+
    /// | Bit | 0-7          | 8-15         |
    /// +-----+--------------+--------------+
    /// | 0   | Option Type  | Opt Data Len |
    /// +-----+--------------+--------------+
    /// | 16  | Refresh Interval            |
    /// +-----+-----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.BindingRefreshAdvice)]
    public sealed class IpV6MobilityOptionBindingRefreshAdvice : IpV6MobilityOptionComplex
    {
        public const int OptionDataLength = sizeof(ushort);

        public IpV6MobilityOptionBindingRefreshAdvice(ushort refreshInterval)
            : base(IpV6MobilityOptionType.BindingRefreshAdvice)
        {
            RefreshInterval = refreshInterval;
        }

        /// <summary>
        /// Measured in units of four seconds, and indicates remaining time until the mobile node should send a new home registration to the home agent.
        /// The Refresh Interval must be set to indicate a smaller time interval than the Lifetime value of the Binding Acknowledgement.
        /// </summary>
        public ushort RefreshInterval { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6MobilityOptionBindingRefreshAdvice(data.ReadUShort(0, Endianity.Big));
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionBindingRefreshAdvice);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, RefreshInterval, Endianity.Big);
        }

        private IpV6MobilityOptionBindingRefreshAdvice()
            : this(0)
        {
        }

        private bool EqualsData(IpV6MobilityOptionBindingRefreshAdvice other)
        {
            return other != null &&
                   RefreshInterval == other.RefreshInterval;
        }
    }
}