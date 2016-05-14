using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Core.Test
{
    [ExcludeFromCodeCoverage]
    internal class WiresharkDatagramComparerVLanTaggedFrame : WiresharkDatagramComparerSimple
    {
        protected override string PropertyName
        {
            get { return "VLanTaggedFrame"; }
        }

        protected override bool CompareField(XElement field, Datagram datagram)
        {
            VLanTaggedFrameDatagram vLanTaggedFrameDatagram = (VLanTaggedFrameDatagram)datagram;
            switch (field.Name())
            {
                case "vlan.priority":
                    field.AssertNoFields();
                    field.AssertShowDecimal((byte)vLanTaggedFrameDatagram.PriorityCodePoint);
                    break;

                case "vlan.cfi":
                    field.AssertNoFields();
                    field.AssertShowDecimal(vLanTaggedFrameDatagram.CanonicalFormatIndicator);
                    break;

                case "vlan.id":
                    field.AssertNoFields();
                    field.AssertShowDecimal(vLanTaggedFrameDatagram.VLanIdentifier);
                    break;

                case "vlan.etype":
                    field.AssertNoFields();
                    field.AssertShowDecimal((ushort)vLanTaggedFrameDatagram.EtherType);
                    break;

                case "vlan.len":
                    field.AssertShowDecimal((ushort)vLanTaggedFrameDatagram.EtherType);
                    if (field.Fields().Any())
                    {
                        field.AssertNumFields(1);
                        field.Fields().First().AssertName("_ws.expert");
                    }
                    break;

                case "vlan.trailer":
                    field.AssertNoFields();
                    if (!new[]
                             {
                                 (EthernetType)1,
                                 (EthernetType)5,
                                 (EthernetType)12,
                                 (EthernetType)17,
                                 (EthernetType)29,
                                 (EthernetType)30,
                                 (EthernetType)43,
                                 (EthernetType)50,
                                 EthernetType.ReverseArp, // TODO: Support RARP
                                 EthernetType.ExtensibleAuthenticationProtocolOverLan, // TODO: Support this protocol.
                             }.Contains(
                                 vLanTaggedFrameDatagram.EtherType))
                    {
                        field.AssertValue(vLanTaggedFrameDatagram.ExtraData);
                    }
                    break;

                case "eth.padding":
                    field.AssertNoFields();
                    if (!new[]
                             {
                                 EthernetType.ExtensibleAuthenticationProtocolOverLan, // TODO: Support this protocol.
                             }.Contains(
                                 vLanTaggedFrameDatagram.EtherType))
                    {
                        field.AssertValue(vLanTaggedFrameDatagram.Trailer);
                    }
                    break;

                default:
                    throw new InvalidOperationException("Invalid VLanTaggedFrame field " + field.Name());
            }

            return true;
        }
    }
}