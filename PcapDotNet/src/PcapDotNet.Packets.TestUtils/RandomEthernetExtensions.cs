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

        public static Packet NextEthernetPacket(this Random random, int packetSize, DateTime timestamp, MacAddress ethernetSource, MacAddress ethernetDestination)
        {
            if (packetSize < EthernetDatagram.HeaderLength)
                throw new ArgumentOutOfRangeException("packetSize", packetSize, "Must be at least the ethernet header length (" + EthernetDatagram.HeaderLength + ")");

            return PacketBuilder.Build(timestamp,
                                       new EthernetLayer
                                       {
                                           Source = ethernetSource,
                                           Destination = ethernetDestination,
                                           EtherType = random.NextEthernetType()
                                       },
                                       random.NextPayloadLayer(packetSize - EthernetDatagram.HeaderLength));
        }

        public static Packet NextEthernetPacket(this Random random, int packetSize, DateTime timestamp, string ethernetSource, string ethernetDestination)
        {
            return random.NextEthernetPacket(packetSize, timestamp, new MacAddress(ethernetSource), new MacAddress(ethernetDestination));
        }

        public static Packet NextEthernetPacket(this Random random, int packetSize, MacAddress ethernetSource, MacAddress ethernetDestination)
        {
            return random.NextEthernetPacket(packetSize, DateTime.Now, ethernetSource, ethernetDestination);
        }

        public static Packet NextEthernetPacket(this Random random, int packetSize, string ethernetSource, string ethernetDestination)
        {
            return random.NextEthernetPacket(packetSize, DateTime.Now, ethernetSource, ethernetDestination);
        }

        public static Packet NextEthernetPacket(this Random random, int packetSize)
        {
            return random.NextEthernetPacket(packetSize, DateTime.Now, random.NextMacAddress(), random.NextMacAddress());
        }
    }
}