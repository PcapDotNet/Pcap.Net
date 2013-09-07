using System;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6554.
    /// <pre>
    /// +-----+-------+-------+------+------------------+--------------+---------------+
    /// | Bit | 0-3   | 4-7   | 8-11 | 12-15            | 16-23        | 24-31         |
    /// +-----+-------+-------+------+------------------+--------------+---------------+
    /// | 0   | Next Header   | Header Extension Length | Routing Type | Segments Left |
    /// +-----+-------+-------+------+------------------+--------------+---------------+
    /// | 32  | CmprI | CmprE | Pad  | Reserved                                        |
    /// +-----+-------+-------+------+-------------------------------------------------+
    /// | 64  | Address[1]                                                             |
    /// | ... |                                                                        |
    /// +-----+------------------------------------------------------------------------+
    /// |     | Address[2]                                                             |
    /// | ... |                                                                        |
    /// +-----+------------------------------------------------------------------------+
    /// | .   | .                                                                      |
    /// | .   | .                                                                      |
    /// | .   | .                                                                      |
    /// +-----+------------------------------------------------------------------------+
    /// |     | Address[n]                                                             |
    /// | ... |                                                                        |
    /// +-----+------------------------------------------------------------------------+
    /// |     | Padding                                                                |
    /// | ... |                                                                        |
    /// +-----+------------------------------------------------------------------------+
    /// </pre>
    /// </summary>
    public class IpV6ExtensionHeaderRoutingRpl : IpV6ExtensionHeaderRouting
    {
        private static class RoutingDataOffset
        {
            public const int CommonPrefixLengthForNonLastAddresses = 0;
            public const int CommonPrefixLengthForLastAddress = CommonPrefixLengthForNonLastAddresses;
            public const int PadSize = CommonPrefixLengthForLastAddress + sizeof(byte);
            public const int Reserved = PadSize;
            public const int Addresses = Reserved + UInt24.SizeOf;
        }

        private static class RoutingDataMask
        {
            public const byte CommonPrefixLengthForNonLastAddresses = 0xF0;
            public const byte CommonPrefixLengthForLastAddress = 0x0F;
            public const byte PadSize = 0xF0;
        }
        
        private static class RoutingDataShift
        {
            public const int CommonPrefixLengthForNonLastAddresses = 4;
            public const int PadSize = 4;
        }

        public const int RoutingDataMinimumLength = RoutingDataOffset.Addresses;
        public const byte MaxCommonPrefixLength = IpV6Address.SizeOf - 1;
        public const byte MaxPadSize = IpV6Address.SizeOf - 1;

        public IpV6ExtensionHeaderRoutingRpl(IpV4Protocol nextHeader, byte segmentsLeft, byte commonPrefixLengthForNonLastAddresses,
                                             byte commonPrefixLengthForLastAddress, byte padSize, params IpV6Address[] addresses)
            : base(nextHeader, segmentsLeft)
        {
            if (commonPrefixLengthForNonLastAddresses > MaxCommonPrefixLength)
            {
                throw new ArgumentOutOfRangeException("commonPrefixLengthForNonLastAddresses", commonPrefixLengthForNonLastAddresses,
                                                      string.Format("Maximum value is {0}", MaxCommonPrefixLength));
            }
            CommonPrefixLengthForNonLastAddresses = commonPrefixLengthForNonLastAddresses;

            if (commonPrefixLengthForLastAddress > MaxCommonPrefixLength)
            {
                throw new ArgumentOutOfRangeException("commonPrefixLengthForLastAddress", commonPrefixLengthForLastAddress,
                                                      string.Format("Maximum value is {0}", MaxCommonPrefixLength));
            }
            CommonPrefixLengthForLastAddress = commonPrefixLengthForLastAddress;

            if (padSize > MaxPadSize)
                throw new ArgumentOutOfRangeException("padSize", padSize, string.Format("Maximum value is {0}", MaxPadSize));
            PadSize = padSize;

            Addresses = addresses.AsReadOnly();
        }

        public override IpV6RoutingType RoutingType
        {
            get { return IpV6RoutingType.SourceRoute; }
        }

        /// <summary>
        /// Number of prefix octets from each segment, except than the last segment, (i.e., segments 1 through n-1) that are elided.
        /// For example, a header carrying full IPv6 addresses in Addresses[1..n-1] sets this to 0.
        /// </summary>
        public byte CommonPrefixLengthForNonLastAddresses { get; private set; }

        /// <summary>
        /// Number of prefix octets from the last segment (i.e., segment n) that are elided.  
        /// For example, a header carrying a full IPv6 address in Addresses[n] sets this to 0. 
        /// </summary>
        public byte CommonPrefixLengthForLastAddress { get; private set; }

        /// <summary>
        /// Number of octets that are used for padding after Address[n] at the end of the header.
        /// </summary>
        public byte PadSize { get; private set; }

        public ReadOnlyCollection<IpV6Address> Addresses { get; private set; }

        internal override int RoutingDataLength
        {
            get
            {
                return RoutingDataMinimumLength +
                       (Addresses.Any()
                            ? Addresses.Count * IpV6Address.SizeOf -
                              (Addresses.Count - 1) * CommonPrefixLengthForNonLastAddresses - CommonPrefixLengthForLastAddress
                            : 0) + PadSize;
            }
        }

        internal static IpV6ExtensionHeaderRoutingRpl ParseRoutingData(IpV4Protocol nextHeader, byte segmentsLeft, DataSegment routingData)
        {
            if (routingData.Length < RoutingDataMinimumLength)
                return null;

            byte commonPrefixLengthForNonLastAddresses =
                (byte)((routingData[RoutingDataOffset.CommonPrefixLengthForNonLastAddresses] & RoutingDataMask.CommonPrefixLengthForNonLastAddresses) >>
                       RoutingDataShift.CommonPrefixLengthForNonLastAddresses);
            if (commonPrefixLengthForNonLastAddresses >= MaxCommonPrefixLength)
                return null;

            byte commonPrefixLengthForLastAddress =
                (byte)(routingData[RoutingDataOffset.CommonPrefixLengthForLastAddress] & RoutingDataMask.CommonPrefixLengthForLastAddress);
            if (commonPrefixLengthForLastAddress > MaxCommonPrefixLength)
                return null;

            byte padSize = (byte)((routingData[RoutingDataOffset.PadSize] & RoutingDataMask.PadSize) >> RoutingDataShift.PadSize);
            if (padSize > MaxPadSize)
                return null;

            int numAddresses = (routingData.Length - RoutingDataOffset.Addresses - padSize - (IpV6Address.SizeOf - commonPrefixLengthForLastAddress)) /
                               (IpV6Address.SizeOf - commonPrefixLengthForNonLastAddresses) + 1;
            if (numAddresses < 0)
                return null;

            IpV6Address[] addresses = new IpV6Address[numAddresses];
            if (numAddresses > 0)
            {
                byte[] addressBytes = new byte[IpV6Address.SizeOf];
                for (int i = 0; i < numAddresses - 1; ++i)
                {
                    DataSegment addressSegment =
                        routingData.Subsegment(RoutingDataOffset.Addresses + i * (IpV6Address.SizeOf - commonPrefixLengthForNonLastAddresses),
                                               commonPrefixLengthForNonLastAddresses);
                    addressSegment.Write(addressBytes, 0);
                    addresses[i] = addressBytes.ReadIpV6Address(0, Endianity.Big);
                }

                addressBytes = new byte[IpV6Address.SizeOf];
                DataSegment lastAddressSegment =
                    routingData.Subsegment(RoutingDataOffset.Addresses + (numAddresses - 1) * (IpV6Address.SizeOf - commonPrefixLengthForNonLastAddresses),
                                           commonPrefixLengthForLastAddress);
                lastAddressSegment.Write(addressBytes, 0);
                addresses[numAddresses - 1] = addressBytes.ReadIpV6Address(0, Endianity.Big);
            }
            return new IpV6ExtensionHeaderRoutingRpl(nextHeader, segmentsLeft, commonPrefixLengthForNonLastAddresses, commonPrefixLengthForLastAddress, padSize,
                                                     addresses);
        }

        internal override void WriteRoutingData(byte[] buffer, int offset)
        {
            buffer.Write(offset + RoutingDataOffset.CommonPrefixLengthForNonLastAddresses,
                         (byte)((CommonPrefixLengthForNonLastAddresses << RoutingDataShift.CommonPrefixLengthForNonLastAddresses) |
                                CommonPrefixLengthForLastAddress));
            buffer.Write(offset + RoutingDataOffset.PadSize, (byte)(PadSize << RoutingDataShift.PadSize));
            if (Addresses.Any())
            {
                int addressOffset = offset + RoutingDataOffset.Addresses;
                byte[] addressBytes = new byte[IpV6Address.SizeOf];
                for (int i = 0; i != Addresses.Count - 1; ++i)
                {
                    addressBytes.Write(0, Addresses[i], Endianity.Big);
                    addressBytes.SubSegment(CommonPrefixLengthForNonLastAddresses, IpV6Address.SizeOf - CommonPrefixLengthForNonLastAddresses).Write(buffer, ref addressOffset);
                }
                addressBytes.Write(0, Addresses[Addresses.Count - 1], Endianity.Big);
                addressBytes.SubSegment(CommonPrefixLengthForLastAddress, IpV6Address.SizeOf - CommonPrefixLengthForLastAddress).Write(buffer, ref addressOffset);
            }
        }
    }
}