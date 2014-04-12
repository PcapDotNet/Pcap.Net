using System;

namespace PcapDotNet.Packets.IpV6
{
    internal sealed class IpV6FlowIdentificationSubOptionTypeRegistrationAttribute : Attribute
    {
        public IpV6FlowIdentificationSubOptionTypeRegistrationAttribute(IpV6FlowIdentificationSubOptionType optionType)
        {
            OptionType = optionType;
        }

        public IpV6FlowIdentificationSubOptionType OptionType { get; private set; }
    }
}