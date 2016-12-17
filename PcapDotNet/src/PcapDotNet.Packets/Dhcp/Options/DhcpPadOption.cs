using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// The pad option can be used to cause subsequent fields to align on
    /// word boundaries.
    /// <pre>
    ///  Code
    /// +-----+
    /// |  0  |
    /// +-----+
    /// </pre>
    /// </summary>
    public class DhcpPadOption : DhcpOption
    {
        /// <summary>
        /// create new PadOption
        /// </summary>
        public DhcpPadOption() : base(DhcpOptionCode.Pad)
        {
        }

        /// <summary>
        /// Length of the Dhcp-Option
        /// </summary>
        public override byte Length
        {
            get { return 0; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "data")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "offset")]
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