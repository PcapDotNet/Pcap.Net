using System;

namespace PcapDotNet.Packets.IpV4
{
    public class IpV4OptionSimple : IpV4Option
    {
        public const int OptionLength = 1;

        public override int Length
        {
            get { return OptionLength; }
        }

        public override bool IsAppearsAtMostOnce
        {
            get { return false; }
        }

        internal IpV4OptionSimple(IpV4OptionType optionType)
            : base(optionType)
        {
        }
    }
}