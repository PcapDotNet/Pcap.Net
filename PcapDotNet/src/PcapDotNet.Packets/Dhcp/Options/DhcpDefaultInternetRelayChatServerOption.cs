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
    /// The IRC server option specifies a list of IRC available to the
    /// client.Servers SHOULD be listed in order of preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// | 74  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpDefaultInternetRelayChatServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpDefaultInternetRelayChatServerOption.
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpDefaultInternetRelayChatServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.DefaultInternetRelayChatServer)
        {
        }

        internal static DhcpDefaultInternetRelayChatServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpDefaultInternetRelayChatServerOption>(data, ref offset, (p) => new DhcpDefaultInternetRelayChatServerOption(p));
        }
    }
}