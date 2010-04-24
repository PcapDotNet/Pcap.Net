using System;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Gre
{
    public class GreLayer : Layer, IIpV4NextLayer, IEquatable<GreLayer>
    {
        public GreVersion Version { get; set; }
        public EthernetType ProtocolType { get; set; }
        public byte RecursionControl { get; set; }
        public byte Flags { get; set; }
        public bool ChecksumPresent { get; set; }
        public ushort? Checksum { get; set; }
        public uint? Key { get; set; }
        public uint? SequenceNumber { get; set; }
        public ushort? RoutingOffset { get; set; }
        public ReadOnlyCollection<GreSourceRouteEntry> Routing { get; set; }
        public bool StrictSourceRoute { get; set; }

        public override int Length
        {
            get
            {
                return GreDatagram.GetHeaderLength(ChecksumPresent, Key != null, SequenceNumber != null, Routing);
            }
        }

        public override void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer)
        {
            GreDatagram.WriteHeader(buffer, offset, RecursionControl, Flags, Version, ProtocolType, ChecksumPresent, Key, SequenceNumber, Routing, RoutingOffset, StrictSourceRoute);
        }

        public override void Finalize(byte[] buffer, int offset, int payloadLength, ILayer nextLayer)
        {
            if (ChecksumPresent)
                GreDatagram.WriteChecksum(buffer, offset, Length + payloadLength, Checksum);
        }

        public bool Equals(GreLayer other)
        {
            return other != null &&
                   Version.Equals(other.Version) &&
                   ProtocolType.Equals(other.ProtocolType) &&
                   RecursionControl.Equals(other.RecursionControl) &&
                   Flags.Equals(other.Flags) &&
                   ChecksumPresent.Equals(other.ChecksumPresent) &&
                   (Checksum == null ? other.Checksum == null : Checksum.Equals(other.Checksum)) &&
                   (Key == null ? other.Key == null : Key.Equals(other.Key)) &&
                   (SequenceNumber == null ? other.SequenceNumber == null : SequenceNumber.Equals(other.SequenceNumber)) &&
                   (RoutingOffset == null ? other.RoutingOffset == null : RoutingOffset.Equals(other.RoutingOffset)) &&
                   (Routing == null ? other.Routing == null : Routing.SequenceEqual(other.Routing)) &&
                   StrictSourceRoute.Equals(other.StrictSourceRoute);
        }

        public override bool Equals(Layer other)
        {
            return Equals(other as GreLayer);
        }

        public IpV4Protocol PreviousLayerProtocol
        {
            get { return IpV4Protocol.Gre; }
        }
    }
}