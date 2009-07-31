namespace Packets
{
    public class IpV4OptionEndOfOptionsList : IpV4Option
    {
        public const int OptionLength = 1;

        public IpV4OptionEndOfOptionsList()
            : base(IpV4OptionType.EndOfOptionList)
        {
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