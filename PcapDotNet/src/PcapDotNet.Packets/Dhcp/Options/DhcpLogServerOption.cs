using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    public class DhcpLogServerOption : DhcpAddressListOption
    {
        public DhcpLogServerOption(IList<IpV4Address> addresses) : base(DhcpOptionCode.LogServer, addresses)
        {
        }

        internal static DhcpLogServerOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            return new DhcpLogServerOption(GetAddresses(data, length, ref offset));
        }
    }
}