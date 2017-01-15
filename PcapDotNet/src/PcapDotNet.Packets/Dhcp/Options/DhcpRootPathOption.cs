using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option specifies the path-name that contains the client's root
    /// disk.
    /// <pre>
    ///  Code   Len      Root Disk Pathname
    /// +-----+-----+-----+-----+-----+-----+---
    /// |  17 |  n  |  n1 |  n2 |  n3 |  n4 | ...
    /// +-----+-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpRootPathOption : DhcpStringOption
    {
        /// <summary>
        /// create new DhcpRootPathOption.
        /// </summary>
        /// <param name="rootDiskPathname">Root Disk Pathname</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Pathname")]
        public DhcpRootPathOption(string rootDiskPathname) : base(rootDiskPathname, DhcpOptionCode.RootPath)
        {
        }

        internal static DhcpRootPathOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpRootPathOption>(data, ref offset, p => new DhcpRootPathOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Root Disk Pathname.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Pathname")]
        public string RootDiskPathname
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}