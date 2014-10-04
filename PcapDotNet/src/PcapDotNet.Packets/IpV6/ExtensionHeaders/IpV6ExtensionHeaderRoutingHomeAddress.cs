using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+-------------+-------------------------+--------------+---------------+
    /// | Bit | 0-7         | 8-15                    | 16-23        | 24-31         |
    /// +-----+-------------+-------------------------+--------------+---------------+
    /// | 0   | Next Header | Header Extension Length | Routing Type | Segments Left |
    /// +-----+-------------+-------------------------+--------------+---------------+
    /// | 32  | Reserved                                                             |
    /// +-----+----------------------------------------------------------------------+
    /// | 64  | Home Address                                                         |
    /// |     |                                                                      |
    /// |     |                                                                      |
    /// |     |                                                                      |
    /// +-----+----------------------------------------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderRoutingHomeAddress : IpV6ExtensionHeaderRouting
    {
        private static class RoutingDataOffset
        {
            public const int HomeAddress = sizeof(uint);
        }

        /// <summary>
        /// The number of bytes the routing data takes.
        /// </summary>
        public const int ConstRoutingDataLength = RoutingDataOffset.HomeAddress + IpV6Address.SizeOf;

        /// <summary>
        /// Creates an instance from next header, segments left and home address.
        /// </summary>
        /// <param name="nextHeader">
        /// Identifies the type of header immediately following this extension header.
        /// </param>
        /// <param name="segmentsLeft">
        /// Number of route segments remaining, i.e., number of explicitly listed intermediate nodes still to be visited before reaching the final destination.
        /// </param>
        /// <param name="homeAddress">
        /// The home address of the destination mobile node.
        /// </param>
        public IpV6ExtensionHeaderRoutingHomeAddress(IpV4Protocol? nextHeader, byte segmentsLeft, IpV6Address homeAddress)
            : base(nextHeader, segmentsLeft)
        {
            HomeAddress = homeAddress;
        }

        /// <summary>
        /// Identifier of a particular Routing header variant.
        /// </summary>
        public override IpV6RoutingType RoutingType
        {
            get { return IpV6RoutingType.Type2RoutingHeader; }
        }

        /// <summary>
        /// The home address of the destination mobile node.
        /// </summary>
        public IpV6Address HomeAddress { get; private set; }

        internal override int RoutingDataLength
        {
            get { return ConstRoutingDataLength; }
        }

        internal static IpV6ExtensionHeaderRoutingHomeAddress ParseRoutingData(IpV4Protocol nextHeader, byte segmentsLeft, DataSegment routingData)
        {
            if (routingData.Length != ConstRoutingDataLength)
                return null;

            IpV6Address homeAddress = routingData.ReadIpV6Address(RoutingDataOffset.HomeAddress, Endianity.Big);
            return new IpV6ExtensionHeaderRoutingHomeAddress(nextHeader, segmentsLeft, homeAddress);
        }

        internal override bool EqualsRoutingData(IpV6ExtensionHeaderRouting other)
        {
            return EqualsRoutingData(other as IpV6ExtensionHeaderRoutingHomeAddress);
        }

        internal override int GetRoutingDataHashCode()
        {
            return HomeAddress.GetHashCode();
        }

        internal override void WriteRoutingData(byte[] buffer, int offset)
        {
            buffer.Write(offset + RoutingDataOffset.HomeAddress, HomeAddress, Endianity.Big);
        }

        private bool EqualsRoutingData(IpV6ExtensionHeaderRoutingHomeAddress other)
        {
            return other != null &&
                   HomeAddress.Equals(other.HomeAddress);
        }
    }
}