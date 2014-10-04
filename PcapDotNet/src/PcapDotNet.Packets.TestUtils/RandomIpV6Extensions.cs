using System;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ip;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.TestUtils
{
    public static class RandomIpV6Extensions
    {
        public static IpV6Address NextIpV6Address(this Random random)
        {
            return new IpV6Address(random.NextUInt128());
        }

        public static IpV6Address NextIpV6AddressWithLeadingZeroBytes(this Random random, int numLeadingZeros)
        {
            return new IpV6Address(random.NextUInt128() >> (8 * numLeadingZeros));
        }

        public static IpV6Address[] NextIpV6AddressArray(this Random random, int count)
        {
            return ((Func<IpV6Address>)(random.NextIpV6Address)).GenerateArray(count);
        }

        public static IpV6Address[] NextIpV6AddressWithLeadingZeroBytesArray(this Random random, int numLeadingZeros, int count)
        {
            return ((Func<IpV6Address>)(() => random.NextIpV6AddressWithLeadingZeroBytes(numLeadingZeros))).GenerateArray(count);
        }

        public static IpV6Layer NextIpV6Layer(this Random random, bool withAutomaticNextLayer)
        {
            IpV4Protocol? lastNextHeader = withAutomaticNextLayer ? (IpV4Protocol?)null : random.NextEnum<IpV4Protocol>(IpV6ExtensionHeader.ExtensionHeaders);
            return random.NextIpV6Layer(lastNextHeader, !withAutomaticNextLayer);
        }

        public static IpV6Layer NextIpV6Layer(this Random random, IpV4Protocol? lastNextHeader, bool allowEncapsulatingSecurityPayload)
        {
            IpV6ExtensionHeaders extensionHeaders = random.NextIpV6ExtensionHeaders(random.NextInt(0, 10), lastNextHeader, allowEncapsulatingSecurityPayload);
            IpV4Protocol nextHeader = extensionHeaders.FirstHeader ?? (lastNextHeader ?? random.NextEnum<IpV4Protocol>(IpV6ExtensionHeader.ExtensionHeaders));
            return new IpV6Layer
                       {
                           TrafficClass = random.NextByte(),
                           FlowLabel = random.NextInt(0, IpV6Datagram.MaxFlowLabel + 1),
                           NextHeader = nextHeader,
                           HopLimit = random.NextByte(),
                           Source = random.NextIpV6Address(),
                           CurrentDestination = random.NextIpV6Address(),
                           ExtensionHeaders = extensionHeaders,
                       };
        }

        public static IpV6ExtensionHeaders NextIpV6ExtensionHeaders(this Random random, int count, IpV4Protocol? nextHeader, bool allowEncapsulatingSecurityPayload)
        {
            if (count == 0)
                return IpV6ExtensionHeaders.Empty;
            IpV6ExtensionHeader[] headers = new IpV6ExtensionHeader[count];
            for (int i = headers.Length - 1; i >= 0; --i)
            {
                headers[i] = random.NextIpV6ExtensionHeader(nextHeader, allowEncapsulatingSecurityPayload && i == headers.Length - 1);
                nextHeader = headers[i].Protocol;
            }
            return new IpV6ExtensionHeaders(headers);
        }

        public static IpV6ExtensionHeader NextIpV6ExtensionHeader(this Random random, IpV4Protocol? nextHeader, bool isEncapsulatingSecurityPayloadPossible)
        {
            IpV4Protocol extensionHeaderType =
                random.NextValue(
                    IpV6ExtensionHeader.ExtensionHeaders.Where(extensionHeader => isEncapsulatingSecurityPayloadPossible ||
                                                                                  extensionHeader != IpV4Protocol.EncapsulatingSecurityPayload).ToList());
            switch (extensionHeaderType)
            {
                case IpV4Protocol.IpV6HopByHopOption: // 0
                    return new IpV6ExtensionHeaderHopByHopOptions(nextHeader, random.NextIpV6Options());

                case IpV4Protocol.IpV6Route: // 43
                    IpV6RoutingType routingType = random.NextEnum(IpV6RoutingType.Nimrod);
                    switch (routingType)
                    {
                        case IpV6RoutingType.SourceRoute:
                            return new IpV6ExtensionHeaderRoutingSourceRoute(nextHeader, random.NextByte(), random.NextIpV6AddressArray(random.NextInt(0, 10)));

                        case IpV6RoutingType.Type2RoutingHeader:
                            return new IpV6ExtensionHeaderRoutingHomeAddress(nextHeader, random.NextByte(), random.NextIpV6Address());

                        case IpV6RoutingType.RoutingProtocolLowPowerAndLossyNetworksSourceRouteHeader:
                            byte commonPrefixLengthForNonLastAddresses = random.NextByte(IpV6ExtensionHeaderRoutingProtocolLowPowerAndLossyNetworks.MaxCommonPrefixLength + 1);
                            byte commonPrefixLengthForLastAddress = random.NextByte(IpV6ExtensionHeaderRoutingProtocolLowPowerAndLossyNetworks.MaxCommonPrefixLength + 1);
                            IpV6Address[] addresses = random.NextIpV6AddressWithLeadingZeroBytesArray(commonPrefixLengthForNonLastAddresses,
                                                                                                      random.NextInt(0, 10));
                            if (addresses.Any() || random.NextBool())
                                addresses = addresses.Concat(random.NextIpV6AddressWithLeadingZeroBytes(commonPrefixLengthForLastAddress)).ToArray();
                            return new IpV6ExtensionHeaderRoutingProtocolLowPowerAndLossyNetworks(nextHeader, random.NextByte(), commonPrefixLengthForNonLastAddresses,
                                                                     commonPrefixLengthForLastAddress, addresses);

                        default:
                            throw new InvalidOperationException(string.Format("Invalid routingType value {0}", routingType));
                    }

                case IpV4Protocol.FragmentHeaderForIpV6: // 44
                    return new IpV6ExtensionHeaderFragmentData(nextHeader, random.NextUShort(IpV6ExtensionHeaderFragmentData.MaxFragmentOffset + 1),
                                                               random.NextBool(), random.NextUInt());

                case IpV4Protocol.IpV6Opts: // 60
                    IpV6Options options = random.NextIpV6Options();
                    return new IpV6ExtensionHeaderDestinationOptions(nextHeader, options);

                case IpV4Protocol.MobilityHeader: // 135
                    return random.NextIpV6ExtensionHeaderMobility(nextHeader);

                case IpV4Protocol.EncapsulatingSecurityPayload: // 50
                    return new IpV6ExtensionHeaderEncapsulatingSecurityPayload(random.NextUInt(), random.NextUInt(), random.NextDataSegment(random.Next(100)));

                case IpV4Protocol.AuthenticationHeader: // 51
                    return new IpV6ExtensionHeaderAuthentication(nextHeader, random.NextUInt(), random.NextUInt(), random.NextDataSegment(random.Next(25) * 4));

                default:
                    throw new InvalidOperationException(string.Format("Invalid extensionHeaderType value {0}", extensionHeaderType));
            }
        }

        public static IpV6ExtensionHeaderMobility NextIpV6ExtensionHeaderMobility(this Random random, IpV4Protocol? nextHeader)
        {
            IpV6MobilityHeaderType mobilityHeaderType = random.NextEnum<IpV6MobilityHeaderType>();
            ushort checksum = random.NextUShort();
            switch (mobilityHeaderType)
            {
                case IpV6MobilityHeaderType.BindingRefreshRequest: // 0
                    return new IpV6ExtensionHeaderMobilityBindingRefreshRequest(nextHeader, checksum, random.NextIpV6MobilityOptions());

                case IpV6MobilityHeaderType.HomeTestInit: // 1
                    return new IpV6ExtensionHeaderMobilityHomeTestInit(nextHeader, checksum, random.NextULong(), random.NextIpV6MobilityOptions());

                case IpV6MobilityHeaderType.CareOfTestInit: // 2
                    return new IpV6ExtensionHeaderMobilityCareOfTestInit(nextHeader, checksum, random.NextULong(), random.NextIpV6MobilityOptions());

                case IpV6MobilityHeaderType.HomeTest: // 3
                    return new IpV6ExtensionHeaderMobilityHomeTest(nextHeader, checksum, random.NextUShort(), random.NextULong(), random.NextULong(),
                                                                   random.NextIpV6MobilityOptions());

                case IpV6MobilityHeaderType.CareOfTest: // 4
                    return new IpV6ExtensionHeaderMobilityCareOfTest(nextHeader, checksum, random.NextUShort(), random.NextULong(), random.NextULong(),
                                                                     random.NextIpV6MobilityOptions());

                case IpV6MobilityHeaderType.BindingUpdate: // 5
                    return new IpV6ExtensionHeaderMobilityBindingUpdate(nextHeader, checksum, random.NextUShort(), random.NextBool(), random.NextBool(),
                                                                        random.NextBool(), random.NextBool(), random.NextBool(), random.NextBool(),
                                                                        random.NextBool(), random.NextBool(), random.NextBool(), random.NextBool(),
                                                                        random.NextUShort(), random.NextIpV6MobilityOptions());

                case IpV6MobilityHeaderType.BindingAcknowledgement: // 6
                    return new IpV6ExtensionHeaderMobilityBindingAcknowledgement(nextHeader, checksum, random.NextEnum<IpV6BindingAcknowledgementStatus>(),
                                                                                 random.NextBool(), random.NextBool(), random.NextBool(), random.NextBool(),
                                                                                 random.NextUShort(), random.NextUShort(), random.NextIpV6MobilityOptions());

                case IpV6MobilityHeaderType.BindingError: // 7
                    return new IpV6ExtensionHeaderMobilityBindingError(nextHeader, checksum, random.NextEnum<IpV6BindingErrorStatus>(), random.NextIpV6Address(),
                                                                       random.NextIpV6MobilityOptions());

                case IpV6MobilityHeaderType.FastBindingUpdate: // 8
                    return new IpV6ExtensionHeaderMobilityFastBindingUpdate(nextHeader, checksum, random.NextUShort(), random.NextBool(), random.NextBool(),
                                                                            random.NextBool(), random.NextBool(), random.NextUShort(),
                                                                            random.NextIpV6MobilityOptions());

                case IpV6MobilityHeaderType.FastBindingAcknowledgement: // 9
                    return new IpV6ExtensionHeaderMobilityFastBindingAcknowledgement(nextHeader, checksum, random.NextEnum<IpV6BindingAcknowledgementStatus>(),
                                                                                     random.NextBool(), random.NextUShort(), random.NextUShort(),
                                                                                     random.NextIpV6MobilityOptions());

                case IpV6MobilityHeaderType.FastNeighborAdvertisement: // 10
                    return new IpV6ExtensionHeaderMobilityFastNeighborAdvertisement(nextHeader, checksum, random.NextIpV6MobilityOptions());

                case IpV6MobilityHeaderType.Experimental: // 11
                    return new IpV6ExtensionHeaderMobilityExperimental(nextHeader, checksum, random.NextDataSegment(2 + random.Next(10) * 8));

                case IpV6MobilityHeaderType.HomeAgentSwitchMessage: // 12
                    return new IpV6ExtensionHeaderMobilityHomeAgentSwitchMessage(nextHeader, checksum, random.NextIpV6AddressArray(random.NextInt(0, 10)),
                                                                                 random.NextIpV6MobilityOptions());

                case IpV6MobilityHeaderType.HeartbeatMessage: // 13
                    return new IpV6ExtensionHeaderMobilityHeartbeatMessage(nextHeader, checksum, random.NextBool(), random.NextBool(), random.NextUInt(),
                                                                           random.NextIpV6MobilityOptions());

                case IpV6MobilityHeaderType.HandoverInitiateMessage: // 14
                    return new IpV6ExtensionHeaderMobilityHandoverInitiateMessage(nextHeader, checksum, random.NextUShort(), random.NextBool(),
                                                                                  random.NextBool(), random.NextEnum<IpV6HandoverInitiateMessageCode>(),
                                                                                  random.NextIpV6MobilityOptions());

                case IpV6MobilityHeaderType.HandoverAcknowledgeMessage: // 15
                    return new IpV6ExtensionHeaderMobilityHandoverAcknowledgeMessage(nextHeader, checksum, random.NextUShort(),
                                                                                     random.NextEnum<IpV6MobilityHandoverAcknowledgeCode>(),
                                                                                     random.NextIpV6MobilityOptions());

                case IpV6MobilityHeaderType.BindingRevocationMessage: // 16
                    IpV6MobilityBindingRevocationType bindingRevocationType = random.NextEnum(IpV6MobilityBindingRevocationType.None);
                    byte revocationTriggerOrStatus = random.NextByte();
                    ushort sequenceNumber = random.NextUShort();
                    bool proxyBinding = random.NextBool();
                    bool ipV4HomeAddressBindingOnly = random.NextBool();
                    bool global = random.NextBool();
                    IpV6MobilityOptions options = random.NextIpV6MobilityOptions();
                    switch (bindingRevocationType)
                    {
                        case IpV6MobilityBindingRevocationType.BindingRevocationIndication:
                            return new IpV6ExtensionHeaderMobilityBindingRevocationIndicationMessage(
                                nextHeader, checksum, (Ipv6MobilityBindingRevocationTrigger)revocationTriggerOrStatus, sequenceNumber, proxyBinding,
                                ipV4HomeAddressBindingOnly, global, options);

                        case IpV6MobilityBindingRevocationType.BindingRevocationAcknowledgement:
                            return new IpV6ExtensionHeaderMobilityBindingRevocationAcknowledgementMessage(
                                nextHeader, checksum, (Ipv6MobilityBindingRevocationStatus)revocationTriggerOrStatus, sequenceNumber, proxyBinding,
                                ipV4HomeAddressBindingOnly, global, options);

                        default:
                            throw new InvalidOperationException(string.Format("Invalid bindingRevocationType value {0}", bindingRevocationType));

                    }

                case IpV6MobilityHeaderType.LocalizedRoutingInitiation: // 17
                    return new IpV6ExtensionHeaderMobilityLocalizedRoutingInitiation(nextHeader, checksum, random.NextUShort(), random.NextUShort(),
                                                                                     random.NextIpV6MobilityOptions());

                case IpV6MobilityHeaderType.LocalizedRoutingAcknowledgement: // 18
                    return new IpV6ExtensionHeaderMobilityLocalizedRoutingAcknowledgement(nextHeader, checksum, random.NextUShort(), random.NextBool(),
                                                                                          random.NextEnum<IpV6MobilityLocalizedRoutingAcknowledgementStatus>(),
                                                                                          random.NextUShort(), random.NextIpV6MobilityOptions());

                default:
                    throw new InvalidOperationException(string.Format("Invalid mobilityHeaderType value {0}", mobilityHeaderType));
            }

        }

        public static IpV6Options NextIpV6Options(this Random random)
        {
            return new IpV6Options(((Func<IpV6Option>)(random.NextIpV6Option)).GenerateArray(random.NextInt(0, 10)));
        }

        public static IpV6Option NextIpV6Option(this Random random)
        {
            IpV6OptionType optionType = random.NextEnum<IpV6OptionType>();
            switch (optionType)
            {
                case IpV6OptionType.Pad1:
                    return new IpV6OptionPad1();

                case IpV6OptionType.PadN:
                    return new IpV6OptionPadN(random.Next(10));

                case IpV6OptionType.JumboPayload:
                    return new IpV6OptionJumboPayload(random.NextUInt());

                case IpV6OptionType.TunnelEncapsulationLimit:
                    return new IpV6OptionTunnelEncapsulationLimit(random.NextByte());

                case IpV6OptionType.RouterAlert:
                    return new IpV6OptionRouterAlert(random.NextEnum<IpV6RouterAlertType>());

                case IpV6OptionType.QuickStart:
                    return new IpV6OptionQuickStart(random.NextEnum<IpV4OptionQuickStartFunction>(),
                                                    random.NextByte(IpOptionQuickStartCommon.RateMaximumValue + 1), random.NextByte(),
                                                    random.NextUInt() & 0xFFFFFFFC);

                case IpV6OptionType.Calipso:
                    return new IpV6OptionCalipso(random.NextEnum<IpV6CalipsoDomainOfInterpretation>(), random.NextByte(), random.NextUShort(),
                                                 random.NextDataSegment(random.NextInt(0, IpV6OptionCalipso.CompartmentBitmapMaxLength + 1) / 4 * sizeof(int)));

                case IpV6OptionType.SimplifiedMulticastForwardingDuplicatePacketDetection:
                    if (random.NextBool())
                        return new IpV6OptionSimplifiedMulticastForwardingDuplicatePacketDetectionSequenceHashAssistValue(random.NextDataSegment(random.NextInt(1, 100)));
                    IpV6TaggerIdType taggerIdType = random.NextEnum<IpV6TaggerIdType>();
                    DataSegment identifier = random.NextDataSegment(random.NextInt(0, 100));
                    switch (taggerIdType)
                    {
                        case IpV6TaggerIdType.Null:
                            return new IpV6OptionSimplifiedMulticastForwardingDuplicatePacketDetectionNull(identifier);

                        case IpV6TaggerIdType.Default:
                            return new IpV6OptionSimplifiedMulticastForwardingDuplicatePacketDetectionDefault(random.NextDataSegment(random.NextInt(1, 17)), identifier);

                        case IpV6TaggerIdType.IpV4:
                            return new IpV6OptionSimplifiedMulticastForwardingDuplicatePacketDetectionIpV4(random.NextIpV4Address(), identifier);

                        case IpV6TaggerIdType.IpV6:
                            return new IpV6OptionSimplifiedMulticastForwardingDuplicatePacketDetectionIpV6(random.NextIpV6Address(), identifier);

                        default:
                            throw new InvalidOperationException(string.Format("Invalid taggerIdType value {0}", taggerIdType));
                    }

                case IpV6OptionType.HomeAddress:
                    return new IpV6OptionHomeAddress(random.NextIpV6Address());

                case IpV6OptionType.EndpointIdentification:
                    return new IpV6OptionEndpointIdentification(random.NextDataSegment(random.Next(10)), random.NextDataSegment(random.Next(10)));

                case IpV6OptionType.RoutingProtocolLowPowerAndLossyNetworksOption:
                    return new IpV6OptionRoutingProtocolLowPowerAndLossyNetworks(random.NextBool(), random.NextBool(), random.NextBool(), random.NextByte(),
                                                                                 random.NextUShort(), random.NextDataSegment(random.Next(10)));

                case IpV6OptionType.IdentifierLocatorNetworkProtocolNonce:
                    return new IpV6OptionIdentifierLocatorNetworkProtocolNonce(random.NextDataSegment(random.Next(10)));

                case IpV6OptionType.LineIdentification:
                    return new IpV6OptionLineIdentificationDestination(random.NextDataSegment(random.Next(10)));

                default:
                    throw new InvalidOperationException(string.Format("Invalid optionType value {0}", optionType));
            }
        }

        public static IpV6MobilityOptions NextIpV6MobilityOptions(this Random random)
        {
            return new IpV6MobilityOptions(((Func<IpV6MobilityOption>)(random.NextIpV6MobilityOption)).GenerateArray(random.NextInt(0, 10)));
        }

        public static IpV6MobilityOption NextIpV6MobilityOption(this Random random)
        {
            IpV6MobilityOptionType optionType = random.NextEnum<IpV6MobilityOptionType>();
            switch (optionType)
            {
                case IpV6MobilityOptionType.Pad1:
                    return new IpV6MobilityOptionPad1();

                case IpV6MobilityOptionType.PadN:
                    return new IpV6MobilityOptionPadN(random.NextInt(0, 10));

                case IpV6MobilityOptionType.BindingRefreshAdvice:
                    return new IpV6MobilityOptionBindingRefreshAdvice(random.NextUShort());

                case IpV6MobilityOptionType.AlternateCareOfAddress:
                    return new IpV6MobilityOptionAlternateCareOfAddress(random.NextIpV6Address());

                case IpV6MobilityOptionType.NonceIndexes:
                    return new IpV6MobilityOptionNonceIndexes(random.NextUShort(), random.NextUShort());

                case IpV6MobilityOptionType.BindingAuthorizationData:
                    return new IpV6MobilityOptionBindingAuthorizationData(random.NextDataSegment(random.NextInt(0, 100)));

                case IpV6MobilityOptionType.MobileNetworkPrefix:
                    return new IpV6MobilityOptionMobileNetworkPrefix(random.NextByte(), random.NextIpV6Address());

                case IpV6MobilityOptionType.LinkLayerAddress:
                    return new IpV6MobilityOptionLinkLayerAddress(random.NextEnum<IpV6MobilityLinkLayerAddressCode>(),
                                                                  random.NextDataSegment(random.NextInt(0, 100)));

                case IpV6MobilityOptionType.MobileNodeIdentifier:
                    IpV6MobileNodeIdentifierSubtype mobileNodeIdentifierSubtype = random.NextEnum<IpV6MobileNodeIdentifierSubtype>();
                    return new IpV6MobilityOptionMobileNodeIdentifier(
                        mobileNodeIdentifierSubtype,
                        random.NextDataSegment(random.NextInt(mobileNodeIdentifierSubtype == IpV6MobileNodeIdentifierSubtype.NetworkAccessIdentifier ? 1 : 0,
                                                              100)));

                case IpV6MobilityOptionType.Authentication:
                    return new IpV6MobilityOptionAuthentication(random.NextEnum<IpV6AuthenticationSubtype>(), random.NextUInt(),
                                                                random.NextDataSegment(random.NextInt(0, 100)));

                case IpV6MobilityOptionType.ReplayProtection:
                    return new IpV6MobilityOptionReplayProtection(random.NextULong());

                case IpV6MobilityOptionType.CryptographicallyGeneratedAddressParametersRequest:
                    return new IpV6MobilityOptionCryptographicallyGeneratedAddressParametersRequest();

                case IpV6MobilityOptionType.CryptographicallyGeneratedAddressParameters:
                    return new IpV6MobilityOptionCryptographicallyGeneratedAddressParameters(random.NextDataSegment(random.NextInt(0, 100)));

                case IpV6MobilityOptionType.Signature:
                    return new IpV6MobilityOptionSignature(random.NextDataSegment(random.NextInt(0, 100)));

                case IpV6MobilityOptionType.PermanentHomeKeygenToken:
                    return new IpV6MobilityOptionPermanentHomeKeygenToken(random.NextDataSegment(random.NextInt(0, 100)));

                case IpV6MobilityOptionType.CareOfTestInit:
                    return new IpV6MobilityOptionCareOfTestInit();

                case IpV6MobilityOptionType.CareOfTest:
                    return new IpV6MobilityOptionCareOfTest(random.NextULong());

                case IpV6MobilityOptionType.DnsUpdate:
                    return new IpV6MobilityOptionDnsUpdate(random.NextEnum<IpV6DnsUpdateStatus>(), random.NextBool(),
                                                           random.NextDataSegment(random.NextInt(0, 100)));

                case IpV6MobilityOptionType.Experimental:
                    return new IpV6MobilityOptionExperimental(random.NextDataSegment(random.NextInt(0, 100)));

                case IpV6MobilityOptionType.VendorSpecific:
                    return new IpV6MobilityOptionVendorSpecific(random.NextUInt(), random.NextByte(), random.NextDataSegment(random.NextInt(0, 100)));

                case IpV6MobilityOptionType.ServiceSelection:
                    return new IpV6MobilityOptionServiceSelection(random.NextDataSegment(random.NextInt(1, 100)));

                case IpV6MobilityOptionType.BindingAuthorizationDataForFmIpV6:
                    return new IpV6MobilityOptionBindingAuthorizationDataForFmIpV6(random.NextUInt(), random.NextDataSegment(random.NextInt(0, 100)));

                case IpV6MobilityOptionType.HomeNetworkPrefix:
                    return new IpV6MobilityOptionHomeNetworkPrefix(random.NextByte(), random.NextIpV6Address());

                case IpV6MobilityOptionType.HandoffIndicator:
                    return new IpV6MobilityOptionHandoffIndicator(random.NextEnum<IpV6HandoffIndicator>());

                case IpV6MobilityOptionType.AccessTechnologyType:
                    return new IpV6MobilityOptionAccessTechnologyType(random.NextEnum<IpV6AccessTechnologyType>());

                case IpV6MobilityOptionType.MobileNodeLinkLayerIdentifier:
                    return new IpV6MobilityOptionMobileNodeLinkLayerIdentifier(random.NextDataSegment(random.NextInt(0, 100)));

                case IpV6MobilityOptionType.LinkLocalAddress:
                    return new IpV6MobilityOptionLinkLocalAddress(random.NextIpV6Address());

                case IpV6MobilityOptionType.Timestamp:
                    return new IpV6MobilityOptionTimestamp(random.NextULong() % (500000000000L << 16));

                case IpV6MobilityOptionType.RestartCounter:
                    return new IpV6MobilityOptionRestartCounter(random.NextUInt());

                case IpV6MobilityOptionType.IpV4HomeAddress:
                    return new IpV6MobilityOptionIpV4HomeAddress(random.NextByte(IpV6MobilityOptionIpV4HomeAddress.MaxPrefixLength + 1), random.NextBool(),
                                                                 random.NextIpV4Address());

                case IpV6MobilityOptionType.IpV4AddressAcknowledgement:
                    return new IpV6MobilityOptionIpV4AddressAcknowledgement(random.NextEnum<IpV6AddressAcknowledgementStatus>(),
                                                                            random.NextByte(IpV6MobilityOptionIpV4AddressAcknowledgement.MaxPrefixLength + 1),
                                                                            random.NextIpV4Address());

                case IpV6MobilityOptionType.NatDetection:
                    return new IpV6MobilityOptionNatDetection(random.NextBool(), random.NextUInt());

                case IpV6MobilityOptionType.IpV4CareOfAddress:
                    return new IpV6MobilityOptionIpV4CareOfAddress(random.NextIpV4Address());

                case IpV6MobilityOptionType.GreKey:
                    return new IpV6MobilityOptionGreKey(random.NextUInt());

                case IpV6MobilityOptionType.IpV6AddressPrefix:
                    return new IpV6MobilityOptionIpV6AddressPrefix(random.NextEnum<IpV6MobilityIpV6AddressPrefixCode>(),
                                                                   random.NextByte(IpV6MobilityOptionIpV6AddressPrefix.MaxPrefixLength + 1),
                                                                   random.NextIpV6Address());

                case IpV6MobilityOptionType.BindingIdentifier:
                    ushort bindingId = random.NextUShort();
                    IpV6BindingAcknowledgementStatus status = random.NextEnum<IpV6BindingAcknowledgementStatus>();
                    bool simultaneousHomeAndForeignBinding = random.NextBool();
                    byte priority = random.NextByte(IpV6MobilityOptionBindingIdentifier.MaxPriority + 1);
                    if (random.NextBool(3))
                        return new IpV6MobilityOptionBindingIdentifier(bindingId, status, simultaneousHomeAndForeignBinding, priority);
                    if (random.NextBool())
                        return new IpV6MobilityOptionBindingIdentifier(bindingId, status, simultaneousHomeAndForeignBinding, priority, random.NextIpV4Address());
                    return new IpV6MobilityOptionBindingIdentifier(bindingId, status, simultaneousHomeAndForeignBinding, priority, random.NextIpV6Address());
                    
                case IpV6MobilityOptionType.IpV4HomeAddressRequest:
                    return new IpV6MobilityOptionIpV4HomeAddressRequest(random.NextByte(IpV6MobilityOptionIpV4HomeAddressRequest.MaxPrefixLength + 1),
                                                                        random.NextIpV4Address());

                case IpV6MobilityOptionType.IpV4HomeAddressReply:
                    return new IpV6MobilityOptionIpV4HomeAddressReply(random.NextEnum<IpV6IpV4HomeAddressReplyStatus>(),
                                                                      random.NextByte(IpV6MobilityOptionIpV4HomeAddressReply.MaxPrefixLength + 1),
                                                                      random.NextIpV4Address());

                case IpV6MobilityOptionType.IpV4DefaultRouterAddress:
                    return new IpV6MobilityOptionIpV4DefaultRouterAddress(random.NextIpV4Address());

                case IpV6MobilityOptionType.IpV4DhcpSupportMode:
                    return new IpV6MobilityOptionIpV4DhcpSupportMode(random.NextBool());

                case IpV6MobilityOptionType.ContextRequest:
                    return
                        new IpV6MobilityOptionContextRequest(
                            ((Func<IpV6MobilityOptionContextRequestEntry>)(() => new IpV6MobilityOptionContextRequestEntry(
                                                                                     random.NextByte(), random.NextDataSegment(random.NextInt(0, 25))))).
                                GenerateArray(random.NextInt(0, 10)));

                case IpV6MobilityOptionType.LocalMobilityAnchorAddress:
                    if (random.NextBool())
                        return new IpV6MobilityOptionLocalMobilityAnchorAddress(random.NextIpV4Address());
                    return new IpV6MobilityOptionLocalMobilityAnchorAddress(random.NextIpV6Address());

                case IpV6MobilityOptionType.MobileNodeLinkLocalAddressInterfaceIdentifier:
                    return new IpV6MobilityOptionMobileNodeLinkLocalAddressInterfaceIdentifier(random.NextULong());

                case IpV6MobilityOptionType.TransientBinding:
                    return new IpV6MobilityOptionTransientBinding(random.NextBool(), random.NextByte());

                case IpV6MobilityOptionType.FlowSummary:
                    return new IpV6MobilityOptionFlowSummary(((Func<ushort>)(random.NextUShort)).GenerateArray(random.NextInt(1, 10)));

                case IpV6MobilityOptionType.FlowIdentification:
                    return new IpV6MobilityOptionFlowIdentification(random.NextUShort(), random.NextUShort(), random.NextEnum<IpV6FlowIdentificationStatus>(),
                                                                    random.NextIpV6FlowIdentificationSubOptions());

                case IpV6MobilityOptionType.RedirectCapability:
                    return new IpV6MobilityOptionRedirectCapability();

                case IpV6MobilityOptionType.Redirect:
                    if (random.NextBool())
                        return new IpV6MobilityOptionRedirect(random.NextIpV4Address());
                    return new IpV6MobilityOptionRedirect(random.NextIpV6Address());

                case IpV6MobilityOptionType.LoadInformation:
                    return new IpV6MobilityOptionLoadInformation(random.NextUShort(), random.NextUInt(), random.NextUInt(), random.NextUInt(), random.NextUInt());

                case IpV6MobilityOptionType.AlternateIpV4CareOfAddress:
                    return new IpV6MobilityOptionAlternateIpV4CareOfAddress(random.NextIpV4Address());

                case IpV6MobilityOptionType.MobileNodeGroupIdentifier:
                    return new IpV6MobilityOptionMobileNodeGroupIdentifier(random.NextEnum<IpV6MobileNodeGroupIdentifierSubtype>(), random.NextUInt());

                case IpV6MobilityOptionType.MobileAccessGatewayIpV6Address:
                    return new IpV6MobilityOptionMobileAccessGatewayIpV6Address(random.NextIpV6Address());

                case IpV6MobilityOptionType.AccessNetworkIdentifier:
                    return new IpV6MobilityOptionAccessNetworkIdentifier(random.NextIpV6AccessNetworkIdentifierSubOptions());

                default:
                    throw new InvalidOperationException(string.Format("Invalid optionType value {0}", optionType));
            }
        }

        public static IpV6FlowIdentificationSubOptions NextIpV6FlowIdentificationSubOptions(this Random random)
        {
            return new IpV6FlowIdentificationSubOptions(((Func<IpV6FlowIdentificationSubOption>)(random.NextIpV6FlowIdentificationSubOption)).GenerateArray(random.NextInt(0, 9)));
        }

        public static IpV6FlowIdentificationSubOption NextIpV6FlowIdentificationSubOption(this Random random)
        {
            IpV6FlowIdentificationSubOptionType optionType = random.NextEnum<IpV6FlowIdentificationSubOptionType>();
            switch (optionType)
            {
                case IpV6FlowIdentificationSubOptionType.Pad1:
                    return new IpV6FlowIdentificationSubOptionPad1();

                case IpV6FlowIdentificationSubOptionType.PadN:
                    return new IpV6FlowIdentificationSubOptionPadN(random.NextInt(0, 10));

                case IpV6FlowIdentificationSubOptionType.BindingReference:
                    return new IpV6FlowIdentificationSubOptionBindingReference(((Func<ushort>)(random.NextUShort)).GenerateArray(random.NextInt(0, 10)));

                case IpV6FlowIdentificationSubOptionType.TrafficSelector:
                    return new IpV6FlowIdentificationSubOptionTrafficSelector(random.NextEnum<IpV6FlowIdentificationTrafficSelectorFormat>(), random.NextDataSegment(random.NextInt(0, 40)));

                default:
                    throw new InvalidOperationException(string.Format("Invalid optionType value {0}", optionType));
            }
        }

        public static IpV6AccessNetworkIdentifierSubOptions NextIpV6AccessNetworkIdentifierSubOptions(this Random random)
        {
            List<IpV6AccessNetworkIdentifierSubOption> subOptions = new List<IpV6AccessNetworkIdentifierSubOption>();
            int numOptions = random.NextInt(0, 10);
            int optionsBytesLength = 0;
            for (int i = 0; i != numOptions; ++i)
            {
                IpV6AccessNetworkIdentifierSubOption subOption = random.NextIpV6AccessNetworkIdentifierSubOption();
                if (optionsBytesLength + subOption.Length > byte.MaxValue - 2)
                    break;
                subOptions.Add(subOption);
                optionsBytesLength += subOption.Length;
            }
            return new IpV6AccessNetworkIdentifierSubOptions(subOptions);
        }

        public static IpV6AccessNetworkIdentifierSubOption NextIpV6AccessNetworkIdentifierSubOption(this Random random)
        {
            IpV6AccessNetworkIdentifierSubOptionType optionType = random.NextEnum(IpV6AccessNetworkIdentifierSubOptionType.None);
            switch (optionType)
            {
                case IpV6AccessNetworkIdentifierSubOptionType.NetworkIdentifier:
                    return new IpV6AccessNetworkIdentifierSubOptionNetworkIdentifier(random.NextBool(), random.NextDataSegment(random.NextInt(0, 100)),
                                                                                     random.NextDataSegment(random.NextInt(0, 100)));

                case IpV6AccessNetworkIdentifierSubOptionType.GeoLocation:
                    double latitude = random.NextDouble() * 180 - 90;
                    double longitude = random.NextDouble() * 360 - 180;
                    return IpV6AccessNetworkIdentifierSubOptionGeoLocation.CreateFromRealValues(latitude, longitude);

                case IpV6AccessNetworkIdentifierSubOptionType.OperatorIdentifier:
                    return new IpV6AccessNetworkIdentifierSubOptionOperatorIdentifier(random.NextEnum<IpV6AccessNetworkIdentifierOperatorIdentifierType>(),
                                                                                      random.NextDataSegment(random.NextInt(0, 100)));

                default:
                    throw new InvalidOperationException(string.Format("Invalid optionType value {0}", optionType));
            }
        }
    }
}