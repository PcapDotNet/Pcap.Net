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
    /// The NetBIOS name server (NBNS) option specifies a list of RFC 1001/1002 [19] [20]
    /// NBNS name servers listed in order of preference.
    /// <pre>
    ///  Code   Len           Address 1              Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+----
    /// |  44 |  n  |  a1 |  a2 |  a3 |  a4 |  b1 |  b2 |  b3 |  b4 | ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+----
    /// </pre>
    /// </summary>
    public class DhcpNetBiosOverTcpIpNameServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpNetBiosOverTcpIpNameServerOption.
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpNetBiosOverTcpIpNameServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.NetBiosOverTcpIpNameServer)
        {
        }

        internal static DhcpNetBiosOverTcpIpNameServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpNetBiosOverTcpIpNameServerOption>(data, ref offset, p => new DhcpNetBiosOverTcpIpNameServerOption(p));
        }
    }
}