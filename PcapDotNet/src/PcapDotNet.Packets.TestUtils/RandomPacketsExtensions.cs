using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcapDotNet.Base;
using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Gre;
using PcapDotNet.Packets.Http;
using PcapDotNet.Packets.Icmp;
using PcapDotNet.Packets.Igmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.Packets.Transport;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.TestUtils
{
    public static class RandomPacketsExtensions
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

        public static PayloadLayer NextPayloadLayer(this Random random, int length)
        {
            return new PayloadLayer
                       {
                           Data = random.NextDatagram(length)
                       };
        }

        // Ethernet

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

        // ARP

        public static ArpLayer NextArpLayer(this Random random)
        {
            byte hardwareAddressLength = random.NextByte();
            byte protocolAddressLength = random.NextByte();
            return new ArpLayer
                   {
                       SenderHardwareAddress = random.NextBytes(hardwareAddressLength).AsReadOnly(),
                       SenderProtocolAddress = random.NextBytes(protocolAddressLength).AsReadOnly(),
                       TargetHardwareAddress = random.NextBytes(hardwareAddressLength).AsReadOnly(),
                       TargetProtocolAddress = random.NextBytes(protocolAddressLength).AsReadOnly(),
                       ProtocolType = random.NextEnum<EthernetType>(),
                       Operation = random.NextEnum<ArpOperation>(),
                   };
        }

        // IPv4

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
                    Destination = random.NextIpV4Address(),
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

        // IPv6

        public static IpV6Address NextIpV6Address(this Random random)
        {
            return new IpV6Address(random.NextUInt128());
        }

        // UDP

        public static UdpLayer NextUdpLayer(this Random random)
        {
            return new UdpLayer
                       {
                           Checksum = random.NextUShort(),
                           SourcePort = random.NextUShort(),
                           DestinationPort = random.NextUShort(),
                           CalculateChecksumValue = random.NextBool()
                       };
        }

        // TCP

        public static TcpLayer NextTcpLayer(this Random random)
        {
            return new TcpLayer
                       {
                           SourcePort = random.NextUShort(),
                           DestinationPort = random.NextUShort(),
                           SequenceNumber = random.NextUInt(),
                           AcknowledgmentNumber = random.NextUInt(),
                           ControlBits = random.NextFlags<TcpControlBits>(),
                           Window = random.NextUShort(),
                           UrgentPointer = random.NextUShort(),
                           Options = random.NextTcpOptions(),
                       };
        }

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
            if (maximumOptionLength < TcpOptionMood.OptionMaximumLength)
                impossibleOptionTypes.Add(TcpOptionType.Mood);

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

                case TcpOptionType.Mood:
                    return new TcpOptionMood(random.NextEnum(TcpOptionMoodEmotion.None));

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

        public static IgmpLayer NextIgmpLayer(this Random random)
        {
            IgmpMessageType igmpMessageType = random.NextEnum(IgmpMessageType.None, IgmpMessageType.CreateGroupRequestVersion0,
                                                              IgmpMessageType.CreateGroupReplyVersion0, IgmpMessageType.JoinGroupRequestVersion0,
                                                              IgmpMessageType.JoinGroupReplyVersion0, IgmpMessageType.LeaveGroupRequestVersion0,
                                                              IgmpMessageType.LeaveGroupReplyVersion0, IgmpMessageType.ConfirmGroupRequestVersion0,
                                                              IgmpMessageType.ConfirmGroupReplyVersion0,
                                                              IgmpMessageType.MulticastTraceRouteResponse); // todo support IGMP traceroute http://www.ietf.org/proceedings/48/I-D/idmr-traceroute-ipm-07.txt.
            IgmpQueryVersion igmpQueryVersion = IgmpQueryVersion.None;
            TimeSpan igmpMaxResponseTime = random.NextTimeSpan(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(256 * 0.1) - TimeSpan.FromTicks(1));
            IpV4Address igmpGroupAddress = random.NextIpV4Address();
            bool? igmpIsSuppressRouterSideProcessing;
            byte? igmpQueryRobustnessVariable;
            TimeSpan? igmpQueryInterval;
            IpV4Address[] igmpSourceAddresses;
            IgmpGroupRecord[] igmpGroupRecords;

            switch (igmpMessageType)
            {
                case IgmpMessageType.MembershipQuery:
                    igmpQueryVersion = random.NextEnum(IgmpQueryVersion.None, IgmpQueryVersion.Unknown);
                    switch (igmpQueryVersion)
                    {
                        case IgmpQueryVersion.Version1:
                            return new IgmpQueryVersion1Layer
                            {
                                GroupAddress = igmpGroupAddress
                            };

                        case IgmpQueryVersion.Version2:
                            return new IgmpQueryVersion2Layer
                            {
                                MaxResponseTime = igmpMaxResponseTime,
                                GroupAddress = igmpGroupAddress
                            };

                        case IgmpQueryVersion.Version3:
                            igmpIsSuppressRouterSideProcessing = random.NextBool();
                            igmpQueryRobustnessVariable = random.NextByte(8);
                            igmpMaxResponseTime = random.NextTimeSpan(TimeSpan.FromSeconds(0.1),
                                                                      IgmpDatagram.MaxVersion3MaxResponseTime - TimeSpan.FromTicks(1));
                            igmpQueryInterval = random.NextTimeSpan(TimeSpan.Zero, IgmpDatagram.MaxQueryInterval - TimeSpan.FromTicks(1));
                            igmpSourceAddresses = random.NextIpV4Addresses(random.Next(1000));
                            return new IgmpQueryVersion3Layer
                            {
                                SourceAddresses = igmpSourceAddresses.AsReadOnly(),
                                MaxResponseTime = igmpMaxResponseTime,
                                GroupAddress = igmpGroupAddress,
                                IsSuppressRouterSideProcessing = igmpIsSuppressRouterSideProcessing.Value,
                                QueryRobustnessVariable = igmpQueryRobustnessVariable.Value,
                                QueryInterval = igmpQueryInterval.Value,
                            };

                        default:
                            throw new InvalidOperationException("Invalid Query Version " + igmpQueryVersion);
                    }

                case IgmpMessageType.MembershipReportVersion1:
                    return new IgmpReportVersion1Layer
                    {
                        GroupAddress = igmpGroupAddress
                    };

                case IgmpMessageType.MembershipReportVersion2:
                    return new IgmpReportVersion2Layer
                    {
                        MaxResponseTime = igmpMaxResponseTime,
                        GroupAddress = igmpGroupAddress
                    };

                case IgmpMessageType.LeaveGroupVersion2:
                    return new IgmpLeaveGroupVersion2Layer
                    {
                        MaxResponseTime = igmpMaxResponseTime,
                        GroupAddress = igmpGroupAddress
                    };

                case IgmpMessageType.MembershipReportVersion3:
                    igmpGroupRecords = random.NextIgmpGroupRecords(random.Next(100));
                    if (igmpGroupRecords.Count() == 0 && random.NextBool())
                        return new IgmpReportVersion3Layer();
                    return new IgmpReportVersion3Layer
                           {
                               GroupRecords = igmpGroupRecords.AsReadOnly()
                           };

                default:
                    throw new InvalidOperationException("Invalid message type " + igmpMessageType);
            }
        }

        // GRE
        public static GreLayer NextGreLayer(this Random random)
        {
            GreVersion version = random.NextEnum<GreVersion>();

            bool isChecksum = random.NextBool();
            GreSourceRouteEntry[] routing = null;
            ushort? routingOffset = null;
            bool strictSourceRoute = false;
            EthernetType protocolType = random.NextEnum(EthernetType.None);
            uint? key = random.NextBool() ? (uint?)random.NextUInt() : null;
            if (version == GreVersion.Gre)
            {
                if (random.NextBool())
                {
                    strictSourceRoute = random.NextBool();
                    routing = new GreSourceRouteEntry[random.Next(5)];

                    GreSourceRouteEntryAddressFamily family;
                    if (random.NextBool())
                        family = random.NextEnum(GreSourceRouteEntryAddressFamily.None);
                    else
                        family = (GreSourceRouteEntryAddressFamily)random.NextUShort();

                    for (int i = 0; i != routing.Length; ++i)
                    {
                        switch (family)
                        {
                            case GreSourceRouteEntryAddressFamily.AsSourceRoute:
                            {
                                ushort[] asNumbers = new ushort[random.Next(1, 5)];
                                for (int j = 0; j != asNumbers.Length; ++j)
                                    asNumbers[j] = random.NextUShort();
                                routing[i] = new GreSourceRouteEntryAs(asNumbers.AsReadOnly(), random.Next(asNumbers.Length + 1));
                                break;
                            }

                            case GreSourceRouteEntryAddressFamily.IpSourceRoute:
                            {
                                IpV4Address[] ips = new IpV4Address[random.Next(1, 5)];
                                for (int j = 0; j != ips.Length; ++j)
                                    ips[j] = random.NextIpV4Address();
                                routing[i] = new GreSourceRouteEntryIp(ips.AsReadOnly(), random.Next(ips.Length + 1));
                                break;
                            }

                            default:
                            {
                                int dataLength = random.Next(1, 100);
                                routing[i] = new GreSourceRouteEntryUnknown(family, random.NextDatagram(dataLength), random.Next(dataLength + 1));
                                break;
                            }
                        }

                    }
                    routingOffset = 0;
                    if (routing.Any())
                    {
                        int routingIndex = random.Next(routing.Length);
                        for (int i = 0; i != routingIndex; ++i)
                            routingOffset += (ushort)routing[i].Length;
                    }
                }
            }
            else
            {
                protocolType = EthernetType.PointToPointProtocol;
                isChecksum = false;
                key = random.NextUInt();
            }

            return new GreLayer
                   {
                       Version = version,
                       ProtocolType = protocolType,
                       ChecksumPresent = isChecksum,
                       Checksum = isChecksum && random.NextBool() ? (ushort?)random.NextUShort() : null,
                       Key = key,
                       SequenceNumber = random.NextBool() ? (uint?)random.NextUInt() : null,
                       AcknowledgmentSequenceNumber = version == GreVersion.EnhancedGre && random.NextBool() ? (uint?)random.NextUInt() : null,
                       RecursionControl = random.NextByte(8),
//                       Flags = random.NextByte(32),
                       Routing = routing == null ? null : routing.AsReadOnly(),
                       RoutingOffset = routingOffset,
                       StrictSourceRoute = strictSourceRoute,
                   };
        }

        // ICMP

        public static IcmpLayer NextIcmpLayer(this Random random)
        {
            IcmpMessageType icmpMessageType = random.NextEnum(IcmpMessageType.DomainNameReply);
            ushort? checksum = random.NextBool() ? (ushort?)random.NextUShort() : null;

            switch (icmpMessageType)
            {
                case IcmpMessageType.DestinationUnreachable:
                    return new IcmpDestinationUnreachableLayer
                           {
                               Code = random.NextEnum<IcmpCodeDestinationUnreachable>(),
                               Checksum = checksum,
                               NextHopMaximumTransmissionUnit = random.NextUShort(),
                           };

                case IcmpMessageType.TimeExceeded:
                    return new IcmpTimeExceededLayer
                           {
                               Code = random.NextEnum<IcmpCodeTimeExceeded>(),
                               Checksum = checksum,
                           };

                case IcmpMessageType.ParameterProblem:
                    return new IcmpParameterProblemLayer
                           {
                               Checksum = checksum,
                               Pointer = random.NextByte()
                           };

                case IcmpMessageType.SourceQuench:
                    return new IcmpSourceQuenchLayer
                           {
                               Checksum = checksum
                           };

                case IcmpMessageType.Redirect:
                    return new IcmpRedirectLayer
                           {
                               Code = random.NextEnum<IcmpCodeRedirect>(),
                               Checksum = checksum,
                               GatewayInternetAddress = random.NextIpV4Address()
                           };

                case IcmpMessageType.Echo:
                    return new IcmpEchoLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort()
                           };

                case IcmpMessageType.EchoReply:
                    return new IcmpEchoReplyLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort()
                           };


                case IcmpMessageType.Timestamp:
                    return new IcmpTimestampLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort(),
                               OriginateTimestamp = random.NextIpV4TimeOfDay(),
                               ReceiveTimestamp = random.NextIpV4TimeOfDay(),
                               TransmitTimestamp = random.NextIpV4TimeOfDay()
                           };

                case IcmpMessageType.TimestampReply:
                    return new IcmpTimestampReplyLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort(),
                               OriginateTimestamp = random.NextIpV4TimeOfDay(),
                               ReceiveTimestamp = random.NextIpV4TimeOfDay(),
                               TransmitTimestamp = random.NextIpV4TimeOfDay()
                           };

                case IcmpMessageType.InformationRequest:
                    return new IcmpInformationRequestLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort(),
                           };

                case IcmpMessageType.InformationReply:
                    return new IcmpInformationReplyLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort(),
                           };


                case IcmpMessageType.RouterAdvertisement:
                    return new IcmpRouterAdvertisementLayer
                           {
                               Entries = random.NextIcmpRouterAdvertisementEntries(random.Next(10)).ToList().AsReadOnly(),
                               Checksum = checksum,
                               Lifetime = random.NextTimeSpan(TimeSpan.Zero, TimeSpan.FromSeconds(ushort.MaxValue)),
                           };

                case IcmpMessageType.RouterSolicitation:
                    return new IcmpRouterSolicitationLayer
                           {
                               Checksum = checksum,
                           };

                case IcmpMessageType.AddressMaskRequest:
                    return new IcmpAddressMaskRequestLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort(),
                               AddressMask = random.NextIpV4Address()
                           };

                case IcmpMessageType.AddressMaskReply:
                    return new IcmpAddressMaskReplyLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort(),
                               AddressMask = random.NextIpV4Address()
                           };

                case IcmpMessageType.TraceRoute:
                    return new IcmpTraceRouteLayer
                           {
                               Code = random.NextEnum<IcmpCodeTraceRoute>(),
                               Checksum = checksum,
                               Identification = random.NextUShort(),
                               OutboundHopCount = random.NextUShort(),
                               ReturnHopCount = random.NextUShort(),
                               OutputLinkSpeed = random.NextUInt(),
                               OutputLinkMaximumTransmissionUnit = random.NextUInt(),
                           };

                case IcmpMessageType.ConversionFailed:
                    return new IcmpConversionFailedLayer
                           {
                               Code = random.NextEnum<IcmpCodeConversionFailed>(),
                               Checksum = checksum,
                               Pointer = random.NextUInt(),
                           };

                case IcmpMessageType.DomainNameRequest:
                    return new IcmpDomainNameRequestLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort(),
                           };

                case IcmpMessageType.DomainNameReply:
                    throw new NotSupportedException("Message Type " + icmpMessageType + " is not supported");

                case IcmpMessageType.SecurityFailures:
                    return new IcmpSecurityFailuresLayer
                           {
                               Code = random.NextEnum<IcmpCodeSecurityFailure>(),
                               Checksum = checksum,
                               Pointer = random.NextUShort()
                           };

                default:
                    throw new InvalidOperationException("Invalid icmpMessageType " + icmpMessageType);
            }
        }

        public static IEnumerable<ILayer> NextIcmpPayloadLayers(this Random random, IcmpLayer icmpLayer)
        {
            IEnumerable<ILayer> icmpPayloadLayers = new List<ILayer>();
            switch (icmpLayer.MessageType)
            {
                case IcmpMessageType.DestinationUnreachable:
                case IcmpMessageType.TimeExceeded:
                case IcmpMessageType.ParameterProblem:
                case IcmpMessageType.SourceQuench:
                case IcmpMessageType.Redirect:
                case IcmpMessageType.SecurityFailures:
                    icmpPayloadLayers = icmpPayloadLayers.Concat(random.NextIpV4Layer(), random.NextPayloadLayer(IcmpIpV4HeaderPlus64BitsPayloadDatagram.OriginalDatagramPayloadLength));
                    break;
                case IcmpMessageType.ConversionFailed:
                    IpV4Layer icmpIpV4Layer = random.NextIpV4Layer();
                    icmpPayloadLayers = icmpPayloadLayers.Concat(icmpIpV4Layer);
                    if (icmpLayer.MessageTypeAndCode == IcmpMessageTypeAndCode.ConversionFailedUnsupportedTransportProtocol)
                    {
                        icmpPayloadLayers =
                            icmpPayloadLayers.Concat(random.NextPayloadLayer(
                                                         IcmpConversionFailedDatagram.OriginalDatagramLengthForUnsupportedTransportProtocol -
                                                         icmpIpV4Layer.Length));
                    }
                    else
                    {
                        switch (icmpIpV4Layer.Protocol)
                        {
                            case IpV4Protocol.Udp:
                                icmpPayloadLayers = icmpPayloadLayers.Concat(random.NextUdpLayer(),
                                                                             random.NextPayloadLayer(random.Next(100)));
                                break;

                            case IpV4Protocol.Tcp:
                                icmpPayloadLayers = icmpPayloadLayers.Concat(random.NextTcpLayer(),
                                                                             random.NextPayloadLayer(random.Next(100)));
                                break;

                            default:
                                icmpPayloadLayers = icmpPayloadLayers.Concat(random.NextPayloadLayer(random.Next(200)));

                                break;
                        }
                    }
                    break;

                case IcmpMessageType.Echo:
                case IcmpMessageType.EchoReply:
                case IcmpMessageType.Timestamp:
                case IcmpMessageType.TimestampReply:
                case IcmpMessageType.InformationRequest:
                case IcmpMessageType.InformationReply:
                case IcmpMessageType.RouterAdvertisement:
                case IcmpMessageType.RouterSolicitation:
                case IcmpMessageType.AddressMaskRequest:
                case IcmpMessageType.AddressMaskReply:
                case IcmpMessageType.TraceRoute:
                case IcmpMessageType.DomainNameRequest:
                    break;

                case IcmpMessageType.DomainNameReply:
                default:
                    throw new InvalidOperationException("Invalid icmpMessageType " + icmpLayer.MessageType);
            }
            return icmpPayloadLayers;
        }


        public static IEnumerable<IcmpRouterAdvertisementEntry> NextIcmpRouterAdvertisementEntries(this Random random, int numEntries)
        {
            for (int i = 0; i != numEntries; ++i)
                yield return new IcmpRouterAdvertisementEntry(random.NextIpV4Address(), random.Next());
        }

        // HTTP
        public static HttpLayer NextHttpLayer(this Random random)
        {
            if (random.NextBool())
            {
                HttpRequestLayer httpRequestLayer = new HttpRequestLayer();
                if (random.NextBool(10))
                {
                    httpRequestLayer.Method = random.NextHttpRequestMethod();
                    httpRequestLayer.Uri = httpRequestLayer.Method == null ? null : random.NextHttpUri();
                    httpRequestLayer.Version = httpRequestLayer.Uri == null || random.NextBool(10)  ? null : random.NextHttpVersion();
                    httpRequestLayer.Header = httpRequestLayer.Version == null ? null : random.NextHttpHeader();
                    httpRequestLayer.Body = httpRequestLayer.Header == null ? null : random.NextHttpBody(true, null, httpRequestLayer.Header);
                }

                return httpRequestLayer;
            }

            HttpResponseLayer httpResponseLayer = new HttpResponseLayer
                            {
                                Version = random.NextHttpVersion(),
                                StatusCode = random.NextBool(10) ? null : (uint?)random.NextUInt(100, 999),
                            };
            httpResponseLayer.ReasonPhrase = httpResponseLayer.StatusCode == null ? null : random.NextHttpReasonPhrase();
            httpResponseLayer.Header = httpResponseLayer.ReasonPhrase == null ? null : random.NextHttpHeader();
            httpResponseLayer.Body = httpResponseLayer.Header == null ? null : random.NextHttpBody(false , httpResponseLayer.StatusCode, httpResponseLayer.Header);

            return httpResponseLayer;
        }

        public static Datagram NextHttpReasonPhrase(this Random random)
        {
            int reasonPhraseLength = random.Next(100);
            StringBuilder reasonPhrase = new StringBuilder(reasonPhraseLength);
            for (int i = 0; i != reasonPhraseLength; ++i)
            {
                if (random.NextBool())
                    reasonPhrase.Append(random.NextHttpTextChar());
                else if (random.NextBool())
                    reasonPhrase.Append(' ');
                else
                    reasonPhrase.Append('\t');
            }
            return new Datagram(EncodingExtensions.Iso88591.GetBytes(reasonPhrase.ToString()));
        }

        public static HttpRequestMethod NextHttpRequestMethod(this Random random)
        {
            HttpRequestKnownMethod knownMethod = random.NextEnum<HttpRequestKnownMethod>();
            if (knownMethod == HttpRequestKnownMethod.Unknown)
                return new HttpRequestMethod(random.NextHttpToken());
            return new HttpRequestMethod(knownMethod);
        }

        public static string NextHttpToken(this Random random)
        {
            int tokenLength = random.Next(1, 100);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i != tokenLength; ++i)
                stringBuilder.Append(random.NextHttpTokenChar());
            return stringBuilder.ToString();
        }

        public static char NextHttpTokenChar(this Random random)
        {
            char result;
            do
            {
                result = random.NextChar((char)33, (char)127);
            } while (new[] {'(', ')', '<', '>', '@', ',', ';', ':', '\\', '"', '/', '[', ']', '?', '=', '{', '}'}.Contains(result));

            return result;
        }

        public static string NextHttpFieldValue(this Random random)
        {
            int valueLength = random.Next(1, 100);
            StringBuilder stringBuilder = new StringBuilder();
            while (stringBuilder.Length < valueLength)
            {
                switch (random.Next(3))
                {
                    case 0:
                        stringBuilder.Append(random.NextHttpTextChar());
                        break;

                    case 1:
                        stringBuilder.Append(random.NextHttpQuotedString());
                        break;

                    case 2:
                        if (stringBuilder.Length > 0 && stringBuilder.Length < valueLength - 3)
                            stringBuilder.Append(random.NextHttpLinearWhiteSpace());
                        break;
                }
            }
            return stringBuilder.ToString();
        }

        public static string NextHttpLinearWhiteSpace(this Random random)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (random.NextBool())
                stringBuilder.Append("\r\n");
            stringBuilder.Append(random.NextBool() ? ' ' : '\t');
            return stringBuilder.ToString();
        }

        public static char NextHttpTextChar(this Random random)
        {
            char text = random.NextChar((char)33, (char)254);
            if (text == '"')
                return (char)254;
            if (text == 127)
                return (char)255;
            return text;
        }

        public static string NextHttpUri(this Random random)
        {
            int uriLength = random.Next(100);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i != uriLength; ++i)
                stringBuilder.Append(random.NextChar((char)33, (char)127));
            return stringBuilder.ToString();
        }

        public static HttpVersion NextHttpVersion(this Random random)
        {
            return new HttpVersion(random.NextUInt(10000000), random.NextUInt(10000000));
        }

        public static HttpHeader NextHttpHeader(this Random random)
        {
            int numFields = random.Next(100);
            List<HttpField> fields = new List<HttpField>(numFields);
            HashSet<string> fieldNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            for (int i = 0; i != numFields; ++i)
            {
                fields.Add(random.NextHttpField(fieldNames));
                fieldNames.Add(fields.Last().Name);
            }

            return new HttpHeader(fields);
        }

        public static HttpField NextHttpField(this Random random, HashSet<string> fieldNames)
        {
            const string unknownField = "Unknown Name";
            List<string> allOptions = new List<string> { unknownField, HttpTransferEncodingField.FieldNameUpper, HttpContentLengthField.FieldNameUpper, HttpContentTypeField.FieldNameUpper };
            List<string> possibleOptions = new List<string>(allOptions.Count);
            foreach (string option in allOptions)
            {
                if (!fieldNames.Contains(option))
                    possibleOptions.Add(option);
            }

            string chosenOption = random.NextValue(possibleOptions);
            switch (chosenOption)
            {
                case unknownField:
                    string fieldName;
                    do
                    {
                        fieldName = random.NextHttpToken();
                    } while (fieldNames.Contains(fieldName));
                    return HttpField.CreateField(fieldName, random.NextHttpFieldValue());

                case HttpTransferEncodingField.FieldNameUpper:
                    int numTransferCodings = random.Next(1, 10);
                    List<string> transferCodings = new List<string>(numTransferCodings);
                    for (int i = 0; i != numTransferCodings; ++i)
                        transferCodings.Add(random.NextHttpTransferCoding());
                    return new HttpTransferEncodingField(transferCodings);

                case HttpContentLengthField.FieldNameUpper:
                    return new HttpContentLengthField(random.NextUInt(1000));

                case HttpContentTypeField.FieldNameUpper:

                    return new HttpContentTypeField(random.NextHttpToken(), random.NextHttpToken(), random.NextHttpFieldParameters());

                default:
                    throw new InvalidOperationException("Invalid option " + chosenOption);
            }
        }

        public static HttpFieldParameters NextHttpFieldParameters(this Random random)
        {
            int numParameters = random.Next(10);
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>(numParameters);
            for (int i = 0; i != numParameters; ++i)
            {
                string parameterName = random.NextHttpToken();
                while (parameters.Any(pair => pair.Key == parameterName))
                    parameterName = random.NextHttpToken();
                parameters.Add(new KeyValuePair<string, string>(parameterName,
                                                                random.NextBool() ? random.NextHttpToken() : random.NextHttpQuotedString()));
            }
            return new HttpFieldParameters(parameters);
        }

        public static string NextHttpTransferCoding(this Random random)
        {
            if (random.NextBool())
                return "chunked";

            StringBuilder transferCoding = new StringBuilder(random.NextHttpToken());
            int numParameters = random.Next(5);
            for (int i = 0; i != numParameters; ++i)
            {
                transferCoding.Append(';');
                transferCoding.Append(random.NextHttpToken());
                transferCoding.Append('=');
                if (random.NextBool())
                    transferCoding.Append(random.NextHttpToken());
                else
                    transferCoding.Append(random.NextHttpQuotedString());
            }

            return transferCoding.ToString();
        }

        public static string NextHttpQuotedString(this Random random)
        {
            StringBuilder quotedString = new StringBuilder();
            quotedString.Append('"');
            int numQuotedValues = random.Next(100);
            for (int i = 0; i != numQuotedValues; ++i)
            {
                char quotedValue = random.NextHttpTextChar();
                if (quotedValue != '\\')
                    quotedString.Append(quotedValue);
                else
                {
                    quotedString.Append('\\');
                    quotedString.Append(random.NextChar((char)0, (char)128));
                }
            }
            quotedString.Append('"');

            return quotedString.ToString();
        }

        public static Datagram NextHttpBody(this Random random, bool isRequest, uint? statusCode, HttpHeader httpHeader)
        {
            if (isRequest && httpHeader.ContentLength == null ||
                !isRequest && statusCode >= 100 && statusCode <= 199 || statusCode == 204 || statusCode == 205 || statusCode == 304)
                return Datagram.Empty;

            if (httpHeader.TransferEncoding != null && 
                httpHeader.TransferEncoding.TransferCodings.Any(coding => coding != "identity"))
            {
                // chunked
                List<byte> chunkedBody = new List<byte>();
                int numChunks = random.Next(10);
                for (int i = 0; i != numChunks; ++i)
                {
                    int chunkSize = random.Next(1, 1000);
                    chunkedBody.AddRange(Encoding.ASCII.GetBytes(chunkSize.ToString("x")));
                    var chunkExtension = random.NextHttpFieldParameters();
                    foreach (var parameter in chunkExtension)
                    {
                        chunkedBody.Add((byte)';');
                        chunkedBody.AddRange(Encoding.ASCII.GetBytes(parameter.Key));
                        chunkedBody.Add((byte)'=');
                        chunkedBody.AddRange(EncodingExtensions.Iso88591.GetBytes(parameter.Key));
                    }
                    chunkedBody.AddRange(Encoding.ASCII.GetBytes("\r\n"));
                    chunkedBody.AddRange(random.NextDatagram(chunkSize));
                    chunkedBody.AddRange(Encoding.ASCII.GetBytes("\r\n"));
                }
                int numZeros = random.Next(1, 10);
                for (int i = 0; i != numZeros; ++i)
                    chunkedBody.Add((byte)'0');
                var lastChunkExtension = random.NextHttpFieldParameters();
                foreach (var parameter in lastChunkExtension)
                {
                    chunkedBody.Add((byte)';');
                    chunkedBody.AddRange(Encoding.ASCII.GetBytes(parameter.Key));
                    chunkedBody.Add((byte)'=');
                    chunkedBody.AddRange(EncodingExtensions.Iso88591.GetBytes(parameter.Key));
                }
                chunkedBody.AddRange(Encoding.ASCII.GetBytes("\r\n"));
                var trailer = random.NextHttpHeader();
                byte[] trailerBuffer = new byte[trailer.BytesLength];
                trailer.Write(trailerBuffer, 0);
                chunkedBody.AddRange(trailerBuffer);

                return new Datagram(chunkedBody.ToArray());
            }

            if (httpHeader.ContentLength != null)
            {
                return random.NextDatagram(random.Next((int)(httpHeader.ContentLength.ContentLength + 1)));
            }

            if (httpHeader.ContentType != null &&
                httpHeader.ContentType.MediaType == "multipart" &&
                httpHeader.ContentType.MediaSubtype == "byteranges" &&
                httpHeader.ContentType.Parameters["boundary"] != null)

            {
                List<byte> boundedBody = new List<byte>(random.NextDatagram(random.Next(1000)));
                boundedBody.AddRange(EncodingExtensions.Iso88591.GetBytes("--" + httpHeader.ContentType.Parameters["boundary"] + "--"));
                return new Datagram(boundedBody.ToArray());
            }

            return random.NextDatagram(random.Next(1000));
        }
    }
}
