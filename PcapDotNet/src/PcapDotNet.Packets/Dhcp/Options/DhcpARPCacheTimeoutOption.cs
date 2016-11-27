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
    ///  Code   Len           Time
    /// +-----+-----+-----+-----+-----+-----+
    /// |  35 |  4  |  t1 |  t2 |  t3 |  t4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpARPCacheTimeoutOption : DhcpOption
    {
        public DhcpARPCacheTimeoutOption() : base(DhcpOptionCode.ARPCacheTimeout)
        {
        }

        public DhcpARPCacheTimeoutOption(uint timeOffset) : this()
        {
            TimeOffset = timeOffset;
        }

        internal static DhcpARPCacheTimeoutOption Read(DataSegment data, ref int offset)
        {
            if (data[offset++] != 4)
            {
                throw new ArgumentException("Length of a DHCP ARPCacheTimeout Option has to be 4");
            }
            DhcpARPCacheTimeoutOption option = new DhcpARPCacheTimeoutOption(data.ReadUInt(offset, Endianity.Big));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, TimeOffset, Endianity.Big);
        }

        public override byte Length
        {
            get { return 4; }
        }

        public uint TimeOffset
        {
            get;
            set;
        }
    }
}