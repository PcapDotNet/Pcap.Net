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
    public class IpV6ExtensionHeaderRoutingHomeAddress : IpV6ExtensionHeaderRouting
    {
        private static class RoutingDataOffset
        {
            public const int HomeAddress = sizeof(uint);
        }

        public const int ConstRoutingDataLength = RoutingDataOffset.HomeAddress + IpV6Address.SizeOf;

        public IpV6ExtensionHeaderRoutingHomeAddress(IpV4Protocol nextHeader, byte segmentsLeft, IpV6Address homeAddress)
            : base(nextHeader, segmentsLeft)
        {
            HomeAddress = homeAddress;
        }

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

        internal override void WriteRoutingData(byte[] buffer, int offset)
        {
            buffer.Write(offset + RoutingDataOffset.HomeAddress, HomeAddress, Endianity.Big);
        }
    }
}