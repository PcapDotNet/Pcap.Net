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
    /// | 69  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpSimpleMailTransportProtocolServerOption : DhcpAddressListOption
    {
        public DhcpSimpleMailTransportProtocolServerOption(IList<IpV4Address> addresses) : base(DhcpOptionCode.SimpleMailTransportProtocolServer, addresses)
        {
        }

        internal static DhcpSimpleMailTransportProtocolServerOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            return new DhcpSimpleMailTransportProtocolServerOption(GetAddresses(data, length, ref offset));
        }
    }
}