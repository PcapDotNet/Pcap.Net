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
    ///  Code   Len             Address
    /// +-----+-----+-----+-----+-----+-----+
    /// |  54 |  4  |  a1 |  a2 |  a3 |  a4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpServerIdentifierOption : DhcpOption
    {
        public DhcpServerIdentifierOption(IpV4Address address) : base(DhcpOptionCode.ServerIdentifier)
        {
            Address = address;
        }

        internal static DhcpServerIdentifierOption Read(DataSegment data, ref int offset)
        {
            if (data[offset++] != 4)
                throw new ArgumentException("Length of a DHCP ServerIdentifier Option has to be 4");
            DhcpServerIdentifierOption option = new DhcpServerIdentifierOption(data.ReadIpV4Address(offset, Endianity.Big));
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