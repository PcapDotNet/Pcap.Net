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
    /// This option specifies the whether or not the client should send TCP
    /// keepalive messages with a octet of garbage for compatibility with
    /// older implementations.
    /// <pre>
    ///  Code   Len  Value
    /// +-----+-----+-----+
    /// |  39 |  1  | 0/1 |
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpTcpKeepaliveGarbageOption : DhcpBooleanOption
    {
        /// <summary>
        /// create new DhcpTcpKeepaliveGarbageOption.
        /// </summary>
        /// <param name="value">Value</param>
        public DhcpTcpKeepaliveGarbageOption(bool value) : base(value, DhcpOptionCode.TcpKeepaliveGarbage)
        {
        }

        internal static DhcpTcpKeepaliveGarbageOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpTcpKeepaliveGarbageOption>(data, ref offset, p => new DhcpTcpKeepaliveGarbageOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Value
        ///  A value of false indicates that a garbage octet
        ///  should not be sent. A value of true indicates that a garbage octet
        ///  should be sent.
        /// </summary>
        public bool Value
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}