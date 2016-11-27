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
    ///  Code   Len      Size
    /// +-----+-----+-----+-----+
    /// |  22 |  2  |  s1 |  s2 |
    /// +-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpMaximumDatagramReassemblySizeOption : DhcpOption
    {
        public DhcpMaximumDatagramReassemblySizeOption() : base(DhcpOptionCode.MaximumDatagramReassemblySize)
        {
        }

        public DhcpMaximumDatagramReassemblySizeOption(ushort size) : this()
        {
            Size = size;
        }

        internal static DhcpMaximumDatagramReassemblySizeOption Read(DataSegment data, ref int offset)
        {
            if (data[offset++] != 2)
            {
                throw new ArgumentException("Length of a DHCP MaximumDatagramReassemblySize Option has to be 2");
            }
            DhcpMaximumDatagramReassemblySizeOption option = new DhcpMaximumDatagramReassemblySizeOption(data.ReadUShort(offset, Endianity.Big));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Size, Endianity.Big);
        }

        public override byte Length
        {
            get { return 2; }
        }

        public ushort Size
        {
            get;
            set;
        }
    }
}