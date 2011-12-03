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
                case "":
                    if (field.Show().StartsWith("Flags and version: "))
                    {
                        XElement[] innerFields = field.Fields().ToArray();
                        bool isEnhanced = greDatagram.ProtocolType == EthernetType.PointToPointProtocol;
                        Assert.AreEqual(isEnhanced ? 9 : 8, innerFields.Length, "innerFields.Length");
                        foreach (var innerField in innerFields)
                        {
                            innerField.AssertName("");
                        }

                        int currentInnerFieldIndex = 0;
                        innerFields[currentInnerFieldIndex++].AssertShow(string.Format("{0}... .... .... .... = {1}", greDatagram.ChecksumPresent.ToInt(),
                                                                                       (greDatagram.ChecksumPresent ? "Checksum" : "No checksum")));
                        innerFields[currentInnerFieldIndex++].AssertShow(string.Format(".{0}.. .... .... .... = {1}", greDatagram.RoutingPresent.ToInt(),
                                                                                       (greDatagram.RoutingPresent ? "Routing" : "No routing")));
                        innerFields[currentInnerFieldIndex++].AssertShow(string.Format("..{0}. .... .... .... = {1}", greDatagram.KeyPresent.ToInt(),
                                                                                       (greDatagram.KeyPresent ? "Key" : "No key")));
                        innerFields[currentInnerFieldIndex++].AssertShow(string.Format("...{0} .... .... .... = {1}",
                                                                                       greDatagram.SequenceNumberPresent.ToInt(),
                                                                                       (greDatagram.SequenceNumberPresent
                                                                                            ? "Sequence number"
                                                                                            : "No sequence number")));
                        innerFields[currentInnerFieldIndex++].AssertShow(string.Format(".... {0}... .... .... = {1}", greDatagram.StrictSourceRoute.ToInt(),
                                                                                       (greDatagram.StrictSourceRoute
                                                                                            ? "Strict source route"
                                                                                            : "No strict source route")));
                        innerFields[currentInnerFieldIndex++].AssertShow(string.Format(".... .{0} .... .... = Recursion control: {1}",
                                                                                       greDatagram.RecursionControl.ToBits().Skip(5).Select(b => b.ToInt()).
                                                                                           SequenceToString(),
                                                                                       greDatagram.RecursionControl));
                        if (isEnhanced)
                        {
                            innerFields[currentInnerFieldIndex++].AssertShow(string.Format(".... .... {0}... .... = {1}",
                                                                                           greDatagram.AcknowledgmentSequenceNumberPresent.ToInt(),
                                                                                           (greDatagram.AcknowledgmentSequenceNumberPresent
                                                                                                ? "Acknowledgment number"
                                                                                                : "No acknowledgment number")));

                            innerFields[currentInnerFieldIndex++].AssertShow(string.Format(".... .... .{0}... = Flags: {1}",
                                                                                           greDatagram.FutureUseBits.ToBits().Skip(3).Take(4).Select(
                                                                                               b => b.ToInt()).
                                                                                               SequenceToString().
                                                                                               Insert(3, " "),
                                                                                           greDatagram.FutureUseBits));
                        }
                        else
                        {
                            byte fullFlags = (byte)(greDatagram.FutureUseBits | (greDatagram.AcknowledgmentSequenceNumberPresent ? 0x10 : 0x00));
                            innerFields[currentInnerFieldIndex++].AssertShow(string.Format(".... .... {0}... = Flags: {1}",
                                                                                           fullFlags.ToBits().Skip(3).Select(b => b.ToInt()).
                                                                                               SequenceToString().
                                                                                               Insert(4, " "),
                                                                                           fullFlags));
                        }
                        innerFields[currentInnerFieldIndex++].AssertShow(string.Format(".... .... .... .{0} = Version: {1}",
                                                                                       ((byte)greDatagram.Version).ToBits().Skip(5).Select(b => b.ToInt()).
                                                                                           SequenceToString(),
                                                                                       (byte)greDatagram.Version));
                    }
                    else if (field.Show().StartsWith("Checksum: "))
                    {
                        field.AssertValue(greDatagram.Checksum);
                    }
                    else if (field.Show().StartsWith("Offset: "))
                    {
                        field.AssertValue(greDatagram.RoutingOffset);
                    }
                    else if (field.Show().StartsWith("Payload length: "))
                    {
                        field.AssertValue(greDatagram.KeyPayloadLength);
                    }
                    else if (field.Show().StartsWith("Call ID: "))
                    {
                        field.AssertValue(greDatagram.KeyCallId);
                    }
                    else if (field.Show().StartsWith("Sequence number: "))
                    {
                        field.AssertValue(greDatagram.SequenceNumber);
                    }
                    else if (field.Show().StartsWith("Acknowledgement number: "))
                    {
                        field.AssertValue(greDatagram.AcknowledgmentSequenceNumber);
                    }
                    else if (field.Show().StartsWith("Address family: "))
                    {
                        ++_currentEntry;
                        if (_currentEntry < greDatagram.Routing.Count)
                            field.AssertValue((ushort)greDatagram.Routing[_currentEntry].AddressFamily);
                        else if (_currentEntry > greDatagram.Routing.Count)
                            Assert.IsFalse(greDatagram.IsValid);
                    }
                    else if (field.Show().StartsWith("SRE offset: "))
                    {
                        if (_currentEntry < greDatagram.Routing.Count)
                            field.AssertValue(greDatagram.Routing[_currentEntry].PayloadOffset);
                        else if (_currentEntry > greDatagram.Routing.Count)
                            Assert.IsFalse(greDatagram.IsValid);
                    }
                    else if (field.Show().StartsWith("SRE length: "))
                    {
                        if (_currentEntry < greDatagram.Routing.Count)
                            field.AssertValue(greDatagram.Routing[_currentEntry].PayloadLength);
                        else if (_currentEntry > greDatagram.Routing.Count)
                            Assert.IsFalse(greDatagram.IsValid);
                    }
                    else
                    {
                        Assert.Fail("Invalid field " + field.Show());
                    }

                    break;

                case "gre.proto":
                    field.AssertShowHex((ushort)greDatagram.ProtocolType);
                    break;

                case "gre.key":
                    field.AssertShowHex(greDatagram.Key);
                    break;

                case "data":
                case "data.data":
                    if (greDatagram.AcknowledgmentSequenceNumberPresent &&
                        (greDatagram.Version != GreVersion.EnhancedGre || !greDatagram.SequenceNumberPresent))
                    {
                        Assert.AreEqual(field.Value().Skip(8).SequenceToString(), greDatagram.Payload.BytesSequenceToHexadecimalString(), "GRE data.data");
                    }
                    else
                    {
                        field.AssertValue(greDatagram.Payload, "GRE data.data");
                    }
                    break;

                case "data.len":
                    field.AssertShowDecimal(
                        greDatagram.Payload.Length + (greDatagram.AcknowledgmentSequenceNumberPresent &&
                                                      (greDatagram.Version != GreVersion.EnhancedGre || !greDatagram.SequenceNumberPresent)
                                                          ? 4
                                                          : 0), "GRE data.len");
                    break;

                default:
                    Assert.Fail("Invalid field name: " + field.Name());
                    break;
            }

            return true;
        }

        private int _currentEntry = -1;
    }
}