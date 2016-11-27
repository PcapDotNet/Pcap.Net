using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    public class DhcpDomainNameServerOption : DhcpAddressListOption
    {
        public DhcpDomainNameServerOption(IList<IpV4Address> addresses) : base(DhcpOptionCode.DomainNameServerServer, addresses)
        {
        }

        internal static DhcpDomainNameServerOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            return new DhcpDomainNameServerOption(GetAddresses(data, length, ref offset));
        }
    }
}