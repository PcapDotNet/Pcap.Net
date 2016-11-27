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
    ///  Code   Len     Broadcast Address
    /// +-----+-----+-----+-----+-----+-----+
    /// |  28 |  4  |  b1 |  b2 |  b3 |  b4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpBroadcastAddressOption : DhcpOption
    {
        public DhcpBroadcastAddressOption(IpV4Address broadcastAddress) : base(DhcpOptionCode.BroadcastAddress)
        {
            BroadcastAddress = broadcastAddress;
        }

        internal static DhcpBroadcastAddressOption Read(DataSegment data, ref int offset)
        {
            if (data[offset++] != 4)
                throw new ArgumentException("Length of a DHCP SubnetMask Option has to be 4");
            DhcpBroadcastAddressOption option = new DhcpBroadcastAddressOption(data.ReadIpV4Address(offset, Endianity.Big));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, BroadcastAddress, Endianity.Big);
        }

        public override byte Length
        {
            get { return 4; }
        }

        public IpV4Address BroadcastAddress
        {
            get;
            set;
        }
    }
}