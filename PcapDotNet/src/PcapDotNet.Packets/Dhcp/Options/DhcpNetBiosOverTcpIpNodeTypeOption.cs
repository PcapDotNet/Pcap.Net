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
    /// The NetBIOS node type option allows NetBIOS over TCP/IP clients which
    /// are configurable to be configured as described in RFC 1001/1002.
    /// <pre>
    ///  Code   Len  Node Type
    /// +-----+-----+-----------+
    /// |  46 |  1  |   flags   |
    /// +-----+-----+-----------+
    /// </pre>
    /// </summary>
    public class DhcpNetBiosOverTcpIpNodeTypeOption : DhcpOption
    {
        /// <summary>
        /// create new DhcpNetBIOSOverTCPIPNodeTypeOption.
        /// </summary>
        /// <param name="type">Type</param>
        public DhcpNetBiosOverTcpIpNodeTypeOption(NodeType type) : base(DhcpOptionCode.NetBiosOverTcpIpNodeType)
        {
            Type = type;
        }

        internal static DhcpNetBiosOverTcpIpNodeTypeOption Read(DataSegment data, ref int offset)
        {
            byte len = data[offset++];
            if (len != 1)
                throw new ArgumentException("Length of a DHCP NetBIOSOverTCPIPNodeType Option has to be 1");
            DhcpNetBiosOverTcpIpNodeTypeOption option = new DhcpNetBiosOverTcpIpNodeTypeOption((NodeType)data[offset]);
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, (byte)Type);
        }

        /// <summary>
        /// Length of the Dhcp-Option.
        /// </summary>
        public override byte Length
        {
            get
            {
                return sizeof(NodeType);
            }
        }

        /// <summary>
        /// RFC 2132.
        /// Node Type
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public NodeType Type
        {
            get { return _type; }
            set
            {
                bool flagSet = false;
                foreach (NodeType type in Enum.GetValues(typeof(NodeType)))
                {
                    if ((value & type) != 0)
                    {
                        flagSet = true;
                        break;
                    }
                }
                if (!flagSet)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Not a valid NodeType");
                _type = value;
            }
        }

        private NodeType _type;

        /// <summary>
        /// RFC 2132.
        /// Node Type
        /// </summary>
        [Flags]
        public enum NodeType : byte
        {
            /// <summary>
            /// RFC 2132.
            /// B-node
            /// </summary>
            BNode = 0x1,

            /// <summary>
            /// RFC 2132.
            /// P-node
            /// </summary>
            PNode = 0x2,

            /// <summary>
            /// RFC 2132.
            /// M-node
            /// </summary>
            MNode = 0x4,

            /// <summary>
            /// RFC 2132.
            /// H-node
            /// </summary>
            HNode = 0x8,
        }
    }
}