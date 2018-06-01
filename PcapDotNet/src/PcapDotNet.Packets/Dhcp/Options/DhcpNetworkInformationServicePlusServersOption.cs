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
    /// This option specifies a list of IP addresses indicating NIS+ servers
    /// available to the client.Servers SHOULD be listed in order of
    /// preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// |  65 |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpNetworkInformationServicePlusServersOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpNetworkInformationServicePlusServersOption.
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpNetworkInformationServicePlusServersOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.NetworkInformationServicePlusServers)
        {
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.NetworkInformationServicePlusServers)]
        internal static DhcpNetworkInformationServicePlusServersOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpNetworkInformationServicePlusServersOption>(data, ref offset, p => new DhcpNetworkInformationServicePlusServersOption(p));
        }
    }
}