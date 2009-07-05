using System;
using Packets;

namespace PcapDotNet.Core.Test
{
    internal static class MoreRandom
    {
        public static Packet BuildRandomPacket(DateTime timestamp, string sourceMac, string destinationMac, int packetSize)
        {
            if (packetSize < EthernetDatagram.HeaderLength)
                throw new ArgumentOutOfRangeException("packetSize", packetSize, "Must be at least the ethernet header length (" + EthernetDatagram.HeaderLength + ")");

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

        public static Packet BuildRandomPacket(int packetSize)
        {
            return BuildRandomPacket(DateTime.Now, GetRandomMacAddress().ToString(), GetRandomMacAddress().ToString(), packetSize);
        }

        private static Datagram GetRandomDatagram(int length)
        {
            byte[] buffer = new byte[length];
            _random.NextBytes(buffer);
            return new Datagram(buffer);
        }

        private static MacAddress GetRandomMacAddress()
        {
            byte[] buffer = new byte[6];
            _random.NextBytes(buffer);
            return new MacAddress(buffer, 0);
        }

        private static readonly Random _random = new Random();
    }
}