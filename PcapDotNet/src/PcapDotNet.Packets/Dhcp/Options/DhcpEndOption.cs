using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// The end option marks the end of valid information in the vendor field.
    /// <pre>
    ///  Code
    /// +-----+
    /// | 255 |
    /// +-----+
    /// </pre>
    /// </summary>
    public class DhcpEndOption : DhcpOption
    {
        /// <summary>
        /// create new EndOption.
        /// </summary>
        public DhcpEndOption() : base(DhcpOptionCode.End)
        {
        }

        /// <summary>
        /// Length of the Dhcp-Option
        /// </summary>
        public override byte Length
        {
            get { return 0; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "offset")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "data")]
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