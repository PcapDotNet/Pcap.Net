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
        public const int OptionLength = sizeof(byte);

        public IpV6MobilityOptionPad1()
            : base(IpV6MobilityOptionType.Pad1)
        {
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