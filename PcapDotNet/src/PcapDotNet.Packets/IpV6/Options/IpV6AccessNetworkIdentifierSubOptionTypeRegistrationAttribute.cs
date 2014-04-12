using System;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6757.
    /// </summary>
    internal sealed class IpV6AccessNetworkIdentifierSubOptionTypeRegistrationAttribute : Attribute
    {
        public IpV6AccessNetworkIdentifierSubOptionTypeRegistrationAttribute(IpV6AccessNetworkIdentifierSubOptionType optionType)
        {
            OptionType = optionType;
        }

        public IpV6AccessNetworkIdentifierSubOptionType OptionType { get; private set; }
    }
}