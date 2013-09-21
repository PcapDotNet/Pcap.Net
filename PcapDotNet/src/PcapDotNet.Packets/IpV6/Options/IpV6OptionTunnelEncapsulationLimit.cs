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
    public class IpV6OptionTunnelEncapsulationLimit : IpV6OptionComplex, IIpV6OptionComplexFactory
    {
        public const int OptionDataLength = sizeof(byte);

        public IpV6OptionTunnelEncapsulationLimit(byte tunnelEncapsulationLimit)
            : base(IpV6OptionType.TunnelEncapsulationLimit)
        {
            TunnelEncapsulationLimit = tunnelEncapsulationLimit;
        }

        /// <summary>
        /// How many further levels of encapsulation are permitted for the packet.
        /// </summary>
        public byte TunnelEncapsulationLimit { get; private set; }

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

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, TunnelEncapsulationLimit);
        }

        private IpV6OptionTunnelEncapsulationLimit() 
            : this(0)
        {
        }
    }
}