using System;
using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Ethernet
{
    /// <summary>
    /// Represents an Ethernet datagram.
    /// 
    /// <pre>
    /// +------+-----------------+------------+------------------+
    /// | Byte | 0-5             | 6-11       | 12-13            |
    /// +------+-----------------+------------+------------------+
    /// | 0    | MAC Destination | MAC Source | EtherType/Length |
    /// +------+-----------------+------------+------------------+
    /// | 14   | Data                                            |
    /// +------+-------------------------------------------------+
    /// </pre>
    /// </summary>
    public class EthernetDatagram : Datagram
    {
        private static class Offset
        {
            public const int Destination = 0;
            public const int Source = 6;
            public const int EtherTypeLength = 12;
        }

        /// <summary>
        /// Ethernet header length in bytes.
        /// </summary>
        public const int HeaderLength = 14;

        /// <summary>
        /// The broadcast MAC address (FF:FF:FF:FF:FF:FF).
        /// </summary>
        public static MacAddress BroadcastAddress
        {
            get { return _broadcastAddress; }
        }

        /// <summary>
        /// The Ethernet payload length in bytes.
        /// </summary>
        public int PayloadLength
        {
            get { return Math.Max(0, Length - HeaderLength); }
        }

        /// <summary>
        /// Ethernet source address.
        /// </summary>
        public MacAddress Source
        {
            get { return ReadMacAddress(Offset.Source, Endianity.Big); }
        }

        /// <summary>
        /// Ethernet destination address.
        /// </summary>
        public MacAddress Destination
        {
            get { return ReadMacAddress(Offset.Destination, Endianity.Big); }
        }

        /// <summary>
        /// Ethernet type (next protocol).
        /// </summary>
        public EthernetType EtherType 
        {
            get
            {
                return (EthernetType)ReadUShort(Offset.EtherTypeLength, Endianity.Big);
            }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new EthernetLayer
                   {
                       Source = Source,
                       Destination = Destination,
                       EtherType = EtherType,
                   };
        }

        /// <summary>
        /// The Ethernet payload.
        /// </summary>
        public Datagram Payload
        {
            get { return PayloadDatagrams.Payload; }
        }

        public Datagram Trailer
        {
            get
            {
                Datagram payloadByEtherType = PayloadByEtherType;
                if (payloadByEtherType == null)
                    return null;

                int payloadLength = PayloadByEtherType.Length;
                Datagram fcs = FrameCheckSequence;
                return new Datagram(Buffer, HeaderLength + payloadLength, Length - HeaderLength - payloadLength - (fcs == null ? 0 : fcs.Length));
            }
        }

        public Datagram FrameCheckSequence
        {
            get
            {
                Datagram payloadByEtherType = PayloadByEtherType;
                if (payloadByEtherType == null)
                    return null;

                if (Length - payloadByEtherType.Length >= 4 && Length >= 68)
                    return new Datagram(Buffer, Length - 4, 4);
                return null;
            }
        }

        /// <summary>
        /// The Ethernet payload as an IPv4 datagram.
        /// </summary>
        public IpV4Datagram IpV4
        {
            get { return PayloadDatagrams.IpV4; }
        }

        /// <summary>
        /// The Ethernet payload as an ARP datagram.
        /// </summary>
        public ArpDatagram Arp
        {
            get { return PayloadDatagrams.Arp; }
        }

        internal EthernetDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal static void WriteHeader(byte[] buffer, int offset, MacAddress ethernetSource, MacAddress ethernetDestination, EthernetType ethernetType)
        {
            buffer.Write(offset + Offset.Source, ethernetSource, Endianity.Big);
            buffer.Write(offset + Offset.Destination, ethernetDestination, Endianity.Big);
            buffer.Write(offset + Offset.EtherTypeLength, (ushort)ethernetType, Endianity.Big);
        }

        /// <summary>
        /// An Ethernet datagram is valid iff its length is big enough for the header and its payload is valid.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            if (Length < HeaderLength)
                return false;

            Datagram payloadByEtherType = PayloadByEtherType;
            return payloadByEtherType == null ? true : payloadByEtherType.IsValid;
        }

        private EthernetPayloadDatagrams PayloadDatagrams
        {
            get
            {
                return _payloadDatagrams ?? (_payloadDatagrams = new EthernetPayloadDatagrams(Length >= HeaderLength
                                                                                                  ? new Datagram(Buffer, StartOffset + HeaderLength,
                                                                                                                 Length - HeaderLength)
                                                                                                  : null));
            }
        }

        private Datagram PayloadByEtherType
        {
            get { return PayloadDatagrams.Get(EtherType); }
        }

        private static readonly MacAddress _broadcastAddress = new MacAddress("FF:FF:FF:FF:FF:FF");
        private EthernetPayloadDatagrams _payloadDatagrams;
    }
}