using System.Reflection;
using System.Xml.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets;

namespace PcapDotNet.Core.Test
{
    internal abstract class WiresharkDatagramComparer
    {
        public Datagram Compare(XElement layer, object datagramParent)
        {
            PropertyInfo property = datagramParent.GetType().GetProperty(PropertyName);
            if (property == null)
                return null;

            Datagram datagram = (Datagram)property.GetValue(datagramParent);
            if (!Ignore(datagram))
                CompareDatagram(layer, datagramParent as Datagram, datagram);
            return datagram;
        }

        protected abstract string PropertyName { get; }

        protected abstract bool CompareField(XElement field, Datagram parentDatagram, Datagram datagram);

        protected virtual bool Ignore(Datagram datagram)
        {
            return false;
        }

        protected void CompareDatagram(XElement layer, Datagram parentDatagram, Datagram datagram)
        {
            foreach (var field in layer.Fields())
            {
                if (!CompareField(field, parentDatagram, datagram))
                    break;
            }

            WiresharkCompareTests.CompareProtocols(datagram, layer);
        }

        public static WiresharkDatagramComparer GetComparer(string name)
        {
            switch (name)
            {
                case "eth":
                    return new WiresharkDatagramComparerEthernet();

                case "arp":
                    return new WiresharkDatagramComparerArp();

                case "ip":
                    return new WiresharkDatagramComparerIpV4();

                case "igmp":
                    return new WiresharkDatagramComparerIgmp();

                case "icmp":
                    return new WiresharkDatagramComparerIcmp();

                case "gre":
                    return new WiresharkDatagramComparerGre();

                case "udp":
                    return new WiresharkDatagramComparerUdp();

                case "tcp":
                    return new WiresharkDatagramComparerTcp();

                case "http":
                    return new WiresharkDatagramComparerHttp();

                default:
                    return null;
            }
        }
    }
}