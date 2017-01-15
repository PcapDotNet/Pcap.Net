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
    /// This option specifies the broadcast address in use on the client's
    /// subnet.
    /// <pre>
    ///  Code   Len     Broadcast Address
    /// +-----+-----+-----+-----+-----+-----+
    /// |  28 |  4  |  b1 |  b2 |  b3 |  b4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpBroadcastAddressOption : DhcpSingleAddressOption
    {
        /// <summary>
        /// create new DhcpBroadcastAddressOption.
        /// </summary>
        /// <param name="broadcastAddress">Broadcast Address</param>
        public DhcpBroadcastAddressOption(IpV4Address broadcastAddress) : base(broadcastAddress, DhcpOptionCode.BroadcastAddress)
        {
        }

        internal static DhcpBroadcastAddressOption Read(DataSegment data, ref int offset)
        {
            return DhcpSingleAddressOption.Read(data, ref offset, p => new DhcpBroadcastAddressOption(p));
        }

        /// <summary>
        /// Broadcast Address
        /// </summary>
        public IpV4Address BroadcastAddress
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}