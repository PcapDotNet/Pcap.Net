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
    /// The subnet mask option specifies the client's subnet mask as per RFC
    /// 950 [5].
    /// If both the subnet mask and the router option are specified in a DHCP
    /// reply, the subnet mask option MUST be first.
    /// <pre>
    ///  Code   Len        Subnet Mask
    /// +-----+-----+-----+-----+-----+-----+
    /// |  1  |  4  |  m1 |  m2 |  m3 |  m4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpSubnetMaskOption : DhcpSingleAddressOption
    {
        /// <summary>
        /// create new DhcpSubnetMaskOption.
        /// </summary>
        /// <param name="subnetMask">Subnet Mask></param>
        public DhcpSubnetMaskOption(IpV4Address subnetMask) : base(subnetMask, DhcpOptionCode.SubnetMask)
        {
        }

        internal static DhcpSubnetMaskOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpSubnetMaskOption>(data, ref offset, p => new Options.DhcpSubnetMaskOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Subnet Mask.
        /// </summary>
        public IpV4Address SubnetMask
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}