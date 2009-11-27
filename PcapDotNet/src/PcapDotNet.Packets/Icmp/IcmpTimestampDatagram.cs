using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// <pre>
    /// +-----+------------+-----------------+
    /// | Bit | 0-15       | 16-31           |
    /// +-----+------------+-----------------+
    /// | 0   | Identifier | Sequence Number |
    /// +-----+------------+-----------------+
    /// | 32  | Originate Timestamp          |
    /// +-----+------------------------------+
    /// | 64  | Receive Timestamp            |
    /// +-----+------------------------------+
    /// | 96  | Transmit Timestamp           |
    /// +-----+------------------------------+
    /// </pre>
    /// </summary>
    public class IcmpTimestampDatagram : IcmpIdentifiedDatagram
    {
        public const int HeaderLength = HeaderMinimumLength + HeaderAdditionalLength;
        public const int HeaderAdditionalLength = 12;

        private class Offset
        {
            public const int OriginateTimestamp = 4;
            public const int ReceiveTimestamp = 8;
            public const int TransmitTimestamp = 12;
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

        internal IcmpTimestampDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal static void WriteHeaderAdditional(byte[] buffer, int offset, IpV4TimeOfDay originateTimestamp, IpV4TimeOfDay receiveTimestamp, IpV4TimeOfDay transmitTimestamp)
        {
            buffer.Write(ref offset, originateTimestamp, Endianity.Big);
            buffer.Write(ref offset, receiveTimestamp, Endianity.Big);
            buffer.Write(offset, transmitTimestamp, Endianity.Big);
        }
    }
}