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
    /// This option specifies a list of IP addresses of systems that are
    /// running the X Window System Display Manager and are available to the
    /// client.
    /// Addresses SHOULD be listed in order of preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+---
    /// |  49 |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |   ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpXWindowSystemDisplayManagerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpXWindowSystemDisplayManagerOption.
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpXWindowSystemDisplayManagerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.XWindowSystemDisplayManager)
        {
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.XWindowSystemDisplayManager)]
        internal static DhcpXWindowSystemDisplayManagerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpXWindowSystemDisplayManagerOption>(data, ref offset, p => new Options.DhcpXWindowSystemDisplayManagerOption(p));
        }
    }
}