namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5213.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved                   |
    /// +-----+----------------------------+
    /// | 32  | Link-layer Identifier      |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.MobileNodeLinkLayerIdentifier)]
    public sealed class IpV6MobilityOptionMobileNodeLinkLayerIdentifier : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int LinkLayerIdentifier = sizeof(ushort);
        }

        public const int OptionDataMinimumLength = Offset.LinkLayerIdentifier;

        public IpV6MobilityOptionMobileNodeLinkLayerIdentifier(DataSegment linkLayerIdentifier)
            : base(IpV6MobilityOptionType.MobileNodeLinkLayerIdentifier)
        {
            LinkLayerIdentifier = linkLayerIdentifier;
        }

        /// <summary>
        /// Contains the mobile node's link-layer identifier.
        /// </summary>
        public DataSegment LinkLayerIdentifier { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            DataSegment linkLayerIdentifier = data.Subsegment(Offset.LinkLayerIdentifier, data.Length - Offset.LinkLayerIdentifier);
            return new IpV6MobilityOptionMobileNodeLinkLayerIdentifier(linkLayerIdentifier);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + LinkLayerIdentifier.Length; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionMobileNodeLinkLayerIdentifier);
        }

        internal override int GetDataHashCode()
        {
            return LinkLayerIdentifier.GetHashCode();
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.LinkLayerIdentifier, LinkLayerIdentifier);
            offset += DataLength;
        }

        private IpV6MobilityOptionMobileNodeLinkLayerIdentifier()
            : this(DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6MobilityOptionMobileNodeLinkLayerIdentifier other)
        {
            return other != null &&
                   LinkLayerIdentifier.Equals(other.LinkLayerIdentifier);
        }
    }
}