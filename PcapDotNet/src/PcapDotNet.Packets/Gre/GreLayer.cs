using System;
using System.Collections.ObjectModel;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Packets.Gre
{
    public class GreLayer : Layer
    {
        public byte RecursionControl { get; set; }
        public GreVersion Version { get; set; }
        public EthernetType ProtocolType { get; set; }
        public ushort? Checksum { get; set; }
        public ushort? RoutingOffset { get; set; }
        public uint? Key { get; set; }
        public uint? SequenceNumber { get; set; }
        public ReadOnlyCollection<GreSourceRouteEntry> Routing { get; set; }

        public override int Length
        {
            get { throw new NotImplementedException(); }
        }

        public override void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer)
        {
            GreDatagram.WriteHeader(buffer, offset);
        }

        public override bool Equals(Layer other)
        {
            throw new NotImplementedException();
        }
    }
}