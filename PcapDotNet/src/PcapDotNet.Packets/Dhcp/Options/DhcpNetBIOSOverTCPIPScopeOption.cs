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
    /// Code   Len       NetBIOS Scope
    /// +-----+-----+-----+-----+-----+-----+----
    /// |  47 |  n  |  s1 |  s2 |  s3 |  s4 | ...
    /// +-----+-----+-----+-----+-----+-----+----
    /// </pre>
    /// </summary>
    public class DhcpNetBIOSOverTCPIPScopeOption : DhcpOption
    {
        public DhcpNetBIOSOverTCPIPScopeOption(DataSegment netBIOSScope) : base(DhcpOptionCode.NetBIOSOverTCPIPScope)
        {
            NetBIOSScope = netBIOSScope;
        }

        internal static DhcpNetBIOSOverTCPIPScopeOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            DhcpNetBIOSOverTCPIPScopeOption option = new DhcpNetBIOSOverTCPIPScopeOption(data.Subsegment(offset, length));
            offset += length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            if (NetBIOSScope == null)
                throw new ArgumentNullException(nameof(NetBIOSScope));
            if (NetBIOSScope.Length > byte.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(NetBIOSScope), NetBIOSScope.Length, "NetBIOSScope.Length has to be less than 256");
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, NetBIOSScope);
        }

        public override byte Length
        {
            get
            {
                if (NetBIOSScope == null)
                {
                    return 0;
                }
                return (byte)NetBIOSScope.Length;
            }
        }

        public DataSegment NetBIOSScope
        {
            get { return _netBIOSScope; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(NetBIOSScope));
                if (value.Length > byte.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(NetBIOSScope), value.Length, "NetBIOSScope.Length has to be less than 256");
                _netBIOSScope = value;
            }
        }

        private DataSegment _netBIOSScope;
    }
}