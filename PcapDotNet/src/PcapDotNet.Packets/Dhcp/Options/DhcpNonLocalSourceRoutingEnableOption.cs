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
    /// This option specifies whether the client should configure its IP
    /// layer to allow forwarding of datagrams with non-local source routes.
    /// <pre>
    ///  Code   Len  Value
    /// +-----+-----+-----+
    /// |  20 |  1  | 0/1 |
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpNonLocalSourceRoutingEnableOption : DhcpBooleanOption
    {
        /// <summary>
        /// create new DhcpNonLocalSourceRoutingEnableOption.
        /// </summary>
        /// <param name="value">Value</param>
        public DhcpNonLocalSourceRoutingEnableOption(bool value) : base(value, DhcpOptionCode.NonLocalSourceRoutingEnable)
        {
        }

        internal static DhcpNonLocalSourceRoutingEnableOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpNonLocalSourceRoutingEnableOption>(data, ref offset, p => new DhcpNonLocalSourceRoutingEnableOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Value
        /// A value of false means disallow forwarding of such datagrams, and a value of true
        /// means allow forwarding.
        /// </summary>
        public bool Value
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}