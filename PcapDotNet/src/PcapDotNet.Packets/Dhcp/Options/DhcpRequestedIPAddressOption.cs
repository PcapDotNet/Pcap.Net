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
    ///  Code   Len          Address
    /// +-----+-----+-----+-----+-----+-----+
    /// |  50 |  4  |  a1 |  a2 |  a3 |  a4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpRequestedIPAddressOption : DhcpOption
    {
        public DhcpRequestedIPAddressOption(IpV4Address address) : base(DhcpOptionCode.RequestedIPAddress)
        {
            Address = address;
        }

        internal static DhcpRequestedIPAddressOption Read(DataSegment data, ref int offset)
        {
            if (data[offset++] != 4)
                throw new ArgumentException("Length of a DHCP RequestedIPAddressOption has to be 4");
            DhcpRequestedIPAddressOption option = new DhcpRequestedIPAddressOption(data.ReadIpV4Address(offset, Endianity.Big));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Address, Endianity.Big);
        }

        public override byte Length
        {
            get { return 4; }
        }

        public IpV4Address Address
        {
            get;
            set;
        }
    }
}