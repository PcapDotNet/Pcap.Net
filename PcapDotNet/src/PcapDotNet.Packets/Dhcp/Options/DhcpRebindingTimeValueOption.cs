using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option specifies the time interval from address assignment until
    /// the client transitions to the REBINDING state.
    /// <pre>
    ///  Code   Len         T2 Interval
    /// +-----+-----+-----+-----+-----+-----+
    /// |  59 |  4  |  t1 |  t2 |  t3 |  t4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpRebindingTimeValueOption : DhcpUIntOption
    {
        /// <summary>
        /// create new DhcpRebindingTimeValueOption.
        /// </summary>
        /// <param name="t2Interval">T1 Interval</param>
        public DhcpRebindingTimeValueOption(uint t2Interval) : base(t2Interval, DhcpOptionCode.RebindingTimeValue)
        {
        }

        internal static DhcpRebindingTimeValueOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpRebindingTimeValueOption>(data, ref offset, p => new Options.DhcpRebindingTimeValueOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// T2 Interval.
        /// </summary>
        public uint T2Interval
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}