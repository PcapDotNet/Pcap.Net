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
    public abstract class IpV6ExtensionHeaderRouting : IpV6ExtensionHeader
    {
        private static class DataOffset
        {
            public const int RoutingType = 0;
            public const int SegmentsLeft = RoutingType + sizeof(byte);
            public const int TypeSpecificData = SegmentsLeft + sizeof(byte);
        }

        public const int DataMinimumLength = DataOffset.TypeSpecificData;

        /// <summary>
        /// Identifier of a particular Routing header variant.
        /// </summary>
        public abstract IpV6RoutingType RoutingType { get; }

        /// <summary>
        /// Number of route segments remaining, i.e., number of explicitly listed intermediate nodes still to be visited before reaching the final destination.
        /// </summary>
        public byte SegmentsLeft { get; private set; }

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

        internal IpV6ExtensionHeaderRouting(IpV4Protocol nextHeader, byte segmentsLeft)
            : base(nextHeader)
        {
            SegmentsLeft = segmentsLeft;
        }
    }
}