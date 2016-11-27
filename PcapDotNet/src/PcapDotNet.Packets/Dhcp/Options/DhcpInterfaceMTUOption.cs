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
    ///  Code   Len      MTU
    /// +-----+-----+-----+-----+
    /// |  26 |  2  |  m1 |  m2 |
    /// +-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpInterfaceMTUOption : DhcpOption
    {
        public DhcpInterfaceMTUOption(ushort mtu) : base(DhcpOptionCode.InterfaceMTU)
        {
            MTU = mtu;
        }

        internal static DhcpInterfaceMTUOption Read(DataSegment data, ref int offset)
        {
            if (data[offset++] != sizeof(ushort))
                throw new ArgumentException($"Length of a DHCP InterfaceMTU Option has to be {sizeof(ushort)}");
            DhcpInterfaceMTUOption option = new DhcpInterfaceMTUOption(data.ReadUShort(offset, Endianity.Big));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, MTU, Endianity.Big);
        }

        public override byte Length
        {
            get { return sizeof(ushort); }
        }

        public ushort MTU
        {
            get;
            set;
        }
    }
}