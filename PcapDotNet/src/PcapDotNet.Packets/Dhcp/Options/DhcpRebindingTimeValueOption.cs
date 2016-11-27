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
    ///  Code   Len         T1 Interval
    /// +-----+-----+-----+-----+-----+-----+
    /// |  59 |  4  |  t1 |  t2 |  t3 |  t4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpRebindingTimeValueOption : DhcpOption
    {
        public DhcpRebindingTimeValueOption(uint interval) : base(DhcpOptionCode.RebindingTimeValue)
        {
            Interval = interval;
        }

        internal static DhcpRebindingTimeValueOption Read(DataSegment data, ref int offset)
        {
            if (data[offset++] != 4)
            {
                throw new ArgumentException("Length of a DHCP RebindingTimeValue Option has to be 4");
            }
            DhcpRebindingTimeValueOption option = new DhcpRebindingTimeValueOption(data.ReadUInt(offset, Endianity.Big));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Interval, Endianity.Big);
        }

        public override byte Length
        {
            get { return 4; }
        }

        public uint Interval
        {
            get;
            set;
        }
    }
}