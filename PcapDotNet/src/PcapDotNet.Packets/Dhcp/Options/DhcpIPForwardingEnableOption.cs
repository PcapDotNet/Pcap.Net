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
    ///  Code   Len  Type
    /// +-----+-----+-----+
    /// |  15 |  1  | 0/1 |
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpIPForwardingEnableOption : DhcpOption
    {
        public DhcpIPForwardingEnableOption(bool value) : base(DhcpOptionCode.IPForwardingEnable)
        {
            Value = value;
        }

        internal static DhcpIPForwardingEnableOption Read(DataSegment data, ref int offset)
        {
            byte len = data[offset++];
            if (len != 1)
                throw new ArgumentException("Length of a DHCP MessageTypeOption has to be 1");
            if (data[offset] != 0 && data[offset] != 1)
                throw new ArgumentException("Value of a  DHCP MessageTypeOption has to be 0 or 1");
            DhcpIPForwardingEnableOption option = new DhcpIPForwardingEnableOption(data[offset] == 1 ? true : false);
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