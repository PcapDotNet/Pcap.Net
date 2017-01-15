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
    /// This option specifies whether or not the client may assume that all
    /// subnets of the IP network to which the client is connected use the
    /// same MTU as the subnet of that network to which the client is
    /// directly connected.
    /// <pre>
    ///  Code   Len  Value
    /// +-----+-----+-----+
    /// |  27 |  1  | 0/1 |
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpAllSubnetsAreLocalOption : DhcpBooleanOption
    {
        /// <summary>
        /// create new AllSubnetsAreLocalOption.
        /// </summary>
        /// <param name="value">Value</param>
        public DhcpAllSubnetsAreLocalOption(bool value) : base(value, DhcpOptionCode.AllSubnetsAreLocal)
        {
        }

        internal static DhcpAllSubnetsAreLocalOption Read(DataSegment data, ref int offset)
        {
            return DhcpBooleanOption.Read(data, ref offset, p => new DhcpAllSubnetsAreLocalOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// True indicates that all subnets share
        /// the same MTU. False means that the client should assume that
        /// some subnets of the directly connected network may have smaller MTUs.
        /// </summary>
        public bool Value
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}