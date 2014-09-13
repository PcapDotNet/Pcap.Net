using System;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Option Data                |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6MobilityOptionUnknown : IpV6MobilityOptionSingleDataSegmentField
    {
        /// <summary>
        /// Creates an instance from type and data.
        /// </summary>
        /// <param name="type">The type of the option.</param>
        /// <param name="data">The data of the option.</param>
        public IpV6MobilityOptionUnknown(IpV6MobilityOptionType type, DataSegment data)
            : base(type, data)
        {
        }

        /// <summary>
        /// The data of the option.
        /// </summary>
        public DataSegment Data
        {
            get { return Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            throw new InvalidOperationException("IpV6MobilityOptionUnknown shouldn't be registered.");
        }
    }
}
