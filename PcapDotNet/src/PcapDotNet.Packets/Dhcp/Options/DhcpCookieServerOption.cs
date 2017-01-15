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
    /// The cookie server option specifies a list of RFC 865 [9] cookie
    /// servers available to the client.Servers SHOULD be listed in order
    /// of preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// |  8  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpCookieServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpCookieServerOption.
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpCookieServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.CookieServer)
        {
        }

        internal static DhcpCookieServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpCookieServerOption>(data, ref offset, (p) => new DhcpCookieServerOption(p));
        }
    }
}