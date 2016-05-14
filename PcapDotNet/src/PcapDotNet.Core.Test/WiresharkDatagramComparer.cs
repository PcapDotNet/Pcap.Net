using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Xml.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;

namespace PcapDotNet.Core.Test
{
    [ExcludeFromCodeCoverage]
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

            if (!CompareDatagram(layer, datagramParent as Datagram, datagram))
                return null;
            return datagram;
        }

        protected abstract string PropertyName { get; }

        protected abstract bool CompareField(XElement field, Datagram parentDatagram, Datagram datagram);

        protected virtual bool Ignore(Datagram datagram)
        {
            return false;
        }

        protected bool CompareDatagram(XElement layer, Datagram parentDatagram, Datagram datagram)
        {
            bool success = true;
            foreach (var element in layer.Fields())
            {
                // TODO: Remove this hack when https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=11802 is fixed.
                IpV6Datagram ipV6ParentDatagram = parentDatagram as IpV6Datagram;
                if (ipV6ParentDatagram != null && (ipV6ParentDatagram.NextHeader == IpV4Protocol.IsoIp || ipV6ParentDatagram.ExtensionHeaders.NextHeader == IpV4Protocol.IsoIp))
                    return false;
                if (!CompareField(element, parentDatagram, datagram))
                {
                    success = false;
                    break;
                }
            }

            WiresharkCompareTests.CompareProtocols(datagram, layer, success);
            return success;
        }

        public static WiresharkDatagramComparer GetComparer(string name, int count, string parentName, bool parentLayerSuccess)
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
                    {
                        // TODO: Remove this condition when https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10705 is fixed.
                        if (parentName == "ipv6")
                            return new WiresharkDatagramComparerIpV6AuthenticationHeader(count);
                    }
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