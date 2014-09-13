namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5096.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Data                       |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.Experimental)]
    public sealed class IpV6MobilityOptionExperimental : IpV6MobilityOptionSingleDataSegmentField
    {
        /// <summary>
        /// Creates an option from the given data.
        /// </summary>
        /// <param name="data">Data related to the experimental protocol extension.</param>
        public IpV6MobilityOptionExperimental(DataSegment data)
            : base(IpV6MobilityOptionType.Experimental, data)
        {
        }

        /// <summary>
        /// Data related to the experimental protocol extension.
        /// </summary>
        public DataSegment Data
        {
            get { return Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            return new IpV6MobilityOptionExperimental(data);
        }

        private IpV6MobilityOptionExperimental() 
            : this(DataSegment.Empty)
        {
        }
    }
}