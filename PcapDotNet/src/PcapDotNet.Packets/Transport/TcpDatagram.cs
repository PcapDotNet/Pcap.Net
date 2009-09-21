using System;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// TCP Header Format
    /// <pre>
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
    /// </pre>
    /// </summary>
    public class TcpDatagram : TransportDatagram
    {
        /// <summary>
        /// The minimum number of bytes the header takes.
        /// </summary>
        public const int HeaderMinimumLength = 20;

        /// <summary>
        /// The maximum number of bytes the header takes.
        /// </summary>
        public const int HeaderMaximumLength = 60;

        internal static class Offset
        {
            public const int SequenceNumber = 4;
            public const int AcknowledgmentNumber = 8;
            public const int HeaderLengthAndFlags = 12;
            public const int Window = 14;
            public const int Checksum = 16;
            public const int UrgentPointer = 18;
            public const int Options = 20;
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
            get { return 4 * (this[Offset.HeaderLengthAndFlags] >> 4); }
        }

        /// <summary>
        /// Returns the actual header length.
        /// </summary>
        public int RealHeaderLength
        {
            get { return Math.Min(HeaderLength, Length); }
        }

        /// <summary>
        /// A collection of bits for the TCP control.
        /// </summary>
        public TcpControlBits ControlBits
        {
            get { return (TcpControlBits)(ReadUShort(Offset.HeaderLengthAndFlags, Endianity.Big) & 0x01FF); }
        }

        /// <summary>
        /// The number of data octets beginning with the one indicated in the acknowledgment field which the sender of this segment is willing to accept.
        /// </summary>
        public ushort Window
        {
            get { return ReadUShort(Offset.Window, Endianity.Big); }
        }

        /// <summary>
        /// The checksum field is the 16 bit one's complement of the one's complement sum of all 16 bit words in the header and text.  
        /// If a segment contains an odd number of header and text octets to be checksummed, 
        /// the last octet is padded on the right with zeros to form a 16 bit word for checksum purposes.  
        /// The pad is not transmitted as part of the segment.  
        /// While computing the checksum, the checksum field itself is replaced with zeros.
        /// 
        /// The checksum also covers a 96 bit pseudo header conceptually prefixed to the TCP header.  
        /// This pseudo header contains the Source Address, the Destination Address, the Protocol, and TCP length.
        /// This gives the TCP protection against misrouted segments.  
        /// This information is carried in the Internet Protocol and is transferred across the TCP/Network interface in the arguments or results of calls 
        /// by the TCP on the IP.
        /// 
        /// +--------+--------+--------+--------+
        /// |           Source Address          |
        /// +--------+--------+--------+--------+
        /// |         Destination Address       |
        /// +--------+--------+--------+--------+
        /// |  zero  |  PTCL  |    TCP Length   |
        /// +--------+--------+--------+--------+
        /// 
        /// The TCP Length is the TCP header length plus the data length in octets (this is not an explicitly transmitted quantity, but is computed), 
        /// and it does not count the 12 octets of the pseudo header.
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
            get { return false; }
        }

        /// <summary>
        /// This field communicates the current value of the urgent pointer as a positive offset from the sequence number in this segment.  
        /// The urgent pointer points to the sequence number of the octet following the urgent data.  
        /// This field is only be interpreted in segments with the URG control bit set.
        /// </summary>
        public ushort UrgentPointer
        {
            get { return ReadUShort(Offset.UrgentPointer, Endianity.Big); }
        }

        /// <summary>
        /// Returns the tcp options contained in this TCP Datagram.
        /// </summary>
        public TcpOptions Options
        {
            get
            {
                if (_options == null)
                    _options = new TcpOptions(Buffer, StartOffset + Offset.Options, RealHeaderLength - HeaderMinimumLength);
                return _options;
            }
        }

        /// <summary>
        /// The payload of the TCP datagram.
        /// </summary>
        public Datagram Payload
        {
            get { return new Datagram(Buffer, StartOffset + HeaderLength, Length - HeaderLength); }
        }

        /// <summary>
        /// True iff the CongestionWindowReduced control bit is turned on.
        /// </summary>
        public bool IsCongestionWindowReduced
        {
            get { return (ControlBits & TcpControlBits.CongestionWindowReduced) == TcpControlBits.CongestionWindowReduced; }
        }

        /// <summary>
        /// True iff the ExplicitCongestionNotificationEcho control bit is turned on.
        /// </summary>
        public bool IsExplicitCongestionNotificationEcho
        {
            get { return (ControlBits & TcpControlBits.ExplicitCongestionNotificationEcho) == TcpControlBits.ExplicitCongestionNotificationEcho; }
        }

        /// <summary>
        /// True iff the Urgent control bit is turned on.
        /// </summary>
        public bool IsUrgent
        {
            get { return (ControlBits & TcpControlBits.Urgent) == TcpControlBits.Urgent; }
        }

        /// <summary>
        /// True iff the Acknowledgment control bit is turned on.
        /// </summary>
        public bool IsAcknowledgment
        {
            get { return (ControlBits & TcpControlBits.Acknowledgment) == TcpControlBits.Acknowledgment; }
        }

        /// <summary>
        /// True iff the Push control bit is turned on.
        /// </summary>
        public bool IsPush
        {
            get { return (ControlBits & TcpControlBits.Push) == TcpControlBits.Push; }
        }

        /// <summary>
        /// True iff the Reset control bit is turned on.
        /// </summary>
        public bool IsReset
        {
            get { return (ControlBits & TcpControlBits.Reset) == TcpControlBits.Reset; }
        }

        /// <summary>
        /// True iff the Synchronize control bit is turned on.
        /// </summary>
        public bool IsSynchronize
        {
            get { return (ControlBits & TcpControlBits.Synchronize) == TcpControlBits.Synchronize; }
        }

        /// <summary>
        /// True iff the Fin control bit is turned on.
        /// </summary>
        public bool IsFin
        {
            get { return (ControlBits & TcpControlBits.Fin) == TcpControlBits.Fin; }
        }

        /// <summary>
        /// The datagram is valid if the length is correct according to the header and the options are valid.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            return Length >= HeaderMinimumLength && Length >= HeaderLength && Options.IsValid;
        }

        internal TcpDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal override int ChecksumOffset
        {
            get { return Offset.Checksum; }
        }

        internal static void WriteHeader(byte[] buffer, int offset,
                                         ushort sourcePort, ushort destinationPort,
                                         uint sequenceNumber, uint acknowledgmentNumber,
                                         TcpControlBits controlBits, ushort window, ushort urgentPointer,
                                         TcpOptions options)
        {
            int headerLength = HeaderMinimumLength + options.BytesLength;

            WriteHeader(buffer, offset, sourcePort, destinationPort);
            buffer.Write(offset + Offset.SequenceNumber, sequenceNumber, Endianity.Big);
            buffer.Write(offset + Offset.AcknowledgmentNumber, acknowledgmentNumber, Endianity.Big);
            buffer.Write(offset + Offset.HeaderLengthAndFlags, (ushort)(((ushort)((headerLength / 4) << 12)) | (ushort)controlBits), Endianity.Big);
            buffer.Write(offset + Offset.Window, window, Endianity.Big);
            buffer.Write(offset + Offset.UrgentPointer, urgentPointer, Endianity.Big);
            options.Write(buffer, offset + Offset.Options);
        }

        private TcpOptions _options;
    }
}