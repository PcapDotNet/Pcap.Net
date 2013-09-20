using System.Collections.ObjectModel;
using System.Linq;
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
    /// | 32  | Reserved                                                             |
    /// +-----+----------------------------------------------------------------------+
    /// | 64  | Address[1]                                                           |
    /// |     |                                                                      |
    /// |     |                                                                      |
    /// |     |                                                                      |
    /// +-----+----------------------------------------------------------------------+
    /// | 192 | Address[2]                                                           |
    /// |     |                                                                      |
    /// |     |                                                                      |
    /// |     |                                                                      |
    /// +-----+----------------------------------------------------------------------+
    /// | .   | .                                                                    |
    /// | .   | .                                                                    |
    /// | .   | .                                                                    |
    /// +-----+----------------------------------------------------------------------+
    /// |     | Address[n]                                                           |
    /// |     |                                                                      |
    /// |     |                                                                      |
    /// |     |                                                                      |
    /// +-----+----------------------------------------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderRoutingSourceRoute : IpV6ExtensionHeaderRouting
    {
        private static class RoutingDataOffset
        {
            public const int Reserved = 0;
            public const int Addresses = Reserved + sizeof(uint);
        }

        public const int RoutingDataMinimumLength = RoutingDataOffset.Addresses;

        public IpV6ExtensionHeaderRoutingSourceRoute(IpV4Protocol nextHeader, byte segmentsLeft, params IpV6Address[] addresses)
            : base(nextHeader, segmentsLeft)
        {
            Addresses = addresses.AsReadOnly();
        }

        public override IpV6RoutingType RoutingType
        {
            get { return IpV6RoutingType.SourceRoute; }
        }

        public ReadOnlyCollection<IpV6Address> Addresses { get; private set; }

        internal override int RoutingDataLength
        {
            get { return RoutingDataMinimumLength + Addresses.Count * IpV6Address.SizeOf; }
        }

        internal static IpV6ExtensionHeaderRoutingSourceRoute ParseRoutingData(IpV4Protocol nextHeader, byte segmentsLeft, DataSegment routingData)
        {
            if (routingData.Length < RoutingDataMinimumLength)
                return null;

            if ((routingData.Length - RoutingDataMinimumLength) % IpV6Address.SizeOf != 0)
                return null;

            int numAddresses = (routingData.Length - RoutingDataMinimumLength) / 8;
            IpV6Address[] addresses = new IpV6Address[numAddresses];
            for (int i = 0; i != numAddresses; ++i)
                addresses[i] = routingData.ReadIpV6Address(RoutingDataOffset.Addresses + i * IpV6Address.SizeOf, Endianity.Big);
            return new IpV6ExtensionHeaderRoutingSourceRoute(nextHeader, segmentsLeft, addresses);
        }

        internal override bool EqualsRoutingData(IpV6ExtensionHeaderRouting other)
        {
            return EqualsRoutingData(other as IpV6ExtensionHeaderRoutingSourceRoute);
        }

        internal override void WriteRoutingData(byte[] buffer, int offset)
        {
            for (int i = 0; i != Addresses.Count; ++i)
                buffer.Write(offset + RoutingDataOffset.Addresses + i * IpV6Address.SizeOf, Addresses[i], Endianity.Big);
        }

        private bool EqualsRoutingData(IpV6ExtensionHeaderRoutingSourceRoute other)
        {
            return other != null &&
                   Addresses.SequenceEqual(other.Addresses);
        }
    }
}