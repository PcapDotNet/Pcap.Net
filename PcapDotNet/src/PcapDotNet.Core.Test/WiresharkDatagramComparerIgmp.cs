﻿using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Igmp;

namespace PcapDotNet.Core.Test
{
    internal class WiresharkDatagramComparerIgmp : WiresharkDatagramComparerSimple
    {
        protected override string PropertyName
        {
            get { return "Igmp"; }
        }

        protected override bool CompareField(XElement field, Datagram datagram)
        {
            IgmpDatagram igmpDatagram = (IgmpDatagram)datagram;
            switch (field.Name())
            {
                case "igmp.version":
                    if (field.Show() == "0")
                        return false; // TODO: support IGMP version 0.

                    if (field.Show() == "3" && igmpDatagram.Version == 1)
                    {
                        // TODO: Fix following https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10526
                        Assert.AreEqual(IgmpMessageType.MembershipQuery, igmpDatagram.MessageType);
                        Assert.AreEqual(8, igmpDatagram.Length);
                    }
                    else
                    {
                        field.AssertShowDecimal(igmpDatagram.Version);
                    }
                    break;

                case "igmp.type":
                    field.AssertShowDecimal((byte)igmpDatagram.MessageType);
                    break;

                case "igmp.checksum":
                    field.AssertShowDecimal(igmpDatagram.Checksum);
                    break;

                case "igmp.maddr":
                    field.AssertShow(igmpDatagram.GroupAddress.ToString());
                    break;

                case "igmp.max_resp":
                    field.AssertShowDecimal((int)((igmpDatagram.MaxResponseTime.TotalSeconds + 0.05) * 10));
                    break;

                case "igmp.checksum_bad":
                    if (igmpDatagram.IsValid)
                        field.AssertShowDecimal(!igmpDatagram.IsChecksumCorrect);
                    break;

                case "igmp.num_grp_recs":
                    field.AssertShowDecimal(igmpDatagram.NumberOfGroupRecords);
                    break;

                case "":
                    switch (igmpDatagram.MessageType)
                    {
                        case IgmpMessageType.MembershipReportVersion3:
                            CompareIgmpGroupRecord(field, igmpDatagram.GroupRecords[_groupRecordIndex++]);
                            break;

                        case IgmpMessageType.MembershipQuery:
                            return CompareDatagram(field, null, igmpDatagram);

                        case IgmpMessageType.MulticastTraceRouteResponse:
                        case IgmpMessageType.MulticastTraceRoute:
                            // todo support IGMP traceroute http://www.ietf.org/proceedings/48/I-D/idmr-traceroute-ipm-07.txt.
                            break;

                        default:
                            if (typeof(IgmpMessageType).GetEnumValues<IgmpMessageType>().Contains(igmpDatagram.MessageType))
                                throw new InvalidOperationException("Invalid message type " + igmpDatagram.MessageType);

                            field.AssertValue(igmpDatagram.Skip(1));
//                                field.AssertShow(igmpDatagram.Skip(1));
                            break;
                    }

                    break;

                case "igmp.s":
                    field.AssertShowDecimal(igmpDatagram.IsSuppressRouterSideProcessing);
                    break;

                case "igmp.qrv":
                    field.AssertShowDecimal(igmpDatagram.QueryRobustnessVariable);
                    break;

                case "igmp.qqic":
                    field.AssertShowDecimal(igmpDatagram.QueryIntervalCode);
                    break;

                case "igmp.num_src":
                    field.AssertShowDecimal(igmpDatagram.NumberOfSources);
                    break;

                case "igmp.saddr":
                    field.AssertShow(igmpDatagram.SourceAddresses[_sourceAddressIndex++].ToString());
                    break;

                case "igmp.identifier":
                    // todo support IGMP version 0 and IGMP identifier.
                    break;

                case "igmp.mtrace.max_hops":
                case "igmp.mtrace.saddr":
                case "igmp.mtrace.raddr":
                case "igmp.mtrace.rspaddr":
                case "igmp.mtrace.resp_ttl":
                case "igmp.mtrace.q_id":
                    // todo support IGMP traceroute http://www.ietf.org/proceedings/48/I-D/idmr-traceroute-ipm-07.txt.
                    Assert.IsTrue(new[] { IgmpMessageType.MulticastTraceRouteResponse, IgmpMessageType.MulticastTraceRoute }.Contains(igmpDatagram.MessageType));
                    break;

                default:
                    throw new InvalidOperationException("Invalid igmp field " + field.Name());
            }

            return true;
        }

        private static void CompareIgmpGroupRecord(XElement groupRecord, IgmpGroupRecordDatagram groupRecordDatagram)
        {
            int sourceAddressIndex = 0;
            foreach (var field in groupRecord.Fields())
            {
                switch (field.Name())
                {
                    case "igmp.record_type":
                        field.AssertShowDecimal((byte)groupRecordDatagram.RecordType);
                        break;

                    case "igmp.aux_data_len":
                        field.AssertShowDecimal(groupRecordDatagram.AuxiliaryDataLength / 4);
                        break;

                    case "igmp.num_src":
                        field.AssertShowDecimal(groupRecordDatagram.NumberOfSources);
                        break;

                    case "igmp.maddr":
                        field.AssertShow(groupRecordDatagram.MulticastAddress.ToString());
                        break;

                    case "igmp.saddr":
                        field.AssertShow(groupRecordDatagram.SourceAddresses[sourceAddressIndex++].ToString());
                        break;

                    case "igmp.aux_data":
                        field.AssertShow(groupRecordDatagram.AuxiliaryData);
                        break;

                    default:
                        throw new InvalidOperationException("Invalid igmp group record field " + field.Name());
                }
            }
        }

        private int _groupRecordIndex;
        private int _sourceAddressIndex;
    }
}