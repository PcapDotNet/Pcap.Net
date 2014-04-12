using System;

namespace PcapDotNet.Packets.IpV6
{
    internal sealed class IpV6MobilityOptionTypeRegistrationAttribute : Attribute
    {
        public IpV6MobilityOptionTypeRegistrationAttribute(IpV6MobilityOptionType optionType)
        {
            OptionType = optionType;
        }

        public IpV6MobilityOptionType OptionType { get; private set; }
    }
}