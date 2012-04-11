using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Gre;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Core.Test
{
    internal class WiresharkDatagramComparerGre : WiresharkDatagramComparerSimple
    {
        protected override string PropertyName
        {
            get { return "Gre"; }
        }

        protected override bool CompareField(XElement field, Datagram datagram)
        {
            GreDatagram greDatagram = (GreDatagram)datagram;
            switch (field.Name())
            {
                case "gre.flags_and_version":
                    XElement[] innerFields = field.Fields().ToArray();
                    MoreAssert.IsInRange(8, 9, innerFields.Length);

                    int currentInnerFieldIndex = 0;
                    innerFields[currentInnerFieldIndex++].AssertShowDecimal(greDatagram.ChecksumPresent);
                    innerFields[currentInnerFieldIndex++].AssertShowDecimal(greDatagram.RoutingPresent);
                    innerFields[currentInnerFieldIndex++].AssertShowDecimal(greDatagram.KeyPresent);
                    innerFields[currentInnerFieldIndex++].AssertShowDecimal(greDatagram.SequenceNumberPresent);
                    innerFields[currentInnerFieldIndex++].AssertShowDecimal(greDatagram.StrictSourceRoute);
                    innerFields[currentInnerFieldIndex++].AssertShowDecimal(greDatagram.RecursionControl);
                    if (innerFields.Length == 9)
                    {
                        innerFields[currentInnerFieldIndex++].AssertShowDecimal(greDatagram.AcknowledgmentSequenceNumberPresent);
                        innerFields[currentInnerFieldIndex++].AssertShowDecimal(greDatagram.FutureUseBits);
                    }
                    else
                    {
                        byte futureUseBitsValue = (byte)((greDatagram.AcknowledgmentSequenceNumberPresent.ToInt() << 4) | greDatagram.FutureUseBits);
                        innerFields[currentInnerFieldIndex++].AssertShowDecimal(futureUseBitsValue);
                    }
                    innerFields[currentInnerFieldIndex].AssertShowDecimal((byte)greDatagram.Version);
                    break;

                case "data.len":
                    field.AssertShowDecimal(
                        greDatagram.Payload.Length + (greDatagram.AcknowledgmentSequenceNumberPresent &&
                                                      (greDatagram.Version != GreVersion.EnhancedGre || !greDatagram.SequenceNumberPresent)
                                                          ? 4
                                                          : 0), "GRE data.len");
                    field.AssertNoFields();
                    break;

                case "gre.checksum":
                    field.AssertShowHex(greDatagram.Checksum);
                    field.AssertNoFields();
                    break;

                case "gre.key.payload_length":
                    field.AssertShowDecimal(greDatagram.KeyPayloadLength);
                    field.AssertNoFields();
                    break;

                case "gre.key.call_id":
                    field.AssertShowDecimal(greDatagram.KeyCallId);
                    field.AssertNoFields();
                    break;

                case "gre.ack_number":
                    field.AssertShowDecimal(greDatagram.AcknowledgmentSequenceNumber);
                    field.AssertNoFields();
                    break;

                case "gre.sequence_number":
                    field.AssertShowDecimal(greDatagram.SequenceNumber);
                    field.AssertNoFields();
                    break;

                case "gre.offset":
                    field.AssertShowDecimal(greDatagram.RoutingOffset);
                    field.AssertNoFields();
                    break;

                case "gre.routing":
                    field.AssertShow("");
                    field.AssertNoFields();
                    break;

                case "gre.routing.address_family":
                    if (_routingEntryIndex == greDatagram.Routing.Count)
                    {
                        if (SupportedGre(greDatagram))
                            field.AssertShowDecimal(0);
                    }
                    else
                        field.AssertShowDecimal((ushort)greDatagram.Routing[_routingEntryIndex].AddressFamily);
                    field.AssertNoFields();
                    break;

                case "gre.routing.sre_offset":
                    if (_routingEntryIndex == greDatagram.Routing.Count)
                    {
                        if (SupportedGre(greDatagram))
                            field.AssertShowDecimal(0);
                    }
                    else
                        field.AssertShowDecimal(greDatagram.Routing[_routingEntryIndex].PayloadOffset);
                    field.AssertNoFields();
                    break;

                case "gre.routing.src_length":
                    if (_routingEntryIndex == greDatagram.Routing.Count)
                    {
                        if (SupportedGre(greDatagram))
                            field.AssertShowDecimal(0);
                    }
                    else
                        field.AssertShowDecimal(greDatagram.Routing[_routingEntryIndex].PayloadLength);
                    field.AssertNoFields();
                    break;

                case "gre.routing.information":
                    if (_routingEntryIndex == greDatagram.Routing.Count)
                    {
                        Assert.IsFalse(SupportedGre(greDatagram));
                    }
                    else
                    {
                        switch (greDatagram.Routing[_routingEntryIndex].AddressFamily)
                        {
                            case GreSourceRouteEntryAddressFamily.IpSourceRoute:
                                field.AssertValue(((GreSourceRouteEntryIp)greDatagram.Routing[_routingEntryIndex]).Addresses);
                                break;

                            case GreSourceRouteEntryAddressFamily.AsSourceRoute:
                                field.AssertValue(((GreSourceRouteEntryAs)greDatagram.Routing[_routingEntryIndex]).AsNumbers);
                                break;

                            default:
                                field.AssertValue(((GreSourceRouteEntryUnknown)greDatagram.Routing[_routingEntryIndex]).Data);
                                break;
                        }

                        ++_routingEntryIndex;
                    }
                    field.AssertNoFields();
                    break;

                case "gre.proto":
                    field.AssertShowHex((ushort)greDatagram.ProtocolType);
                    field.AssertNoFields();
                    break;

                case "gre.key":
                    field.AssertShowHex(greDatagram.Key);
                    field.AssertNoFields();
                    break;

                case "data":
                case "data.data":
                    if (SupportedGre(greDatagram))
                        field.AssertDataField(greDatagram.Payload);
                    break;

                default:
                    Assert.Fail("Invalid field name: " + field.Name());
                    break;
            }

            return true;
        }

        private static bool SupportedGre(GreDatagram greDatagram)
        {
            return greDatagram.IsValid &&
                   (greDatagram.ProtocolType == EthernetType.PointToPointProtocol ||
                    greDatagram.Version == GreVersion.EnhancedGre ||
                    !greDatagram.AcknowledgmentSequenceNumberPresent);
        }

        private int _routingEntryIndex = 0;
    }
}