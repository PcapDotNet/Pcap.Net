using System;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2460.
    /// <pre>
    /// +-----+-------------+-------------------------+-----------------+-------+----+
    /// | Bit | 0-7         | 8-15                    | 16-28           | 29-30 | 31 |
    /// +-----+-------------+-------------------------+-----------------+-------+----+
    /// | 0   | Next Header | Header Extension Length | Fragment Offset | Res   | M  |
    /// +-----+-------------+-------------------------+-----------------+-------+----+
    /// | 32  | Identification                                                       |
    /// +-----+----------------------------------------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderFragmentData : IpV6ExtensionHeaderStandard
    {
        private static class DataOffset
        {
            public const int FragmentOffset = 0;
            public const int MoreFragments = FragmentOffset + sizeof(byte);
            public const int Identification = MoreFragments + sizeof(byte);
        }

        private static class DataMask
        {
            public const ushort FragmentOffset = 0xFFF8;
            public const byte MoreFragments = 0x01;
        }

        private static class DataShift
        {
            public const int FragmentOffset = 3;
        }

        /// <summary>
        /// The number of bytes the extension header data takes.
        /// </summary>
        public const int ExtensionHeaderDataLength = DataOffset.Identification + sizeof(uint);

        /// <summary>
        /// The maximum value for the fragment offset.
        /// </summary>
        public const ushort MaxFragmentOffset = 0x1FFF;

        /// <summary>
        /// Creates an instance from next header, fragment offset, more fragments and identification.
        /// </summary>
        /// <param name="nextHeader">Identifies the type of header immediately following this extension header.</param>
        /// <param name="fragmentOffset">
        /// The offset, in 8-octet units, of the data following this header, relative to the start of the Fragmentable Part of the original packet.
        /// </param>
        /// <param name="moreFragments">
        /// True - more fragments.
        /// False - last fragment.
        /// </param>
        /// <param name="identification">
        /// For every packet that is to be fragmented, the source node generates an Identification value. 
        /// The Identification must be different than that of any other fragmented packet sent recently with the same Source Address and Destination Address.
        /// If a Routing header is present, the Destination Address of concern is that of the final destination.
        /// </param>
        public IpV6ExtensionHeaderFragmentData(IpV4Protocol? nextHeader, ushort fragmentOffset, bool moreFragments, uint identification)
            : base(nextHeader)
        {
            if (fragmentOffset > MaxFragmentOffset)
                throw new ArgumentOutOfRangeException("fragmentOffset", fragmentOffset, "Max value is " + MaxFragmentOffset);
            FragmentOffset = fragmentOffset;
            MoreFragments = moreFragments;
            Identification = identification;
        }

        /// <summary>
        /// The offset, in 8-octet units, of the data following this header, relative to the start of the Fragmentable Part of the original packet.
        /// </summary>
        public ushort FragmentOffset { get; private set; }

        /// <summary>
        /// True - more fragments.
        /// False - last fragment.
        /// </summary>
        public bool MoreFragments { get; private set; }

        /// <summary>
        /// For every packet that is to be fragmented, the source node generates an Identification value. 
        /// The Identification must be different than that of any other fragmented packet sent recently with the same Source Address and Destination Address.
        /// If a Routing header is present, the Destination Address of concern is that of the final destination.
        /// </summary>
        public uint Identification { get; private set; }

        /// <summary>
        /// Identifies the type of this extension header.
        /// </summary>
        public override IpV4Protocol Protocol
        {
            get { return IpV4Protocol.FragmentHeaderForIpV6; }
        }

        /// <summary>
        /// True iff the extension header parsing didn't encounter an issue.
        /// </summary>
        public override bool IsValid
        {
            get { return true; }
        }

        internal override int DataLength
        {
            get { return ExtensionHeaderDataLength; }
        }

        internal static IpV6ExtensionHeaderFragmentData ParseData(IpV4Protocol nextHeader, DataSegment data)
        {
            if (data.Length != ExtensionHeaderDataLength)
                return null;

            ushort fragmentOffset = (ushort)((data.ReadUShort(DataOffset.FragmentOffset, Endianity.Big) & DataMask.FragmentOffset) >> DataShift.FragmentOffset);
            bool moreFragments = data.ReadBool(DataOffset.MoreFragments, DataMask.MoreFragments);
            uint identification = data.ReadUInt(DataOffset.Identification, Endianity.Big);

            return new IpV6ExtensionHeaderFragmentData(nextHeader, fragmentOffset, moreFragments, identification);
        }

        internal override bool EqualsData(IpV6ExtensionHeader other)
        {
            return EqualsData(other as IpV6ExtensionHeaderFragmentData);
        }

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge(FragmentOffset, MoreFragments.ToByte()), Identification);
        }

        internal override void WriteData(byte[] buffer, int offset)
        {
            ushort fragmentOffsetAndMoreFragments = (ushort)(FragmentOffset << DataShift.FragmentOffset);
            if (MoreFragments)
                fragmentOffsetAndMoreFragments |= DataMask.MoreFragments;

            buffer.Write(offset + DataOffset.FragmentOffset, fragmentOffsetAndMoreFragments, Endianity.Big);
            buffer.Write(offset + DataOffset.Identification, Identification, Endianity.Big);
        }

        private bool EqualsData(IpV6ExtensionHeaderFragmentData other)
        {
            return other != null &&
                   FragmentOffset == other.FragmentOffset && MoreFragments == other.MoreFragments && Identification == other.Identification;
        }
    }
}
