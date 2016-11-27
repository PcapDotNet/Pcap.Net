using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132
    /// <pre>
    ///  Code   Len     Size 1      Size 2
    /// +-----+-----+-----+-----+-----+-----+---
    /// |  25 |  n  |  s1 |  s2 |  s1 |  s2 | ...
    /// +-----+-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpPathMTUPlateauTableOption : DhcpOption
    {
        internal const int MAX_SIZES = 255 / sizeof(ushort);

        public DhcpPathMTUPlateauTableOption(IList<ushort> sizes) : base(DhcpOptionCode.PathMTUPlateauTable)
        {
            if (sizes == null)
                throw new ArgumentNullException(nameof(sizes));
            if (sizes.Count > MAX_SIZES)
                throw new ArgumentOutOfRangeException(nameof(sizes), sizes.Count, $"The maximum items in addresses is {MAX_SIZES}");

            Sizes = new ReadOnlyCollection<ushort>(sizes);
        }

        internal static DhcpPathMTUPlateauTableOption Read(DataSegment data, ref int offset)
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
            DhcpPathMTUPlateauTableOption option = new DhcpPathMTUPlateauTableOption(sizes);
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

        public override byte Length
        {
            get
            {
                if (Sizes.Count > MAX_SIZES)
                    throw new ArgumentOutOfRangeException(nameof(Sizes), Sizes.Count, $"The maximum items in addresses is {MAX_SIZES}");

                return (byte)(Sizes.Count * sizeof(ushort));
            }
        }

        public IReadOnlyCollection<ushort> Sizes
        {
            get;
            private set;
        }
    }
}