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
    ///  Code   Len           Timeout
    /// +-----+-----+-----+-----+-----+-----+
    /// |  24 |  4  |  t1 |  t2 |  t3 |  t4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpPathMTUAgingTimeoutOption : DhcpOption
    {
        public DhcpPathMTUAgingTimeoutOption() : base(DhcpOptionCode.PathMTUAgingTimeout)
        {
        }

        public DhcpPathMTUAgingTimeoutOption(uint timeout) : this()
        {
            Timeout = timeout;
        }

        internal static DhcpPathMTUAgingTimeoutOption Read(DataSegment data, ref int offset)
        {
            if (data[offset++] != 4)
            {
                throw new ArgumentException("Length of a DHCP PathMTUAgingTimeout Option has to be 4");
            }
            DhcpPathMTUAgingTimeoutOption option = new DhcpPathMTUAgingTimeoutOption(data.ReadUInt(offset, Endianity.Big));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Timeout, Endianity.Big);
        }

        public override byte Length
        {
            get { return 4; }
        }

        public uint Timeout
        {
            get;
            set;
        }
    }
}