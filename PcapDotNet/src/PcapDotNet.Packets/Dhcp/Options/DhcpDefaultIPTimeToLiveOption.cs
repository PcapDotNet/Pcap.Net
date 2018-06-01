using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option specifies the default time-to-live that the client should
    /// use on outgoing datagrams.
    /// <pre>
    ///  Code   Len   TTL
    /// +-----+-----+-----+
    /// |  23 |  1  | ttl |
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpDefaultIPTimeToLiveOption : DhcpByteOption
    {
        /// <summary>
        /// create new DhcpDefaultIPTimeToLiveOption.
        /// </summary>
        /// <param name="ttl">TTL</param>
        public DhcpDefaultIPTimeToLiveOption(byte ttl) : base(ttl, DhcpOptionCode.DefaultIpTimeToLive)
        {
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.DefaultIpTimeToLive)]
        internal static DhcpDefaultIPTimeToLiveOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpDefaultIPTimeToLiveOption>(data, ref offset, p => new DhcpDefaultIPTimeToLiveOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// TTL.
        /// </summary>
        public byte Ttl
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}