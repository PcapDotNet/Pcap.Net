using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Core.Test
{
    [ExcludeFromCodeCoverage]
    internal class WiresharkDatagramComparerIpV6 : WiresharkDatagramComparerSimple
    {
        public WiresharkDatagramComparerIpV6()
        {
        }

        protected override string PropertyName
        {
            get { return "IpV6"; }
        }

        protected override bool CompareField(XElement field, Datagram datagram)
        {
            IpV6Datagram ipV6Datagram = (IpV6Datagram)datagram;
            SkipAuthenticationHeaders(ipV6Datagram);
            int optionsIndex = 0;
            switch (field.Name())
            {
                case "ipv6.version":
                    field.AssertShowDecimal(ipV6Datagram.Version);
                    foreach (XElement subfield in field.Fields())
                    {
                        switch (subfield.Name())
                        {
                            case "ip.version":
                                subfield.AssertShowDecimal(ipV6Datagram.Version);
                                break;

                            default:
                                throw new InvalidOperationException(string.Format("Invalid ipv6 version subfield {0}", subfield.Name()));
                        }
                    }
                    break;

                case "ipv6.class":
                    field.AssertShowDecimal(ipV6Datagram.TrafficClass);
                    break;

                case "ipv6.flow":
                    field.AssertShowDecimal(ipV6Datagram.FlowLabel);
                    field.AssertNoFields();
                    break;

                case "ipv6.plen":
                    field.AssertShowDecimal(ipV6Datagram.PayloadLength);
                    field.AssertNoFields();
                    break;

                case "ipv6.nxt":
                    field.AssertShowDecimal((byte)ipV6Datagram.NextHeader);
                    field.AssertNoFields();
                    break;

                case "ipv6.hlim":
                    field.AssertShowDecimal(ipV6Datagram.HopLimit);
                    field.AssertNoFields();
                    break;

                case "ipv6.src":
                case "ipv6.src_host":
                    field.AssertShow(ipV6Datagram.Source.GetWiresharkString());
                    field.AssertNoFields();
                    break;

                case "ipv6.src_6to4_gw_ipv4":
                case "ipv6.src_6to4_sla_id":
                case "ipv6.6to4_gw_ipv4":
                case "ipv6.6to4_sla_id":
                    field.AssertNoFields();
                    break;

                case "ipv6.dst":
                case "ipv6.dst_host":
                    field.AssertShow(ipV6Datagram.CurrentDestination.GetWiresharkString());
                    field.AssertNoFields();
                    break;

                case "ipv6.addr":
                case "ipv6.host":
                    Assert.IsTrue(field.Show() == ipV6Datagram.Source.GetWiresharkString() ||
                                  field.Show() == ipV6Datagram.CurrentDestination.GetWiresharkString());
                    field.AssertNoFields();
                    break;

                case "ipv6.hop_opt":
                    if (_currentExtensionHeaderIndex >= ipV6Datagram.ExtensionHeaders.Headers.Count)
                    {
                        Assert.IsFalse(ipV6Datagram.ExtensionHeaders.IsValid);
                        int maxLength = ipV6Datagram.Length - IpV6Datagram.HeaderLength - ipV6Datagram.ExtensionHeaders.BytesLength;
                        if (field.Fields().Any(subfield => subfield.Name() == "ipv6.opt.length"))
                        {
                            int length = int.Parse(field.Fields().First(subfield => subfield.Name() == "ipv6.opt.length").Show());
                            MoreAssert.IsBigger(maxLength, length);
                        }
                        else 
                        {
                            Assert.AreEqual(6, maxLength);
                        }
                    }
                    else
                    {
                        IpV6ExtensionHeaderHopByHopOptions hopByHopOptions =
                            (IpV6ExtensionHeaderHopByHopOptions)ipV6Datagram.ExtensionHeaders[_currentExtensionHeaderIndex];
                        IncrementCurrentExtensionHeaderIndex(ipV6Datagram);
                        CompareOptions(field, ref optionsIndex, ipV6Datagram, hopByHopOptions);
                    }
                    break;

                case "ipv6.routing_hdr":
                    if (!ipV6Datagram.IsValid)
                        return false;
                    IpV6ExtensionHeaderRouting routing = (IpV6ExtensionHeaderRouting)ipV6Datagram.ExtensionHeaders[_currentExtensionHeaderIndex];
                        IpV6ExtensionHeaderRoutingProtocolLowPowerAndLossyNetworks routingProtocolLowPowerAndLossyNetworks =
                            routing as IpV6ExtensionHeaderRoutingProtocolLowPowerAndLossyNetworks;
                    int routingProtocolLowPowerAndLossyNetworksAddressIndex = 0;
                    IncrementCurrentExtensionHeaderIndex(ipV6Datagram);
                    int sourceRouteAddressIndex = 0;
                    foreach (var headerField in field.Fields())
                    {
                        switch (headerField.Name())
                        {
                            case "":
                                headerField.AssertNoFields();
                                ValidateExtensionHeaderUnnamedField(routing, headerField);
                                break;

                            case "ipv6.routing_hdr.type":
                                headerField.AssertNoFields();
                                headerField.AssertShowDecimal((byte)routing.RoutingType);
                                break;

                            case "ipv6.routing_hdr.left":
                                headerField.AssertNoFields();
                                headerField.AssertShowDecimal(routing.SegmentsLeft);
                                break;

                            case "ipv6.mipv6_home_address":
                                headerField.AssertNoFields();
                                IpV6ExtensionHeaderRoutingHomeAddress routingHomeAddress = (IpV6ExtensionHeaderRoutingHomeAddress)routing;
                                headerField.AssertShow(routingHomeAddress.HomeAddress.ToString("x"));
                                break;

                            case "ipv6.routing_hdr.addr":
                                headerField.AssertNoFields();
                                IpV6ExtensionHeaderRoutingSourceRoute routingSourceRoute = (IpV6ExtensionHeaderRoutingSourceRoute)routing;
                                headerField.AssertShow(routingSourceRoute.Addresses[sourceRouteAddressIndex++].ToString("x"));
                                break;

                            case "ipv6.routing_hdr.rpl.cmprI":
                                headerField.AssertNoFields();
                                headerField.AssertShowDecimal(routingProtocolLowPowerAndLossyNetworks.CommonPrefixLengthForNonLastAddresses);
                                break;

                            case "ipv6.routing_hdr.rpl.cmprE":
                                headerField.AssertNoFields();
                                headerField.AssertShowDecimal(routingProtocolLowPowerAndLossyNetworks.CommonPrefixLengthForLastAddress);
                                break;

                            case "ipv6.routing_hdr.rpl.pad":
                                headerField.AssertNoFields();
                                headerField.AssertShowDecimal(routingProtocolLowPowerAndLossyNetworks.PadSize);
                                break;

                            case "ipv6.routing_hdr.rpl.reserved":
                                headerField.AssertNoFields();
                                headerField.AssertShowDecimal(0);
                                break;

                            case "ipv6.routing_hdr.rpl.segments":
                                if (headerField.Fields().Any())
                                {
                                    headerField.AssertNumFields(1);
                                    headerField.Fields().First().AssertName("_ws.expert");
                                }
                                // TODO: Uncomment when https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10560 is fixed.
//                                headerField.AssertShowDecimal(routingProtocolLowPowerAndLossyNetworks.Addresses.Count);
                                break;

                            case "ipv6.routing_hdr.rpl.address":
                                headerField.AssertNoFields();
                                // TODO: Implement when https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10560 is fixed.
                                break;

                            case "ipv6.routing_hdr.rpl.full_address":
                                headerField.AssertNoFields();
                                // TODO: Implement when https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10673 is fixed.
                                ++routingProtocolLowPowerAndLossyNetworksAddressIndex;
                                break;

                            default:
                                throw new InvalidOperationException("Invalid IPv6 routing source route field " + headerField.Name());
                        }
                    }
                    break;

                case "ipv6.dst_opt":
                    if (_currentExtensionHeaderIndex >= ipV6Datagram.ExtensionHeaders.Headers.Count)
                    {
                        int expectedExtensionHeaderLength = (int.Parse(field.Fields().Skip(1).First().Value(), NumberStyles.HexNumber) + 1) * 8;
                        int actualMaxPossibleLength = ipV6Datagram.RealPayloadLength -
                                                   ipV6Datagram.ExtensionHeaders.Take(_currentExtensionHeaderIndex).Sum(
                                                       extensionHeader => extensionHeader.Length);
                        MoreAssert.IsSmaller(expectedExtensionHeaderLength, actualMaxPossibleLength);
                        return false;
                    }
                    IpV6ExtensionHeaderDestinationOptions destinationOptions = (IpV6ExtensionHeaderDestinationOptions)ipV6Datagram.ExtensionHeaders[_currentExtensionHeaderIndex];
                    IncrementCurrentExtensionHeaderIndex(ipV6Datagram);
                    CompareOptions(field, ref optionsIndex, ipV6Datagram, destinationOptions);
                    break;

                case "ipv6.shim6":
                    // TODO: Implement Shim6.
                    IpV4Protocol nextHeader = _currentExtensionHeaderIndex > 0
                                                  ? ipV6Datagram.ExtensionHeaders[_currentExtensionHeaderIndex - 1].NextHeader.Value
                                                  : ipV6Datagram.NextHeader;
                    Assert.IsTrue(nextHeader == IpV4Protocol.Shim6);
                    break;

                case "ipv6.unknown_hdr":
                    Assert.AreEqual(ipV6Datagram.ExtensionHeaders.Count(), _currentExtensionHeaderIndex);
                    // TODO: Fix according to https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=9996
                    return false;

                case "ipv6.src_sa_mac":
                case "ipv6.dst_sa_mac":
                case "ipv6.sa_mac":
                case "ipv6.dst_6to4_gw_ipv4":
                case "ipv6.dst_6to4_sla_id":
                    // TODO: Understand how these are calculated.
                    break;

                case "":
                    switch (field.Show())
                    {
                        case "Fragmentation Header":
                            if (_currentExtensionHeaderIndex >= ipV6Datagram.ExtensionHeaders.Headers.Count && !ipV6Datagram.IsValid)
                                return false;
                            IpV6ExtensionHeaderFragmentData fragmentData =
                                (IpV6ExtensionHeaderFragmentData)ipV6Datagram.ExtensionHeaders[_currentExtensionHeaderIndex];
                            IncrementCurrentExtensionHeaderIndex(ipV6Datagram);
                            foreach (var headerField in field.Fields())
                            {
                                switch (headerField.Name())
                                {
                                    case "ipv6.fragment.nxt":
                                        headerField.AssertValue((byte)fragmentData.NextHeader.Value);
                                        break;

                                    case "ipv6.fragment.offset":
                                        headerField.AssertShowDecimal(fragmentData.FragmentOffset);
                                        break;

                                    case "ipv6.fragment.more":
                                        headerField.AssertShowDecimal(fragmentData.MoreFragments);
                                        break;

                                    case "ipv6.fragment.id":
                                        headerField.AssertShowDecimal(fragmentData.Identification);
                                        break;

                                    case "ipv6.fragment.reserved_octet":
                                    case "ipv6.fragment.reserved_bits":
                                        headerField.AssertShowDecimal(0);
                                        break;

                                    default:
                                        throw new InvalidOperationException("Invalid ipv6 fragmentation field " + headerField.Name());
                                }
                            }
                            break;

                        default:
                            throw new InvalidOperationException(string.Format("Invalid ipv6 field {0}", field.Show()));
                    }
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Invalid ipv6 field {0}", field.Name()));
            }

            return true;
        }

        private void CompareOptions(XElement field, ref int optionsIndex, IpV6Datagram ipV6Datagram, IpV6ExtensionHeaderOptions header)
        {
            foreach (var headerField in field.Fields())
            {
                switch (headerField.Name())
                {
                    case "ipv6.nxt":
                        headerField.AssertNoFields();
                        headerField.AssertShowDecimal((byte)header.NextHeader);
                        break;

                    case "ipv6.opt.length":
                        headerField.AssertNoFields();
                        headerField.AssertShowDecimal((header.Length - 8) / 8);
                        break;

                    case "ipv6.opt":
                        foreach (XElement headerSubfield in headerField.Fields())
                        {
                            IpV6Option option = header.Options[optionsIndex];
                            var optionCalipso = option as IpV6OptionCalipso;
                            var optionQuickStart = option as IpV6OptionQuickStart;
                            switch (headerSubfield.Name())
                            {
                                case "ipv6.opt.type":
                                    headerSubfield.AssertNoFields();
                                    headerSubfield.AssertShowDecimal((byte)option.OptionType);
                                    break;

                                case "ipv6.opt.length":
                                    headerSubfield.AssertNoFields();
                                    headerSubfield.AssertShowDecimal(option.Length - 2);
                                    break;

                                case "ipv6.opt.tel":
                                    headerSubfield.AssertNoFields();
                                    IpV6OptionTunnelEncapsulationLimit optionTunnelEncapsulationLimit = (IpV6OptionTunnelEncapsulationLimit)option;
                                    headerSubfield.AssertShowDecimal(optionTunnelEncapsulationLimit.TunnelEncapsulationLimit);
                                    ++optionsIndex;
                                    break;

                                case "ipv6.opt.rpl.flag":
                                    IpV6OptionRoutingProtocolLowPowerAndLossyNetworks optionRoutingProtocolLowPowerAndLossyNetworks =
                                        (IpV6OptionRoutingProtocolLowPowerAndLossyNetworks)option;
                                    foreach (XElement optionSubfield in headerSubfield.Fields())
                                    {
                                        optionSubfield.AssertNoFields();
                                        switch (optionSubfield.Name())
                                        {
                                            case "ipv6.opt.rpl.flag.o":
                                                optionSubfield.AssertShowDecimal(optionRoutingProtocolLowPowerAndLossyNetworks.Down);
                                                break;

                                            case "ipv6.opt.rpl.flag.r":
                                                optionSubfield.AssertShowDecimal(optionRoutingProtocolLowPowerAndLossyNetworks.RankError);
                                                break;

                                            case "ipv6.opt.rpl.flag.f":
                                                optionSubfield.AssertShowDecimal(optionRoutingProtocolLowPowerAndLossyNetworks.ForwardingError);
                                                break;

                                            case "ipv6.opt.rpl.flag.rsv":
                                                optionSubfield.AssertShowDecimal(0);
                                                break;

                                            case "ipv6.opt.rpl.instance_id":
                                                optionSubfield.AssertShowDecimal(
                                                    optionRoutingProtocolLowPowerAndLossyNetworks.RoutingProtocolLowPowerAndLossyNetworksInstanceId);
                                                break;

                                            case "ipv6.opt.rpl.sender_rank":
                                                optionSubfield.AssertShowDecimal(optionRoutingProtocolLowPowerAndLossyNetworks.SenderRank);
                                                break;

                                            default:
                                                throw new InvalidOperationException("Invalid ipv6 option subfield " + optionSubfield.Name());
                                        }
                                    }
                                    ++optionsIndex;
                                    // TODO: change to break; after https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10559 is fixed.
                                    return; 

                                case "ipv6.opt.calipso.doi":
                                    headerSubfield.AssertNoFields();
                                    headerSubfield.AssertShowDecimal((uint)optionCalipso.DomainOfInterpretation);
                                    break;

                                case "ipv6.opt.calipso.cmpt.length":
                                    headerSubfield.AssertNoFields();
                                    headerSubfield.AssertShowDecimal(optionCalipso.CompartmentLength);
                                    break;

                                case "ipv6.opt.calipso.sens_level":
                                    headerSubfield.AssertNoFields();
                                    headerSubfield.AssertShowDecimal(optionCalipso.SensitivityLevel);
                                    break;

                                case "ipv6.opt.calipso.checksum":
                                    headerSubfield.AssertNoFields();
                                    headerSubfield.AssertShowDecimal(optionCalipso.Checksum);
                                    break;

                                case "ipv6.opt.calipso.cmpt_bitmap":
                                    headerSubfield.AssertNoFields();
                                    headerSubfield.AssertValue(optionCalipso.CompartmentBitmap);
                                    ++optionsIndex;
                                    break;

                                case "ipv6.opt.router_alert":
                                    headerSubfield.AssertNoFields();
                                    var optionRouterAlert = (IpV6OptionRouterAlert)option;
                                    headerSubfield.AssertShowDecimal((ushort)optionRouterAlert.RouterAlertType);
                                    ++optionsIndex;
                                    break;

                                case "ipv6.opt.padn":
                                    headerSubfield.AssertNoFields();
                                    var optionPadN = (IpV6OptionPadN)option;
                                    headerSubfield.AssertValue(new byte[optionPadN.PaddingDataLength]);
                                    ++optionsIndex;
                                    break;

                                case "ipv6.opt.qs_func":
                                    headerSubfield.AssertNoFields();
                                    headerSubfield.AssertShowDecimal((byte)optionQuickStart.QuickStartFunction);
                                    break;

                                case "ipv6.opt.qs_rate":
                                    headerSubfield.AssertNoFields();
                                    headerSubfield.AssertShowDecimal(optionQuickStart.Rate);
                                    break;
                                
                                case "ipv6.opt.qs_ttl":
                                    headerSubfield.AssertNoFields();
                                    headerSubfield.AssertShowDecimal(optionQuickStart.Ttl);
                                    break;
                                
                                case "ipv6.opt.qs_ttl_diff":
                                    headerSubfield.AssertNoFields();
                                    headerSubfield.AssertShowDecimal((256 + ipV6Datagram.HopLimit - optionQuickStart.Ttl) % 256);
                                    break;

                                case "ipv6.opt.qs_unused":
                                    headerSubfield.AssertNoFields();
                                    headerSubfield.AssertShowDecimal(optionQuickStart.Ttl);
                                    break;

                                case "ipv6.opt.qs_nonce":
                                    headerSubfield.AssertNoFields();
                                    headerSubfield.AssertShowDecimal(optionQuickStart.Nonce);
                                    break;

                                case "ipv6.opt.qs_reserved":
                                    headerSubfield.AssertNoFields();
                                    headerSubfield.AssertShowDecimal(0);
                                    ++optionsIndex;
                                    break;

                                case "ipv6.opt.pad1":
                                    headerSubfield.AssertNoFields();
                                    headerSubfield.AssertShow("");
                                    Assert.IsTrue(option is IpV6OptionPad1);
                                    ++optionsIndex;
                                    break;

                                case "ipv6.opt.jumbo":
                                    headerSubfield.AssertNoFields();
                                    headerSubfield.AssertShowDecimal(((IpV6OptionJumboPayload)option).JumboPayloadLength);
                                    ++optionsIndex;
                                    break;

                                case "ipv6.mipv6_home_address":
                                    headerSubfield.AssertNoFields();
                                    headerSubfield.AssertShow(((IpV6OptionHomeAddress)option).HomeAddress.GetWiresharkString());
                                    ++optionsIndex;
                                    break;
                               
                                case "ipv6.opt.unknown":
                                    headerSubfield.AssertNoFields();
                                    Assert.IsTrue(new[]
                                                      {
                                                          IpV6OptionType.LineIdentification,
                                                          IpV6OptionType.IdentifierLocatorNetworkProtocolNonce,
                                                          IpV6OptionType.SimplifiedMulticastForwardingDuplicatePacketDetection,
                                                          IpV6OptionType.EndpointIdentification,
                                                      }.Contains(option.OptionType),
                                                  option.OptionType.ToString());
                                    ++optionsIndex;
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid ipv6 header subfield " + headerSubfield.Name());
                            }
                        }
                        break;

                    default:
                        throw new InvalidOperationException("Invalid ipv6 options field " + headerField.Name());
                }
            }
        }

        private void IncrementCurrentExtensionHeaderIndex(IpV6Datagram ipV6Datagram)
        {
            ++_currentExtensionHeaderIndex;
            SkipAuthenticationHeaders(ipV6Datagram);
        }

        private void SkipAuthenticationHeaders(IpV6Datagram ipV6Datagram)
        {
            while (_currentExtensionHeaderIndex < ipV6Datagram.ExtensionHeaders.Headers.Count &&
                   ipV6Datagram.ExtensionHeaders[_currentExtensionHeaderIndex].Protocol == IpV4Protocol.AuthenticationHeader)
            {
                ++_currentExtensionHeaderIndex;
            }
        }

        private void ValidateExtensionHeaderUnnamedField(IpV6ExtensionHeader header, XElement headerField)
        {
            int optionIndex = -1;
            ValidateExtensionHeaderUnnamedField(header, headerField, ref optionIndex);
        }

        private void ValidateExtensionHeaderUnnamedField(IpV6ExtensionHeader header, XElement headerField, ref int optionsIndex)
        {
            IpV6ExtensionHeaderOptions headerOptions = header as IpV6ExtensionHeaderOptions;

            string[] headerFieldShowParts = headerField.Show().Split(':');
            string headerFieldShowName = headerFieldShowParts[0];
            string headerFieldShowValue = headerFieldShowParts[1];
            switch (headerFieldShowName)
            {
                case "Next header":
                    headerField.AssertValue((byte)header.NextHeader.Value);
                    break;

                case "Length":
                    if (header.IsValid)
                        Assert.IsTrue(headerFieldShowValue.EndsWith(" (" + header.Length + " bytes)"));
                    break;

                case "Router alert":
                    IpV6OptionRouterAlert routerAlert = (IpV6OptionRouterAlert)headerOptions.Options[optionsIndex++];
                    switch (headerFieldShowValue)
                    {
                        case " MLD (4 bytes)":
                            Assert.AreEqual(IpV6RouterAlertType.MulticastListenerDiscovery, routerAlert.RouterAlertType);
                            break;

                        case " RSVP (4 bytes)":
                            Assert.AreEqual(IpV6RouterAlertType.Rsvp, routerAlert.RouterAlertType);
                            break;

                        case " Unknown (4 bytes)":
                            MoreAssert.IsInRange((ushort)IpV6RouterAlertType.ActiveNetwork, (ushort)IpV6RouterAlertType.NextStepsInSignalingNatFirewallLayerProtocol, (ushort)routerAlert.RouterAlertType);
                            headerField.AssertValueInRange(0x05020002, 0x05020044);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid ipv6 header route Router alert value " + headerFieldShowValue);
                    }
                    break;

                case "Jumbo payload":
                    IpV6OptionJumboPayload jumboPayload = (IpV6OptionJumboPayload)headerOptions.Options[optionsIndex++];
                    Assert.AreEqual(" " + jumboPayload.JumboPayloadLength + " (6 bytes)", headerFieldShowValue);
                    break;

                default:
                    throw new InvalidOperationException("Invalid ipv6 header unnamed field show name " + headerFieldShowName);
            }
        }
        
        private int _currentExtensionHeaderIndex;
    }
}
