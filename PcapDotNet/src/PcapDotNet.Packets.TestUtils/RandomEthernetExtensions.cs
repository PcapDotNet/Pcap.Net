using System;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.TestUtils
{
    public static class RandomEthernetExtensions
    {
        public static EthernetLayer NextEthernetLayer(this Random random, EthernetType etherType)
        {
            return new EthernetLayer
                   {
                       Source = random.NextMacAddress(),
                       Destination = random.NextMacAddress(),
                       EtherType = etherType
                   };
        }

        public static EthernetLayer NextEthernetLayer(this Random random)
        {
            return random.NextEthernetLayer(random.NextEnum(EthernetType.None));
        }

        public static MacAddress NextMacAddress(this Random random)
        {
            return new MacAddress(random.NextUInt48());
        }

        public static EthernetType NextEthernetType(this Random random)
        {
            return random.NextEnum(EthernetType.None);
        }

        public static Packet NextEthernetPacket(this Random random, int packetSize, DateTime timestamp, uint originalLength, MacAddress ethernetSource, MacAddress ethernetDestination)
        {
            if (packetSize < EthernetDatagram.HeaderLengthValue)
                throw new ArgumentOutOfRangeException("packetSize", packetSize,
                                                      "Must be at least the ethernet header length (" + EthernetDatagram.HeaderLengthValue + ")");

            return PacketBuilder.Build(timestamp, originalLength,
                                       new EthernetLayer
                                       {
                                           Source = ethernetSource,
                                           Destination = ethernetDestination,
                                           EtherType = random.NextEthernetType()
                                       },
                                       random.NextPayloadLayer(packetSize - EthernetDatagram.HeaderLengthValue));
        }

        public static Packet NextEthernetPacket(this Random random, int packetSize, DateTime timestamp, uint originalLength, string ethernetSource, string ethernetDestination)
        {
            return random.NextEthernetPacket(packetSize, timestamp, originalLength, new MacAddress(ethernetSource), new MacAddress(ethernetDestination));
        }

        public static Packet NextEthernetPacket(this Random random, int packetSize, DateTime timestamp, string ethernetSource, string ethernetDestination)
        {
            return random.NextEthernetPacket(packetSize, timestamp, 0, ethernetSource, ethernetDestination);
        }

        public static Packet NextEthernetPacket(this Random random, int packetSize, uint originalLength, MacAddress ethernetSource, MacAddress ethernetDestination)
        {
            return random.NextEthernetPacket(packetSize, DateTime.Now, originalLength, ethernetSource, ethernetDestination);
        }

        public static Packet NextEthernetPacket(this Random random, int packetSize, MacAddress ethernetSource, MacAddress ethernetDestination)
        {
            return random.NextEthernetPacket(packetSize, 0, ethernetSource, ethernetDestination);
        }

        public static Packet NextEthernetPacket(this Random random, int packetSize, uint originalLength, string ethernetSource, string ethernetDestination)
        {
            return random.NextEthernetPacket(packetSize, DateTime.Now, originalLength, ethernetSource, ethernetDestination);
        }

        public static Packet NextEthernetPacket(this Random random, int packetSize, string ethernetSource, string ethernetDestination)
        {
            return random.NextEthernetPacket(packetSize, 0, ethernetSource, ethernetDestination);
        }

        public static Packet NextEthernetPacket(this Random random, int packetSize, uint originalLength)
        {
            return random.NextEthernetPacket(packetSize, DateTime.Now, originalLength, random.NextMacAddress(), random.NextMacAddress());
        }

        public static Packet NextEthernetPacket(this Random random, int packetSize)
        {
            return random.NextEthernetPacket(packetSize, 0);
        }
    }
}