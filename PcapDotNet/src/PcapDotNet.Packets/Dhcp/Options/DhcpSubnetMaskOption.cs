using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132
    /// <pre>
    ///  Code   Len        Subnet Mask
    /// +-----+-----+-----+-----+-----+-----+
    /// |  1  |  4  |  m1 |  m2 |  m3 |  m4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpSubnetMaskOption : DhcpOption
    {
        public DhcpSubnetMaskOption(IpV4Address subnetMask) : base(DhcpOptionCode.SubnetMask)
        {
            SubnetMask = subnetMask;
        }

        internal static DhcpSubnetMaskOption Read(DataSegment data, ref int offset)
        {
            if (data[offset++] != IpV4Address.SizeOf)
                throw new ArgumentException("Length of a DHCP SubnetMask Option has to be 4");
            DhcpSubnetMaskOption option = new DhcpSubnetMaskOption(data.ReadIpV4Address(offset, Endianity.Big));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, SubnetMask, Endianity.Big);
        }

        public override byte Length
        {
            get { return IpV4Address.SizeOf; }
        }

        public IpV4Address SubnetMask
        {
            get;
            set;
        }
    }
}