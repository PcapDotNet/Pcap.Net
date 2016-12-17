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
    /// This option specifies a list of IP addresses indicating NTP [18]
    /// servers available to the client.Servers SHOULD be listed in order
    /// of preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// |  42 |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpNetworkTimeProtocolServersOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpNetworkTimeProtocolServersOption
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpNetworkTimeProtocolServersOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.NetworkTimeProtocolServers)
        {
        }

        internal static DhcpNetworkTimeProtocolServersOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpNetworkTimeProtocolServersOption>(data, ref offset, p => new Options.DhcpNetworkTimeProtocolServersOption(p));
        }
    }
}