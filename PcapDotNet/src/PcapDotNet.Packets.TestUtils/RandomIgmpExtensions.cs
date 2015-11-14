using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.Igmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.TestUtils
{
    [ExcludeFromCodeCoverage]
    public static class RandomIgmpExtensions
    {
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
            IgmpMessageType igmpMessageType = random.NextEnum(IgmpMessageType.None,
                                                              IgmpMessageType.MulticastTraceRouteResponse, IgmpMessageType.MulticastTraceRoute); // todo support IGMP traceroute http://www.ietf.org/proceedings/48/I-D/idmr-traceroute-ipm-07.txt.
            TimeSpan igmpMaxResponseTime = random.NextTimeSpan(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(256 * 0.1) - TimeSpan.FromTicks(1));
            IpV4Address igmpGroupAddress = random.NextIpV4Address();
            ulong accessKey = random.NextULong();
            uint identifier = random.NextUInt();

            switch (igmpMessageType)
            {
                case IgmpMessageType.CreateGroupRequestVersion0:
                    return new IgmpCreateGroupRequestVersion0Layer
                    {
                        IsPrivate = random.NextBool(),
                        Identifier = identifier,
                    };

                case IgmpMessageType.CreateGroupReplyVersion0:
                case IgmpMessageType.JoinGroupReplyVersion0:
                case IgmpMessageType.LeaveGroupReplyVersion0:
                case IgmpMessageType.ConfirmGroupReplyVersion0:
                    IgmpVersion0ReplyCode code = random.NextEnum<IgmpVersion0ReplyCode>();
                    return new IgmpReplyVersion0Layer
                    {
                        MessageType = igmpMessageType,
                        Code = code,
                        RetryInThisManySeconds = (byte)(code == IgmpVersion0ReplyCode.RequestPendingRetryInThisManySeconds? random.NextByte((byte)IgmpVersion0ReplyCode.RequestPendingRetryInThisManySeconds, byte.MaxValue) : 0),
                        Identifier = identifier,
                        GroupAddress = igmpGroupAddress,
                        AccessKey = accessKey,
                    };

                case IgmpMessageType.JoinGroupRequestVersion0:
                case IgmpMessageType.LeaveGroupRequestVersion0:
                    return new IgmpRequestVersion0Layer
                    {
                        MessageType = igmpMessageType,
                        Identifier = identifier,
                        GroupAddress = igmpGroupAddress,
                        AccessKey = accessKey,
                    };

                case IgmpMessageType.ConfirmGroupRequestVersion0:
                    return new IgmpConfirmGroupRequestVersion0Layer
                    {
                        GroupAddress = igmpGroupAddress,
                        AccessKey = accessKey,
                    };

                case IgmpMessageType.MembershipQuery:
                    IgmpQueryVersion igmpQueryVersion = random.NextEnum(IgmpQueryVersion.None, IgmpQueryVersion.Unknown);
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
                            bool igmpIsSuppressRouterSideProcessing = random.NextBool();
                            byte igmpQueryRobustnessVariable = random.NextByte(8);
                            igmpMaxResponseTime = random.NextTimeSpan(TimeSpan.FromSeconds(0.1),
                                                                      IgmpDatagram.MaxVersion3MaxResponseTime - TimeSpan.FromTicks(1));
                            TimeSpan igmpQueryInterval = random.NextTimeSpan(TimeSpan.Zero, IgmpDatagram.MaxQueryInterval - TimeSpan.FromTicks(1));
                            IpV4Address[] igmpSourceAddresses = random.NextIpV4Addresses(random.Next(1000));
                            return new IgmpQueryVersion3Layer
                                   {
                                       SourceAddresses = igmpSourceAddresses.AsReadOnly(),
                                       MaxResponseTime = igmpMaxResponseTime,
                                       GroupAddress = igmpGroupAddress,
                                       IsSuppressRouterSideProcessing = igmpIsSuppressRouterSideProcessing,
                                       QueryRobustnessVariable = igmpQueryRobustnessVariable,
                                       QueryInterval = igmpQueryInterval,
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
                    IgmpGroupRecord[] igmpGroupRecords = random.NextIgmpGroupRecords(random.Next(100));
                    if (!igmpGroupRecords.Any() && random.NextBool())
                        return new IgmpReportVersion3Layer();
                    return new IgmpReportVersion3Layer
                           {
                               GroupRecords = igmpGroupRecords.AsReadOnly()
                           };

                default:
                    throw new InvalidOperationException("Invalid message type " + igmpMessageType);
            }
        }
    }
}