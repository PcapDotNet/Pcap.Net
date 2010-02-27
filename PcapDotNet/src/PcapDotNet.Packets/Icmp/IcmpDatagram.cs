using System;
using System.Linq;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// Generic
    /// <pre>
    /// +-----+------+------+-----------+
    /// | Bit | 0-7  | 8-15 | 16-31     |
    /// +-----+------+------+-----------+
    /// | 0   | Type | Code | Checksum  |
    /// +-----+------+------+-----------+
    /// | 32  | Value according to Type |
    /// +-----+-------------------------+
    /// | 64  | Payload                 |
    /// | ... |                         |
    /// +-----+-------------------------+
    /// </pre>
    /// </summary>
    public abstract class IcmpDatagram : Datagram
    {
        /// <summary>
        /// The number of bytes the ICMP header takes.
        /// </summary>
        public const int HeaderLength = 8;

        private static class Offset
        {
            public const int Type = 0;
            public const int Code = 1;
            public const int Checksum = 2;
            public const int Variable = 4;
            public const int Payload = 8;
        }

        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public IcmpMessageType MessageType
        {
            get { return (IcmpMessageType)this[Offset.Type]; }
        }

        public byte Code
        {
            get { return this[Offset.Code]; }
        }

        public IcmpMessageTypeAndCode MessageTypeAndCode
        {
            get { return (IcmpMessageTypeAndCode)ReadUShort(Offset.Type, Endianity.Big); }
        }

        /// <summary>
        /// The checksum is the 16-bit ones's complement of the one's complement sum of the ICMP message starting with the ICMP Type.
        /// For computing the checksum, the checksum field should be zero.
        /// This checksum may be replaced in the future.
        /// </summary>
        public ushort Checksum
        {
            get { return ReadUShort(Offset.Checksum, Endianity.Big); }
        }

        public uint Variable
        {
            get { return ReadUInt(Offset.Variable, Endianity.Big); }
        }

        /// <summary>
        /// True iff the checksum value is correct according to the datagram data.
        /// </summary>
        public bool IsChecksumCorrect
        {
            get
            {
                if (_isChecksumCorrect == null)
                    _isChecksumCorrect = (CalculateChecksum() == Checksum);
                return _isChecksumCorrect.Value;
            }
        }

        public override abstract ILayer ExtractLayer();

        public Datagram Payload
        {
            get
            {
                if (_payload == null && Length >= Offset.Payload)
                    _payload = new Datagram(Buffer, StartOffset + Offset.Payload, Length - Offset.Payload);
                return _payload;
            }
        }

        internal IcmpDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal static void WriteHeader(byte[] buffer, int offset,
                                         IcmpMessageType messageType, byte code, uint valueAccordingToType)
        {
            buffer.Write(offset + Offset.Type, (byte)messageType);
            buffer.Write(offset + Offset.Code, code);
            buffer.Write(offset + Offset.Variable, valueAccordingToType, Endianity.Big);
        }

        internal static void WriteChecksum(byte[] buffer, int offset, int length, ushort? checksum)
        {
            ushort checksumValue = checksum == null
                                       ? CalculateChecksum(buffer, offset, length)
                                       : checksum.Value;
            buffer.Write(offset + Offset.Checksum, checksumValue, Endianity.Big);
        }

        protected override bool CalculateIsValid()
        {
            return Length >= HeaderLength && IsChecksumCorrect && Code >= MinCodeValue && Code <= MaxCodeValue;
        }

        protected virtual byte MinCodeValue
        {
            get { return 0; }
        }

        protected virtual byte MaxCodeValue
        {
            get { return 0; }
        }

        private ushort CalculateChecksum()
        {
            return CalculateChecksum(Buffer, StartOffset, Length);
        }

        private static ushort CalculateChecksum(byte[] buffer, int offset, int length)
        {
            uint sum = Sum16Bits(buffer, offset, Math.Min(Offset.Checksum, length)) +
                       Sum16Bits(buffer, offset + Offset.Checksum + sizeof(ushort), length - Offset.Checksum - sizeof(ushort));

            return Sum16BitsToChecksum(sum);
        }

        private bool? _isChecksumCorrect;
        private Datagram _payload;

        public static IcmpDatagram CreateDatagram(byte[] buffer, int offset, int length)
        {
            if (length <= Offset.Type)
                return new IcmpUnknownDatagram(buffer, offset, length);

            IcmpMessageType messageType = (IcmpMessageType)buffer[offset + Offset.Type];
            switch (messageType)
            {
                case IcmpMessageType.DestinationUnreachable:
                    return new IcmpDestinationUnreachableDatagram(buffer, offset, length);

                case IcmpMessageType.TimeExceeded:
                    return new IcmpTimeExceededDatagram(buffer, offset, length);

                case IcmpMessageType.SourceQuench:
                    return new IcmpSourceQuenchDatagram(buffer, offset, length);

                case IcmpMessageType.ParameterProblem:
                    return new IcmpParameterProblemDatagram(buffer, offset, length);

                case IcmpMessageType.Redirect:
                    return new IcmpRedirectDatagram(buffer, offset, length);

                case IcmpMessageType.Echo:
                    return new IcmpEchoDatagram(buffer, offset, length);

                case IcmpMessageType.EchoReply:
                    return new IcmpEchoReplyDatagram(buffer, offset, length);
                    
                case IcmpMessageType.Timestamp:
                    return new IcmpTimestampDatagram(buffer, offset, length);

                case IcmpMessageType.TimestampReply:
                    return new IcmpTimestampReplyDatagram(buffer, offset, length);

                case IcmpMessageType.InformationRequest:
                    return new IcmpInformationRequestDatagram(buffer, offset, length);

                case IcmpMessageType.InformationReply:
                    return new IcmpInformationReplyDatagram(buffer, offset, length);

                case IcmpMessageType.DomainNameRequest:
                    return new IcmpDomainNameRequestDatagram(buffer, offset, length);
                
                case IcmpMessageType.RouterAdvertisement:
                    return new IcmpRouterAdvertisementDatagram(buffer, offset, length);

                case IcmpMessageType.AddressMaskRequest:
                    return new IcmpAddressMaskRequestDatagram(buffer, offset, length);

                case IcmpMessageType.AddressMaskReply:
                    return new IcmpAddressMaskReplyDatagram(buffer, offset, length);

                case IcmpMessageType.TraceRoute:
                    return new IcmpTraceRouteDatagram(buffer, offset, length);

                case IcmpMessageType.ConversionFailed:
                    return new IcmpConversionFailedDatagram(buffer, offset, length);

                case IcmpMessageType.SecurityFailures:
                    return new IcmpSecurityFailuresDatagram(buffer, offset, length);

                case IcmpMessageType.RouterSolicitation:
                    return new IcmpRouterSolicitationDatagram(buffer, offset, length);

                case IcmpMessageType.DomainNameReply: // Domain Name Reply is unsupported
                default:
                    return new IcmpUnknownDatagram(buffer, offset, length);
            }
        }
    }
}