using System;
using PcapDotNet.Packets.Ip;
using PcapDotNet.Packets.Transport;

namespace PcapDotNet.Packets
{
    internal sealed class TcpOptionTypeRegistrationAttribute : OptionTypeRegistrationAttribute
    {
        public TcpOptionTypeRegistrationAttribute(TcpOptionType optionType)
        {
            TcpOptionType = optionType;
        }

        public TcpOptionType TcpOptionType { get; private set; }

        public override object OptionType
        {
            get { return TcpOptionType; }
        }

        public override Type OptionTypeType
        {
            get { return typeof(TcpOptionType); }
        }
    }
}