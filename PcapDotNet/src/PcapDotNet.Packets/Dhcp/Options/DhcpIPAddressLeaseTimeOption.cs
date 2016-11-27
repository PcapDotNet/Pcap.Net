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
    public class DhcpIPAddressLeaseTimeOption : DhcpOption
    {
        public DhcpIPAddressLeaseTimeOption(uint leaseTime) : base(DhcpOptionCode.IPAddressLeaseTime)
        {
            LeaseTime = leaseTime;
        }

        internal static DhcpIPAddressLeaseTimeOption Read(DataSegment data, ref int offset)
        {
            if (data[offset++] != 4)
                throw new ArgumentException("Length of a DHCP IPAddressLeaseTime Option has to be 4");

            DhcpIPAddressLeaseTimeOption option = new DhcpIPAddressLeaseTimeOption(data.ReadUInt(offset, Endianity.Big));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, LeaseTime, Endianity.Big);
        }

        public override byte Length
        {
            get { return 4; }
        }

        public uint LeaseTime
        {
            get;
            set;
        }
    }
}