﻿using System;
using System.Xml.Linq;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Core.Test
{
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
                    field.AssertShowHex((ushort)ethernetDatagram.EtherType);
                    break;

                case "eth.trailer":
                    if (ethernetDatagram.Trailer != null)
                        field.AssertValue(ethernetDatagram.Trailer);
                    break;

                case "":
                    if (ethernetDatagram.Trailer != null)
                        field.AssertValue(ethernetDatagram.FrameCheckSequence);
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
                        field.AssertShow(address.ToString().ToLower());
                        break;

                    case "eth.ig":
                    case "eth.lg":
                        break;

                    default:
                        throw new InvalidOperationException("Invalid ethernet address field " + field.Name());
                }
            }
        }
    }
}