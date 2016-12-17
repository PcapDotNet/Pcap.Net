using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option specifies the maximum size datagram that the client
    /// should be prepared to reassemble.unsigned integer.
    /// <pre>
    ///  Code   Len      Size
    /// +-----+-----+-----+-----+
    /// |  22 |  2  |  s1 |  s2 |
    /// +-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpMaximumDatagramReassemblySizeOption : DhcpUShortOption
    {
        private const ushort MIN_SIZE = 576;

        /// <summary>
        /// create new DhcpMaximumDatagramReassemblySizeOption
        /// </summary>
        /// <param name="size">Size</param>
        public DhcpMaximumDatagramReassemblySizeOption(ushort size) : base(size, DhcpOptionCode.MaximumDatagramReassemblySize)
        {
            if (size < MIN_SIZE)
                throw new ArgumentOutOfRangeException(nameof(size), size, "Minimum value of Size is " + MIN_SIZE);
        }

        internal static DhcpMaximumDatagramReassemblySizeOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpMaximumDatagramReassemblySizeOption>(data, ref offset, p => new DhcpMaximumDatagramReassemblySizeOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Size
        /// </summary>
        public ushort Size
        {
            get { return InternalValue; }
            set
            {
                if (value < MIN_SIZE)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Minimum value of Size is " + MIN_SIZE);
                InternalValue = value;
            }
        }
    }
}