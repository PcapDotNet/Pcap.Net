using System;
using System.Xml.Linq;
using PcapDotNet.Packets;
using PcapDotNet.Packets.VLanTaggedFrame;

namespace PcapDotNet.Core.Test
{
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
                    field.AssertShowDecimal((byte)vLanTaggedFrameDatagram.PriorityCodePoint);
                    break;

                case "vlan.cfi":
                    field.AssertShowDecimal(vLanTaggedFrameDatagram.CanonicalFormatIndicator);
                    break;

                case "vlan.id":
                    field.AssertShowDecimal(vLanTaggedFrameDatagram.VLanIdentifier);
                    break;

                case "vlan.etype":
                    field.AssertShowHex((ushort)vLanTaggedFrameDatagram.EtherType);
                    break;

                case "vlan.len":
                    field.AssertShowDecimal((ushort)vLanTaggedFrameDatagram.EtherType);
                    break;

                default:
                    throw new InvalidOperationException("Invalid VLanTaggedFrame field " + field.Name());
            }
            field.AssertNoFields();

            return true;
        }
    }
}