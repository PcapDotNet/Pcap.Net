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
    /// The NNTP server option specifies a list of NNTP available to the
    /// client.Servers SHOULD be listed in order of preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// | 71  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpNetworkNewsTransportProtocolServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpNetworkNewsTransportProtocolServerOption.
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpNetworkNewsTransportProtocolServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.NetworkNewsTransportProtocolServer)
        {
        }

        internal static DhcpNetworkNewsTransportProtocolServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpNetworkNewsTransportProtocolServerOption>(data, ref offset, p => new Options.DhcpNetworkNewsTransportProtocolServerOption(p));
        }
    }
}