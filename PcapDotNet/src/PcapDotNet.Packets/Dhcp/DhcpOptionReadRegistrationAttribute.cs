using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.Dhcp.Options;

namespace PcapDotNet.Packets.Dhcp
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal sealed class DhcpOptionReadRegistrationAttribute : Attribute
    {
        public DhcpOptionReadRegistrationAttribute(DhcpOptionCode optionCode)
        {
            OptionCode = optionCode;
        }

        public DhcpOptionCode OptionCode { get; private set; }
    }
}
