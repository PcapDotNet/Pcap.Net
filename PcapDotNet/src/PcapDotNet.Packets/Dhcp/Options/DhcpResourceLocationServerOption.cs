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
    /// This option specifies a list of RFC 887 [11] Resource Location
    /// servers available to the client.Servers SHOULD be listed in order
    /// of preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// |  11 |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpResourceLocationServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpResourceLocationServerOption
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpResourceLocationServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.ResourceLocationServer)
        {
        }

        internal static DhcpResourceLocationServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpResourceLocationServerOption>(data, ref offset, p => new Options.DhcpResourceLocationServerOption(p));
        }
    }
}