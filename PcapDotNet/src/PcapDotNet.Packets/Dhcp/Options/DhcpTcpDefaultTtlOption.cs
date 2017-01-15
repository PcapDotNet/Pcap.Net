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
    /// This option specifies the default TTL that the client should use when
    /// sending TCP segments.
    /// <pre>
    ///  Code   Len   TTL
    /// +-----+-----+-----+
    /// |  37 |  1  |  n  |
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpTcpDefaultTtlOption : DhcpByteOption
    {
        /// <summary>
        /// create new DhcpTcpDefaultTtlOption.
        /// </summary>
        /// <param name="ttl">TTL</param>
        public DhcpTcpDefaultTtlOption(byte ttl) : base(ttl, DhcpOptionCode.TcpDefaultTtl)
        {
        }

        internal static DhcpTcpDefaultTtlOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpTcpDefaultTtlOption>(data, ref offset, p => new DhcpTcpDefaultTtlOption(p));
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