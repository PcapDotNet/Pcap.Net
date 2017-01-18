using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// The NetBIOS scope option specifies the NetBIOS over TCP/IP scope
    /// parameter for the client as specified in RFC 1001/1002.
    /// <pre>
    /// Code   Len       NetBIOS Scope
    /// +-----+-----+-----+-----+-----+-----+----
    /// |  47 |  n  |  s1 |  s2 |  s3 |  s4 | ...
    /// +-----+-----+-----+-----+-----+-----+----
    /// </pre>
    /// </summary>
    public class DhcpNetBiosOverTcpIpScopeOption : DhcpOption
    {
        /// <summary>
        /// create new DhcpNetBiosOverTcpIpScopeOption.
        /// </summary>
        /// <param name="netBiosScope">NetBIOS Scope</param>
        public DhcpNetBiosOverTcpIpScopeOption(DataSegment netBiosScope) : base(DhcpOptionCode.NetBiosOverTcpIpScope)
        {
            NetBiosScope = netBiosScope;
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.NetBiosOverTcpIpScope)]
        internal static DhcpNetBiosOverTcpIpScopeOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            DhcpNetBiosOverTcpIpScopeOption option = new DhcpNetBiosOverTcpIpScopeOption(data.Subsegment(offset, length));
            offset += length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, NetBiosScope);
        }

        /// <summary>
        /// Length of the Dhcp-Option.
        /// </summary>
        public override byte Length
        {
            get
            {
                return (byte)NetBiosScope.Length;
            }
        }

        /// <summary>
        /// RFC 2132.
        /// NetBIOS Scope.
        /// </summary>
        public DataSegment NetBiosScope
        {
            get { return _netBiosScope; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value.Length > byte.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value), value.Length, "NetBiosScope.Length has to be less than 256");
                _netBiosScope = value;
            }
        }

        private DataSegment _netBiosScope;
    }
}