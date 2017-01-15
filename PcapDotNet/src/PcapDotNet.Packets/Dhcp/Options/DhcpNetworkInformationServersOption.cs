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
    /// This option specifies a list of IP addresses indicating NIS servers
    /// available to the client.Servers SHOULD be listed in order of
    /// preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// |  41 |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpNetworkInformationServersOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpNetworkInformationServersOption.
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpNetworkInformationServersOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.NetworkInformationServers)
        {
        }

        internal static DhcpNetworkInformationServersOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpNetworkInformationServersOption>(data, ref offset, p => new Options.DhcpNetworkInformationServersOption(p));
        }
    }
}