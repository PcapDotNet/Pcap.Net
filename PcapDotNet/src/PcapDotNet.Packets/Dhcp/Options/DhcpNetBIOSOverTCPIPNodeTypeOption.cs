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
    ///  Code   Len  Node Type
    /// +-----+-----+-----------+
    /// |  46 |  1  |   flags   |
    /// +-----+-----+-----------+
    /// </pre>
    /// </summary>
    public class DhcpNetBIOSOverTCPIPNodeTypeOption : DhcpOption
    {
        public DhcpNetBIOSOverTCPIPNodeTypeOption(NodeType type) : base(DhcpOptionCode.NetBIOSOverTCPIPNodeType)
        {
            Type = type;
        }

        internal static DhcpNetBIOSOverTCPIPNodeTypeOption Read(DataSegment data, ref int offset)
        {
            byte len = data[offset++];
            if (len != 1)
                throw new ArgumentException("Length of a DHCP NetBIOSOverTCPIPNodeType Option has to be 1");
            DhcpNetBIOSOverTCPIPNodeTypeOption option = new DhcpNetBIOSOverTCPIPNodeTypeOption((NodeType)data[offset]);
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, (byte)Type);
        }

        public override byte Length
        {
            get
            {
                return sizeof(NodeType);
            }
        }

        public NodeType Type
        {
            get { return _type; }
            set
            {
                bool flagSet = false;
                foreach(NodeType type in Enum.GetValues(typeof(NodeType)))
                {
                    if ((value & type) != 0)
                    {
                        flagSet = true;
                        break;
                    }
                }
                if (!flagSet)
                    throw new ArgumentOutOfRangeException(nameof(Type), value, "Not a valid NodeType");
                _type = value;
            }
        }

        private NodeType _type;

        [Flags]
        public enum NodeType : byte
        {
            BNode = 0x1,
            PNode = 0x2,
            MNode = 0x4,
            HNode = 0x8,
        }
    }
}