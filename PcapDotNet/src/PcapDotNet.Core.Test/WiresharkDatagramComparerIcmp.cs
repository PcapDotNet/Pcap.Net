﻿using System;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Icmp;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Core.Test
{
    internal class WiresharkDatagramComparerIcmp : WiresharkDatagramComparerSimple
    {
        protected override string PropertyName
        {
            get { return "Icmp"; }
        }

        protected override bool CompareField(XElement field, Datagram datagram)
        {
            IcmpDatagram icmpDatagram = (IcmpDatagram)datagram;
            switch (field.Name())
            {
                case "icmp.type":
                    field.AssertShowDecimal((byte)icmpDatagram.MessageType);
                    field.AssertNoFields();
                    break;

                case "icmp.code":
                    field.AssertShowDecimal(icmpDatagram.Code);
                    field.AssertNoFields();
                    break;

                case "icmp.checksum_bad":
                    field.AssertShowDecimal(!icmpDatagram.IsChecksumCorrect);
                    field.AssertNoFields();
                    break;

                case "icmp.checksum":
                    field.AssertShowHex(icmpDatagram.Checksum);
                    field.AssertNoFields();
                    break;

                case "data":
                    var casted1 = icmpDatagram as IcmpIpV4HeaderPlus64BitsPayloadDatagram;
                    if (casted1 != null)
                    {
                        if (casted1.IpV4.Protocol != IpV4Protocol.IpComp) // TODO: Support IpComp.
                            field.AssertDataField(casted1.IpV4.Payload);
                    }
                    else
                    {
                        field.AssertDataField(icmpDatagram.Payload);
                    }
                    break;

                case "":
                    switch (icmpDatagram.MessageType)
                    {
                        case IcmpMessageType.ParameterProblem:
                            if (field.Show() != "Unknown session type")
                                field.AssertShow("Pointer: " + ((IcmpParameterProblemDatagram)icmpDatagram).Pointer);
                            break;

                        case IcmpMessageType.RouterAdvertisement:
                            IcmpRouterAdvertisementDatagram routerAdvertisementDatagram = (IcmpRouterAdvertisementDatagram)icmpDatagram;
                            string fieldName = field.Show().Split(':')[0];
                            switch (fieldName)
                            {
                                case "Number of addresses":
                                    field.AssertShow(fieldName + ": " + routerAdvertisementDatagram.NumberOfAddresses);
                                    break;

                                case "Address entry size":
                                    field.AssertShow(fieldName + ": " + routerAdvertisementDatagram.AddressEntrySize);
                                    break;

                                case "Lifetime":
                                    TimeSpan actualLifetime = routerAdvertisementDatagram.Lifetime;
                                    StringBuilder actualLifetimeString = new StringBuilder(fieldName + ": ");
                                    if (actualLifetime.Hours != 0)
                                    {
                                        actualLifetimeString.Append(actualLifetime.Hours + " hour");
                                        if (actualLifetime.Hours != 1)
                                            actualLifetimeString.Append('s');
                                    }
                                    if (actualLifetime.Minutes != 0)
                                    {
                                        if (actualLifetime.Hours != 0)
                                            actualLifetimeString.Append(", ");
                                        actualLifetimeString.Append(actualLifetime.Minutes + " minute");
                                        if (actualLifetime.Minutes != 1)
                                            actualLifetimeString.Append('s');
                                    }
                                    if (actualLifetime.Seconds != 0)
                                    {
                                        if (actualLifetime.Hours != 0 || actualLifetime.Minutes != 0)
                                            actualLifetimeString.Append(", ");
                                        actualLifetimeString.Append(actualLifetime.Seconds + " second");
                                        if (actualLifetime.Seconds != 1)
                                            actualLifetimeString.Append('s');
                                    }
                                    break;

                                case "Router address":
                                    field.AssertShow(fieldName + ": " + routerAdvertisementDatagram.Entries[_routerIndex].RouterAddress);
                                    break;

                                case "Preference level":
                                    field.AssertShow(fieldName + ": " + routerAdvertisementDatagram.Entries[_routerIndex++].RouterAddressPreference);
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid icmp " + icmpDatagram.MessageType + " field " + fieldName);
                            }
                            break;
                    }
                    field.AssertNoFields();
                    break;

                case "icmp.ident":
                    ushort identifier = ((IcmpIdentifiedDatagram)icmpDatagram).Identifier;
                    field.AssertShowDecimal(field.Showname().StartsWith("Identifier (BE): ") ? identifier : identifier.ReverseEndianity());
                    field.AssertNoFields();
                    break;

                case "icmp.seq":
                    field.AssertShowDecimal(((IcmpIdentifiedDatagram)icmpDatagram).SequenceNumber);
                    field.AssertNoFields();
                    break;

                case "icmp.seq_le":
                    byte[] sequenceNumberBuffer = new byte[sizeof(ushort)];
                    sequenceNumberBuffer.Write(0, ((IcmpIdentifiedDatagram)icmpDatagram).SequenceNumber, Endianity.Big);
                    ushort lowerEndianSequenceNumber = sequenceNumberBuffer.ReadUShort(0, Endianity.Small);
                    field.AssertShowDecimal(lowerEndianSequenceNumber);
                    field.AssertNoFields();
                    break;

                case "icmp.redir_gw":
                    field.AssertShow(((IcmpRedirectDatagram)icmpDatagram).GatewayInternetAddress.ToString());
                    field.AssertNoFields();
                    break;

                case "icmp.mtu":
                    field.AssertShowDecimal(((IcmpDestinationUnreachableDatagram)icmpDatagram).NextHopMaximumTransmissionUnit);
                    field.AssertNoFields();
                    break;

                case "l2tp.l2_spec_def":
                    field.AssertShow("");
                    field.AssertNoFields();
                    break;

                case "icmp.resp_to":
                case "icmp.resptime":
                    break;

                default:
                    if (!field.Name().StartsWith("lt2p.") &&
                        !field.Name().StartsWith("pweth."))
                        throw new InvalidOperationException("Invalid icmp field " + field.Name());
                    break;
            }

            return true;
        }

        private int _routerIndex;
    }
}