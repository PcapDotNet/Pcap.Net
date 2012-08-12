using System;

namespace PcapDotNet.Packets.Ip
{
    internal abstract class OptionTypeRegistrationAttribute : Attribute
    {
        public abstract object OptionType { get; }
        public abstract Type OptionTypeType { get; }
    }
}