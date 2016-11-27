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
    ///  Code   Len  Value
    /// +-----+-----+-----+
    /// |  39 |  1  | 0/1 |
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpTCPKeepaliveGarbageOption : DhcpOption
    {
        public DhcpTCPKeepaliveGarbageOption(bool enabled) : base(DhcpOptionCode.TCPKeepaliveGarbage)
        {
            Value = enabled;
        }

        internal static DhcpTCPKeepaliveGarbageOption Read(DataSegment data, ref int offset)
        {
            byte len = data[offset++];
            if (len != 1)
                throw new ArgumentException("Length of a DHCP TCPKeepaliveGarbage Option has to be 1");
            if (data[offset] != 0 && data[offset] != 1)
                throw new ArgumentException("Value of a DHCP TCPKeepaliveGarbage Option has to be 0 or 1");
            DhcpTCPKeepaliveGarbageOption option = new DhcpTCPKeepaliveGarbageOption(data[offset] == 1 ? true : false);
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Value ? (byte)1 : (byte)0);
        }

        public override byte Length
        {
            get
            {
                return 1;
            }
        }

        public bool Value
        {
            get;
            set;
        }
    }
}