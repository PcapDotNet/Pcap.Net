using System;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6275.
    /// +-----+-----+
    /// | Bit | 0-7 |
    /// +-----+-----+
    /// | 0   | 0   |
    /// +-----+-----+
    /// </summary>
    public sealed class IpV6MobilityOptionPad1 : IpV6MobilityOption
    {
        /// <summary>
        /// The number of bytes the option takes.
        /// </summary>
        public const int OptionLength = sizeof(byte);

        /// <summary>
        /// Constructs an instance.
        /// </summary>
        public IpV6MobilityOptionPad1()
            : base(IpV6MobilityOptionType.Pad1)
        {
        }

        public override int Length
        {
            get { return sizeof(byte); }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return true;
        }

        internal override int GetDataHashCode()
        {
            return 0;
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            throw new InvalidOperationException("Pad1 options shouldn't be registered.");
        }
    }
}