namespace Packets
{
    public class IpV4OptionNoOperation : IpV4Option
    {
        public const int OptionLength = 1;

        public IpV4OptionNoOperation()
            : base(IpV4OptionType.NoOperation)
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