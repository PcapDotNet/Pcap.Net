using System;

namespace BPacket
{
    public static class PacketBuilder
    {
        public static Packet Ethernet(DateTime timestamp, MacAddress ethernetSource, MacAddress ethernetDestination, EthernetType ethernetType, Datagram etherentPayload)
        {
            byte[] buffer = new byte[EthernetDatagram.HeaderLength + etherentPayload.Length];
            EthernetDatagram.WriteHeader(buffer, 0, ethernetSource, ethernetDestination, ethernetType);
            etherentPayload.Write(buffer, EthernetDatagram.HeaderLength);
            return new Packet(buffer, timestamp, new DataLink(DataLinkKind.Ethernet));
        }
    }
}