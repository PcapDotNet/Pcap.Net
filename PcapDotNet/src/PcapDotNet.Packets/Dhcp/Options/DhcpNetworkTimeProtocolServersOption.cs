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
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// |  42 |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpNetworkTimeProtocolServersOption : DhcpAddressListOption
    {
        public DhcpNetworkTimeProtocolServersOption(IList<IpV4Address> addresses) : base(DhcpOptionCode.NetworkTimeProtocolServers, addresses)
        {
        }

        internal static DhcpNetworkTimeProtocolServersOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            return new DhcpNetworkTimeProtocolServersOption(GetAddresses(data, length, ref offset));
        }
    }
}