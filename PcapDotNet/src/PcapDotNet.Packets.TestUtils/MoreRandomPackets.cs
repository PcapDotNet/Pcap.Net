using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Packets;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.TestUtils
{
    public static class MoreRandomPackets
    {
        public static IpV4Address NextIpV4Address(this Random random)
        {
            return new IpV4Address(random.NextUInt());
        }

        public static IpV4Options NextIpV4Options(this Random random)
        {
            int optionsLength = random.Next(IpV4Options.MaximumLength) / 4 * 4;
            List<IpV4Option> options = new List<IpV4Option>();
            while (optionsLength > 0)
            {
                IpV4Option option = null;
                IpV4OptionType optionType = random.NextEnum<IpV4OptionType>();
                switch (optionType)
                {
                    case IpV4OptionType.EndOfOptionList:
                        option = new IpV4OptionEndOfOptionsList();
                        break;

                    case IpV4OptionType.NoOperation:
                        option = new IpV4OptionNoOperation();
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
                                option = new IpV4OptionRecordRoute(addresses, pointedAddressIndex);
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
                        byte overflow = random.NextByte(16);
                        byte pointedIndex;
                        if (random.NextBool())
                            pointedIndex = random.NextByte(IpV4OptionTimestamp.PointedIndexMaxValue + 1);
                        else
                            pointedIndex = random.NextByte(10);

                        switch (timestampType)
                        {
                            case IpV4OptionTimestampType.TimestampOnly:
                                int numTimestamps = random.Next((optionsLength - IpV4OptionTimestamp.OptionMinimumLength) / 4 + 1);
                                TimeSpan[] timestamps = new TimeSpan[numTimestamps];
                                for (int i = 0; i != numTimestamps; ++i)
                                    timestamps[i] = TimeSpan.FromMilliseconds((uint)random.NextDateTime().TimeOfDay.TotalMilliseconds);
                                option = new IpV4OptionTimestampOnly(overflow, pointedIndex, timestamps);
                                break;

                            case IpV4OptionTimestampType.AddressAndTimestamp:
                                int numPairs = random.Next((optionsLength - IpV4OptionTimestamp.OptionMinimumLength) / 8 + 1);
                                KeyValuePair<IpV4Address, TimeSpan>[] pairs = new KeyValuePair<IpV4Address, TimeSpan>[numPairs];
                                for (int i = 0; i != numPairs; ++i)
                                    pairs[i] = new KeyValuePair<IpV4Address, TimeSpan>(random.NextIpV4Address(), TimeSpan.FromMilliseconds((uint)random.NextDateTime().TimeOfDay.TotalMilliseconds));

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

                if (option is IpV4OptionEndOfOptionsList)
                    break;
            }
            return new IpV4Options(options);
        }

    }
}
