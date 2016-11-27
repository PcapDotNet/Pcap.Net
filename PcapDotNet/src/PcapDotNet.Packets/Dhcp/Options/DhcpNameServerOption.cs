using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    public class DhcpNameServerOption : DhcpAddressListOption
    {
        public DhcpNameServerOption(IList<IpV4Address> addresses) : base(DhcpOptionCode.NameServer, addresses)
        {
        }

        internal static DhcpNameServerOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            return new DhcpNameServerOption(GetAddresses(data, length, ref offset));
        }
    }
}