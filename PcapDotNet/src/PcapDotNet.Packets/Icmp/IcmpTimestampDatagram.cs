using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// <pre>
    /// +-----+------+------+-----------------+
    /// | Bit | 0-7  | 8-15 | 16-31           |
    /// +-----+------+------+-----------------+
    /// | 0   | Type | Code | Checksum        |
    /// +-----+------+------+-----------------+
    /// | 32  | Identifier  | Sequence Number |
    /// +-----+-------------+-----------------+
    /// | 64  | Originate Timestamp           |
    /// +-----+-------------------------------+
    /// | 96  | Receive Timestamp             |
    /// +-----+-------------------------------+
    /// | 128 | Transmit Timestamp            |
    /// +-----+-------------------------------+
    /// </pre>
    /// </summary>
    public class IcmpTimestampDatagram : IcmpIdentifiedDatagram
    {
        /// <summary>
        /// The number of bytes this datagram should take.
        /// </summary>
        public const int DatagramLength = HeaderLength + PayloadLength;

        /// <summary>
        /// The number of bytes this ICMP payload should take.
        /// </summary>
        public const int PayloadLength = 12;

        private static class Offset
        {
            public const int OriginateTimestamp = 8;
            public const int ReceiveTimestamp = 12;
            public const int TransmitTimestamp = 16;
        }

        internal IcmpTimestampDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        /// <summary>
        /// The time the sender last touched the message before sending it.
        /// </summary>
        public IpV4TimeOfDay OriginateTimestamp
        {
            get { return ReadIpV4TimeOfDay(Offset.OriginateTimestamp, Endianity.Big); }
        }

        /// <summary>
        /// The time the echoer first touched it on receipt.
        /// </summary>
        public IpV4TimeOfDay ReceiveTimestamp
        {
            get { return ReadIpV4TimeOfDay(Offset.ReceiveTimestamp, Endianity.Big); }
        }

        /// <summary>
        /// The time the echoer last touched the message on sending it.
        /// </summary>
        public IpV4TimeOfDay TransmitTimestamp
        {
            get { return ReadIpV4TimeOfDay(Offset.TransmitTimestamp, Endianity.Big); }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IcmpTimestampLayer
                       {
                           Checksum = Checksum,
                           Identifier = Identifier,
                           SequenceNumber = SequenceNumber,
                           OriginateTimestamp = OriginateTimestamp,
                           ReceiveTimestamp = ReceiveTimestamp,
                           TransmitTimestamp = TransmitTimestamp
                       };
        }

        protected override bool CalculateIsValid()
        {
            return base.CalculateIsValid() && Length == DatagramLength;
        }

        internal static void WriteHeaderAdditional(byte[] buffer, int offset, IpV4TimeOfDay originateTimestamp, IpV4TimeOfDay receiveTimestamp, IpV4TimeOfDay transmitTimestamp)
        {
            buffer.Write(ref offset, originateTimestamp, Endianity.Big);
            buffer.Write(ref offset, receiveTimestamp, Endianity.Big);
            buffer.Write(offset, transmitTimestamp, Endianity.Big);
        }
    }
}