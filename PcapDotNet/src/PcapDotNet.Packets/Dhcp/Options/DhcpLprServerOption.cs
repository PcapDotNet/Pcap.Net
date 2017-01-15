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
    /// The LPR server option specifies a list of RFC 1179 [10] line printer
    /// servers available to the client. Servers SHOULD be listed in order
    /// of preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// |  9  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpLprServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpLprServerOption.
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpLprServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.LprServer)
        {
        }

        internal static DhcpLprServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpLprServerOption>(data, ref offset, p => new DhcpLprServerOption(p));
        }
    }
}