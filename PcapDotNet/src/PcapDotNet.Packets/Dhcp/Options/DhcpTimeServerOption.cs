using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    public class DhcpTimeServerOption : DhcpAddressListOption
    {
        public DhcpTimeServerOption(IList<IpV4Address> addresses) : base(DhcpOptionCode.TimeServer, addresses)
        {
        }

        internal static DhcpTimeServerOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            return new DhcpTimeServerOption(GetAddresses(data, length, ref offset));
        }
    }
}