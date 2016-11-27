using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    public class DhcpLPRServerOption : DhcpAddressListOption
    {
        public DhcpLPRServerOption(IList<IpV4Address> addresses) : base(DhcpOptionCode.LPRServer, addresses)
        {
        }

        internal static DhcpLPRServerOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            return new DhcpLPRServerOption(GetAddresses(data, length, ref offset));
        }
    }
}