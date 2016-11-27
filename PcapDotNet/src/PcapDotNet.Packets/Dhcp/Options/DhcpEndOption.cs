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
    /// | 255 |
    /// +-----+
    /// </pre>
    /// </summary>
    public class DhcpEndOption : DhcpOption
    {
        public DhcpEndOption() : base(DhcpOptionCode.End)
        {
        }

        public override byte Length
        {
            get { return 0; }
        }

        internal static DhcpEndOption Read(DataSegment data, ref int offset)
        {
            return new Options.DhcpEndOption();
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(offset++, (byte)OptionCode);
        }
    }
}