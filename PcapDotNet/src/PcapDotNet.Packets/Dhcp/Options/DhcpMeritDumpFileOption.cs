using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option specifies the path-name of a file to which the client's
    /// core image should be dumped in the event the client crashes.
    /// <pre>
    ///  Code   Len      Dump File Pathname
    /// +-----+-----+-----+-----+-----+-----+---
    /// |  14 |  n  |  n1 |  n2 |  n3 |  n4 | ...
    /// +-----+-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpMeritDumpFileOption : DhcpStringOption
    {
        /// <summary>
        /// create new DhcpMeritDumpFileOption
        /// </summary>
        /// <param name="dumpFilePathname">Dump File Pathname</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Pathname")]
        public DhcpMeritDumpFileOption(string dumpFilePathname) : base(dumpFilePathname, DhcpOptionCode.MeritDumpFile)
        {
        }

        internal static DhcpMeritDumpFileOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpMeritDumpFileOption>(data, ref offset, p => new DhcpMeritDumpFileOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Dump File Pathname
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Pathname")]
        public string DumpFilePathname
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}