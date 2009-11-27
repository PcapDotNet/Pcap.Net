using System;
using System.Collections.Generic;
using System.Linq;
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
            Source = IpV4Address.Zero;
            Destination = IpV4Address.Zero;
            Options = IpV4Options.None;
        }

        public byte TypeOfService { get; set; }

        public ushort Identification { get; set; }

        public IpV4Fragmentation Fragmentation { get; set; }

        public byte Ttl { get; set; }

        public IpV4Protocol? Protocol { get; set; }

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
                                     Ttl, protocol,
                                     Source, Destination,
                                     Options, payloadLength);
        }

        public override void Finalize(byte[] buffer, int offset, int payloadLength, ILayer nextLayer)
        {
            IIpV4NextTransportLayer nextTransportLayer = nextLayer as IIpV4NextTransportLayer;
            if (nextTransportLayer == null || !nextTransportLayer.NextLayerCalculateChecksum)
                return;

            IpV4Datagram.WriteTransportChecksum(buffer, offset, Length, (ushort)payloadLength,
                                                nextTransportLayer.NextLayerChecksumOffset, nextTransportLayer.NextLayerIsChecksumOptional);
            
        }
    }

    public abstract class TransportLayer : Layer, IIpV4NextTransportLayer
    {
        public ushort SourcePort { get; set; }

        public ushort DestinationPort { get; set; }

        public abstract IpV4Protocol PreviousLayerProtocol { get; }
        public abstract int NextLayerChecksumOffset { get; }
        public abstract bool NextLayerIsChecksumOptional { get; }
        public abstract bool NextLayerCalculateChecksum { get; }
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

        public byte[] SenderHardwareAddress { get; set; }

        public byte[] SenderProtocolAddress { get; set; }

        public byte[] TargetHardwareAddress { get; set; }

        public byte[] TargetProtocolAddress { get; set; }

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
            get { return ArpDatagram.GetHeaderLength(SenderHardwareAddress.Length, SenderProtocolAddress.Length); }
        }

        public override void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer)
        {
            if (previousLayer == null)
                throw new ArgumentException("Must have a previous layer");

            IArpPreviousLayer arpPreviousLayer = previousLayer as IArpPreviousLayer;
            if (arpPreviousLayer == null)
                throw new ArgumentException("The layer before the ARP layer must be an ARP previous layer and can't be " + previousLayer.GetType());

            if (SenderHardwareAddress.Length != TargetHardwareAddress.Length)
            {
                throw new ArgumentException("Sender hardware address length is " + SenderHardwareAddress.Length + " bytes " +
                                            "while target hardware address length is " + TargetHardwareAddress.Length + " bytes");
            }
            if (SenderProtocolAddress.Length != TargetProtocolAddress.Length)
            {
                throw new ArgumentException("Sender protocol address length is " + SenderProtocolAddress.Length + " bytes " +
                                            "while target protocol address length is " + TargetProtocolAddress.Length + " bytes");
            }

            ArpDatagram.WriteHeader(buffer, offset,
                                    arpPreviousLayer.PreviousLayerHardwareType, ProtocolType, Operation,
                                    SenderHardwareAddress, SenderProtocolAddress, TargetHardwareAddress, TargetProtocolAddress);
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
    }

    public interface IIgmpLayerWithGroupAddress
    {
        IpV4Address GroupAddress { get; set; }
    }

    public abstract class SimpleIgmpLayer : IgmpLayer, IIgmpLayerWithGroupAddress
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
    }

    public abstract class IgmpVersion1Layer : SimpleIgmpLayer
    {
        public override TimeSpan MaxResponseTimeValue
        {
            get { return TimeSpan.Zero; }
        }
    }

    public abstract class IgmpVersion2Layer : SimpleIgmpLayer
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

        public IpV4Address[] SourceAddresses{get; set ;}

        public override int Length
        {
            get { return IgmpDatagram.GetQueryVersion3Length(SourceAddresses.Length); }
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
        public IgmpGroupRecord[] GroupRecords{get; set ;}

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
    }

    public abstract class IcmpLayer : SimpleLayer, IIpV4NextLayer
    {
        public abstract IcmpMessageType MessageType { get; }
        public virtual byte CodeValue
        {
            get { return 0; }
        }
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
            IcmpDatagram.WriteChecksum(buffer, offset, Length + payloadLength);
        }

        public IpV4Protocol PreviousLayerProtocol
        {
            get { return IpV4Protocol.InternetControlMessageProtocol; }
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
        public ushort Pointer { get; set; }

        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.ParameterProblem; }
        }

        protected override uint Value
        {
            get
            {
                return (uint)(Pointer << 16);
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