using System.Collections;
using System.Linq;
using System.Text;

namespace PcapDotNet.Packets
{
    /// <summary>
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
    /// </summary>
    public class IpV4Datagram : Datagram
    {
        public const int HeaderMinimumLength = 20;
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

        public const int Version = 0x4;

        public int HeaderLength
        {
            get { return (this[Offset.VersionAndHeaderLength] & 0x0F) * 4; }
        }

        public byte TypeOfService
        {
            get { return this[Offset.TypeOfService];}
        }

        public ushort TotalLength
        {
            get { return ReadUShort(Offset.TotalLength, Endianity.Big); }
        }

        public ushort Identification
        {
            get { return ReadUShort(Offset.Identification, Endianity.Big); }
        }

        public IpV4Fragmentation Fragmentation
        {
            get { return new IpV4Fragmentation(ReadUShort(Offset.Fragmentation, Endianity.Big)); }
        }

        public byte Ttl
        {
            get { return this[Offset.Ttl]; }
        }

        public IpV4Protocol Protocol
        {
            get { return (IpV4Protocol)this[Offset.Protocol]; }
        }

        public ushort HeaderChecksum
        {
            get { return ReadUShort(Offset.HeaderChecksum, Endianity.Big); }
        }

        public bool IsHeaderChecksumCorrect
        {
            get 
            { 
                if (_isHeaderChecksumCorrect == null)
                    _isHeaderChecksumCorrect = (CalculateHeaderChecksum() == HeaderChecksum);
                return _isHeaderChecksumCorrect.Value;
            }
        }

        public IpV4Address Source
        {
            get { return new IpV4Address(ReadUInt(Offset.Source, Endianity.Big)); }
        }

        public IpV4Address Destination
        {
            get { return new IpV4Address(ReadUInt(Offset.Destination, Endianity.Big)); }
        }

        public IpV4Options Options
        {
            get { return new IpV4Options(Buffer, StartOffset + Offset.Options, HeaderLength - HeaderMinimumLength); }
        }

        public Datagram Payload
        {
            get { return Tcp; }
        }

        public Datagram Tcp
        {
            get
            {
                if (_tcp == null && Length >= HeaderLength)
                    _tcp = new Datagram(Buffer, StartOffset + HeaderLength, Length - HeaderLength);

                return _tcp;
            }
        }

        internal IpV4Datagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal static void WriteHeader(byte[] buffer, int offset,
                                         byte typeOfService, ushort identification,
                                         IpV4Fragmentation fragmentation,
                                         byte ttl, IpV4Protocol protocol,
                                         IpV4Address source, IpV4Address destination,
                                         IpV4Options options, int payloadLength)
        {
            int headerLength = HeaderMinimumLength + options.Length;
            buffer[offset + Offset.VersionAndHeaderLength] = (byte)((Version << 4) + headerLength / 4);
            buffer[offset + Offset.TypeOfService] = typeOfService;
            buffer.Write(offset + Offset.TotalLength, (ushort)(headerLength + payloadLength), Endianity.Big);
            buffer.Write(offset + Offset.Identification, identification, Endianity.Big);
            fragmentation.Write(buffer, offset + Offset.Fragmentation);
            buffer[offset + Offset.Ttl] = ttl;
            buffer[offset + Offset.Protocol] = (byte)protocol;

            buffer.Write(offset + Offset.Source, source.ToValue(), Endianity.Big);
            buffer.Write(offset + Offset.Destination, destination.ToValue(), Endianity.Big);
            options.Write(buffer, offset + Offset.Options);

            buffer.Write(offset + Offset.HeaderChecksum, Sum16BitsToChecksum(Sum16Bits(buffer, offset, headerLength)), Endianity.Big);
        }

        private ushort CalculateHeaderChecksum()
        {
            uint sum = Sum16Bits(Buffer, StartOffset, Offset.HeaderChecksum) +
                       Sum16Bits(Buffer, StartOffset + Offset.HeaderChecksum + 2, HeaderLength - Offset.HeaderChecksum - 2);

            return Sum16BitsToChecksum(sum);
        }

        private static ushort Sum16BitsToChecksum(uint sum)
        {
            // take only 16 bits out of the 32 bit sum and add up the carrier
            sum = (sum & 0x0000FFFF) + (sum >> 16);

            // one's complement the result
            sum = ~sum;
            return (ushort)sum;
        }

        private static uint Sum16Bits(byte[] buffer, int offset, int length)
        {
            int endOffset = offset + length;
            uint sum = 0;
            while (offset < endOffset - 1)
                sum += buffer.ReadUShort(ref offset, Endianity.Big);
            return sum;
        }

        private bool? _isHeaderChecksumCorrect;
        private Datagram _tcp;
    }
}
