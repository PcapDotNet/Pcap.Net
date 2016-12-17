using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option specifies a table of MTU sizes to use when performing
    /// Path MTU Discovery as defined in RFC 1191.  The table is formatted as
    /// a list of 16-bit unsigned integers, ordered from smallest to largest.
    /// <pre>
    ///  Code   Len     Size 1      Size 2
    /// +-----+-----+-----+-----+-----+-----+---
    /// |  25 |  n  |  s1 |  s2 |  s1 |  s2 | ...
    /// +-----+-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpPathMtuPlateauTableOption : DhcpOption
    {
        internal const int MAX_SIZES = 255 / sizeof(ushort);

        /// <summary>
        /// create new DhcpPathMtuPlateauTableOption
        /// </summary>
        /// <param name="sizes">Sizes</param>
        public DhcpPathMtuPlateauTableOption(IList<ushort> sizes) : base(DhcpOptionCode.PathMtuPlateauTable)
        {
            if (sizes == null)
                throw new ArgumentNullException(nameof(sizes));
            if (sizes.Count < 1)
                throw new ArgumentOutOfRangeException(nameof(sizes), sizes.Count, "The minimum items in sizes is 1");
            if (sizes.Count > MAX_SIZES)
                throw new ArgumentOutOfRangeException(nameof(sizes), sizes.Count, "The maximum items in addresses is " + MAX_SIZES);

            Sizes = new ReadOnlyCollection<ushort>(sizes);
        }

        internal static DhcpPathMtuPlateauTableOption Read(DataSegment data, ref int offset)
        {
            byte len = data[offset++];
            if (len % sizeof(ushort) != 0)
            {
                throw new ArgumentException("Length of a DHCP PathMTUPlateauTable Option has to be a multiple of 2");
            }
            List<ushort> sizes = new List<ushort>();
            for (int i = 0; i < len; i += 2)
            {
                sizes.Add(data.ReadUShort(offset + i, Endianity.Big));
            }
            DhcpPathMtuPlateauTableOption option = new DhcpPathMtuPlateauTableOption(sizes);
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            foreach (ushort size in Sizes)
            {
                buffer.Write(ref offset, size, Endianity.Big);
            }
        }

        /// <summary>
        /// Length of the Dhcp-Option
        /// </summary>
        public override byte Length
        {
            get
            {
                return (byte)(Sizes.Count * sizeof(ushort));
            }
        }

        /// <summary>
        /// RFC 2132.
        /// Sizes
        /// </summary>
        public IReadOnlyCollection<ushort> Sizes
        {
            get;
            private set;
        }
    }
}