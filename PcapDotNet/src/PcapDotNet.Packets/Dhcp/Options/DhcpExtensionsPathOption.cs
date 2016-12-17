using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// A string to specify a file, retrievable via TFTP, which contains
    /// information which can be interpreted in the same way as the 64-octet
    /// vendor-extension field within the BOOTP response, with the following
    /// exceptions:
    /// - the length of the file is unconstrained;
    /// - all references to Tag 18 (i.e., instances of the BOOTP Extensions Path field) within the file are ignored.
    /// <pre>
    ///  Code   Len      Extensions Pathname
    /// +-----+-----+-----+-----+-----+-----+---
    /// |  18 |  n  |  n1 |  n2 |  n3 |  n4 | ...
    /// +-----+-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpExtensionsPathOption : DhcpStringOption
    {
        /// <summary>
        /// create new DhcpExtensionsPathOption
        /// </summary>
        /// <param name="extensionsPathname">Extensions Pathname</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Pathname")]
        public DhcpExtensionsPathOption(string extensionsPathname) : base(extensionsPathname, DhcpOptionCode.ExtensionsPath)
        {
        }

        internal static DhcpExtensionsPathOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpExtensionsPathOption>(data, ref offset, p => new Options.DhcpExtensionsPathOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Extensions Pathname
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Pathname")]
        public string ExtensionsPathname
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}