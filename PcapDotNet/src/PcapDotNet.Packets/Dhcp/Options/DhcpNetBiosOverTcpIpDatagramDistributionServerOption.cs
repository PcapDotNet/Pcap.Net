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
    /// The NetBIOS datagram distribution server (NBDD) option specifies a
    /// list of RFC 1001/1002 NBDD servers listed in order of preference.
    /// <pre>
    ///  Code   Len           Address 1              Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+----
    /// |  45 |  n  |  a1 |  a2 |  a3 |  a4 |  b1 |  b2 |  b3 |  b4 | ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+----
    /// </pre>
    /// </summary>
    public class DhcpNetBiosOverTcpIpDatagramDistributionServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpNetBiosOverTcpIpDatagramDistributionServerOption.
        /// </summary>
        /// <param name="addresses"></param>
        public DhcpNetBiosOverTcpIpDatagramDistributionServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.NetBiosOverTcpIpDatagramDistributionServer)
        {
        }

        internal static DhcpNetBiosOverTcpIpDatagramDistributionServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpNetBiosOverTcpIpDatagramDistributionServerOption>(data, ref offset, p => new DhcpNetBiosOverTcpIpDatagramDistributionServerOption(p));
        }
    }
}