namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5213.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved    | ATT          |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.AccessTechnologyType)]
    public sealed class IpV6MobilityOptionAccessTechnologyType : IpV6MobilityOptionReservedByteValueByte
    {
        /// <summary>
        /// Creates an option according to the given access technology type.
        /// </summary>
        /// <param name="accessTechnologyType">Specifies the access technology through which the mobile node is connected to the access link on the mobile access gateway.</param>
        public IpV6MobilityOptionAccessTechnologyType(IpV6AccessTechnologyType accessTechnologyType)
            : base(IpV6MobilityOptionType.AccessTechnologyType, (byte)accessTechnologyType)
        {
        }

        /// <summary>
        /// Specifies the access technology through which the mobile node is connected to the access link on the mobile access gateway.
        /// </summary>
        public IpV6AccessTechnologyType AccessTechnologyType
        {
            get { return (IpV6AccessTechnologyType)Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            byte value;
            if (!Read(data, out value))
                return null;

            return new IpV6MobilityOptionAccessTechnologyType((IpV6AccessTechnologyType)value);
        }

        private IpV6MobilityOptionAccessTechnologyType()
            : this(IpV6AccessTechnologyType.Reserved)
        {
        }
    }
}