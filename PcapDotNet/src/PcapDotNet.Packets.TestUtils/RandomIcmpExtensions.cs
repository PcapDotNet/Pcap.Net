using System;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Packets.Icmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.TestUtils;
using IEnumerableExtensions = PcapDotNet.Base.IEnumerableExtensions;

namespace PcapDotNet.Packets.TestUtils
{
    public static class RandomIcmpExtensions
    {
        public static IcmpLayer NextIcmpLayer(this Random random)
        {
            IcmpMessageType icmpMessageType = random.NextEnum(IcmpMessageType.DomainNameReply);
            ushort? checksum = random.NextBool() ? (ushort?)random.NextUShort() : null;

            switch (icmpMessageType)
            {
                case IcmpMessageType.DestinationUnreachable:
                    return new IcmpDestinationUnreachableLayer
                           {
                               Code = random.NextEnum<IcmpCodeDestinationUnreachable>(),
                               Checksum = checksum,
                               NextHopMaximumTransmissionUnit = random.NextUShort(),
                           };

                case IcmpMessageType.TimeExceeded:
                    return new IcmpTimeExceededLayer
                           {
                               Code = random.NextEnum<IcmpCodeTimeExceeded>(),
                               Checksum = checksum,
                           };

                case IcmpMessageType.ParameterProblem:
                    return new IcmpParameterProblemLayer
                           {
                               Checksum = checksum,
                               Pointer = random.NextByte(),
                               OriginalDatagramLength = random.NextByte() * sizeof(uint),
                           };

                case IcmpMessageType.SourceQuench:
                    return new IcmpSourceQuenchLayer
                           {
                               Checksum = checksum
                           };

                case IcmpMessageType.Redirect:
                    return new IcmpRedirectLayer
                           {
                               Code = random.NextEnum<IcmpCodeRedirect>(),
                               Checksum = checksum,
                               GatewayInternetAddress = random.NextIpV4Address()
                           };

                case IcmpMessageType.Echo:
                    return new IcmpEchoLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort()
                           };

                case IcmpMessageType.EchoReply:
                    return new IcmpEchoReplyLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort()
                           };


                case IcmpMessageType.Timestamp:
                    return new IcmpTimestampLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort(),
                               OriginateTimestamp = random.NextIpV4TimeOfDay(),
                               ReceiveTimestamp = random.NextIpV4TimeOfDay(),
                               TransmitTimestamp = random.NextIpV4TimeOfDay()
                           };

                case IcmpMessageType.TimestampReply:
                    return new IcmpTimestampReplyLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort(),
                               OriginateTimestamp = random.NextIpV4TimeOfDay(),
                               ReceiveTimestamp = random.NextIpV4TimeOfDay(),
                               TransmitTimestamp = random.NextIpV4TimeOfDay()
                           };

                case IcmpMessageType.InformationRequest:
                    return new IcmpInformationRequestLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort(),
                           };

                case IcmpMessageType.InformationReply:
                    return new IcmpInformationReplyLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort(),
                           };


                case IcmpMessageType.RouterAdvertisement:
                    return new IcmpRouterAdvertisementLayer
                           {
                               Entries = random.NextIcmpRouterAdvertisementEntries(random.Next(10)).ToList().AsReadOnly(),
                               Checksum = checksum,
                               Lifetime = random.NextTimeSpan(TimeSpan.Zero, TimeSpan.FromSeconds(ushort.MaxValue)),
                           };

                case IcmpMessageType.RouterSolicitation:
                    return new IcmpRouterSolicitationLayer
                           {
                               Checksum = checksum,
                           };

                case IcmpMessageType.AddressMaskRequest:
                    return new IcmpAddressMaskRequestLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort(),
                               AddressMask = random.NextIpV4Address()
                           };

                case IcmpMessageType.AddressMaskReply:
                    return new IcmpAddressMaskReplyLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort(),
                               AddressMask = random.NextIpV4Address()
                           };

                case IcmpMessageType.TraceRoute:
                    return new IcmpTraceRouteLayer
                           {
                               Code = random.NextEnum<IcmpCodeTraceRoute>(),
                               Checksum = checksum,
                               Identification = random.NextUShort(),
                               OutboundHopCount = random.NextUShort(),
                               ReturnHopCount = random.NextUShort(),
                               OutputLinkSpeed = random.NextUInt(),
                               OutputLinkMaximumTransmissionUnit = random.NextUInt(),
                           };

                case IcmpMessageType.ConversionFailed:
                    return new IcmpConversionFailedLayer
                           {
                               Code = random.NextEnum<IcmpCodeConversionFailed>(),
                               Checksum = checksum,
                               Pointer = random.NextUInt(),
                           };

                case IcmpMessageType.DomainNameRequest:
                    return new IcmpDomainNameRequestLayer
                           {
                               Checksum = checksum,
                               Identifier = random.NextUShort(),
                               SequenceNumber = random.NextUShort(),
                           };

                case IcmpMessageType.DomainNameReply:
                    throw new NotSupportedException("Message Type " + icmpMessageType + " is not supported");

                case IcmpMessageType.SecurityFailures:
                    return new IcmpSecurityFailuresLayer
                           {
                               Code = random.NextEnum<IcmpCodeSecurityFailure>(),
                               Checksum = checksum,
                               Pointer = random.NextUShort()
                           };

                default:
                    throw new InvalidOperationException("Invalid icmpMessageType " + icmpMessageType);
            }
        }

        public static IEnumerable<ILayer> NextIcmpPayloadLayers(this Random random, IcmpLayer icmpLayer)
        {
            IEnumerable<ILayer> icmpPayloadLayers = new List<ILayer>();
            switch (icmpLayer.MessageType)
            {
                case IcmpMessageType.DestinationUnreachable:
                case IcmpMessageType.TimeExceeded:
                case IcmpMessageType.SourceQuench:
                case IcmpMessageType.Redirect:
                case IcmpMessageType.SecurityFailures:
                    icmpPayloadLayers = IEnumerableExtensions.Concat(icmpPayloadLayers, random.NextIpV4Layer(), random.NextPayloadLayer(IcmpIpV4HeaderPlus64BitsPayloadDatagram.OriginalDatagramPayloadLength));
                    break;
                case IcmpMessageType.ParameterProblem:
                    IpV4Layer ipV4Layer = random.NextIpV4Layer();
                    icmpPayloadLayers =
                        IEnumerableExtensions.Concat(icmpPayloadLayers, ipV4Layer,
                                                     random.NextPayloadLayer(random.NextInt(0,
                                                                                            IcmpParameterProblemLayer.OriginalDatagramLengthMaxValue + 1 -
                                                                                            ipV4Layer.Length)));
                    break;
                case IcmpMessageType.ConversionFailed:
                    IpV4Layer icmpIpV4Layer = random.NextIpV4Layer();
                    icmpPayloadLayers = IEnumerableExtensions.Concat(icmpPayloadLayers, icmpIpV4Layer);
                    if (icmpLayer.MessageTypeAndCode == IcmpMessageTypeAndCode.ConversionFailedUnsupportedTransportProtocol)
                    {
                        icmpPayloadLayers =
                            IEnumerableExtensions.Concat(icmpPayloadLayers, random.NextPayloadLayer(
                                IcmpConversionFailedDatagram.OriginalDatagramLengthForUnsupportedTransportProtocol -
                                icmpIpV4Layer.Length));
                    }
                    else
                    {
                        switch (icmpIpV4Layer.Protocol)
                        {
                            case IpV4Protocol.Udp:
                                icmpPayloadLayers = IEnumerableExtensions.Concat(icmpPayloadLayers, random.NextUdpLayer(),
                                                                             random.NextPayloadLayer(random.Next(100)));
                                break;

                            case IpV4Protocol.Tcp:
                                icmpPayloadLayers = IEnumerableExtensions.Concat(icmpPayloadLayers, random.NextTcpLayer(),
                                                                             random.NextPayloadLayer(random.Next(100)));
                                break;

                            default:
                                icmpPayloadLayers = IEnumerableExtensions.Concat(icmpPayloadLayers, random.NextPayloadLayer(random.Next(200)));

                                break;
                        }
                    }
                    break;

                case IcmpMessageType.Echo:
                case IcmpMessageType.EchoReply:
                case IcmpMessageType.Timestamp:
                case IcmpMessageType.TimestampReply:
                case IcmpMessageType.InformationRequest:
                case IcmpMessageType.InformationReply:
                case IcmpMessageType.RouterAdvertisement:
                case IcmpMessageType.RouterSolicitation:
                case IcmpMessageType.AddressMaskRequest:
                case IcmpMessageType.AddressMaskReply:
                case IcmpMessageType.TraceRoute:
                case IcmpMessageType.DomainNameRequest:
                    break;

                case IcmpMessageType.DomainNameReply:
                default:
                    throw new InvalidOperationException("Invalid icmpMessageType " + icmpLayer.MessageType);
            }
            return icmpPayloadLayers;
        }


        public static IEnumerable<IcmpRouterAdvertisementEntry> NextIcmpRouterAdvertisementEntries(this Random random, int numEntries)
        {
            for (int i = 0; i != numEntries; ++i)
                yield return new IcmpRouterAdvertisementEntry(random.NextIpV4Address(), random.Next());
        }
    }
}