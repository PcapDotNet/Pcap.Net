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
    /// This option is used in a client request (DHCPDISCOVER) to allow the
    /// client to request that a particular IP address be assigned.
    /// <pre>
    ///  Code   Len          Address
    /// +-----+-----+-----+-----+-----+-----+
    /// |  50 |  4  |  a1 |  a2 |  a3 |  a4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpRequestedIPAddressOption : DhcpSingleAddressOption
    {
        /// <summary>
        /// create new DhcpRequestedIPAddressOption.
        /// </summary>
        /// <param name="address">Address</param>
        public DhcpRequestedIPAddressOption(IpV4Address address) : base(address, DhcpOptionCode.RequestedIPAddress)
        {
        }

        internal static DhcpRequestedIPAddressOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpRequestedIPAddressOption>(data, ref offset, p => new Options.DhcpRequestedIPAddressOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Address.
        /// </summary>
        public IpV4Address Address
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}