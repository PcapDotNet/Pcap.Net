using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Gre;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.TestUtils
{
    [ExcludeFromCodeCoverage]
    public static class RandomGreExtensions
    {
        public static GreLayer NextGreLayer(this Random random)
        {
            GreVersion version = random.NextEnum<GreVersion>();

            bool isChecksum = random.NextBool();
            GreSourceRouteEntry[] routing = null;
            ushort? routingOffset = null;
            bool strictSourceRoute = false;
            EthernetType protocolType = random.NextEnum(EthernetType.None);
            uint? key = random.NextBool() ? (uint?)random.NextUInt() : null;
            if (version == GreVersion.Gre)
            {
                if (random.NextBool())
                {
                    strictSourceRoute = random.NextBool();
                    routing = new GreSourceRouteEntry[random.Next(5)];

                    GreSourceRouteEntryAddressFamily family;
                    if (random.NextBool())
                        family = random.NextEnum(GreSourceRouteEntryAddressFamily.None);
                    else
                        family = (GreSourceRouteEntryAddressFamily)random.NextUShort();

                    for (int i = 0; i != routing.Length; ++i)
                    {
                        switch (family)
                        {
                            case GreSourceRouteEntryAddressFamily.AsSourceRoute:
                            {
                                ushort[] asNumbers = ((Func<ushort>)(() => random.NextUShort())).GenerateArray(random.NextInt(1, 5));
                                routing[i] = new GreSourceRouteEntryAs(asNumbers.AsReadOnly(), random.Next(asNumbers.Length + 1));
                                break;
                            }

                            case GreSourceRouteEntryAddressFamily.IpSourceRoute:
                            {
                                IpV4Address[] ips = ((Func<IpV4Address>)(() => random.NextIpV4Address())).GenerateArray(random.NextInt(1, 5));
                                routing[i] = new GreSourceRouteEntryIp(ips.AsReadOnly(), random.Next(ips.Length + 1));
                                break;
                            }

                            default:
                            {
                                int dataLength = random.NextInt(1, 100);
                                routing[i] = new GreSourceRouteEntryUnknown(family, random.NextDatagram(dataLength), random.Next(dataLength + 1));
                                break;
                            }
                        }

                    }
                    routingOffset = 0;
                    if (routing.Any())
                    {
                        int routingIndex = random.Next(routing.Length);
                        for (int i = 0; i != routingIndex; ++i)
                            routingOffset += (ushort)routing[i].Length;
                    }
                }
            }
            else
            {
                protocolType = EthernetType.PointToPointProtocol;
                isChecksum = false;
                key = random.NextUInt();
            }

            return new GreLayer
                   {
                       Version = version,
                       ProtocolType = protocolType,
                       ChecksumPresent = isChecksum,
                       Checksum = isChecksum && random.NextBool() ? (ushort?)random.NextUShort() : null,
                       Key = key,
                       SequenceNumber = random.NextBool() ? (uint?)random.NextUInt() : null,
                       AcknowledgmentSequenceNumber = version == GreVersion.EnhancedGre && random.NextBool() ? (uint?)random.NextUInt() : null,
                       RecursionControl = random.NextByte(8),
//                       Flags = random.NextByte(32),
                       Routing = routing == null ? null : routing.AsReadOnly(),
                       RoutingOffset = routingOffset,
                       StrictSourceRoute = strictSourceRoute,
                   };
        }
    }
}