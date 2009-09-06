﻿using System;
using System.Collections;
using System.Linq;
using System.Text;
using PcapDotNet.Packets.Transport;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Represents an IPv4 datagram.
    /// 
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
    /// </summary>
    public class IpV4Datagram : Datagram
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
        public const int DefaultVersion = 0x4;

        /// <summary>
        /// The header length in bytes.
        /// </summary>
        public int Version
        {
            get { return (this[Offset.VersionAndHeaderLength] & 0xF0) >> 4; }
        }

        /// <summary>
        /// The header length in bytes.
        /// </summary>
        public int HeaderLength
        {
            get { return (this[Offset.VersionAndHeaderLength] & 0x0F) * 4; }
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
        public ushort TotalLength
        {
            get { return ReadUShort(Offset.TotalLength, Endianity.Big); }
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
        /// The header check sum value.
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
            get { return new IpV4Address(ReadUInt(Offset.Source, Endianity.Big)); }
        }

        /// <summary>
        /// The destination address.
        /// </summary>
        public IpV4Address Destination
        {
            get { return new IpV4Address(ReadUInt(Offset.Destination, Endianity.Big)); }
        }

        /// <summary>
        /// The options field with all the parsed options if any exist.
        /// </summary>
        public IpV4Options Options
        {
            get
            {
                if (_options == null)
                    _options = new IpV4Options(Buffer, StartOffset + Offset.Options, RealHeaderLength - HeaderMinimumLength);
                return _options;
            }
        }

        /// <summary>
        /// Returns whether the TCP or UDP checksum is correct.
        /// The protocol must be TCP or UDP.
        /// For UDP, the checksum is optional, so 0 checksum is still correct.
        /// </summary>
        public bool IsTransportChecksumCorrect
        {
            get
            {
                if (_isTransportChecksumCorrect == null)
                {
                    ushort transportChecksum = Transport.Checksum;
                    _isTransportChecksumCorrect = (Transport.IsChecksumOptional && transportChecksum == 0) ||
                                                  (CalculateTransportChecksum() == transportChecksum);
                }
                return _isTransportChecksumCorrect.Value;
            }
        }

        /// <summary>
        /// The payload of the datagram.
        /// </summary>
        public Datagram Payload
        {
            get { return Tcp; }
        }

        /// <summary>
        /// The payload of the datagram as a TCP datagram.
        /// </summary>
        public TcpDatagram Tcp
        {
            get
            {
                if (_tcp == null && Length >= HeaderLength)
                    _tcp = new TcpDatagram(Buffer, StartOffset + HeaderLength, Length - HeaderLength);

                return _tcp;
            }
        }

        /// <summary>
        /// The payload of the datagram as a UDP datagram.
        /// </summary>
        public UdpDatagram Udp
        {
            get
            {
                if (_udp == null && Length >= HeaderLength)
                    _udp = new UdpDatagram(Buffer, StartOffset + HeaderLength, Length - HeaderLength);

                return _udp;
            }
        }

        public TransportDatagram Transport
        {
            get
            {
                switch (Protocol)
                {
                    case IpV4Protocol.Tcp:
                        return Tcp;

                    case IpV4Protocol.Udp:
                        return Udp;

                    default:
                        return null;
                }
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

            buffer.Write(offset + Offset.HeaderChecksum, Sum16BitsToChecksum(Sum16Bits(buffer, offset, headerLength)), Endianity.Big);
        }

        internal static void WriteTransportChecksum(byte[] buffer, int offset, int headerLength, ushort transportLength, int transportChecksumOffset, bool isChecksumOptional)
        {
            ushort checksum = CalculateTransportChecksum(buffer, offset, headerLength, transportLength, transportChecksumOffset, isChecksumOptional);
            buffer.Write(offset + headerLength + transportChecksumOffset, checksum, Endianity.Big);
        }

        private ushort CalculateTransportChecksum()
        {
            return CalculateTransportChecksum(Buffer, StartOffset, HeaderLength, (ushort)(TotalLength - HeaderLength), Transport.ChecksumOffset, Transport.IsChecksumOptional);
        }

        private static ushort CalculateTransportChecksum(byte[] buffer, int offset, int headerLength, ushort transportLength, int transportChecksumOffset, bool isChecksumOptional)
        {
            uint sum = Sum16Bits(buffer, offset + Offset.Source, 2 * IpV4Address.SizeOf) +
                       buffer[offset + Offset.Protocol] + transportLength +
                       Sum16Bits(buffer, offset + headerLength, transportChecksumOffset) +
                       Sum16Bits(buffer, offset + headerLength + transportChecksumOffset + 2, transportLength - transportChecksumOffset - 2);

            ushort checksumResult = Sum16BitsToChecksum(sum);
            if (checksumResult == 0 && isChecksumOptional)
                return 0xFFFF;
            return checksumResult;
        }

        /// <summary>
        /// An IPv4 datagram is valid if its length is big enough for the header, the header checksum is correct and the payload is valid.
        /// </summary>
        /// <returns></returns>
        protected override bool CalculateIsValid()
        {
            if (Length < HeaderMinimumLength || Length < HeaderLength)
                return false;

            if (!IsHeaderChecksumCorrect)
                return false;

            switch (Protocol)
            {
//                case IpV4Protocol.Tcp:
//                    return Tcp.IsValid; //&& IsTransportChecksumCorrect;
                case IpV4Protocol.Tcp:
                case IpV4Protocol.Udp:
                    return Transport.IsValid && (Transport.IsChecksumOptional && Transport.Checksum == 0 ||
                                                 IsTransportChecksumCorrect);
                default:
                    // Todo check more protocols
                    return true;
            }
        }

        private ushort CalculateHeaderChecksum()
        {
            uint sum = Sum16Bits(Buffer, StartOffset, Math.Min(Offset.HeaderChecksum, Length)) +
                       Sum16Bits(Buffer, StartOffset + Offset.HeaderChecksum + 2, RealHeaderLength - Offset.HeaderChecksum - 2);

            return Sum16BitsToChecksum(sum);
        }

        private static ushort Sum16BitsToChecksum(uint sum)
        {
            // Take only 16 bits out of the 32 bit sum and add up the carrier.
            // if the results overflows - do it again.
            while (sum > 0xFFFF)
                sum = (sum & 0xFFFF) + (sum >> 16);

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
            if (offset < endOffset)
                sum += (ushort)(buffer[offset] << 8);
            return sum;
        }

        private bool? _isHeaderChecksumCorrect;
        private bool? _isTransportChecksumCorrect;
        private IpV4Options _options;
        private TcpDatagram _tcp;
        private UdpDatagram _udp;
    }
}