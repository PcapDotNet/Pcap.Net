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
    ///  Code
    /// +-----+
    /// |  0  |
    /// +-----+
    /// </pre>
    /// </summary>
    public class DhcpPadOption : DhcpOption
    {
        public DhcpPadOption() : base(DhcpOptionCode.Pad)
        {
        }

        public override byte Length
        {
            get { return 0; }
        }

        internal static DhcpPadOption Read(DataSegment data, ref int offset)
        {
            return new Options.DhcpPadOption();
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(offset++, (byte)OptionCode);
        }
    }
}