using System;
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
        /// The Ethernet payload.
        /// </summary>
        public Datagram Payload
        {
            get { return IpV4; }
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
            get
            {
                return ReadMacAddress(Offset.Source, Endianity.Big);
            }
        }

        /// <summary>
        /// Ethernet destination address.
        /// </summary>
        public MacAddress Destination
        { 
            get
            {
                return ReadMacAddress(Offset.Destination, Endianity.Big);
            }
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
        /// The Ethernet payload as an IPv4 datagram.
        /// </summary>
        public IpV4Datagram IpV4
        {
            get
            {
                if (_ipV4 == null && Length >= HeaderLength)
                    _ipV4 = new IpV4Datagram(Buffer, StartOffset + HeaderLength, Length - HeaderLength);
                return _ipV4;
            }
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

            switch (EtherType)
            {
                case EthernetType.IpV4:
                    return IpV4.IsValid;
                default:
                    return true;
            }
        }

        private IpV4Datagram _ipV4;
    }
}