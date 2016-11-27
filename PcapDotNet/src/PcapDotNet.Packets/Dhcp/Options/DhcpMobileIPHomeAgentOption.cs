using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// <pre>
    ///  Code Len    Home Agent Addresses (zero or more)
    /// +-----+-----+-----+-----+-----+-----+--
    /// | 68  |  n  | a1  | a2  | a3  | a4  | ...
    /// +-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpMobileIPHomeAgentOption : DhcpAddressListOption
    {
        public DhcpMobileIPHomeAgentOption(IList<IpV4Address> addresses) : base(DhcpOptionCode.MobileIPHomeAgent, addresses)
        {
        }

        internal static DhcpMobileIPHomeAgentOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            return new DhcpMobileIPHomeAgentOption(GetAddresses(data, length, ref offset));
        }
    }
}