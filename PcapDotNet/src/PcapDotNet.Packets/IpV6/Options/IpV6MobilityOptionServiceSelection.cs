namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5149.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Identifier                 |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.ServiceSelection)]
    public sealed class IpV6MobilityOptionServiceSelection : IpV6MobilityOptionSingleDataSegmentField
    {
        public IpV6MobilityOptionServiceSelection(DataSegment data)
            : base(IpV6MobilityOptionType.ServiceSelection, data)
        {
        }

        /// <summary>
        /// Encoded service identifier string used to identify the requested service.
        /// The identifier string length is between 1 and 255 octets.
        /// This specification allows international identifier strings that are based on the use of Unicode characters, encoded as UTF-8,
        /// and formatted using Normalization Form KC (NFKC).
        /// 
        /// 'ims', 'voip', and 'voip.companyxyz.example.com' are valid examples of Service Selection option Identifiers.
        /// At minimum, the Identifier must be unique among the home agents to which the mobile node is authorized to register.
        /// </summary>
        public DataSegment Identifier
        {
            get { return Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            return new IpV6MobilityOptionServiceSelection(data);
        }

        private IpV6MobilityOptionServiceSelection()
            : this(DataSegment.Empty)
        {
        }
    }
}