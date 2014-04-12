namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 4285.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Timestamp                  |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.ReplayProtection)]
    public sealed class IpV6MobilityOptionReplayProtection : IpV6MobilityOptionULong
    {
        public IpV6MobilityOptionReplayProtection(ulong timestamp)
            : base(IpV6MobilityOptionType.ReplayProtection, timestamp)
        {
        }

        /// <summary>
        /// 64 bit timestamp.
        /// </summary>
        public ulong Timestamp
        {
            get { return Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            ulong timestamp;
            if (!Read(data, out timestamp))
                return null;

            return new IpV6MobilityOptionReplayProtection(timestamp);
        }

        private IpV6MobilityOptionReplayProtection()
            : this(0)
        {
        }
    }
}