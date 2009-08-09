namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// TCP Header Format
    /// +-----+-------------+----------+----+-----+-----+-----+-----+-----+-----+-----+-----+------------------+
    /// | Bit | 0-3         | 4-6      | 7  | 8   | 9   | 10  | 11  | 12  | 13  | 14  | 15  | 16-31            |
    /// +-----+-------------+----------+----+-----+-----+-----+-----+-----+-----+-----+-----+------------------+
    /// | 0   | Source Port                                                                 | Destination Port |
    /// +-----+-----------------------------------------------------------------------------+------------------+
    /// | 32  | Sequence Number                                                                                |
    /// +-----+------------------------------------------------------------------------------------------------+
    /// | 64  | Acknowledgment Number                                                                          |
    /// +-----+-------------+----------+----+-----+-----+-----+-----+-----+-----+-----+-----+------------------+
    /// | 96  | Data Offset | Reserved | NS | CWR | ECE | URG | ACK | PSH | RST | SYN | FIN | Window           |
    /// +-----+-------------+----------+----+-----+-----+-----+-----+-----+-----+-----+-----+------------------+
    /// | 128 | Checksum                                                                    | Urgent Pointer   |
    /// +-----+-----------------------------------------------------------------------------+------------------+
    /// | 160 | Options + Padding                                                                              |
    /// +-----+------------------------------------------------------------------------------------------------+
    /// </summary>
    public class TcpDatagram : TransportDatagram
    {
        private static class Offset
        {
            public const int SequenceNumber = 4;
            public const int AcknowledgmentNumber = 8;
            public const int HeaderLength = 12;
            public const int Flags = 12;
            public const int Window = 14;
            public const int Checksum = 16;
            public const int UrgentPointer = 18;
            public const int Options = 20;
        }

        public TcpDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        /// <summary>
        /// The sequence number of the first data octet in this segment (except when SYN is present). 
        /// If SYN is present the sequence number is the initial sequence number (ISN) and the first data octet is ISN+1.
        /// </summary>
        public uint SequenceNumber
        {
            get { return ReadUInt(Offset.SequenceNumber, Endianity.Big); }
        }

        /// <summary>
        /// If the ACK control bit is set this field contains the value of the next sequence number 
        /// the sender of the segment is expecting to receive.  
        /// Once a connection is established this is always sent.
        /// </summary>
        public uint AcknowledgmentNumber
        {
            get { return ReadUInt(Offset.AcknowledgmentNumber, Endianity.Big); }
        }

        /// <summary>
        /// The number of bytes in the TCP Header.
        /// This indicates where the data begins.
        /// The TCP header (even one including options) is an integral number of 32 bits (4 bytes) long.
        /// </summary>
        public int HeaderLength
        {
            get { return 4 * (this[Offset.HeaderLength] >> 4); }
        }

        public TcpFlags Flags
        {
            get { return (TcpFlags)(ReadUShort(Offset.Flags, Endianity.Big) & 0x01FF); }
        }

        public ushort Window
        {
            get { return ReadUShort(Offset.Window, Endianity.Big); }
        }

        public ushort Checksum
        {
            get { return ReadUShort(Offset.Checksum, Endianity.Big); }
        }

        public ushort UrgentPointer
        {
            get { return ReadUShort(Offset.UrgentPointer, Endianity.Big); }
        }
    }
}