using System;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.Igmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.TestUtils
{
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
    }
}