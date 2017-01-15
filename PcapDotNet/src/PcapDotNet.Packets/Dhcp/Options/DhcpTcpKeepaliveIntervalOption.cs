using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option specifies the interval (in seconds) that the client TCP
    /// should wait before sending a keepalive message on a TCP connection.
    /// <pre>
    ///  Code   Len           Time
    /// +-----+-----+-----+-----+-----+-----+
    /// |  38 |  4  |  t1 |  t2 |  t3 |  t4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpTcpKeepaliveIntervalOption : DhcpUIntOption
    {
        /// <summary>
        /// create new DhcpTcpKeepaliveIntervalOption.
        /// </summary>
        /// <param name="time">Time</param>
        public DhcpTcpKeepaliveIntervalOption(uint time) : base(time, DhcpOptionCode.TcpKeepaliveInterval)
        {
        }

        internal static DhcpTcpKeepaliveIntervalOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpTcpKeepaliveIntervalOption>(data, ref offset, p => new DhcpTcpKeepaliveIntervalOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Time.
        /// A value of zero
        /// indicates that the client should not generate keepalive messages on
        /// connections unless specifically requested by an application.
        /// </summary>
        public uint Time
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}