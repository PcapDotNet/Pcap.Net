using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132
    /// <pre>
    ///  Code   Len   TTL
    /// +-----+-----+-----+
    /// |  23 |  1  | ttl |
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpDefaultIPTimeToLiveOption : DhcpOption
    {
        public DhcpDefaultIPTimeToLiveOption(byte ttl) : base(DhcpOptionCode.DefaultIPTimeToLive)
        {
            TTL = ttl;
        }

        internal static DhcpDefaultIPTimeToLiveOption Read(DataSegment data, ref int offset)
        {
            if (data[offset++] != 1)
            {
                throw new ArgumentException("Length of a DHCP DefaultIPTimeToLive Option has to be 1");
            }
            DhcpDefaultIPTimeToLiveOption option = new DhcpDefaultIPTimeToLiveOption(data[offset++]);
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, TTL);
        }

        public override byte Length
        {
            get { return 1; }
        }

        public byte TTL
        {
            get;
            set;
        }
    }
}