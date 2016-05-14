using System;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Packets.Gre;
using PcapDotNet.Packets.Icmp;
using PcapDotNet.Packets.Igmp;
using PcapDotNet.Packets.Ip;
using PcapDotNet.Packets.Transport;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// RFC 791.
    /// Represents an IPv4 datagram.
    /// <pre>
    /// +-----+---------+-----+-----------------+-------+-----------------+
    /// | Bit | 0-3     | 4-7 | 8-15            | 16-18 | 19-31           |
    /// +-----+---------+-----+-----------------+-------+-----------------+
    /// | 0   | Version | IHL | Type of Service | Total Length            |
    /// +-----+---------+-----+-----------------+-------+-----------------+
    /// | 32  | Identification                  | Flags | Fragment Offset |
    /// +-----+---------------+-----------------+-------+-----------------+
    /// | 64  | Time to Live  | Protocol        | Header Checksum         |
    /// +-----+---------------+-----------------+-------------------------+
    /// | 96  | Source Address                                            |
    /// +-----+-----------------------------------------------------------+
    /// | 128 | Destination Address                                       |
    /// +-----+-----------------------------------------------------------+
    /// | 160 | Options with padding                                      |
    /// +-----+-----------------------------------------------------------+
    /// | 160 | Data                                                      |
    /// | to  |                                                           |
    /// | 360 |                                                           |
    /// +-----+-----------------------------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV4Datagram : IpDatagram
    {
        /// <summary>
        /// The minimum length of the header in bytes.
        /// </summary>
        public const int HeaderMinimumLength = 20;

        /// <summary>
        /// The maximum length of the header in bytes.
        /// </summary>
        public const int HeaderMaximumLength = 60;

        private static class Offset
        {
            public const int VersionAndHeaderLength = 0;
            public const int TypeOfService = 1;
            public const int TotalLength = 2;
            public const int Identification = 4;
            public const int Fragmentation = 6;
            public const int Ttl = 8;
            public const int Protocol = 9;
            public const int HeaderChecksum = 10;
            public const int Source = 12;
            public const int Destination = 16;
            public const int Options = 20;
        }

        /// <summary>
        /// The version (4).
        /// </summary>
        public const byte DefaultVersion = 0x4;

        /// <summary>
        /// The header length in bytes.
        /// </summary>
        public int HeaderLength
        {
            get { return ReadHeaderLength(this); }
        }

        /// <summary>
        /// The real number of bytes in the header (different than HeaderLength when the datagram is too small).
        /// </summary>
        public int RealHeaderLength
        {
            get { return Math.Min(HeaderLength, Length); }
        }

        /// <summary>
        /// Type of Service field.
        /// </summary>
        public byte TypeOfService
        {
            get { return this[Offset.TypeOfService];}
        }

        /// <summary>
        /// The length of the entire datagram as stated in the total length field.
        /// </summary>
        public override int TotalLength
        {
            get { return ReadTotalLength(this); }
        }

        /// <summary>
        /// The value of the IPv4 ID field.
        /// </summary>
        public ushort Identification
        {
            get { return ReadUShort(Offset.Identification, Endianity.Big); }
        }

        /// <summary>
        /// The fragmentation information field.
        /// </summary>
        public IpV4Fragmentation Fragmentation
        {
            get { return new IpV4Fragmentation(ReadUShort(Offset.Fragmentation, Endianity.Big)); }
        }

        /// <summary>
        /// The TTL field.
        /// </summary>
        public byte Ttl
        {
            get { return this[Offset.Ttl]; }
        }

        /// <summary>
        /// The IPv4 (next) protocol field.
        /// </summary>
        public IpV4Protocol Protocol
        {
            get { return (IpV4Protocol)this[Offset.Protocol]; }
        }

        /// <summary>
        /// The header checksum value.
        /// </summary>
        public ushort HeaderChecksum
        {
            get { return ReadUShort(Offset.HeaderChecksum, Endianity.Big); }
        }

        /// <summary>
        /// True iff the header checksum value is correct according to the header.
        /// </summary>
        public bool IsHeaderChecksumCorrect
        {
            get 
            { 
                if (_isHeaderChecksumCorrect == null)
                    _isHeaderChecksumCorrect = (CalculateHeaderChecksum() == HeaderChecksum);
                return _isHeaderChecksumCorrect.Value;
            }
        }

        /// <summary>
        /// The source address.
        /// </summary>
        public IpV4Address Source
        {
            get { return ReadIpV4Address(Offset.Source, Endianity.Big); }
        }

        /// <summary>
        /// The current destination address.
        /// This might not be the final destination when source routing options exist.
        /// </summary>
        public IpV4Address CurrentDestination
        {
            get { return ReadIpV4Address(Offset.Destination, Endianity.Big); }
        }

        /// <summary>
        /// The final destination address.
        /// Takes into account the current destination and source routing options if they exist.
        /// </summary>
        public IpV4Address Destination
        {
            get
            {
                if (_destination == null)
                    _destination = CalculateDestination(CurrentDestination, Options);

                return _destination.Value;
            }
        }

        /// <summary>
        /// The options field with all the parsed options if any exist.
        /// </summary>
        public IpV4Options Options
        {
            get
            {
                if (_options == null && RealHeaderLength >= HeaderMinimumLength)
                    _options = new IpV4Options(Buffer, StartOffset + Offset.Options, RealHeaderLength - HeaderMinimumLength);
                
                return _options;
            }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IpV4Layer
                       {
                           TypeOfService = TypeOfService,
                           Identification = Identification,
                           Fragmentation = Fragmentation,
                           Ttl = Ttl,
                           Protocol = Protocol,
                           HeaderChecksum = HeaderChecksum,
                           Source = Source,
                           CurrentDestination = CurrentDestination,
                           Options = Options,
                       };
        }

        internal override IpV4Protocol PayloadProtocol
        {
            get { return Protocol; }
        }

        internal override DataSegment GetPayload()
        {
            if (Length < HeaderMinimumLength || Length < HeaderLength)
                return null;
            return Subsegment(HeaderLength, Length - HeaderLength);
        }

        internal static int GetHeaderLength(DataSegment ipV4Datagram)
        {
            if (ipV4Datagram.Length < HeaderMinimumLength)
                return ipV4Datagram.Length;

            int headerLength = ReadHeaderLength(ipV4Datagram);
            if (ipV4Datagram.Length < headerLength)
                return ipV4Datagram.Length;
            return headerLength;
        }

        internal static int GetTotalLength(DataSegment ipV4Datagram)
        {
            if (ipV4Datagram.Length < HeaderMinimumLength)
                return ipV4Datagram.Length;

            ushort totalLength = ReadTotalLength(ipV4Datagram);
            if (ipV4Datagram.Length < totalLength ||
                totalLength == 0) // Large segment offload.
            {
                return ipV4Datagram.Length;
            }

            return totalLength;
        }

        internal static IpV4Address CalculateDestination(IpV4Address currentDestination, IpV4Options options)
        {
            if (options == null)
                return currentDestination;

            IpV4OptionRoute destinationControllerRouteOption =
                (IpV4OptionRoute)options.OptionsCollection.FirstOrDefault(option => option.OptionType == IpV4OptionType.LooseSourceRouting ||
                                                                                    option.OptionType == IpV4OptionType.StrictSourceRouting);
            if (destinationControllerRouteOption != null)
            {
                ReadOnlyCollection<IpV4Address> route = destinationControllerRouteOption.Route;
                if (destinationControllerRouteOption.PointedAddressIndex < route.Count)
                    return route[route.Count - 1];
            }

            return currentDestination;
        }

        internal IpV4Datagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal static void WriteHeader(byte[] buffer, int offset,
                                         byte typeOfService, ushort identification,
                                         IpV4Fragmentation fragmentation,
                                         byte ttl, IpV4Protocol protocol, ushort? headerChecksum,
                                         IpV4Address source, IpV4Address destination,
                                         IpV4Options options, int payloadLength)
        {
            int headerLength = HeaderMinimumLength + options.BytesLength;

            buffer[offset + Offset.VersionAndHeaderLength] = (byte)((DefaultVersion << 4) + headerLength / 4);
            buffer[offset + Offset.TypeOfService] = typeOfService;
            buffer.Write(offset + Offset.TotalLength, (ushort)(headerLength + payloadLength), Endianity.Big);
            buffer.Write(offset + Offset.Identification, identification, Endianity.Big);
            fragmentation.Write(buffer, offset + Offset.Fragmentation);
            buffer[offset + Offset.Ttl] = ttl;
            buffer[offset + Offset.Protocol] = (byte)protocol;

            buffer.Write(offset + Offset.Source, source, Endianity.Big);
            buffer.Write(offset + Offset.Destination, destination, Endianity.Big);
            options.Write(buffer, offset + Offset.Options);

            ushort headerChecksumValue =
                headerChecksum == null
                    ? Sum16BitsToChecksum(Sum16Bits(buffer, offset, headerLength))
                    : headerChecksum.Value;
            buffer.Write(offset + Offset.HeaderChecksum, headerChecksumValue, Endianity.Big);
        }

        internal static void WriteTransportChecksum(byte[] buffer, int offset, int headerLength, ushort transportLength, int transportChecksumOffset, bool isChecksumOptional, ushort? checksum, IpV4Address destination)
        {
            ushort checksumValue =
                checksum == null
                    ? CalculateTransportChecksum(buffer, offset, headerLength, transportLength, transportChecksumOffset, isChecksumOptional, destination)
                    : checksum.Value;
            buffer.Write(offset + headerLength + transportChecksumOffset, checksumValue, Endianity.Big);
        }

        /// <summary>
        /// An IPv4 datagram is valid if its length is big enough for the header, the header checksum is correct and the payload is valid.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            if (Length < HeaderMinimumLength || Length < HeaderLength)
                return false;

            if (!IsHeaderChecksumCorrect)
                return false;

            return IsPayloadValid;
        }

        /// <summary>
        /// Calculates the Transport checksum field value.
        /// </summary>
        /// <returns>The calculated checksum value.</returns>
        protected override ushort CalculateTransportChecksum()
        {
            return CalculateTransportChecksum(Buffer, StartOffset, HeaderLength, (ushort)Transport.Length, Transport.ChecksumOffset, Transport.IsChecksumOptional, Destination);
        }

        private static int ReadHeaderLength(DataSegment ipV4Datagram)
        {
            return (ipV4Datagram[Offset.VersionAndHeaderLength] & 0x0F) * 4;
        }

        private static ushort ReadTotalLength(DataSegment ipV4Datagram)
        {
            return ipV4Datagram.ReadUShort(Offset.TotalLength, Endianity.Big);
        }

        private ushort CalculateHeaderChecksum()
        {
            uint sum = Sum16Bits(Buffer, StartOffset, Math.Min(Offset.HeaderChecksum, Length)) +
                       Sum16Bits(Buffer, StartOffset + Offset.HeaderChecksum + sizeof(ushort), RealHeaderLength - Offset.HeaderChecksum - sizeof(ushort));

            return Sum16BitsToChecksum(sum);
        }

        private static ushort CalculateTransportChecksum(byte[] buffer, int offset, int headerLength, ushort transportLength, int transportChecksumOffset, bool isChecksumOptional, IpV4Address destination)
        {
            int offsetAfterChecksum = offset + headerLength + transportChecksumOffset + sizeof(ushort);
            uint sum = Sum16Bits(buffer, offset + Offset.Source, IpV4Address.SizeOf) +
                       Sum16Bits(destination) +
                       buffer[offset + Offset.Protocol] + transportLength +
                       Sum16Bits(buffer, offset + headerLength, transportChecksumOffset) +
                       Sum16Bits(buffer, offsetAfterChecksum, transportLength - transportChecksumOffset - sizeof(ushort));

            ushort checksumResult = Sum16BitsToChecksum(sum);
            if (checksumResult == 0 && isChecksumOptional)
                return 0xFFFF;
            return checksumResult;
        }

        private IpV4Address? _destination;
        private bool? _isHeaderChecksumCorrect;
        private IpV4Options _options;
    }
}