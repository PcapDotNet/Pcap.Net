using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// <pre>
    ///  Code   Len           Time
    /// +-----+-----+-----+-----+-----+-----+
    /// |  38 |  4  |  t1 |  t2 |  t3 |  t4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpTCPKeepaliveIntervalOption : DhcpOption
    {
        public DhcpTCPKeepaliveIntervalOption(uint time) : base(DhcpOptionCode.TCPKeepaliveInterval)
        {
            Time = time;
        }

        internal static DhcpTCPKeepaliveIntervalOption Read(DataSegment data, ref int offset)
        {
            if (data[offset++] != 4)
            {
                throw new ArgumentException("Length of a DHCP TCPKeepaliveInterval Option has to be 4");
            }
            DhcpTCPKeepaliveIntervalOption option = new DhcpTCPKeepaliveIntervalOption(data.ReadUInt(offset, Endianity.Big));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Time, Endianity.Big);
        }

        public override byte Length
        {
            get { return 4; }
        }

        public uint Time
        {
            get;
            set;
        }
    }
}