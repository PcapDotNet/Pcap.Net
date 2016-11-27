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
    ///  Code   Len           Address 1              Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+----
    /// |  44 |  n  |  a1 |  a2 |  a3 |  a4 |  b1 |  b2 |  b3 |  b4 | ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+----
    /// </pre>
    /// </summary>
    public class DhcpNetBIOSOverTCPIPNameServerOption : DhcpAddressListOption
    {
        public DhcpNetBIOSOverTCPIPNameServerOption(IList<IpV4Address> addresses) : base(DhcpOptionCode.NetBIOSOverTCPIPNameServer, addresses)
        {
        }

        internal static DhcpNetBIOSOverTCPIPNameServerOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            return new DhcpNetBIOSOverTCPIPNameServerOption(GetAddresses(data, length, ref offset));
        }
    }
}