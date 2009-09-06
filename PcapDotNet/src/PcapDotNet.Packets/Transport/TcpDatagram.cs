using System;

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
        public const int HeaderMinimumLength = 20;
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
            get { return 4 * (this[Offset.HeaderLengthAndFlags] >> 4); }
        }

        public int RealHeaderLength
        {
            get { return Math.Min(HeaderLength, Length); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
        public TcpFlags Flags
        {
            get { return (TcpFlags)(ReadUShort(Offset.HeaderLengthAndFlags, Endianity.Big) & 0x01FF); }
        }

        public ushort Window
        {
            get { return ReadUShort(Offset.Window, Endianity.Big); }
        }

        public override ushort Checksum
        {
            get { return ReadUShort(Offset.Checksum, Endianity.Big); }
        }

        public override bool IsChecksumOptional
        {
            get { return false; }
        }

        public ushort UrgentPointer
        {
            get { return ReadUShort(Offset.UrgentPointer, Endianity.Big); }
        }

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

        public bool IsCongestionWindowReduced
        {
            get { return (Flags & TcpFlags.CongestionWindowReduced) == TcpFlags.CongestionWindowReduced; }
        }

        public bool IsExplicitCongestionNotificationEcho
        {
            get { return (Flags & TcpFlags.ExplicitCongestionNotificationEcho) == TcpFlags.ExplicitCongestionNotificationEcho; }
        }

        public bool IsUrgent
        {
            get { return (Flags & TcpFlags.Urgent) == TcpFlags.Urgent; }
        }

        public bool IsAcknowledgment
        {
            get { return (Flags & TcpFlags.Acknowledgment) == TcpFlags.Acknowledgment; }
        }

        public bool IsPush
        {
            get { return (Flags & TcpFlags.Push) == TcpFlags.Push; }
        }

        public bool IsReset
        {
            get { return (Flags & TcpFlags.Reset) == TcpFlags.Reset; }
        }

        public bool IsSynchronize
        {
            get { return (Flags & TcpFlags.Synchronize) == TcpFlags.Synchronize; }
        }

        public bool IsFin
        {
            get { return (Flags & TcpFlags.Fin) == TcpFlags.Fin; }
        }

        internal override int ChecksumOffset
        {
            get { return Offset.Checksum; }
        }

        internal static void WriteHeader(byte[] buffer, int offset,
                                         ushort sourcePort, ushort destinationPort,
                                         uint sequenceNumber, uint acknowledgmentNumber,
                                         TcpFlags flags, ushort window, ushort urgentPointer,
                                         TcpOptions options)
        {
            int headerLength = HeaderMinimumLength + options.BytesLength;

            WriteHeader(buffer, offset, sourcePort, destinationPort);
            buffer.Write(offset + Offset.SequenceNumber, sequenceNumber, Endianity.Big);
            buffer.Write(offset + Offset.AcknowledgmentNumber, acknowledgmentNumber, Endianity.Big);
            buffer.Write(offset + Offset.HeaderLengthAndFlags, (ushort)(((ushort)((headerLength / 4) << 12)) | (ushort)flags), Endianity.Big);
            buffer.Write(offset + Offset.Window, window, Endianity.Big);
            buffer.Write(offset + Offset.UrgentPointer, urgentPointer, Endianity.Big);
            options.Write(buffer, offset + Offset.Options);
        }

        private TcpOptions _options;
    }
}