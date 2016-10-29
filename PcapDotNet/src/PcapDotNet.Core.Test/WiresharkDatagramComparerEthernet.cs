using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Core.Test
{
    [ExcludeFromCodeCoverage]
    internal class WiresharkDatagramComparerEthernet : WiresharkDatagramComparerSimple
    {
        protected override string PropertyName
        {
            get { return "Ethernet"; }
        }

        protected override bool CompareField(XElement field, Datagram datagram)
        {
            EthernetDatagram ethernetDatagram = (EthernetDatagram)datagram;
            switch (field.Name())
            {
                case "eth.dst":
                    CompareEthernetAddress(field, ethernetDatagram.Destination);
                    break;

                case "eth.src":
                    CompareEthernetAddress(field, ethernetDatagram.Source);
                    break;

                case "eth.type":
                    field.AssertNoFields();
                    field.AssertShowDecimal((ushort)ethernetDatagram.EtherType);
                    break;

                case "eth.trailer":
                    // TODO: Support RARP.
                    if (ethernetDatagram.EtherType != EthernetType.ReverseArp)
                        field.AssertValue(ethernetDatagram.Trailer);
                    break;

                case "eth.fcs":
                    // TODO: Support RARP.
                    if (ethernetDatagram.EtherType != EthernetType.ReverseArp)
                        field.AssertValue(ethernetDatagram.FrameCheckSequence);
                    break;

                case "eth.padding":
                    field.AssertNoFields();
                    // TODO: Support RARP.
                    if (ethernetDatagram.EtherType != EthernetType.ReverseArp)
                        field.AssertValue(ethernetDatagram.Padding);
                    break;

                default:
                    throw new InvalidOperationException("Invalid etherent field " + field.Name());
            }

            return true;
        }

        private static void CompareEthernetAddress(XElement element, MacAddress address)
        {
            foreach (var field in element.Fields())
            {
                switch (field.Name())
                {
                    case "eth.addr":
                    case "eth.dst_resolved":
                    case "eth.addr_resolved":
                    case "eth.src_resolved":
                        field.AssertShow(address.ToString().ToLower());
                        field.AssertNoFields();
                        break;

                    case "eth.ig":
                    case "eth.lg":
                        field.AssertNoFields();
                        break;

                    case "_ws.expert":
                        field.AssertNumFields(4);
                        break;

                    default:
                        throw new InvalidOperationException("Invalid ethernet address field " + field.Name());
                }
            }
        }
    }
}