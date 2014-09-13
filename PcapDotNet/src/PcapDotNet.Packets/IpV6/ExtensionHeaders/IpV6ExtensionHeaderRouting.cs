using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2460.
    /// <pre>
    /// +-----+-------------+-------------------------+--------------+---------------+
    /// | Bit | 0-7         | 8-15                    | 16-23        | 24-31         |
    /// +-----+-------------+-------------------------+--------------+---------------+
    /// | 0   | Next Header | Header Extension Length | Routing Type | Segments Left |
    /// +-----+-------------+-------------------------+--------------+---------------+
    /// | 32  | Routing Data - type-specific data                                    |
    /// | ... |                                                                      |
    /// +-----+----------------------------------------------------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6ExtensionHeaderRouting : IpV6ExtensionHeaderStandard
    {
        private static class DataOffset
        {
            public const int RoutingType = 0;
            public const int SegmentsLeft = RoutingType + sizeof(byte);
            public const int TypeSpecificData = SegmentsLeft + sizeof(byte);
        }

        public const int DataMinimumLength = DataOffset.TypeSpecificData;

        public override bool IsValid
        {
            get { return true; }
        }

        /// <summary>
        /// Identifier of a particular Routing header variant.
        /// </summary>
        public abstract IpV6RoutingType RoutingType { get; }

        /// <summary>
        /// Number of route segments remaining, i.e., number of explicitly listed intermediate nodes still to be visited before reaching the final destination.
        /// </summary>
        public byte SegmentsLeft { get; private set; }

        public override IpV4Protocol Protocol
        {
            get { return IpV4Protocol.IpV6Route; }
        }

        internal IpV6ExtensionHeaderRouting(IpV4Protocol nextHeader, byte segmentsLeft)
            : base(nextHeader)
        {
            SegmentsLeft = segmentsLeft;
        }

        internal sealed override int DataLength
        {
            get { return DataMinimumLength + RoutingDataLength; }
        }

        internal abstract int RoutingDataLength { get; }

        internal sealed override bool EqualsData(IpV6ExtensionHeader other)
        {
            return EqualsData(other as IpV6ExtensionHeaderRouting);
        }

        internal abstract bool EqualsRoutingData(IpV6ExtensionHeaderRouting other);

        internal sealed override int GetDataHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge((byte)RoutingType, SegmentsLeft), GetRoutingDataHashCode());
        }

        internal abstract int GetRoutingDataHashCode();

        internal static IpV6ExtensionHeaderRouting ParseData(IpV4Protocol nextHeader, DataSegment data)
        {
            if (data.Length < DataMinimumLength)
                return null;
            IpV6RoutingType routingType = (IpV6RoutingType)data[DataOffset.RoutingType];
            byte segmentsLeft = data[DataOffset.SegmentsLeft];
            DataSegment routingData = data.Subsegment(DataOffset.TypeSpecificData, data.Length - DataOffset.TypeSpecificData);
            switch (routingType)
            {
                case IpV6RoutingType.SourceRoute:
                    return IpV6ExtensionHeaderRoutingSourceRoute.ParseRoutingData(nextHeader, segmentsLeft, routingData);

                case IpV6RoutingType.Nimrod:
                    // Unused.
                    return null;

                case IpV6RoutingType.Type2RoutingHeader:
                    return IpV6ExtensionHeaderRoutingHomeAddress.ParseRoutingData(nextHeader, segmentsLeft, routingData);

                case IpV6RoutingType.RplSourceRouteHeader:
                    return IpV6ExtensionHeaderRoutingRpl.ParseRoutingData(nextHeader, segmentsLeft, routingData);

                default:
                    return null;
            }
        }

        internal sealed override void WriteData(byte[] buffer, int offset)
        {
            buffer.Write(offset + DataOffset.RoutingType, (byte)RoutingType);
            buffer.Write(offset + DataOffset.SegmentsLeft, SegmentsLeft);
            WriteRoutingData(buffer, offset + DataOffset.TypeSpecificData);
        }

        internal abstract void WriteRoutingData(byte[] buffer, int offset);

        private bool EqualsData(IpV6ExtensionHeaderRouting other)
        {
            return other != null &&
                   RoutingType == other.RoutingType && SegmentsLeft == other.SegmentsLeft && EqualsRoutingData(other);
        }
    }
}