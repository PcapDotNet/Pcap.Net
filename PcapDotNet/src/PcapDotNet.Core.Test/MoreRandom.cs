using System;
using Packets;

namespace PcapDotNet.Core.Test
{
    internal static class MoreRandom
    {
        public static Packet BuildRandomPacket(DateTime timestamp, string sourceMac, string destinationMac, int packetSize)
        {
            if (packetSize < EthernetDatagram.HeaderLength)
                throw new ArgumentException("Packet size (" + packetSize + ") must be at least the ethernet header length", "packetSize");

            return PacketBuilder.Ethernet(timestamp,
                                          new MacAddress(sourceMac),
                                          new MacAddress(destinationMac),
                                          EthernetType.IpV4,
                                          GetRandomDatagram(packetSize - EthernetDatagram.HeaderLength));

        }

        public static Packet BuildRandomPacket(string sourceMac, string destinationMac, int packetSize)
        {
            return BuildRandomPacket(DateTime.Now, sourceMac, destinationMac, packetSize);
        }

        private static Datagram GetRandomDatagram(int length)
        {
            byte[] buffer = new byte[length];
            _random.NextBytes(buffer);
            return new Datagram(buffer);
        }

        private static readonly Random _random = new Random();
    }
}