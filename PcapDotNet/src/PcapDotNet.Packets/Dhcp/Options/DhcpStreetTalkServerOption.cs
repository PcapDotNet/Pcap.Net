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
    /// The StreetTalk server option specifies a list of StreetTalk servers
    /// available to the client.Servers SHOULD be listed in order of
    /// preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// | 75  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpStreetTalkServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpStreetTalkServerOption
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpStreetTalkServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.StreetTalkServer)
        {
        }

        internal static DhcpStreetTalkServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpStreetTalkServerOption>(data, ref offset, p => new DhcpStreetTalkServerOption(p));
        }
    }
}