using System;

namespace PcapDotNet.Packets
{
    internal sealed class OptionTypeRegistrationAttribute : Attribute
    {
        public OptionTypeRegistrationAttribute(Type optionTypeType, object optionType)
        {
            OptionTypeType = optionTypeType;
            OptionType = optionType;
        }

        public object OptionType { get; private set; }
        public Type OptionTypeType { get; private set; }
    }
}