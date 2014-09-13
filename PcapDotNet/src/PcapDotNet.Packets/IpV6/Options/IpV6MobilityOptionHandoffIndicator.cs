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
    /// | 16  | Reserved    | HI           |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.HandoffIndicator)]
    public sealed class IpV6MobilityOptionHandoffIndicator : IpV6MobilityOptionReservedByteValueByte
    {
        /// <summary>
        /// Creates an instance from the given handoff indicator.
        /// </summary>
        /// <param name="handoffIndicator">Specifies the type of handoff.</param>
        public IpV6MobilityOptionHandoffIndicator(IpV6HandoffIndicator handoffIndicator)
            : base(IpV6MobilityOptionType.HandoffIndicator, (byte)handoffIndicator)
        {
        }

        /// <summary>
        /// Specifies the type of handoff.
        /// </summary>
        public IpV6HandoffIndicator HandoffIndicator
        {
            get { return (IpV6HandoffIndicator)Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            byte value;
            if (!Read(data, out value))
                return null;

            return new IpV6MobilityOptionHandoffIndicator((IpV6HandoffIndicator)value);
        }

        private IpV6MobilityOptionHandoffIndicator()
            : this(IpV6HandoffIndicator.AttachmentOverNewInterface)
        {
        }
    }
}