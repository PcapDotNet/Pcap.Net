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
    /// the client transitions to the RENEWING state.
    /// <pre>
    ///  Code   Len         T1 Interval
    /// +-----+-----+-----+-----+-----+-----+
    /// |  58 |  4  |  t1 |  t2 |  t3 |  t4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpRenewalTimeValueOption : DhcpUIntOption
    {
        /// <summary>
        /// DhcpRenewalTimeValueOption
        /// </summary>
        /// <param name="t1Interval"></param>
        public DhcpRenewalTimeValueOption(uint t1Interval) : base(t1Interval, DhcpOptionCode.RenewalTimeValue)
        {
        }

        internal static DhcpRenewalTimeValueOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpRenewalTimeValueOption>(data, ref offset, p => new DhcpRenewalTimeValueOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// T1 Interval
        /// </summary>
        public uint T1Interval
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}