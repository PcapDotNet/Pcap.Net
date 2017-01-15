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
    /// The POP3 server option specifies a list of POP3 available to the
    /// client.Servers SHOULD be listed in order of preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// | 70  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpPostOfficeProtocolServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpPostOfficeProtocolServerOption.
        /// </summary>
        /// <param name="addresses"></param>
        public DhcpPostOfficeProtocolServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.PostOfficeProtocolServer)
        {
        }

        internal static DhcpPostOfficeProtocolServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpPostOfficeProtocolServerOption>(data, ref offset, p => new DhcpPostOfficeProtocolServerOption(p));
        }
    }
}