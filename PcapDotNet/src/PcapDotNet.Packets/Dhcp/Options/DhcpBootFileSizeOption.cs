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
    /// This option specifies the length in 512-octet blocks of the default
    /// boot image for the client.
    /// <pre>
    ///  Code   Len   File Size
    /// +-----+-----+-----+-----+
    /// |  13 |  2  |  l1 |  l2 |
    /// +-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpBootFileSizeOption : DhcpUShortOption
    {
        /// <summary>
        /// create new DhcpBootFileSizeOption.
        /// </summary>
        /// <param name="fileSize">File Size</param>
        public DhcpBootFileSizeOption(ushort fileSize) : base(fileSize, DhcpOptionCode.BootFileSize)
        {
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.BootFileSize)]
        internal static DhcpBootFileSizeOption Read(DataSegment data, ref int offset)
        {
            return DhcpUShortOption.Read(data, ref offset, p => new DhcpBootFileSizeOption(p));
        }

        /// <summary>
        /// File Size.
        /// </summary>
        public ushort FileSize
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}