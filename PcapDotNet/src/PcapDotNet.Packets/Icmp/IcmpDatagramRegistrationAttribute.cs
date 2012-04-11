using System;

namespace PcapDotNet.Packets.Icmp
{
    internal sealed class IcmpDatagramRegistrationAttribute : Attribute
    {
        public IcmpDatagramRegistrationAttribute(IcmpMessageType messageType)
        {
            MessageType = messageType;
        }

        public IcmpMessageType MessageType { get; private set; }
    }
}