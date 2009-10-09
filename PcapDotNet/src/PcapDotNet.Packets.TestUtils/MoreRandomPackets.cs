using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcapDotNet.Base;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Igmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
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

        // Ethernet

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

            return PacketBuilder.Ethernet(timestamp,
                                          ethernetSource, ethernetDestination, random.NextEthernetType(),
                                          random.NextDatagram(packetSize - EthernetDatagram.HeaderLength));
        }

        public static Packet NextEthernetPacket(this Random random, int packetSize, DateTime timestamp, string ethernetSource, string ethernetDestination)
        {
            return random.NextEthernetPacket(packetSize, timestamp, new MacAddress(ethernetSource), new MacAddress(ethernetDestination));
        }

        public static Packet NextEthernetPacket(this Random random, int packetSize, string ethernetSource, string ethernetDestination)
        {
            return random.NextEthernetPacket(packetSize, DateTime.Now, ethernetSource, ethernetDestination);
        }

        public static Packet NextEthernetPacket(this Random random, int packetSize)
        {
            return random.NextEthernetPacket(packetSize, DateTime.Now, random.NextMacAddress(), random.NextMacAddress());
        }

        // IPv4

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

        public static IpV4TimeOfDay NextIpV4OptionTimeOfDay(this Random random)
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

            List<IpV4OptionType> impossibleOptionTypes = new List<IpV4OptionType>();
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
                            IpV4TimeOfDay[] timestamps = ((Func<IpV4TimeOfDay>)random.NextIpV4OptionTimeOfDay).GenerateArray(numTimestamps);
                            return new IpV4OptionTimestampOnly(overflow, pointedIndex, timestamps);

                        case IpV4OptionTimestampType.AddressAndTimestamp:
                        case IpV4OptionTimestampType.AddressPrespecified:
                            int numPairs = random.Next((maximumOptionLength - IpV4OptionTimestamp.OptionMinimumLength) / 8 + 1);
                            IpV4OptionTimedAddress[] pairs = new IpV4OptionTimedAddress[numPairs];
                            for (int i = 0; i != numPairs; ++i)
                                pairs[i] = new IpV4OptionTimedAddress(random.NextIpV4Address(), random.NextIpV4OptionTimeOfDay());

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
                                                    random.NextUInt() & 0xFFFFFFFC);

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

        public static TcpOptionUnknown NextTcpOptionUnknown(this Random random, int maximumOptionLength)
        {
            TcpOptionType unknownOptionType;
            byte unknownOptionTypeValue;
            do
            {
                unknownOptionTypeValue = random.NextByte();
                unknownOptionType = (TcpOptionType)unknownOptionTypeValue;
            } while (unknownOptionType.ToString() != unknownOptionTypeValue.ToString());

            byte[] unknownOptionData = random.NextBytes(maximumOptionLength - TcpOptionUnknown.OptionMinimumLength + 1);

            return new TcpOptionUnknown(unknownOptionType, unknownOptionData);
        }

        public static TcpOption NextTcpOption(this Random random, int maximumOptionLength)
        {
            if (maximumOptionLength == 0)
                throw new ArgumentOutOfRangeException("maximumOptionLength", maximumOptionLength, "option length must be positive");

            if (maximumOptionLength >= TcpOptionUnknown.OptionMinimumLength && random.Next(100) > 90)
                return random.NextTcpOptionUnknown(maximumOptionLength);

            List<TcpOptionType> impossibleOptionTypes = new List<TcpOptionType>();
            if (maximumOptionLength < TcpOptionMaximumSegmentSize.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.MaximumSegmentSize);
            if (maximumOptionLength < TcpOptionWindowScale.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.WindowScale);
            if (maximumOptionLength < TcpOptionSelectiveAcknowledgment.OptionMinimumLength)
                impossibleOptionTypes.Add(TcpOptionType.SelectiveAcknowledgment);
            if (maximumOptionLength < TcpOptionSelectiveAcknowledgmentPermitted.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.SelectiveAcknowledgmentPermitted);
            if (maximumOptionLength < TcpOptionEcho.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.Echo);
            if (maximumOptionLength < TcpOptionEchoReply.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.EchoReply);
            if (maximumOptionLength < TcpOptionTimestamp.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.Timestamp);
            if (maximumOptionLength < TcpOptionPartialOrderServiceProfile.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.PartialOrderServiceProfile);
            if (maximumOptionLength < TcpOptionPartialOrderConnectionPermitted.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.PartialOrderConnectionPermitted);
            if (maximumOptionLength < TcpOptionConnectionCountBase.OptionLength)
            {
                impossibleOptionTypes.Add(TcpOptionType.ConnectionCount);
                impossibleOptionTypes.Add(TcpOptionType.ConnectionCountNew);
                impossibleOptionTypes.Add(TcpOptionType.ConnectionCountEcho);
            }
            if (maximumOptionLength < TcpOptionAlternateChecksumRequest.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.AlternateChecksumRequest);
            if (maximumOptionLength < TcpOptionAlternateChecksumData.OptionMinimumLength)
                impossibleOptionTypes.Add(TcpOptionType.AlternateChecksumData);
            if (maximumOptionLength < TcpOptionMd5Signature.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.Md5Signature);

            impossibleOptionTypes.Add(TcpOptionType.QuickStartResponse);
            impossibleOptionTypes.Add(TcpOptionType.UserTimeout);

            TcpOptionType optionType = random.NextEnum<TcpOptionType>(impossibleOptionTypes);
            switch (optionType)
            {
                case TcpOptionType.EndOfOptionList:
                    return TcpOption.End;

                case TcpOptionType.NoOperation:
                    return TcpOption.Nop;

                case TcpOptionType.MaximumSegmentSize:
                    return new TcpOptionMaximumSegmentSize(random.NextUShort());

                case TcpOptionType.WindowScale:
                    return new TcpOptionWindowScale(random.NextByte());

                case TcpOptionType.SelectiveAcknowledgment:
                    int numBlocks = random.Next((maximumOptionLength - TcpOptionSelectiveAcknowledgment.OptionMinimumLength) / 8 + 1);
                    TcpOptionSelectiveAcknowledgmentBlock[] blocks = new TcpOptionSelectiveAcknowledgmentBlock[numBlocks];
                    for (int i = 0; i != numBlocks; ++i)
                        blocks[i] = new TcpOptionSelectiveAcknowledgmentBlock(random.NextUInt(), random.NextUInt());
                    return new TcpOptionSelectiveAcknowledgment(blocks);

                case TcpOptionType.SelectiveAcknowledgmentPermitted:
                    return new TcpOptionSelectiveAcknowledgmentPermitted();

                case TcpOptionType.Echo:
                    return new TcpOptionEcho(random.NextUInt());

                case TcpOptionType.EchoReply:
                    return new TcpOptionEchoReply(random.NextUInt());

                case TcpOptionType.Timestamp:
                    return new TcpOptionTimestamp(random.NextUInt(), random.NextUInt());

                case TcpOptionType.PartialOrderServiceProfile:
                    return new TcpOptionPartialOrderServiceProfile(random.NextBool(), random.NextBool());

                case TcpOptionType.PartialOrderConnectionPermitted:
                    return new TcpOptionPartialOrderConnectionPermitted();

                case TcpOptionType.ConnectionCount:
                    return new TcpOptionConnectionCount(random.NextUInt());

                case TcpOptionType.ConnectionCountEcho:
                    return new TcpOptionConnectionCountEcho(random.NextUInt());

                case TcpOptionType.ConnectionCountNew:
                    return new TcpOptionConnectionCountNew(random.NextUInt());

                case TcpOptionType.AlternateChecksumRequest:
                    return new TcpOptionAlternateChecksumRequest(random.NextEnum<TcpOptionAlternateChecksumType>());

                case TcpOptionType.AlternateChecksumData:
                    return new TcpOptionAlternateChecksumData(random.NextBytes(random.Next(maximumOptionLength - TcpOptionAlternateChecksumData.OptionMinimumLength + 1)));

                case TcpOptionType.Md5Signature:
                    return new TcpOptionMd5Signature(random.NextBytes(TcpOptionMd5Signature.OptionValueLength));

                default:
                    throw new InvalidOperationException("optionType = " + optionType);
            }
        }

        // IGMP

        public static IgmpGroupRecord NextIgmpGroupRecord(this Random random)
        {
            IpV4Address[] sourceAddresses = random.NextIpV4Addresses(random.Next(10));
            return new IgmpGroupRecord(random.NextEnum<IgmpRecordType>(), random.NextIpV4Address(), sourceAddresses, random.NextDatagram(random.Next(10) * 4));
        }

        public static IgmpGroupRecord[] NextIgmpGroupRecords(this Random random, int count)
        {
            return ((Func<IgmpGroupRecord>)random.NextIgmpGroupRecord).GenerateArray(count);
        }

        // TCP

        public static TcpOptions NextTcpOptions(this Random random)
        {
            int optionsLength = random.Next(TcpOptions.MaximumBytesLength) / 4 * 4;
            List<TcpOption> options = new List<TcpOption>();
            while (optionsLength > 0)
            {
                TcpOption option = random.NextTcpOption(optionsLength);

                if (option.IsAppearsAtMostOnce &&
                    options.FindIndex(option.Equivalent) != -1)
                {
                    continue;
                }

                options.Add(option);
                optionsLength -= option.Length;

                if (option.OptionType == TcpOptionType.EndOfOptionList)
                    break;
            }
            return new TcpOptions(options);
        }
    }
}
