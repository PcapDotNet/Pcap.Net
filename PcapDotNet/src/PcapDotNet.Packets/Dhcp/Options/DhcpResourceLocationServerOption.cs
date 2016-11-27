using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    public class DhcpResourceLocationServerOption : DhcpAddressListOption
    {
        public DhcpResourceLocationServerOption(IList<IpV4Address> addresses) : base(DhcpOptionCode.ResourceLocationServer, addresses)
        {
        }

        internal static DhcpResourceLocationServerOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            return new DhcpResourceLocationServerOption(GetAddresses(data, length, ref offset));
        }
    }
}