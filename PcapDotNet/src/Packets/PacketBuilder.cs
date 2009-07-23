using System;

namespace Packets
{
    public static class PacketBuilder
    {
        public static Packet Ethernet(DateTime timestamp, MacAddress ethernetSource, MacAddress ethernetDestination, EthernetType ethernetType, Datagram ethernetPayload)
        {
            byte[] buffer = new byte[EthernetDatagram.HeaderLength + ethernetPayload.Length];
            EthernetDatagram.WriteHeader(buffer, 0, ethernetSource, ethernetDestination, ethernetType);
            ethernetPayload.Write(buffer, EthernetDatagram.HeaderLength);
            return new Packet(buffer, timestamp, new DataLink(DataLinkKind.Ethernet));
        }
    }
}