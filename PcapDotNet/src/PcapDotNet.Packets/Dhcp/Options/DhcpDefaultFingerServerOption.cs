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
    /// The Finger server option specifies a list of Finger available to the
    /// client.Servers SHOULD be listed in order of preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// | 73  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpDefaultFingerServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpDefaultFingerServerOption.
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpDefaultFingerServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.DefaultFingerServer)
        {
        }

        internal static DhcpDefaultFingerServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpDefaultFingerServerOption>(data, ref offset, (p) => new DhcpDefaultFingerServerOption(p));
        }
    }
}