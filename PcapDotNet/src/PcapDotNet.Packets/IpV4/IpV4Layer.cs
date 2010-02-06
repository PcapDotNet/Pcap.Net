using System;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Packets.IpV4
{
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
}