using System;
using System.Linq;

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
    public class IcmpDatagram : Datagram
    {
        /// <summary>
        /// The number of bytes the ICMP header takes.
        /// </summary>
        public const int HeaderLength = 8;

        public const int HeaderWithoutValueLength = HeaderLength - sizeof(uint);

        private static class Offset
        {
            public const int Type = 0;
            public const int Code = 1;
            public const int Checksum = 2;
            public const int Variable = 4;
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

        public override ILayer ExtractLayer()
        {
            switch (MessageType)
            {
                case IcmpMessageType.DestinationUnreachable:
                    return new IcmpDestinationUnreachableLayer
                    {
                        Code = (IcmpCodeDestinationUnrechable)Code,
                        Checksum = Checksum
                    };

                case IcmpMessageType.TimeExceeded:
                    return new IcmpTimeExceededLayer
                    {
                        Code = (IcmpCodeTimeExceeded)Code,
                        Checksum = Checksum
                    };

                case IcmpMessageType.ParameterProblem:
                    return new IcmpParameterProblemLayer
                               {
                        Checksum = Checksum,
                                   Pointer = ParameterProblem.Pointer
                               };

                case IcmpMessageType.SourceQuench:
                    return new IcmpSourceQuenchLayer
                               {
                                   Checksum = Checksum
                               };
                
                case IcmpMessageType.Redirect:
                    return new IcmpRedirectLayer
                               {
                                   Code = (IcmpCodeRedirect)Code,
                        Checksum = Checksum,
                                   GatewayInternetAddress = Redirect.GatewayInternetAddress
                               };

                case IcmpMessageType.Echo:
                    return new IcmpEchoLayer
                               {
                                   Checksum = Checksum,
                                   Identifier = Echo.Identifier,
                                   SequenceNumber = Echo.SequenceNumber
                               };
                    
                case IcmpMessageType.EchoReply:
                    return new IcmpEchoReplyLayer
                    {
                        Identifier = EchoReply.Identifier,
                        SequenceNumber = EchoReply.SequenceNumber
                    };


                case IcmpMessageType.Timestamp:
                    return new IcmpTimestampLayer
                               {
                                   Checksum = Checksum,
                                   Identifier = Timestamp.Identifier,
                                   SequenceNumber = Timestamp.SequenceNumber,
                                   OriginateTimestamp = Timestamp.OriginateTimestamp,
                                   ReceiveTimestamp = Timestamp.ReceiveTimestamp,
                                   TransmitTimestamp = Timestamp.TransmitTimestamp
                               };
                                        
                case IcmpMessageType.TimestampReply:
                    return new IcmpTimestampReplyLayer
                               {
                                   Checksum = Checksum,
                                   Identifier = TimestampReply.Identifier,
                                   SequenceNumber = TimestampReply.SequenceNumber,
                                   OriginateTimestamp = TimestampReply.OriginateTimestamp,
                                   ReceiveTimestamp = TimestampReply.ReceiveTimestamp,
                                   TransmitTimestamp = TimestampReply.TransmitTimestamp,
                               };
                                 
                case IcmpMessageType.InformationRequest:
                    return new IcmpInformationRequestLayer
                               {
                                   Checksum = Checksum,
                                   Identifier = InformationRequest.Identifier,
                                   SequenceNumber = InformationRequest.SequenceNumber
                               };
                    
                case IcmpMessageType.InformationReply:
                    return new IcmpInformationReplyLayer
                               {
                                   Identifier = InformationReply.Identifier,
                                   SequenceNumber = InformationReply.SequenceNumber
                               };


                case IcmpMessageType.RouterAdvertisement:
                    return new IcmpRouterAdvertisementLayer
                               {
                                   Checksum = Checksum,
                                   Lifetime = RouterAdvertisement.Lifetime,
                                   Entries = RouterAdvertisement.Entries.ToList()
                               };

                case IcmpMessageType.RouterSolicitation:
                    return new IcmpRouterSolicitationLayer
                               {
                                   Checksum = Checksum,
                               };

                case IcmpMessageType.AddressMaskRequest:
                    return new IcmpAddressMaskRequestLayer
                               {
                                   Checksum = Checksum,
                                   Identifier = AddressMaskRequest.Identifier,
                                   SequenceNumber = AddressMaskRequest.SequenceNumber,
                                   AddressMask = AddressMaskRequest.AddressMask
                               };

                case IcmpMessageType.AddressMaskReply:
                    return new IcmpAddressMaskReplyLayer
                               {
                                   Checksum = Checksum,
                                   Identifier = AddressMaskReply.Identifier,
                                   SequenceNumber = AddressMaskReply.SequenceNumber,
                                   AddressMask = AddressMaskReply.AddressMask
                               };
                
                case IcmpMessageType.Traceroute:
                    return new IcmpTracerouteLayer
                               {
                                   Code = (IcmpCodeTraceroute)Code,
                                   Checksum = Checksum,
                                   Identification = Traceroute.Identification,
                                   OutboundHopCount = Traceroute.OutboundHopCount,
                                   ReturnHopCount = Traceroute.ReturnHopCount,
                                   OutputLinkSpeed = Traceroute.OutputLinkSpeed,
                                   OutputLinkMtu = Traceroute.OutputLinkMtu
                               };

                case IcmpMessageType.ConversionFailed:
                    return new IcmpConversionFailedLayer
                               {
                                   Code = (IcmpCodeConversionFailed)Code,
                                   Checksum = Checksum,
                                   Pointer = ConversionFailed.Pointer
                               };

                case IcmpMessageType.DomainNameRequest:
                    return new IcmpDomainNameRequestLayer
                               {
                                   Checksum = Checksum,
                                   Identifier = DomainNameRequest.Identifier,
                                   SequenceNumber = DomainNameRequest.SequenceNumber
                               };

                case IcmpMessageType.DomainNameReply:
                    throw new NotSupportedException("Message Type " + MessageType + " is not supported");

                case IcmpMessageType.SecurityFailures:
                    return new IcmpSecurityFailuresLayer
                               {
                                   Code = (IcmpCodeSecurityFailures)Code,
                                   Checksum = Checksum,
                                   Pointer = SecurityFailures.Pointer
                               };

                default:
                    throw new InvalidOperationException("Invalid icmpMessageType " + MessageType);
            }
        }

        public Datagram Payload
        {
            get { return Typed; }
        }

        public IcmpIpV4HeaderPlus64BitsPayloadDatagram DestinationUncreachable
        {
            get { return IpV4HeaderPlus64BitsPayload; }
        }

        public IcmpIpV4HeaderPlus64BitsPayloadDatagram TimeExceeded
        {
            get { return IpV4HeaderPlus64BitsPayload; }
        }

        public IcmpParameterProblemDatagram ParameterProblem
        {
            get
            {
                if (_parameterProblem == null && Length >= Offset.Variable)
                    _parameterProblem = new IcmpParameterProblemDatagram(Buffer, StartOffset + Offset.Variable, Length - Offset.Variable);
                return _parameterProblem;
            }
        }

        public IcmpIpV4HeaderPlus64BitsPayloadDatagram SourceQuench
        {
            get { return IpV4HeaderPlus64BitsPayload; }
        }


        public IcmpRedirectDatagram Redirect
        {
            get
            {
                if (_redirect == null && Length >= Offset.Variable)
                    _redirect = new IcmpRedirectDatagram(Buffer, StartOffset + Offset.Variable, Length - Offset.Variable);
                return _redirect;
            }
        }

        public IcmpEchoDatagram Echo
        {
            get
            {
                if (_echo == null && Length >= Offset.Variable)
                    _echo = new IcmpEchoDatagram(Buffer, StartOffset + Offset.Variable, Length - Offset.Variable);
                return _echo;
            }
        }

        public IcmpEchoDatagram EchoReply
        {
            get { return Echo; }
        }

        public IcmpTimestampDatagram Timestamp
        {
            get
            {
                if (_timestamp == null && Length >= Offset.Variable)
                    _timestamp = new IcmpTimestampDatagram(Buffer, StartOffset + Offset.Variable, Length - Offset.Variable);
                return _timestamp;
            }
        }

        public IcmpTimestampDatagram TimestampReply
        {
            get { return Timestamp; }
        }

        public IcmpIdentifiedDatagram InformationRequest
        {
            get { return Identified; }
        }

        public IcmpIdentifiedDatagram InformationReply
        {
            get { return Identified; }
        }

        public IcmpRouterAdvertisementDatagram RouterAdvertisement
        {
            get
            {
                if (_routerAdvertisement == null && Length >= Offset.Variable)
                    _routerAdvertisement = new IcmpRouterAdvertisementDatagram(Buffer, StartOffset + Offset.Variable, Length - Offset.Variable);
                return _routerAdvertisement;
            }
        }

        public IcmpTypedDatagram RouterSolicitation
        {
            get { return Typed; }
        }

        public IcmpAddressMaskDatagram AddressMaskRequest
        {
            get
            {
                if (_addressMask == null && Length >= Offset.Variable)
                    _addressMask = new IcmpAddressMaskDatagram(Buffer, StartOffset + Offset.Variable, Length - Offset.Variable);
                return _addressMask;
            }
        }

        public IcmpAddressMaskDatagram AddressMaskReply
        {
            get { return AddressMaskRequest; }
        }

        public IcmpTracerouteDatagram Traceroute
        {
            get
            {
                if (_traceroute == null && Length >= Offset.Variable)
                    _traceroute = new IcmpTracerouteDatagram(Buffer, StartOffset + Offset.Variable, Length - Offset.Variable);
                return _traceroute;
            }
        }

        public IcmpConversionFailedDatagram ConversionFailed
        {
            get
            {
                if (_conversionFailed == null && Length >= Offset.Variable)
                    _conversionFailed = new IcmpConversionFailedDatagram(Buffer, StartOffset + Offset.Variable, Length - Offset.Variable);
                return _conversionFailed;
            }
        }

        public IcmpIdentifiedDatagram DomainNameRequest
        {
            get { return Identified; }
        }

        public IcmpSecurityFailuresDatagram SecurityFailures
        {
            get
            {
                if (_securityFailures == null && Length >= Offset.Variable)
                    _securityFailures = new IcmpSecurityFailuresDatagram(Buffer, StartOffset + Offset.Variable, Length - Offset.Variable);
                return _securityFailures;
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
            if (Length < HeaderLength || !IsChecksumCorrect)
                return false;

            switch (MessageType)
            {
                default:
                    return false;
            }
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

        private IcmpTypedDatagram Typed
        {
            get
            {
                if (_typed == null && Length >= Offset.Variable)
                    _typed = new IcmpTypedDatagram(Buffer, StartOffset + Offset.Variable, Length - Offset.Variable);
                return _typed;
            }
        }

        private IcmpIdentifiedDatagram Identified
        {
            get
            {
                if (_identified == null && Length >= Offset.Variable)
                    _identified = new IcmpIdentifiedDatagram(Buffer, StartOffset + Offset.Variable, Length - Offset.Variable);
                return _identified;
            }
        }

        private IcmpIpV4HeaderPlus64BitsPayloadDatagram IpV4HeaderPlus64BitsPayload
        {
            get
            {
                if (_ipV4HeaderPlus64BitsPayload == null && Length >= Offset.Variable)
                    _ipV4HeaderPlus64BitsPayload = new IcmpIpV4HeaderPlus64BitsPayloadDatagram(Buffer, StartOffset + Offset.Variable, Length - Offset.Variable);
                return _ipV4HeaderPlus64BitsPayload;
            }
        }


        private bool? _isChecksumCorrect;
        private IcmpIpV4HeaderPlus64BitsPayloadDatagram _ipV4HeaderPlus64BitsPayload;
        private IcmpParameterProblemDatagram _parameterProblem;
        private IcmpRedirectDatagram _redirect;
        private IcmpEchoDatagram _echo;
        private IcmpTimestampDatagram _timestamp;
        private IcmpIdentifiedDatagram _identified;
        private IcmpRouterAdvertisementDatagram _routerAdvertisement;
        private IcmpTypedDatagram _typed;
        private IcmpAddressMaskDatagram _addressMask;
        private IcmpTracerouteDatagram _traceroute;
        private IcmpConversionFailedDatagram _conversionFailed;
        private IcmpSecurityFailuresDatagram _securityFailures;
    }
}