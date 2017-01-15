using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option specifies the timeout in seconds for ARP cache entries.
    /// <pre>
    ///  Code   Len           Time
    /// +-----+-----+-----+-----+-----+-----+
    /// |  35 |  4  |  t1 |  t2 |  t3 |  t4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpArpCacheTimeoutOption : DhcpUIntOption
    {
        /// <summary>
        /// create new ArpCacheTimeoutOption.
        /// </summary>
        /// <param name="time">Time</param>
        public DhcpArpCacheTimeoutOption(uint time) : base(time, DhcpOptionCode.ArpCacheTimeout)
        {
        }

        internal static DhcpArpCacheTimeoutOption Read(DataSegment data, ref int offset)
        {
            return DhcpUIntOption.Read(data, ref offset, p => new DhcpArpCacheTimeoutOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Time
        /// timeout in seconds for ARP cache entries.
        /// </summary>
        public uint Time
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}