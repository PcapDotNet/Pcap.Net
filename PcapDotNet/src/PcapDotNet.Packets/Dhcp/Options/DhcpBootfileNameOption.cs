using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option is used to identify a bootfile when the 'file' field in
    /// the DHCP header has been used for DHCP options.
    /// <pre>
    ///  Code  Len    Bootfile name
    /// +-----+-----+-----+-----+-----+---
    /// | 67  |  n  |  c1 |  c2 |  c3 | ...
    /// +-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpBootfileNameOption : DhcpStringOption
    {
        /// <summary>
        /// create new DhcpBootfileNameOption.
        /// </summary>
        /// <param name="bootfileName">Bootfilename</param>
        public DhcpBootfileNameOption(string bootfileName) : base(bootfileName, DhcpOptionCode.BootfileName)
        {
        }

        internal static DhcpBootfileNameOption Read(DataSegment data, ref int offset)
        {
            return DhcpStringOption.Read(data, ref offset, p => new DhcpBootfileNameOption(p));
        }

        /// <summary>
        /// Bootfilename.
        /// </summary>
        public string BootfileName
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}