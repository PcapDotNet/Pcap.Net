namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2473.
    /// <pre>
    /// +-----+----------------------------+--------+
    /// | Bit | 0-7                        | 8-15   |
    /// +-----+----------------------------+--------+
    /// | 0   | Option Type                | 1      |
    /// +-----+----------------------------+--------+
    /// | 16  | Tunnel Encapsulation Limit |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.TunnelEncapsulationLimit)]
    public sealed class IpV6OptionTunnelEncapsulationLimit : IpV6OptionComplex, IIpV6OptionComplexFactory
    {
        /// <summary>
        /// The number of bytes the option data takes.
        /// </summary>
        public const int OptionDataLength = sizeof(byte);

        /// <summary>
        /// Creates an option from the given tunnel encapsulation limit.
        /// </summary>
        /// <param name="tunnelEncapsulationLimit">How many further levels of encapsulation are permitted for the packet.</param>
        public IpV6OptionTunnelEncapsulationLimit(byte tunnelEncapsulationLimit)
            : base(IpV6OptionType.TunnelEncapsulationLimit)
        {
            TunnelEncapsulationLimit = tunnelEncapsulationLimit;
        }

        /// <summary>
        /// How many further levels of encapsulation are permitted for the packet.
        /// </summary>
        public byte TunnelEncapsulationLimit { get; private set; }

        /// <summary>
        /// Parses an option from the given data.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The option if parsing was successful, null otherwise.</returns>
        public IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6OptionTunnelEncapsulationLimit(data[0]);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6Option other)
        {
            return EqualsData(other as IpV6OptionTunnelEncapsulationLimit);
        }

        internal override int GetDataHashCode()
        {
            return TunnelEncapsulationLimit.GetHashCode();
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, TunnelEncapsulationLimit);
        }

        private IpV6OptionTunnelEncapsulationLimit() 
            : this(0)
        {
        }

        private bool EqualsData(IpV6OptionTunnelEncapsulationLimit other)
        {
            return other != null &&
                   TunnelEncapsulationLimit == other.TunnelEncapsulationLimit;
        }
    }
}