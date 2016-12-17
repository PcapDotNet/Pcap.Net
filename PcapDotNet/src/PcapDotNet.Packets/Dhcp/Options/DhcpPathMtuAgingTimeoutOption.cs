using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option specifies the timeout (in seconds) to use when aging Path
    /// MTU values discovered by the mechanism defined in RFC 1191 [12].
    /// <pre>
    ///  Code   Len           Timeout
    /// +-----+-----+-----+-----+-----+-----+
    /// |  24 |  4  |  t1 |  t2 |  t3 |  t4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpPathMtuAgingTimeoutOption : DhcpUIntOption
    {
        /// <summary>
        /// create new DhcpPathMtuAgingTimeoutOption
        /// </summary>
        /// <param name="timeout">Timeout</param>
        public DhcpPathMtuAgingTimeoutOption(uint timeout) : base(timeout, DhcpOptionCode.PathMtuAgingTimeout)
        {
        }

        internal static DhcpPathMtuAgingTimeoutOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpPathMtuAgingTimeoutOption>(data, ref offset, p => new DhcpPathMtuAgingTimeoutOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Timeout
        /// </summary>
        public uint Timeout
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}