using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Base;

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

        /// <summary>
        /// A sub-type of the message. Specific method of this message type.
        /// </summary>
        public byte Code
        {
            get { return this[Offset.Code]; }
        }

        /// <summary>
        /// A combination of the ICMP Message Type and Code.
        /// </summary>
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

        /// <summary>
        /// A value that should be interpreted according to the specific message.
        /// </summary>
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

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override abstract ILayer ExtractLayer();

        /// <summary>
        /// The payload of the ICMP.
        /// All the data without the header.
        /// </summary>
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

        /// <summary>
        /// ICMP is valid if the datagram's length is OK, the checksum is correct and the code is in the expected range.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            return Length >= HeaderLength && IsChecksumCorrect && Code >= MinCodeValue && Code <= MaxCodeValue;
        }

        /// <summary>
        /// The minimum valid ICMP code for this type of ICMP datagram.
        /// </summary>
        protected virtual byte MinCodeValue
        {
            get { return 0; }
        }

        /// <summary>
        /// The maximum valid ICMP code for this type of ICMP datagram.
        /// </summary>
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

        /// <summary>
        /// Creates an IcmpDatagram from a buffer according to the message type.
        /// </summary>
        /// <param name="buffer">The buffer of the datagram.</param>
        /// <param name="offset">The offset where the datagram starts.</param>
        /// <param name="length">The length of the datagram in the buffer.</param>
        /// <returns>An IcmpDatagram according to the Icmp message type.</returns>
        public static IcmpDatagram CreateDatagram(byte[] buffer, int offset, int length)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");

            if (length <= Offset.Type)
                return new IcmpUnknownDatagram(buffer, offset, length);

            IcmpMessageType messageType = (IcmpMessageType)buffer[offset + Offset.Type];

            return IcmpDatagramFactory.CreateInstance(messageType, buffer, offset, length);
//            switch (messageType)
//            {
//                case IcmpMessageType.DestinationUnreachable:
//                    return new IcmpDestinationUnreachableDatagram(buffer, offset, length);
//
//                case IcmpMessageType.TimeExceeded:
//                    return new IcmpTimeExceededDatagram(buffer, offset, length);
//
//                case IcmpMessageType.SourceQuench:
//                    return new IcmpSourceQuenchDatagram(buffer, offset, length);
//
//                case IcmpMessageType.ParameterProblem:
//                    return new IcmpParameterProblemDatagram(buffer, offset, length);
//
//                case IcmpMessageType.Redirect:
//                    return new IcmpRedirectDatagram(buffer, offset, length);
//
//                case IcmpMessageType.Echo:
//                    return new IcmpEchoDatagram(buffer, offset, length);
//
//                case IcmpMessageType.EchoReply:
//                    return new IcmpEchoReplyDatagram(buffer, offset, length);
//                    
//                case IcmpMessageType.Timestamp:
//                    return new IcmpTimestampDatagram(buffer, offset, length);
//
//                case IcmpMessageType.TimestampReply:
//                    return new IcmpTimestampReplyDatagram(buffer, offset, length);
//
//                case IcmpMessageType.InformationRequest:
//                    return new IcmpInformationRequestDatagram(buffer, offset, length);
//
//                case IcmpMessageType.InformationReply:
//                    return new IcmpInformationReplyDatagram(buffer, offset, length);
//
//                case IcmpMessageType.DomainNameRequest:
//                    return new IcmpDomainNameRequestDatagram(buffer, offset, length);
//                
//                case IcmpMessageType.RouterAdvertisement:
//                    return new IcmpRouterAdvertisementDatagram(buffer, offset, length);
//
//                case IcmpMessageType.AddressMaskRequest:
//                    return new IcmpAddressMaskRequestDatagram(buffer, offset, length);
//
//                case IcmpMessageType.AddressMaskReply:
//                    return new IcmpAddressMaskReplyDatagram(buffer, offset, length);
//
//                case IcmpMessageType.TraceRoute:
//                    return new IcmpTraceRouteDatagram(buffer, offset, length);
//
//                case IcmpMessageType.ConversionFailed:
//                    return new IcmpConversionFailedDatagram(buffer, offset, length);
//
//                case IcmpMessageType.SecurityFailures:
//                    return new IcmpSecurityFailuresDatagram(buffer, offset, length);
//
//                case IcmpMessageType.RouterSolicitation:
//                    return new IcmpRouterSolicitationDatagram(buffer, offset, length);
//
//                case IcmpMessageType.DomainNameReply: // Domain Name Reply is unsupported
//                default:
//                    return new IcmpUnknownDatagram(buffer, offset, length);
//            }
        }

        internal abstract IcmpDatagram CreateInstance(byte[] buffer, int offset, int length);

        private bool? _isChecksumCorrect;
        private Datagram _payload;
    }

    internal static class IcmpDatagramFactory
    {
        internal static IcmpDatagram CreateInstance(IcmpMessageType messageType, byte[] buffer, int offset, int length)
        {
            IcmpDatagram prototype;
            if (!_prototypes.TryGetValue(messageType, out prototype))
                return new IcmpUnknownDatagram(buffer, offset, length);

            return prototype.CreateInstance(buffer, offset, length);
        }

        private static Dictionary<IcmpMessageType, IcmpDatagram> InitializeComplexOptions()
        {
            var prototypes =
                from type in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(IcmpDatagram).IsAssignableFrom(type) &&
                      GetRegistrationAttribute(type) != null
                let constructor =
                    type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, new[] {typeof(byte[]), typeof(int), typeof(int)}, null)
                select new
                           {
                               GetRegistrationAttribute(type).MessageType,
                               Datagram = (IcmpDatagram)constructor.Invoke(new object[] {null, 0, 0})
                           };

            return prototypes.ToDictionary(prototype => prototype.MessageType, prototype => prototype.Datagram);
        }

        private static IcmpDatagramRegistrationAttribute GetRegistrationAttribute(Type type)
        {
            var registrationAttributes =
                from attribute in type.GetCustomAttributes<IcmpDatagramRegistrationAttribute>(false)
                select attribute;

            if (registrationAttributes.IsEmpty())
                return null;

            return registrationAttributes.First();
        }

        private static readonly Dictionary<IcmpMessageType, IcmpDatagram> _prototypes = InitializeComplexOptions();
    }

    internal sealed class IcmpDatagramRegistrationAttribute : Attribute
    {
        public IcmpDatagramRegistrationAttribute(IcmpMessageType messageType)
        {
            MessageType = messageType;
        }

        public IcmpMessageType MessageType { get; private set; }
    }
}