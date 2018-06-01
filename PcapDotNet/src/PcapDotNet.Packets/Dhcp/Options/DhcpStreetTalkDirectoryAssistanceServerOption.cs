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
    /// The StreetTalk Directory Assistance (STDA) server option specifies a
    /// list of STDA servers available to the client.Servers SHOULD be
    /// listed in order of preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// | 76  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpStreetTalkDirectoryAssistanceServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpStreetTalkDirectoryAssistanceServerOption.
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpStreetTalkDirectoryAssistanceServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.StreetTalkDirectoryAssistanceServer)
        {
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.StreetTalkDirectoryAssistanceServer)]
        internal static DhcpStreetTalkDirectoryAssistanceServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpStreetTalkDirectoryAssistanceServerOption>(data, ref offset, p => new Options.DhcpStreetTalkDirectoryAssistanceServerOption(p));
        }
    }
}