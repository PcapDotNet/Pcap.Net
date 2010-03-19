using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcapDotNet.Base;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Gre;
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
            return new ArpLayer(random.NextBytes(hardwareAddressLength).AsReadOnly(), random.NextBytes(protocolAddressLength).AsReadOnly(),
                random.NextBytes(hardwareAddressLength).AsReadOnly(),random.NextBytes(protocolAddressLength).AsReadOnly())
                       {
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

        public static IgmpLayer NextIgmpLayer(this Random random)
        {
            IgmpMessageType igmpMessageType = random.NextEnum(IgmpMessageType.None);
            IgmpQueryVersion igmpQueryVersion = IgmpQueryVersion.None;
            TimeSpan igmpMaxResponseTime = random.NextTimeSpan(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(256 * 0.1) - TimeSpan.FromTicks(1));
            IpV4Address igmpGroupAddress = random.NextIpV4Address();
            bool? igmpIsSuppressRouterSideProcessing = null;
            byte? igmpQueryRobustnessVariable = null;
            TimeSpan? igmpQueryInterval = null;
            IpV4Address[] igmpSourceAddresses = null;
            IgmpGroupRecord[] igmpGroupRecords = null;

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
                            return new IgmpQueryVersion3Layer(igmpSourceAddresses)
                            {
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
                    return new IgmpReportVersion3Layer(igmpGroupRecords);

                default:
                    throw new InvalidOperationException("Invalid message type " + igmpMessageType);
            }
        }

        // GRE
        public static GreLayer NextGreLayer(this Random random)
        {
            return new GreLayer();
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
                    return new IcmpRouterAdvertisementLayer(random.NextIcmpRouterAdvertisementEntries(random.Next(10)).ToList())
                               {
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
                    IpV4Protocol icmpIpV4Protocol = random.NextBool()
                                                        ? random.NextValue(new[] {IpV4Protocol.Udp, IpV4Protocol.Tcp})
                                                        : random.NextEnum<IpV4Protocol>();

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
