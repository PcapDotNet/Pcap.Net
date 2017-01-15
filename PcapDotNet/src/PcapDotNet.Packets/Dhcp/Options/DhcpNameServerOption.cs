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
    /// The name server option specifies a list of IEN 116 [7] name servers
    /// available to the client.Servers SHOULD be listed in order of preference.
    /// <pre>
    ///  Code Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// |  5  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpNameServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpNameServerOption.
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpNameServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.NameServer)
        {
        }

        internal static DhcpNameServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpNameServerOption>(data, ref offset, p => new DhcpNameServerOption(p));
        }
    }
}