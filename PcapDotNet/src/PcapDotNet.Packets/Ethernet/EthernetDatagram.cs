using System;

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
    public sealed class EthernetDatagram : EthernetBaseDatagram
    {
        private static class Offset
        {
            public const int Destination = 0;
            public const int Source = Destination + MacAddress.SizeOf;
            public const int EtherTypeLength = Source + MacAddress.SizeOf;
        }

        /// <summary>
        /// Ethernet header length in bytes.
        /// </summary>
        public const int HeaderLengthValue = Offset.EtherTypeLength + sizeof(ushort);

        /// <summary>
        /// Header length in bytes.
        /// </summary>
        public override int HeaderLength
        {
            get { return HeaderLengthValue; }
        } 

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
        public override EthernetType EtherType 
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
            return payloadByEtherType == null || payloadByEtherType.IsValid;
        }

        private static readonly MacAddress _broadcastAddress = new MacAddress("FF:FF:FF:FF:FF:FF");
    }
}