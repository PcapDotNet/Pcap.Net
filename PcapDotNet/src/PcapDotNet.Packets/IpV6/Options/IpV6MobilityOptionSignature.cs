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
    /// | 16  | Signature                  |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.Signature)]
    public sealed class IpV6MobilityOptionSignature : IpV6MobilityOptionSingleDataSegmentField
    {
        /// <summary>
        /// Creates an option from the given signature.
        /// </summary>
        /// <param name="signature">Contains the mobile or correspondent node's signature, generated with the mobile or correspondent node's private key.</param>
        public IpV6MobilityOptionSignature(DataSegment signature)
            : base(IpV6MobilityOptionType.Signature, signature)
        {
        }

        /// <summary>
        /// Contains the mobile or correspondent node's signature, generated with the mobile or correspondent node's private key.
        /// </summary>
        public DataSegment Signature
        {
            get { return Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            return new IpV6MobilityOptionSignature(data);
        }

        private IpV6MobilityOptionSignature()
            : this(DataSegment.Empty)
        {
        }
    }
}