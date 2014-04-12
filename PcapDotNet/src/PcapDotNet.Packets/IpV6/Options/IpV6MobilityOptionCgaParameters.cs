namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 4866.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | CGA Parameters             |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.CgaParameters)]
    public sealed class IpV6MobilityOptionCgaParameters : IpV6MobilityOptionSingleDataSegmentField
    {
        public const int OptionDataMaxLength = 255;

        public IpV6MobilityOptionCgaParameters(DataSegment cgaParameters)
            : base(IpV6MobilityOptionType.CgaParameters, cgaParameters)
        {
        }

        /// <summary>
        /// Contains up to 255 bytes of the CGA Parameters data structure defined in RFC 3972.
        /// The concatenation of all CGA Parameters options in the order they appear in the Binding Update message 
        /// must result in the original CGA Parameters data structure.
        /// All CGA Parameters options in the Binding Update message except the last one must contain exactly 255 bytes in the CGA Parameters field,
        /// and the Option Length field must be set to 255 accordingly.
        /// All CGA Parameters options must appear directly one after another, that is, 
        /// a mobility option of a different type must not be placed in between two CGA Parameters options.
        /// </summary>
        public DataSegment CgaParameters
        {
            get { return Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length > OptionDataMaxLength)
                return null;

            return new IpV6MobilityOptionCgaParameters(data);
        }

        private IpV6MobilityOptionCgaParameters()
            : this(DataSegment.Empty)
        {
        }
    }
}