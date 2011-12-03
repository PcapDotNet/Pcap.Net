using System;
using System.Xml.Linq;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Arp;

namespace PcapDotNet.Core.Test
{
    internal class WiresharkDatagramComparerArp : WiresharkDatagramComparerSimple
    {
        protected override string PropertyName
        {
            get { return "Arp"; }
        }

        protected override bool CompareField(XElement field, Datagram datagram)
        {
            ArpDatagram arpDatagram = (ArpDatagram)datagram;
            switch (field.Name())
            {
                case "arp.hw.type":
                    field.AssertShowHex((ushort)arpDatagram.HardwareType);
                    break;

                case "arp.proto.type":
                    field.AssertShowHex((ushort)arpDatagram.ProtocolType);
                    break;

                case "arp.hw.size":
                    field.AssertShowDecimal(arpDatagram.HardwareLength);
                    break;

                case "arp.proto.size":
                    field.AssertShowDecimal(arpDatagram.ProtocolLength);
                    break;

                case "arp.opcode":
                    field.AssertShowHex((ushort)arpDatagram.Operation);
                    break;

                case "arp.src.hw":
                case "arp.src.hw_mac":
                    field.AssertShow(arpDatagram.SenderHardwareAddress);
                    break;


                case "arp.src.proto":
                    field.AssertShow(arpDatagram.SenderProtocolAddress);
                    break;

                case "arp.src.proto_ipv4":
                    field.AssertShow(arpDatagram.SenderProtocolIpV4Address.ToString());
                    break;

                case "arp.dst.hw":
                case "arp.dst.hw_mac":
                    field.AssertShow(arpDatagram.TargetHardwareAddress);
                    break;

                case "arp.dst.proto":
                    field.AssertShow(arpDatagram.TargetProtocolAddress);
                    break;

                case "arp.dst.proto_ipv4":
                    field.AssertShow(arpDatagram.TargetProtocolIpV4Address.ToString());
                    break;

                case "arp.isgratuitous":
                    break;

                default:
                    throw new InvalidOperationException("Invalid arp field " + field.Name());
            }

            return true;
        }
    }
}