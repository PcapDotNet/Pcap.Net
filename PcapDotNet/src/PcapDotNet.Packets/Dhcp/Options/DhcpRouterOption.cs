using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    public class DhcpRouterOption : DhcpAddressListOption
    {
        public DhcpRouterOption(IList<IpV4Address> addresses) : base(DhcpOptionCode.Router, addresses)
        {
        }

        internal static DhcpRouterOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            return new DhcpRouterOption(GetAddresses(data, length, ref offset));
        }
    }
}