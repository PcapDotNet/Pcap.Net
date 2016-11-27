using PcapDotNet.Packets.Dhcp;
using PcapDotNet.Packets.Dns;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// RFC 768.
    /// This User Datagram Protocol (UDP) is defined to make available a datagram mode of packet-switched computer communication
    /// in  the environment of an interconnected set of computer networks.
    /// This protocol assumes that the Internet Protocol (IP) is used as the underlying protocol.
    ///
    /// <para>
    /// This protocol provides a procedure for application programs to send messages  to other programs
    /// with a minimum of protocol mechanism.
    /// The protocol is transaction oriented, and delivery and duplicate protection are not guaranteed.
    /// Applications requiring ordered reliable delivery of streams of data should use the Transmission Control Protocol (TCP).
    /// </para>
    ///
    /// <para>
    /// Format
    /// <pre>
    ///  0      7 8     15 16    23 24    31
    /// +--------+--------+--------+--------+
    /// |     Source      |   Destination   |
    /// |      Port       |      Port       |
    /// +--------+--------+--------+--------+
    /// |                 |                 |
    /// |     Length      |    Checksum     |
    /// +--------+--------+--------+--------+
    /// |
    /// |          data octets ...
    /// +---------------- ...
    /// </pre>
    /// </para>
    /// </summary>
    public sealed class UdpDatagram : TransportDatagram
    {
        /// <summary>
        /// The number of bytes the datagram header takes.
        /// </summary>
        public const int HeaderLength = 8;

        internal static class Offset
        {
            //            public const int SourcePort = 0;
            //            public const int DestinationPort = 2;
            public const int TotalLength = 4;

            public const int Checksum = 6;
        }

        /// <summary>
        /// The length in octets of this user datagram including this header and the data.
        /// (This  means  the minimum value of the length is eight.)
        /// </summary>
        public ushort TotalLength
        {
            get { return ReadUShort(Offset.TotalLength, Endianity.Big); }
        }

        /// <summary>
        /// Checksum is the 16-bit one's complement of the one's complement sum of a pseudo header of information from the IP header,
        /// the UDP header, and the data, padded  with zero octets at the end (if  necessary) to  make  a multiple of two octets.
        /// </summary>
        public override ushort Checksum
        {
            get { return ReadUShort(Offset.Checksum, Endianity.Big); }
        }

        /// <summary>
        /// True iff the checksum for the transport type is optional.
        /// </summary>
        public override bool IsChecksumOptional
        {
            get { return true; }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new UdpLayer
            {
                Checksum = Checksum,
                SourcePort = SourcePort,
                DestinationPort = DestinationPort,
                CalculateChecksumValue = (Checksum != 0)
            };
        }

        /// <summary>
        /// The payload of the UDP datagram.
        /// </summary>
        public override Datagram Payload
        {
            get { return new Datagram(Buffer, StartOffset + HeaderLength, Length - HeaderLength); }
        }

        /// <summary>
        /// The payload of the datagram as a DNS datagram.
        /// </summary>
        public DnsDatagram Dns
        {
            get
            {
                if (_dns == null && Length >= HeaderLength)
                    _dns = new DnsDatagram(Buffer, StartOffset + HeaderLength, Length - HeaderLength);

                return _dns;
            }
        }

        /// <summary>
        /// The payload of the datagram as a DHCP datagram.
        /// </summary>
        public DhcpDatagram Dhcp
        {
            get
            {
                if (_dhcp == null && Length >= HeaderLength)
                    _dhcp = new DhcpDatagram(Buffer, StartOffset + HeaderLength, Length - HeaderLength);

                return _dhcp;
            }
        }

        internal override int ChecksumOffset
        {
            get { return Offset.Checksum; }
        }

        internal UdpDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal static void WriteHeader(byte[] buffer, int offset, ushort sourcePort, ushort destinationPort, int payloadLength)
        {
            WriteHeader(buffer, offset, sourcePort, destinationPort);
            buffer.Write(offset + Offset.TotalLength, (ushort)(HeaderLength + payloadLength), Endianity.Big);
        }

        /// <summary>
        /// A udp datagram is valid if it has a full header.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            return Length >= HeaderLength;
        }

        private DnsDatagram _dns;
        private DhcpDatagram _dhcp;
    }
}