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
    /// layer for packet forwarding.
    /// <pre>
    ///  Code   Len  Value
    /// +-----+-----+-----+
    /// |  19 |  1  | 0/1 |
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpIPForwardingEnableOption : DhcpBooleanOption
    {
        /// <summary>
        /// create new DhcpIPForwardingEnableOption.
        /// </summary>
        /// <param name="value">Value</param>
        public DhcpIPForwardingEnableOption(bool value) : base(value, DhcpOptionCode.IPForwardingEnable)
        {
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.IPForwardingEnable)]
        internal static DhcpIPForwardingEnableOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpIPForwardingEnableOption>(data, ref offset, p => new DhcpIPForwardingEnableOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// A value of false means disable IP forwarding, and a value of true means enable IP forwarding.
        /// </summary>
        public bool Value
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}