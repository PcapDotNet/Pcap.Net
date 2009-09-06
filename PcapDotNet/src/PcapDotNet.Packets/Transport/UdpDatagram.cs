using System;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// This User Datagram Protocol (UDP) is defined to make available a datagram mode of packet-switched computer communication  
    /// in  the environment of an interconnected set of computer networks.   
    /// This protocol assumes that the Internet Protocol (IP) is used as the underlying protocol.
    /// 
    /// This protocol provides a procedure for application programs to send messages  to other programs 
    /// with a minimum of protocol mechanism. 
    /// The protocol is transaction oriented, and delivery and duplicate protection are not guaranteed.  
    /// Applications requiring ordered reliable delivery of streams of data should use the Transmission Control Protocol (TCP).
    /// 
    /// Format
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
    /// </summary>
    public class UdpDatagram : TransportDatagram
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

        public override bool IsChecksumOptional
        {
            get { return true; }
        }

        /// <summary>
        /// The payload of the UDP datagram.
        /// </summary>
        public Datagram Payload
        {
            get { return new Datagram(Buffer, StartOffset + HeaderLength, Length - HeaderLength); }
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

        protected override bool CalculateIsValid()
        {
            return Length >= HeaderLength;
        }
    }
}