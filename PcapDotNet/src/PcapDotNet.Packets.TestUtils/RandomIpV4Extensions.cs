using System;
using System.Collections.Generic;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.TestUtils
{
    public static class RandomIpV4Extensions
    {
        public static IpV4Layer NextIpV4Layer(this Random random, IpV4Protocol? protocol)
        {
            return new IpV4Layer
                   {
                       TypeOfService = random.NextByte(),
                       Identification = random.NextUShort(),
                       Ttl = random.NextByte(),
                       Protocol = protocol,
                       HeaderChecksum = random.NextBool() ? (ushort?)random.NextUShort() : null,
                       Fragmentation = random.NextBool() ? random.NextIpV4Fragmentation() : IpV4Fragmentation.None,
                       Source = random.NextIpV4Address(),
                       CurrentDestination = random.NextIpV4Address(),
                       Options = random.NextIpV4Options()
                   };
        }

        public static IpV4Layer NextIpV4Layer(this Random random)
        {
            return random.NextIpV4Layer(random.NextEnum<IpV4Protocol>());
        }

        public static IpV4Address NextIpV4Address(this Random random)
        {
            return new IpV4Address(random.NextUInt());
        }

        public static IpV4Address[] NextIpV4Addresses(this Random random, int count)
        {
            return ((Func<IpV4Address>)random.NextIpV4Address).GenerateArray(count);
        }

        public static IpV4Fragmentation NextIpV4Fragmentation(this Random random)
        {
            IpV4FragmentationOptions ipV4FragmentationFlags = random.NextEnum<IpV4FragmentationOptions>();
            ushort ipV4FragmentationOffset = (ushort)(random.NextUShort() / 8 * 8);
            return new IpV4Fragmentation(ipV4FragmentationFlags, ipV4FragmentationOffset);
        }

        public static IpV4TimeOfDay NextIpV4TimeOfDay(this Random random)
        {
            return new IpV4TimeOfDay(random.NextUInt());
        }

        public static IpV4OptionUnknown NextIpV4OptionUnknown(this Random random, int maximumOptionLength)
        {
            IpV4OptionType unknownOptionType;
            byte unknownOptionTypeValue;
            do
            {
                unknownOptionTypeValue = random.NextByte();
                unknownOptionType = (IpV4OptionType)unknownOptionTypeValue;
            } while (unknownOptionType.ToString() != unknownOptionTypeValue.ToString());
            
            Byte[] unknownOptionData = new byte[random.Next(maximumOptionLength - IpV4OptionUnknown.OptionMinimumLength + 1)];
            random.NextBytes(unknownOptionData);

            return new IpV4OptionUnknown(unknownOptionType, unknownOptionData);
        }

        public static IpV4Option NextIpV4Option(this Random random, int maximumOptionLength)
        {
            if (maximumOptionLength == 0)
                throw new ArgumentOutOfRangeException("maximumOptionLength", maximumOptionLength, "option length must be positive");

            if (maximumOptionLength >= IpV4OptionUnknown.OptionMinimumLength && random.Next(100) > 90)
                return random.NextIpV4OptionUnknown(maximumOptionLength);

            // TODO: Support MtuProbe.
            List<IpV4OptionType> impossibleOptionTypes = new List<IpV4OptionType> {IpV4OptionType.MtuProbe};
            if (maximumOptionLength < IpV4OptionBasicSecurity.OptionMinimumLength)
                impossibleOptionTypes.Add(IpV4OptionType.BasicSecurity);
            if (maximumOptionLength < IpV4OptionRoute.OptionMinimumLength)
            {
                impossibleOptionTypes.Add(IpV4OptionType.LooseSourceRouting);
                impossibleOptionTypes.Add(IpV4OptionType.StrictSourceRouting);
                impossibleOptionTypes.Add(IpV4OptionType.RecordRoute);
            }
            if (maximumOptionLength < IpV4OptionStreamIdentifier.OptionLength)
                impossibleOptionTypes.Add(IpV4OptionType.StreamIdentifier);
            if (maximumOptionLength < IpV4OptionTimestamp.OptionMinimumLength)
                impossibleOptionTypes.Add(IpV4OptionType.InternetTimestamp);
            if (maximumOptionLength < IpV4OptionTraceRoute.OptionLength)
                impossibleOptionTypes.Add(IpV4OptionType.TraceRoute);
            if (maximumOptionLength < IpV4OptionQuickStart.OptionLength)
                impossibleOptionTypes.Add(IpV4OptionType.QuickStart);

            IpV4OptionType optionType = random.NextEnum<IpV4OptionType>(impossibleOptionTypes);
            switch (optionType)
            {
                case IpV4OptionType.EndOfOptionList:
                    return IpV4Option.End;
                case IpV4OptionType.NoOperation:
                    return IpV4Option.Nop;

                case IpV4OptionType.BasicSecurity:
                    IpV4OptionSecurityProtectionAuthorities protectionAuthorities = IpV4OptionSecurityProtectionAuthorities.None;
                    int protectionAuthorityLength = random.Next(maximumOptionLength - IpV4OptionBasicSecurity.OptionMinimumLength);
                    if (protectionAuthorityLength > 0)
                        protectionAuthorities = random.NextEnum<IpV4OptionSecurityProtectionAuthorities>();

                    return new IpV4OptionBasicSecurity(random.NextEnum(IpV4OptionSecurityClassificationLevel.None),
                                                       protectionAuthorities,
                                                       (byte)(IpV4OptionBasicSecurity.OptionMinimumLength + protectionAuthorityLength));

                case IpV4OptionType.LooseSourceRouting:
                case IpV4OptionType.StrictSourceRouting:
                case IpV4OptionType.RecordRoute:
                    int numAddresses = random.Next((maximumOptionLength - IpV4OptionRoute.OptionMinimumLength) / 4 + 1);
                    IpV4Address[] addresses = random.NextIpV4Addresses(numAddresses);

                    byte pointedAddressIndex;
                    if (random.NextBool())
                        pointedAddressIndex = random.NextByte(IpV4OptionRoute.PointedAddressIndexMaxValue + 1);
                    else
                        pointedAddressIndex = random.NextByte(10);

                    switch (optionType)
                    {
                        case IpV4OptionType.LooseSourceRouting:
                            return new IpV4OptionLooseSourceRouting(addresses, pointedAddressIndex);

                        case IpV4OptionType.StrictSourceRouting:
                            return new IpV4OptionStrictSourceRouting(addresses, pointedAddressIndex);

                        case IpV4OptionType.RecordRoute:
                            return new IpV4OptionRecordRoute(pointedAddressIndex, addresses);

                        default:
                            throw new InvalidOperationException("optionType = " + optionType);
                    }

                case IpV4OptionType.StreamIdentifier:
                    return new IpV4OptionStreamIdentifier(random.NextUShort());

                case IpV4OptionType.InternetTimestamp:
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
                            int numTimestamps = random.Next((maximumOptionLength - IpV4OptionTimestamp.OptionMinimumLength) / 4 + 1);
                            IpV4TimeOfDay[] timestamps = ((Func<IpV4TimeOfDay>)random.NextIpV4TimeOfDay).GenerateArray(numTimestamps);
                            return new IpV4OptionTimestampOnly(overflow, pointedIndex, timestamps);

                        case IpV4OptionTimestampType.AddressAndTimestamp:
                        case IpV4OptionTimestampType.AddressPrespecified:
                            int numPairs = random.Next((maximumOptionLength - IpV4OptionTimestamp.OptionMinimumLength) / 8 + 1);
                            IpV4OptionTimedAddress[] pairs = new IpV4OptionTimedAddress[numPairs];
                            for (int i = 0; i != numPairs; ++i)
                                pairs[i] = new IpV4OptionTimedAddress(random.NextIpV4Address(), random.NextIpV4TimeOfDay());

                            return new IpV4OptionTimestampAndAddress(timestampType, overflow, pointedIndex, pairs);

                        default:
                            throw new InvalidOperationException("timestampType = " + timestampType);
                    }

                case IpV4OptionType.TraceRoute:
                    return new IpV4OptionTraceRoute(random.NextUShort(), random.NextUShort(), random.NextUShort(), random.NextIpV4Address());

                case IpV4OptionType.RouterAlert:
                    return new IpV4OptionRouterAlert(random.NextUShort());

                case IpV4OptionType.QuickStart:
                    return new IpV4OptionQuickStart(random.NextEnum<IpV4OptionQuickStartFunction>(),
                                                    random.NextByte(IpV4OptionQuickStart.RateMaximumValue + 1), 
                                                    random.NextByte(),
                                                    random.NextUInt() & 0x3FFFFFFF);

                default:
                    throw new InvalidOperationException("optionType = " + optionType);
            }
        }

        public static IpV4Options NextIpV4Options(this Random random)
        {
            int optionsLength = random.Next(IpV4Options.MaximumBytesLength) / 4 * 4;
            List<IpV4Option> options = new List<IpV4Option>();
            while (optionsLength > 0)
            {
                IpV4Option option = random.NextIpV4Option(optionsLength);

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