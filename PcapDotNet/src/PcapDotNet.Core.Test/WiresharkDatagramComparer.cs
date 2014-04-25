using System;
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
            Datagram datagram;
            if (PropertyName == "")
            {
                datagram = (Datagram)datagramParent;
                datagramParent = null;
            }
            else
            {
                PropertyInfo property = datagramParent.GetType().GetProperty(PropertyName);
                if (property == null)
                    return null;

                datagram = (Datagram)property.GetValue(datagramParent);
            }
            if (Ignore(datagram))
                return null;

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
            bool success = true;
            foreach (var element in layer.Fields())
            {
                if (!CompareField(element, parentDatagram, datagram))
                {
                    success = false;
                    break;
                }
            }

            WiresharkCompareTests.CompareProtocols(datagram, layer, success);
        }

        public static WiresharkDatagramComparer GetComparer(string name, int count, bool parentLayerSuccess)
        {
            switch (name)
            {
                case "eth":
                    return new WiresharkDatagramComparerEthernet();

                case "vlan":
                    return new WiresharkDatagramComparerVLanTaggedFrame();

                case "arp":
                    return new WiresharkDatagramComparerArp();

                case "ip":
                    return new WiresharkDatagramComparerIpV4();

                case "ipv6":
                    return new WiresharkDatagramComparerIpV6();

                case "ah":
                    if (parentLayerSuccess)
                        return new WiresharkDatagramComparerIpV6AuthenticationHeader(count);
                    return null;

                case "mipv6":
                    return new WiresharkDatagramComparerIpV6MobilityHeader();

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

                case "dns":
                    return new WiresharkDatagramComparerDns();

                case "http":
                    return new WiresharkDatagramComparerHttp();

                default:
                    return null;
            }
        }
    }
}