using System;

namespace PcapDotNet.Packets
{
    public class IpV4OptionSimple : IpV4Option
    {
        public const int OptionLength = 1;

        public IpV4OptionSimple(IpV4OptionType optionType)
            : base(optionType)
        {
            if (optionType != IpV4OptionType.EndOfOptionList &&
                optionType != IpV4OptionType.NoOperation)
            {
                throw new ArgumentException("OptionType " + optionType + " Can't be a simple option", "optionType");
            }

        }

        public override int Length
        {
            get { return OptionLength; }
        }

        public override bool IsAppearsAtMostOnce
        {
            get { return false; }
        }
    }
}