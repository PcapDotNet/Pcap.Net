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
    /// This option specifies the address to which the client should transmit
    /// router solicitation requests.
    /// <pre>
    ///  Code   Len            Address
    /// +-----+-----+-----+-----+-----+-----+
    /// |  32 |  4  |  a1 |  a2 |  a3 |  a4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpRouterSolicitationAddressOption : DhcpSingleAddressOption
    {
        /// <summary>
        /// create new DhcpRouterSolicitationAddressOption.
        /// </summary>
        /// <param name="address">Address</param>
        public DhcpRouterSolicitationAddressOption(IpV4Address address) : base(address, DhcpOptionCode.RouterSolicitationAddress)
        {
        }

        internal static DhcpRouterSolicitationAddressOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpRouterSolicitationAddressOption>(data, ref offset, p => new DhcpRouterSolicitationAddressOption(p));
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