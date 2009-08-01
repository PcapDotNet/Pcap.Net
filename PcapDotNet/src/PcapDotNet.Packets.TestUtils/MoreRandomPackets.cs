using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcapDotNet.Packets;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.TestUtils
{
    public static class MoreRandomPackets
    {
        public static Datagram NextDatagram(this Random random, int length)
        {
            byte[] buffer = new byte[length];
            random.NextBytes(buffer);
            return new Datagram(buffer);
        }

        public static DataLinkKind NextDataLinkKind(this Random random)
        {
            return random.NextEnum<DataLinkKind>();
        }

        public static Packet NextPacket(this Random random, int length)
        {
            byte[] buffer = new byte[length];
            random.NextBytes(buffer);
            return new Packet(buffer, DateTime.Now, random.NextDataLinkKind());
        }

        public static MacAddress NextMacAddress(this Random random)
        {
            return new MacAddress(random.NextUInt48());
        }

        public static EthernetType NextEthernetType(this Random random)
        {
            return random.NextEnum(EthernetType.None);
        }

        public static Packet NextEthernet(this Random random, int packetSize, DateTime timestamp, MacAddress ethernetSource, MacAddress ethernetDestination)
        {
            if (packetSize < EthernetDatagram.HeaderLength)
                throw new ArgumentOutOfRangeException("packetSize", packetSize, "Must be at least the ethernet header length (" + EthernetDatagram.HeaderLength + ")");

            return PacketBuilder.Ethernet(timestamp,
                                          ethernetSource, ethernetDestination, random.NextEthernetType(),
                                          random.NextDatagram(packetSize - EthernetDatagram.HeaderLength));
        }

        public static Packet NextEthernet(this Random random, int packetSize, DateTime timestamp, string ethernetSource, string ethernetDestination)
        {
            return random.NextEthernet(packetSize, timestamp, new MacAddress(ethernetSource), new MacAddress(ethernetDestination));
        }

        public static Packet NextEthernet(this Random random, int packetSize, string ethernetSource, string ethernetDestination)
        {
            return random.NextEthernet(packetSize, DateTime.Now, ethernetSource, ethernetDestination);
        }

        public static Packet NextEthernet(this Random random, int packetSize)
        {
            return random.NextEthernet(packetSize, DateTime.Now, random.NextMacAddress(), random.NextMacAddress());
        }

        public static IpV4Address NextIpV4Address(this Random random)
        {
            return new IpV4Address(random.NextUInt());
        }

        public static IpV4Options NextIpV4Options(this Random random)
        {
            int optionsLength = random.Next(IpV4Options.MaximumBytesLength) / 4 * 4;
            List<IpV4Option> options = new List<IpV4Option>();
            while (optionsLength > 0)
            {
                IpV4Option option = null;
                IpV4OptionType optionType = random.NextEnum<IpV4OptionType>();
                switch (optionType)
                {
                    case IpV4OptionType.EndOfOptionList:
                    case IpV4OptionType.NoOperation:
                        option = new IpV4OptionSimple(optionType);
                        break;

                    case IpV4OptionType.Security:
                        if (optionsLength < IpV4OptionSecurity.OptionLength)
                            break;

                        option = new IpV4OptionSecurity(random.NextEnum<IpV4OptionSecurityLevel>(), random.NextUShort(), random.NextUShort(),
                                                        random.NextUInt24());
                        break;
                    case IpV4OptionType.LooseSourceRouting:
                    case IpV4OptionType.StrictSourceRouting:
                    case IpV4OptionType.RecordRoute:
                        if (optionsLength < IpV4OptionRoute.OptionMinimumLength)
                            break;

                        int numAddresses = random.Next((optionsLength - IpV4OptionRoute.OptionMinimumLength) / 4 + 1);
                        IpV4Address[] addresses = new IpV4Address[numAddresses];
                        for (int addressIndex = 0; addressIndex != numAddresses; ++addressIndex)
                            addresses[addressIndex] = random.NextIpV4Address();

                        byte pointedAddressIndex;
                        if (random.NextBool())
                            pointedAddressIndex = random.NextByte(IpV4OptionRoute.PointedAddressIndexMaxValue + 1);
                        else
                            pointedAddressIndex = random.NextByte(10);

                        switch (optionType)
                        {
                            case IpV4OptionType.LooseSourceRouting:
                                option = new IpV4OptionLooseSourceRouting(addresses, pointedAddressIndex);
                                break;

                            case IpV4OptionType.StrictSourceRouting:
                                option = new IpV4OptionStrictSourceRouting(addresses, pointedAddressIndex);
                                break;

                            case IpV4OptionType.RecordRoute:
                                option = new IpV4OptionRecordRoute(pointedAddressIndex, addresses);
                                break;
                        }
                        break;

                    case IpV4OptionType.StreamIdentifier:
                        if (optionsLength < IpV4OptionStreamIdentifier.OptionLength)
                            break;

                        option = new IpV4OptionStreamIdentifier(random.NextUShort());
                        break;

                    case IpV4OptionType.InternetTimestamp:
                        if (optionsLength < IpV4OptionTimestamp.OptionMinimumLength)
                            break;

                        IpV4OptionTimestampType timestampType = random.NextEnum<IpV4OptionTimestampType>();
                        byte overflow = random.NextByte(IpV4OptionTimestamp.OverflowMaxValue + 1);
                        byte pointedIndex;
                        if (random.NextBool())
                            pointedIndex = random.NextByte(IpV4OptionTimestamp.PointedIndexMaxValue + 1);
                        else
                            pointedIndex = random.NextByte(10);

                        switch (timestampType)
                        {
                            case IpV4OptionTimestampType.TimestampOnly:
                                int numTimestamps = random.Next((optionsLength - IpV4OptionTimestamp.OptionMinimumLength) / 4 + 1);
                                uint[] timestamps = new uint[numTimestamps];
                                for (int i = 0; i != numTimestamps; ++i)
                                    timestamps[i] = random.NextUInt();
                                option = new IpV4OptionTimestampOnly(overflow, pointedIndex, timestamps);
                                break;

                            case IpV4OptionTimestampType.AddressAndTimestamp:
                                int numPairs = random.Next((optionsLength - IpV4OptionTimestamp.OptionMinimumLength) / 8 + 1);
                                KeyValuePair<IpV4Address, uint>[] pairs = new KeyValuePair<IpV4Address, uint>[numPairs];
                                for (int i = 0; i != numPairs; ++i)
                                    pairs[i] = new KeyValuePair<IpV4Address, uint>(random.NextIpV4Address(), random.NextUInt());

                                option = new IpV4OptionTimestampAndAddress(timestampType, overflow, pointedIndex, pairs);
                                break;
                        }
                        break;
                }

                if (option == null)
                    continue;

                if (option.IsAppearsAtMostOnce &&
                    options.FindIndex(option.Equivalent) != -1)
                {
                    continue;
                }

                options.Add(option);
                optionsLength -= option.Length;

                if (option.OptionType == IpV4OptionType.EndOfOptionList)
                    break;
            }
            return new IpV4Options(options);
        }

    }
}
