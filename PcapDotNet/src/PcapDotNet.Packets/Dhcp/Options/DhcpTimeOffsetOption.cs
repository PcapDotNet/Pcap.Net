using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// The time offset field specifies the offset of the client's subnet in
    /// seconds from Coordinated Universal Time(UTC).
    /// <pre>
    ///  Code   Len        Time Offset
    /// +-----+-----+-----+-----+-----+-----+
    /// |  2  |  4  |  n1 |  n2 |  n3 |  n4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpTimeOffsetOption : DhcpIntOption
    {
        /// <summary>
        /// create new DhcpTimeOffsetOption
        /// </summary>
        /// <param name="timeOffset">Time Offset</param>
        public DhcpTimeOffsetOption(int timeOffset) : base(timeOffset, DhcpOptionCode.TimeOffset)
        {
        }

        internal static DhcpTimeOffsetOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpTimeOffsetOption>(data, ref offset, p => new DhcpTimeOffsetOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Time Offset
        /// The offset is
        /// expressed as a two's complement 32-bit integer.  A positive offset
        /// indicates a location east of the zero meridian and a negative offset
        /// indicates a location west of the zero meridian.
        /// </summary>
        public int TimeOffset
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}