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
    /// | 16  | Timestamp                  |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.Timestamp)]
    public sealed class IpV6MobilityOptionTimestamp : IpV6MobilityOptionULong
    {
        public IpV6MobilityOptionTimestamp(ulong timestamp)
            : base(IpV6MobilityOptionType.Timestamp, timestamp)
        {
        }

        /// <summary>
        /// Timestamp.  
        /// The value indicates the number of seconds since January 1, 1970, 00:00 UTC, by using a fixed point format.
        /// In this format, the integer number of seconds is contained in the first 48 bits of the field, and the remaining 16 bits indicate the number of 1/65536 fractions of a second.
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

            return new IpV6MobilityOptionTimestamp(timestamp);
        }

        private IpV6MobilityOptionTimestamp()
            : this(0)
        {
        }
    }
}