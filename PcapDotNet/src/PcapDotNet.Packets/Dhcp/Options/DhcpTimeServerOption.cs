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
    /// The time server option specifies a list of RFC 868 [6] time servers
    /// available to the client.Servers SHOULD be listed in order of
    /// preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// |  4  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpTimeServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpTimeServerOption.
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpTimeServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.TimeServer)
        {
        }

        internal static DhcpTimeServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpTimeServerOption>(data, ref offset, p => new Options.DhcpTimeServerOption(p));
        }
    }
}