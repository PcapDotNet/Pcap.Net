using System;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Icmp;
using PcapDotNet.Packets.Igmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;

namespace PcapDotNet.Packets
{
    public interface ILayer
    {
        int Length { get; }
        void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer);
        void Finalize(byte[] buffer, int offset, int payloadLength, ILayer nextLayer);
        DataLinkKind? DataLink { get; }
    }

    public interface IEthernetNextLayer : ILayer
    {
        EthernetType PreviousLayerEtherType { get; }
        MacAddress? PreviousLayerDefaultDestination { get; }
    }

    public interface IIpV4NextLayer : ILayer
    {
        IpV4Protocol PreviousLayerProtocol { get; }
    }

    public interface IIpV4NextTransportLayer : IIpV4NextLayer
    {
        ushort? Checksum { get; set; }
        bool CalculateChecksum { get; }
        int NextLayerChecksumOffset { get; }
        bool NextLayerIsChecksumOptional { get; }
        bool NextLayerCalculateChecksum { get; }
    }

    public interface IArpPreviousLayer : ILayer
    {
        ArpHardwareType PreviousLayerHardwareType { get; }   
    }

    public abstract class Layer : ILayer
    {
        public abstract int Length { get; }
        public abstract void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer);
        public virtual void Finalize(byte[] buffer, int offset, int payloadLength, ILayer nextLayer)
        {
        }

        public virtual DataLinkKind? DataLink
        {
            get { return null; }
        }

        public virtual bool Equals(Layer other)
        {
            return other != null &&
                   Length == other.Length && DataLink == other.DataLink;
        }

        public override sealed bool Equals(object obj)
        {
            return Equals(obj as Layer);
        }
    }

    public abstract class SimpleLayer : Layer
    {
        public override sealed void Write(byte[] buffer, int offset, int payloadLength, ILayer nextLayer, ILayer nextLayer1)
        {
            Write(buffer, offset);
        }

        protected abstract void Write(byte[] buffer, int offset);
    }

    public class EthernetLayer : Layer, IArpPreviousLayer
    {
        public MacAddress Source { get; set; }
        public MacAddress Destination { get; set; }
        public EthernetType EtherType { get; set; }

        public EthernetLayer()
        {
            Source = MacAddress.Zero;
            Destination = MacAddress.Zero;
            EtherType = EthernetType.None;
        }

        public override int Length
        {
            get { return EthernetDatagram.HeaderLength; }
        }

        public override void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer)
        {
            EthernetType etherType = EtherType;
            MacAddress destination = Destination;

            IEthernetNextLayer ethernetNextLayer = nextLayer as IEthernetNextLayer;
            if (etherType == EthernetType.None)
            {
                if (nextLayer == null)
                    throw new ArgumentException("Can't determine ether type automatically from next layer because there is not next layer");
                if (ethernetNextLayer == null)
                    throw new ArgumentException("Can't determine ether type automatically from next layer (" + nextLayer.GetType() + ")");
                etherType = ethernetNextLayer.PreviousLayerEtherType;
            }
            if (destination == MacAddress.Zero)
            {
                if (ethernetNextLayer != null && ethernetNextLayer.PreviousLayerDefaultDestination != null)
                    destination = ethernetNextLayer.PreviousLayerDefaultDestination.Value;
            }

            EthernetDatagram.WriteHeader(buffer, 0, Source, destination, etherType);
        }

        public override DataLinkKind? DataLink
        {
            get { return DataLinkKind.Ethernet; }
        }

        public ArpHardwareType PreviousLayerHardwareType
        {
            get { return ArpHardwareType.Ethernet; }
        }

        public bool Equals(EthernetLayer other)
        {
            return other != null &&
                   Source == other.Source && Destination == other.Destination && EtherType == other.EtherType;
        }

        public override sealed bool Equals(Layer other)
        {
            return base.Equals(other) && Equals(other as EthernetLayer);
        }

        public override string ToString()
        {
            return Source + " -> " + Destination + " (" + EtherType + ")";
        }
    }

    public class PayloadLayer : SimpleLayer
    {
        public Datagram Data { get; set; }

        public PayloadLayer()
        {
            Data = Datagram.Empty;
        }

        public override int Length
        {
            get { return Data.Length; }
        }

        public bool Equals(PayloadLayer other)
        {
            return other != null &&
                   Data.Equals(other.Data);
        }

        public override sealed bool Equals(Layer other)
        {
            return base.Equals(other) && Equals(other as PayloadLayer);
        }

        protected override void Write(byte[] buffer, int offset)
        {
            Data.Write(buffer, offset);
        }
    }

    public class IpV4Layer : Layer, IEthernetNextLayer
    {
        public IpV4Layer()
        {
            TypeOfService = 0;
            Identification = 0;
            Fragmentation = IpV4Fragmentation.None;
            Ttl = 0;
            Protocol = null;
            HeaderChecksum = null;
            Source = IpV4Address.Zero;
            Destination = IpV4Address.Zero;
            Options = IpV4Options.None;
        }

        public byte TypeOfService { get; set; }

        public ushort Identification { get; set; }

        public IpV4Fragmentation Fragmentation { get; set; }

        public byte Ttl { get; set; }

        public IpV4Protocol? Protocol { get; set; }

        public ushort? HeaderChecksum { get; set; }

        public IpV4Address Source { get; set; }

        public IpV4Address Destination { get; set; }

        public IpV4Options Options { get; set; }

        public EthernetType PreviousLayerEtherType
        {
            get { return EthernetType.IpV4; }
        }

        public MacAddress? PreviousLayerDefaultDestination
        {
            get { return null; }
        }

        public override int Length
        {
            get { return IpV4Datagram.HeaderMinimumLength + Options.BytesLength; }
        }

        public override void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer)
        {
            IpV4Protocol protocol;
            if (Protocol == null)
            {
                if (nextLayer == null)
                    throw new ArgumentException("Can't determine protocol automatically from next layer because there is no next layer");
                IIpV4NextLayer ipV4NextLayer = nextLayer as IIpV4NextLayer;
                if (ipV4NextLayer == null)
                    throw new ArgumentException("Can't determine protocol automatically from next layer (" + nextLayer.GetType() + ")");
                protocol = ipV4NextLayer.PreviousLayerProtocol;
            }
            else
                protocol = Protocol.Value;

            IpV4Datagram.WriteHeader(buffer, offset,
                                     TypeOfService, Identification, Fragmentation,
                                     Ttl, protocol, HeaderChecksum,
                                     Source, Destination,
                                     Options, payloadLength);
        }

        public override void Finalize(byte[] buffer, int offset, int payloadLength, ILayer nextLayer)
        {
            IIpV4NextTransportLayer nextTransportLayer = nextLayer as IIpV4NextTransportLayer;
            if (nextTransportLayer == null || !nextTransportLayer.NextLayerCalculateChecksum)
                return;

            if (nextTransportLayer.CalculateChecksum)
                IpV4Datagram.WriteTransportChecksum(buffer, offset, Length, (ushort)payloadLength,
                                                    nextTransportLayer.NextLayerChecksumOffset, nextTransportLayer.NextLayerIsChecksumOptional,
                                                    nextTransportLayer.Checksum);
        }

        public bool Equals(IpV4Layer other)
        {
            return other != null &&
                   TypeOfService == other.TypeOfService && Identification == other.Identification &&
                   Fragmentation == other.Fragmentation && Ttl == other.Ttl &&
                   Protocol == other.Protocol &&
                   HeaderChecksum == other.HeaderChecksum &&
                   Source == other.Source && Destination == other.Destination &&
                   Options.Equals(other.Options);
        }

        public override sealed bool Equals(Layer other)
        {
            return base.Equals(other) && Equals(other as IpV4Layer);
        }

        public override string ToString()
        {
            return Source + " -> " + Destination + " (" + Protocol + ")";
        }
    }

    public abstract class TransportLayer : Layer, IIpV4NextTransportLayer
    {
        public ushort? Checksum { get; set; }

        public ushort SourcePort { get; set; }

        public ushort DestinationPort { get; set; }

        public abstract IpV4Protocol PreviousLayerProtocol { get; }
        public bool CalculateChecksum
        {
            get { return true; }
        }

        public abstract int NextLayerChecksumOffset { get; }
        public abstract bool NextLayerIsChecksumOptional { get; }
        public abstract bool NextLayerCalculateChecksum { get; }

        public virtual bool Equals(TransportLayer other)
        {
            return other != null &&
                   PreviousLayerProtocol == other.PreviousLayerProtocol &&
                   Checksum == other.Checksum &&
                   SourcePort == other.SourcePort && DestinationPort == other.DestinationPort;
        }

        public override sealed bool Equals(Layer other)
        {
            return base.Equals(other) && Equals(other as TransportLayer);
        }
    }

    public class TcpLayer : TransportLayer
    {
        public uint SequenceNumber{get;set;}

        public uint AcknowledgmentNumber{get;set;}

        public TcpControlBits ControlBits{get;set;}

        public ushort Window{get; set;}

        public ushort UrgentPointer{get; set;}

        public TcpOptions Options{get;set;}

        public override IpV4Protocol PreviousLayerProtocol
        {
            get { return IpV4Protocol.Tcp; }
        }

        public override int NextLayerChecksumOffset
        {
            get { return TcpDatagram.Offset.Checksum; }
        }

        public override bool NextLayerIsChecksumOptional
        {
            get { return false; }
        }

        public override bool NextLayerCalculateChecksum
        {
            get { return true; }
        }

        public override int Length
        {
            get { return TcpDatagram.HeaderMinimumLength + Options.BytesLength; }
        }

        public override void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer)
        {
            TcpDatagram.WriteHeader(buffer, offset,
                                    SourcePort, DestinationPort,
                                    SequenceNumber, AcknowledgmentNumber,
                                    ControlBits, Window, UrgentPointer,
                                    Options);
        }

        public bool Equals(TcpLayer other)
        {
            return other != null &&
                   SequenceNumber == other.SequenceNumber && AcknowledgmentNumber == other.AcknowledgmentNumber &&
                   ControlBits == other.ControlBits &&
                   Window == other.Window && UrgentPointer == other.UrgentPointer &&
                   Options.Equals(other.Options);
        }

        public override sealed bool Equals(TransportLayer other)
        {
            return base.Equals(other) && Equals(other as TcpLayer);
        }
    }

    public class UdpLayer : TransportLayer
    {
        public bool CalculateChecksum { get; set; }

        public override IpV4Protocol PreviousLayerProtocol
        {
            get { return IpV4Protocol.Udp; }
        }

        public override int NextLayerChecksumOffset
        {
            get { return UdpDatagram.Offset.Checksum; }
        }

        public override bool NextLayerIsChecksumOptional
        {
            get { return true; }
        }

        public override bool NextLayerCalculateChecksum
        {
            get { return CalculateChecksum; }
        }

        public override int Length
        {
            get { return UdpDatagram.HeaderLength; }
        }

        public override void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer)
        {
            UdpDatagram.WriteHeader(buffer, offset, SourcePort, DestinationPort, payloadLength);
        }
    }

    public class ArpLayer : Layer, IEthernetNextLayer
    {
        public EthernetType ProtocolType { get; set; }

        public ArpOperation Operation { get; set; }

        public IList<byte> SenderHardwareAddress { get; set; }

        public IList<byte> SenderProtocolAddress { get; set; }

        public IList<byte> TargetHardwareAddress { get; set; }

        public IList<byte> TargetProtocolAddress { get; set; }

        public EthernetType PreviousLayerEtherType
        {
            get { return EthernetType.Arp; }
        }

        public MacAddress? PreviousLayerDefaultDestination
        {
            get { return EthernetDatagram.BroadcastAddress; }
        }

        public override int Length
        {
            get { return ArpDatagram.GetHeaderLength(SenderHardwareAddress.Count, SenderProtocolAddress.Count); }
        }

        public override void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer)
        {
            if (previousLayer == null)
                throw new ArgumentException("Must have a previous layer");

            IArpPreviousLayer arpPreviousLayer = previousLayer as IArpPreviousLayer;
            if (arpPreviousLayer == null)
                throw new ArgumentException("The layer before the ARP layer must be an ARP previous layer and can't be " + previousLayer.GetType());

            if (SenderHardwareAddress.Count != TargetHardwareAddress.Count)
            {
                throw new ArgumentException("Sender hardware address length is " + SenderHardwareAddress.Count + " bytes " +
                                            "while target hardware address length is " + TargetHardwareAddress.Count + " bytes");
            }
            if (SenderProtocolAddress.Count != TargetProtocolAddress.Count)
            {
                throw new ArgumentException("Sender protocol address length is " + SenderProtocolAddress.Count + " bytes " +
                                            "while target protocol address length is " + TargetProtocolAddress.Count + " bytes");
            }

            ArpDatagram.WriteHeader(buffer, offset,
                                    arpPreviousLayer.PreviousLayerHardwareType, ProtocolType, Operation,
                                    SenderHardwareAddress, SenderProtocolAddress, TargetHardwareAddress, TargetProtocolAddress);
        }

        public bool Equals(ArpLayer other)
        {
            return other != null &&
                   ProtocolType == other.ProtocolType && Operation == other.Operation &&
                   SenderHardwareAddress.SequenceEqual(other.SenderHardwareAddress) &&
                   SenderProtocolAddress.SequenceEqual(other.SenderProtocolAddress) &&
                   TargetHardwareAddress.SequenceEqual(other.TargetHardwareAddress) &&
                   TargetProtocolAddress.SequenceEqual(other.TargetProtocolAddress);
        }

        public override sealed bool Equals(Layer other)
        {
            return base.Equals(other) && Equals(other as ArpLayer);
        }
    }

    public abstract class IgmpLayer : SimpleLayer, IIpV4NextLayer
    {
        public abstract IgmpMessageType MessageType { get; }
        public virtual IgmpQueryVersion QueryVersion
        {
            get { return IgmpQueryVersion.None; }
        }
        public abstract TimeSpan MaxResponseTimeValue { get; }

        public IpV4Protocol PreviousLayerProtocol
        {
            get { return IpV4Protocol.InternetGroupManagementProtocol; }
        }

        public virtual bool Equals(IgmpLayer other)
        {
            return other != null &&
                   MessageType == other.MessageType &&
                   QueryVersion == other.QueryVersion &&
                   MaxResponseTimeValue.Divide(2) <= other.MaxResponseTimeValue && MaxResponseTimeValue.Multiply(2) >= other.MaxResponseTimeValue;
        }

        public sealed override bool Equals(Layer other)
        {
            return base.Equals(other) && Equals(other as IgmpLayer);
        }
    }

    public interface IIgmpLayerWithGroupAddress
    {
        IpV4Address GroupAddress { get; set; }
    }

    public abstract class IgmpSimpleLayer : IgmpLayer, IIgmpLayerWithGroupAddress
    {
        public IpV4Address GroupAddress { get; set; }
        public override int Length
        {
            get { return IgmpDatagram.HeaderLength; }
        }

        protected override void Write(byte[] buffer, int offset)
        {
            IgmpDatagram.WriteHeader(buffer, offset,
                                     MessageType, MaxResponseTimeValue, GroupAddress);
        }

        public bool Equals(IgmpSimpleLayer other)
        {
            return other != null &&
                   GroupAddress == other.GroupAddress;
        }

        public override sealed bool Equals(IgmpLayer other)
        {
            return base.Equals(other) && Equals(other as IgmpSimpleLayer);
        }
    }

    public abstract class IgmpVersion1Layer : IgmpSimpleLayer
    {
        public override TimeSpan MaxResponseTimeValue
        {
            get { return TimeSpan.Zero; }
        }
    }

    public abstract class IgmpVersion2Layer : IgmpSimpleLayer
    {
        public TimeSpan MaxResponseTime { get; set; }
        public override TimeSpan MaxResponseTimeValue
        {
            get { return MaxResponseTime; }
        }
    }

    public class IgmpQueryVersion1Layer : IgmpVersion1Layer
    {
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.MembershipQuery; }
        }

        public override IgmpQueryVersion QueryVersion
        {
            get
            {
                return IgmpQueryVersion.Version1;
            }
        }
    }

    public class IgmpQueryVersion2Layer : IgmpVersion2Layer
    {
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.MembershipQuery; }
        }

        public override IgmpQueryVersion QueryVersion
        {
            get
            {
                return IgmpQueryVersion.Version2;
            }
        }
    }

    public class IgmpQueryVersion3Layer : IgmpLayer, IIgmpLayerWithGroupAddress
    {
        public TimeSpan MaxResponseTime { get; set; }
        public IpV4Address GroupAddress { get; set; }
        public bool IsSuppressRouterSideProcessing { get; set; }

        public byte QueryRobustnessVariable{get; set ;}

        public TimeSpan QueryInterval{get; set ;}

        public IList<IpV4Address> SourceAddresses{get; set ;}

        public override int Length
        {
            get { return IgmpDatagram.GetQueryVersion3Length(SourceAddresses.Count); }
        }

        protected override void Write(byte[] buffer, int offset)
        {
            IgmpDatagram.WriteQueryVersion3(buffer, offset,
                                            MaxResponseTime, GroupAddress, IsSuppressRouterSideProcessing, QueryRobustnessVariable,
                                            QueryInterval, SourceAddresses);
        }

        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.MembershipQuery; }
        }

        public override IgmpQueryVersion QueryVersion
        {
            get
            {
                return IgmpQueryVersion.Version3;
            }
        }

        public override TimeSpan MaxResponseTimeValue
        {
            get { return MaxResponseTime; }
        }

        public bool Equals(IgmpQueryVersion3Layer other)
        {
            return other != null &&
                   GroupAddress == other.GroupAddress &&
                   IsSuppressRouterSideProcessing == other.IsSuppressRouterSideProcessing &&
                   QueryRobustnessVariable == other.QueryRobustnessVariable &&
                   QueryInterval.Divide(2) <= other.QueryInterval && QueryInterval.Multiply(2) >= other.QueryInterval &&
                   SourceAddresses.SequenceEqual(other.SourceAddresses);
        }

        public override sealed bool Equals(IgmpLayer other)
        {
            return base.Equals(other) && Equals(other as IgmpQueryVersion3Layer);
        }
    }

    public class IgmpReportVersion1Layer : IgmpVersion1Layer
    {
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.MembershipReportVersion1; }
        }
    }

    public class IgmpReportVersion2Layer : IgmpVersion2Layer
    {
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.MembershipReportVersion2; }
        }
    }

    public class IgmpLeaveGroupVersion2Layer : IgmpVersion2Layer
    {
        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.LeaveGroupVersion2; }
        }
    }

    public class IgmpReportVersion3Layer : IgmpLayer
    {
        public IList<IgmpGroupRecord> GroupRecords{get; set ;}

        public override int Length
        {
            get { return IgmpDatagram.GetReportVersion3Length(GroupRecords); }
        }

        protected override void Write(byte[] buffer, int offset)
        {
            IgmpDatagram.WriteReportVersion3(buffer, offset, GroupRecords);
        }

        public override IgmpMessageType MessageType
        {
            get { return IgmpMessageType.MembershipReportVersion3; }
        }

        public override TimeSpan MaxResponseTimeValue
        {
            get { return TimeSpan.Zero; }
        }

        public bool Equals(IgmpReportVersion3Layer other)
        {
            return other != null &&
                   GroupRecords.SequenceEqual(other.GroupRecords);
        }

        public sealed override bool Equals(IgmpLayer other)
        {
            return base.Equals(other) && Equals(other as IgmpReportVersion3Layer);
        }
    }

    public abstract class IcmpLayer : SimpleLayer, IIpV4NextLayer
    {
        public abstract IcmpMessageType MessageType { get; }
        public virtual byte CodeValue
        {
            get { return 0; }
        }
        public ushort? Checksum { get; set; }
        protected virtual uint Value 
        { 
            get { return 0; }
        }

        public override int Length
        {
            get { return IcmpDatagram.HeaderLength; } 
        }

        protected override sealed void Write(byte[] buffer, int offset)
        {
            IcmpDatagram.WriteHeader(buffer, offset, MessageType, CodeValue, Value);
            WriteHeaderAdditional(buffer, offset + IcmpDatagram.HeaderLength);
        }

        protected virtual void WriteHeaderAdditional(byte[] buffer, int offset)
        {
        }

        public override void Finalize(byte[] buffer, int offset, int payloadLength, ILayer nextLayer)
        {
            IcmpDatagram.WriteChecksum(buffer, offset, Length + payloadLength, Checksum);
        }

        public IpV4Protocol PreviousLayerProtocol
        {
            get { return IpV4Protocol.InternetControlMessageProtocol; }
        }

        public virtual bool Equals(IcmpLayer other)
        {
            return other != null &&
                   MessageType == other.MessageType && CodeValue == other.CodeValue &&
                   Checksum == other.Checksum &&
                   Value == other.Value;
        }

        public sealed override bool Equals(Layer other)
        {
            return base.Equals(other) && Equals(other as IcmpLayer);
        }
    }

    public class IcmpDestinationUnreachableLayer : IcmpLayer
    {
        public IcmpCodeDestinationUnrechable Code { get; set; }

        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.DestinationUnreachable; }
        }

        public override byte CodeValue
        {
            get { return (byte)Code; }
        }
    }

    public class IcmpTimeExceededLayer : IcmpLayer
    {
        public IcmpCodeTimeExceeded Code { get; set; }

        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.TimeExceeded; }
        }

        public override byte CodeValue
        {
            get { return (byte)Code; }
        }
    }

    public class IcmpParameterProblemLayer : IcmpLayer
    {
        public byte Pointer { get; set; }

        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.ParameterProblem; }
        }

        protected override uint Value
        {
            get
            {
                return (uint)(Pointer << 24);
            }
        }
    }

    public class IcmpSourceQuenchLayer : IcmpLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.SourceQuench; }
        }
    }

    public class IcmpRedirectLayer : IcmpLayer
    {
        public IcmpCodeRedirect Code { get; set; }
        public IpV4Address GatewayInternetAddress{get;set;}
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.Redirect; }
        }

        public override byte CodeValue
        {
            get
            {
                return (byte)Code;
            }
        }

        protected override uint Value
        {
            get
            {
                return GatewayInternetAddress.ToValue();
            }
        }
    }

    public abstract class IcmpIdentifiedLayer : IcmpLayer
    {
        public ushort Identifier { get; set; }

        public ushort SequenceNumber { get; set; }

        protected override sealed uint Value
        {
            get
            {
                return (uint)((Identifier << 16) | SequenceNumber);
            }
        }
    }

    public class IcmpEchoLayer : IcmpIdentifiedLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.Echo; }
        }
    }

    public class IcmpEchoReplyLayer : IcmpIdentifiedLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.EchoReply; }
        }
    }

    public class IcmpTimestampLayer : IcmpIdentifiedLayer
    {
        public IpV4TimeOfDay OriginateTimestamp { get; set; }
        public IpV4TimeOfDay ReceiveTimestamp { get; set; }
        public IpV4TimeOfDay TransmitTimestamp { get; set; }
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.Timestamp; }
        }

        public override int Length
        {
            get
            {
                return base.Length + IcmpTimestampDatagram.HeaderAdditionalLength;
            }
        }

        protected override void WriteHeaderAdditional(byte[] buffer, int offset)
        {
            IcmpTimestampDatagram.WriteHeaderAdditional(buffer, offset,
                                                        OriginateTimestamp, ReceiveTimestamp, TransmitTimestamp);
        }

        public bool Equals(IcmpTimestampLayer other)
        {
            return other != null &&
                   OriginateTimestamp == other.OriginateTimestamp &&
                   ReceiveTimestamp == other.ReceiveTimestamp &&
                   TransmitTimestamp == other.TransmitTimestamp;
        }

        public sealed override bool Equals(IcmpLayer other)
        {
            return base.Equals(other) && Equals(other as IcmpTimestampLayer);
        }
    }

    public class IcmpTimestampReplyLayer : IcmpTimestampLayer
    {
        public override IcmpMessageType MessageType
        {
            get
            {
                return IcmpMessageType.TimestampReply;
            }
        }
    }

    public class IcmpInformationRequestLayer : IcmpIdentifiedLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.InformationRequest; }
        }
    }

    public class IcmpInformationReplyLayer : IcmpIdentifiedLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.InformationReply; }
        }
    }

    public class IcmpRouterAdvertisementLayer : IcmpLayer
    {
        public TimeSpan Lifetime { get; set; }
        public List<IcmpRouterAdvertisementEntry> Entries { get; set; }
        
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.RouterAdvertisement; }
        }

        protected override uint Value
        {
            get
            {
                return (uint)(((byte)Entries.Count << 24) |
                              (IcmpRouterAdvertisementDatagram.DefaultAddressEntrySize << 16) |
                              ((ushort)Lifetime.TotalSeconds));
            }
        }

        public override int Length
        {
            get
            {
                return base.Length + IcmpRouterAdvertisementDatagram.GetHeaderAdditionalLength(Entries.Count);
            }
        }

        protected override void WriteHeaderAdditional(byte[] buffer, int offset)
        {
            IcmpRouterAdvertisementDatagram.WriteHeaderAdditional(buffer, offset, Entries);
        }

        public bool Equals(IcmpRouterAdvertisementLayer other)
        {
            return other != null &&
                   Entries.SequenceEqual(other.Entries);
        }

        public sealed override bool Equals(IcmpLayer other)
        {
            return base.Equals(other) && Equals(other as IcmpRouterAdvertisementLayer);
        }
    }

    public class IcmpRouterSolicitationLayer : IcmpLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.RouterSolicitation; }
        }
    }

    public class IcmpAddressMaskRequestLayer : IcmpIdentifiedLayer
    {
        public IpV4Address AddressMask { get; set; }

        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.AddressMaskRequest; }
        }

        public override int Length
        {
            get
            {
                return base.Length + IcmpAddressMaskDatagram.HeaderAdditionalLength;
            }
        }

        protected override void WriteHeaderAdditional(byte[] buffer, int offset)
        {
            IcmpAddressMaskDatagram.WriteHeaderAdditional(buffer, offset, AddressMask);
        }

        public bool Equals(IcmpAddressMaskRequestLayer other)
        {
            return other != null &&
                   AddressMask == other.AddressMask;
        }

        public override sealed bool Equals(IcmpLayer other)
        {
            return base.Equals(other) && Equals(other as IcmpAddressMaskRequestLayer);
        }
    }

    public class IcmpAddressMaskReplyLayer : IcmpAddressMaskRequestLayer
    {
        public override IcmpMessageType MessageType
        {
            get
            {
                return IcmpMessageType.AddressMaskReply;
            }
        }
    }

    public class IcmpConversionFailedLayer : IcmpLayer
    {
        public IcmpCodeConversionFailed Code { get; set; }
        public uint Pointer { get; set; }
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.ConversionFailed; }
        }

        public override byte CodeValue
        {
            get
            {
                return (byte)Code;
            }
        }

        protected override uint Value
        {
            get
            {
                return Pointer;
            }
        }
    }

    public class IcmpDomainNameRequestLayer : IcmpIdentifiedLayer
    {
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.DomainNameRequest; }
        }
    }

    public class IcmpSecurityFailuresLayer : IcmpLayer
    {
        public IcmpCodeSecurityFailures Code{get; set ;}
        public ushort Pointer{get; set ;}
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.SecurityFailures; }
        }

        public override byte CodeValue
        {
            get
            {
                return (byte)Code;
            }
        }

        protected override uint Value
        {
            get
            {
                return Pointer;
            }
        }
    }

    public class PacketBuilder2
    {
        public static Packet Build(DateTime timestamp, params ILayer[] layers)
        {
            return new PacketBuilder2(layers).Build(timestamp);
        }

        public PacketBuilder2(params ILayer[] layers)
        {
            if (layers.Length == 0)
                throw new ArgumentException("At least one layer must be given", "layers");

            DataLinkKind? dataLinkKind = layers[0].DataLink;
            if (dataLinkKind == null)
                throw new ArgumentException("First layer (" + layers[0].GetType() + ") must provide a DataLink", "layers");

            _layers = layers;
            _dataLink = new DataLink(dataLinkKind.Value);
        }


        public Packet Build(DateTime timestamp)
        {
            int length = _layers.Select(layer => layer.Length).Sum();
            byte[] buffer = new byte[length];

            WriteLayers(buffer, length);
            FinalizeLayers(buffer, length);

            return new Packet(buffer, timestamp, _dataLink);
        }

        private void WriteLayers(byte[] buffer, int length)
        {
            int offset = 0;
            for (int i = 0; i != _layers.Length; ++i)
            {
                ILayer layer = _layers[i];
                ILayer previousLayer = i == 0 ? null : _layers[i - 1];
                ILayer nextLayer = i == _layers.Length - 1 ? null : _layers[i + 1];
                layer.Write(buffer, offset, length - offset - layer.Length, previousLayer, nextLayer);
                offset += layer.Length;
            }
        }

        private void FinalizeLayers(byte[] buffer, int length)
        {
            int offset = length;
            for (int i = _layers.Length - 1; i >= 0; --i)
            {
                ILayer layer = _layers[i];
                ILayer nextLayer = i == _layers.Length - 1 ? null : _layers[i + 1];
                offset -= layer.Length;
                layer.Finalize(buffer, offset, length - offset - layer.Length, nextLayer);
            }
        }

        private readonly ILayer[] _layers;
        private readonly DataLink _dataLink;
    }
}