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
    ///  Code   Len        Time Offset
    /// +-----+-----+-----+-----+-----+-----+
    /// |  1  |  4  |  n1 |  n2 |  n3 |  n4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpTimeOffsetOption : DhcpOption
    {
        public DhcpTimeOffsetOption(uint timeOffset) : base(DhcpOptionCode.TimeOffset)
        {
            TimeOffset = timeOffset;
        }

        internal static DhcpTimeOffsetOption Read(DataSegment data, ref int offset)
        {
            if (data[offset++] != 4)
            {
                throw new ArgumentException("Length of a DHCP TimeOffset Option has to be 4");
            }
            DhcpTimeOffsetOption option = new DhcpTimeOffsetOption(data.ReadUInt(offset, Endianity.Big));
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