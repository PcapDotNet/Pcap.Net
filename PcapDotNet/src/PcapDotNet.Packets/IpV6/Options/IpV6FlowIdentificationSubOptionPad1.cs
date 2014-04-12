namespace PcapDotNet.Packets.IpV6
{
    public sealed class IpV6FlowIdentificationSubOptionPad1 : IpV6FlowIdentificationSubOptionSimple
    {
        public const int OptionLength = sizeof(byte);

        public IpV6FlowIdentificationSubOptionPad1()
            : base(IpV6FlowIdentificationSubOptionType.Pad1)
        {
        }
    }
}