namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+--------------+--------------+
    /// | Bit | 0-7          | 8-15         |
    /// +-----+--------------+--------------+
    /// | 0   | Option Type  | Opt Data Len |
    /// +-----+--------------+--------------+
    /// | 16  | Home Nonce Index            |
    /// +-----+-----------------------------+
    /// | 32  | Care-of Nonce Index         |
    /// +-----+-----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.NonceIndices)]
    public sealed class IpV6MobilityOptionNonceIndices : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int HomeNonceIndex = 0;
            public const int CareOfNonceIndex = HomeNonceIndex + sizeof(ushort);
        }

        public const int OptionDataLength = Offset.CareOfNonceIndex + sizeof(ushort);

        public IpV6MobilityOptionNonceIndices(ushort homeNonceIndex, ushort careOfNonceIndex)
            : base(IpV6MobilityOptionType.NonceIndices)
        {
            HomeNonceIndex = homeNonceIndex;
            CareOfNonceIndex = careOfNonceIndex;
        }

        /// <summary>
        /// Tells the correspondent node which nonce value to use when producing the home keygen token.
        /// </summary>
        public ushort HomeNonceIndex { get; private set; }

        /// <summary>
        /// Ignored in requests to delete a binding.
        /// Otherwise, it tells the correspondent node which nonce value to use when producing the care-of keygen token.
        /// </summary>
        public ushort CareOfNonceIndex { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            ushort homeNonceIndex = data.ReadUShort(Offset.HomeNonceIndex, Endianity.Big);
            ushort careOfNonceIndex = data.ReadUShort(Offset.CareOfNonceIndex, Endianity.Big);

            return new IpV6MobilityOptionNonceIndices(homeNonceIndex, careOfNonceIndex);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionNonceIndices);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, HomeNonceIndex, Endianity.Big);
            buffer.Write(ref offset, CareOfNonceIndex, Endianity.Big);
        }

        private IpV6MobilityOptionNonceIndices()
            : this(0, 0)
        {
        }

        private bool EqualsData(IpV6MobilityOptionNonceIndices other)
        {
            return other != null &&
                   HomeNonceIndex == other.HomeNonceIndex && CareOfNonceIndex == other.CareOfNonceIndex;
        }
    }
}