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
    /// |  58 |  4  |  t1 |  t2 |  t3 |  t4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpRenewalTimeValueOption : DhcpOption
    {
        public DhcpRenewalTimeValueOption(uint interval) : base(DhcpOptionCode.RenewalTimeValue)
        {
            Interval = interval;
        }

        internal static DhcpRenewalTimeValueOption Read(DataSegment data, ref int offset)
        {
            if (data[offset++] != 4)
            {
                throw new ArgumentException("Length of a DHCP RenewalTimeValue Option has to be 4");
            }
            DhcpRenewalTimeValueOption option = new DhcpRenewalTimeValueOption(data.ReadUInt(offset, Endianity.Big));
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