using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// <pre>
    ///  Code   Len     Length
    /// +-----+-----+-----+-----+
    /// |  57 |  2  |  l1 |  l2 |
    /// +-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpMaximumDhcpMessageSizeOption : DhcpOption
    {
        public DhcpMaximumDhcpMessageSizeOption(ushort maxLength) : base(DhcpOptionCode.MaximumDhcpMessageSize)
        {
            MaxLength = maxLength;
        }

        internal static DhcpMaximumDhcpMessageSizeOption Read(DataSegment data, ref int offset)
        {
            byte len = data[offset++];
            if (len != 2)
                throw new ArgumentException("Length of a DHCP MaximumDhcpMessageSize Option has to be 2");
            DhcpMaximumDhcpMessageSizeOption option = new DhcpMaximumDhcpMessageSizeOption(data.ReadUShort(offset, Endianity.Big));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, MaxLength, Endianity.Big);
        }

        public override byte Length
        {
            get
            {
                return 2;
            }
        }

        public ushort MaxLength
        {
            get;
            set;
        }
    }
}