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
    /// This option specifies a list of X Window System [21] Font servers
    /// available to the client. Servers SHOULD be listed in order of
    /// preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+---
    /// |  48 |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |   ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpXWindowSystemFontServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpXWindowSystemFontServerOption
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpXWindowSystemFontServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.XWindowSystemFontServer)
        {
        }

        internal static DhcpXWindowSystemFontServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpXWindowSystemFontServerOption>(data, ref offset, p => new DhcpXWindowSystemFontServerOption(p));
        }
    }
}