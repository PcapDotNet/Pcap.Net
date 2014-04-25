using System;
using System.Collections.Generic;
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
                    field.AssertShowHex((uint)ipV6Datagram.TrafficClass);
                    //field.AssertNoFields();
                    break;

                case "ipv6.flow":
                    field.AssertShowHex((uint)ipV6Datagram.FlowLabel);
                    field.AssertNoFields();
                    break;

                case "ipv6.plen":
                    field.AssertShowDecimal(ipV6Datagram.PayloadLength);
                    field.AssertNoFields();
                    break;

                case "ipv6.nxt":
                    field.AssertShowHex((byte)ipV6Datagram.NextHeader);
                    field.AssertNoFields();
                    break;

                case "ipv6.hlim":
                    field.AssertShowDecimal(ipV6Datagram.HopLimit);
                    field.AssertNoFields();
                    break;

                case "ipv6.src":
                case "ipv6.src_host":
                    field.AssertShow(ipV6Datagram.Source.ToString("x"));
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
                    field.AssertShow(ipV6Datagram.CurrentDestination.ToString("x"));
                    field.AssertNoFields();
                    break;

                case "ipv6.addr":
                case "ipv6.host":
                    Assert.IsTrue(field.Show() == ipV6Datagram.Source.ToString("x") ||
                                  field.Show() == ipV6Datagram.CurrentDestination.ToString("x"));
                    field.AssertNoFields();
                    break;

                case "ipv6.hop_opt":
                    IpV6ExtensionHeaderHopByHopOptions hopByHopOptions = (IpV6ExtensionHeaderHopByHopOptions)ipV6Datagram.ExtensionHeaders[_currentExtensionHeaderIndex];
                    IncrementCurrentExtensionHeaderIndex(ipV6Datagram);
                    CompareOptions(field, ref optionsIndex, hopByHopOptions);
                    break;

                case "ipv6.routing_hdr":
                    if (!ipV6Datagram.IsValid)
                        return false;
                    IpV6ExtensionHeaderRouting routing = (IpV6ExtensionHeaderRouting)ipV6Datagram.ExtensionHeaders[_currentExtensionHeaderIndex];
                    IncrementCurrentExtensionHeaderIndex(ipV6Datagram);
                    int sourceRouteAddressIndex = 0;
                    foreach (var headerField in field.Fields())
                    {
                        headerField.AssertNoFields();
                        switch (headerField.Name())
                        {
                            case "":
                                ValidateExtensionHeaderUnnamedField(routing, headerField);
                                break;

                            case "ipv6.routing_hdr.type":
                                headerField.AssertShowDecimal((byte)routing.RoutingType);
                                break;

                            case "ipv6.routing_hdr.left":
                                headerField.AssertShowDecimal(routing.SegmentsLeft);
                                break;

                            case "ipv6.mipv6_home_address":
                                IpV6ExtensionHeaderRoutingHomeAddress routingHomeAddress = (IpV6ExtensionHeaderRoutingHomeAddress)routing;
                                headerField.AssertShow(routingHomeAddress.HomeAddress.ToString("x"));
                                break;

                            case "ipv6.routing_hdr.addr":
                                IpV6ExtensionHeaderRoutingSourceRoute routingSourceRoute = (IpV6ExtensionHeaderRoutingSourceRoute)routing;
                                headerField.AssertShow(routingSourceRoute.Addresses[sourceRouteAddressIndex++].ToString("x"));
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
                    CompareOptions(field, ref optionsIndex, destinationOptions);
                    break;

                case "ipv6.shim6":
                    // TODO: Fix according to https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=9995
                    IpV4Protocol nextHeader = _currentExtensionHeaderIndex > 0
                                                  ? ipV6Datagram.ExtensionHeaders[_currentExtensionHeaderIndex - 1].NextHeader.Value
                                                  : ipV6Datagram.NextHeader;

                    Assert.AreEqual(IpV4Protocol.AnyHostInternal, nextHeader);
                    return false;

                case "ipv6.unknown_hdr":
                    Assert.AreEqual(ipV6Datagram.ExtensionHeaders.Count(), _currentExtensionHeaderIndex);
                    // TODO: Fix according to https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=9996
                    return false;

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
                                    case "":
                                        headerField.AssertValue((byte)fragmentData.NextHeader.Value);
                                        break;

                                    case "ipv6.fragment.offset":
                                        headerField.AssertShowDecimal(fragmentData.FragmentOffset);
                                        break;

                                    case "ipv6.fragment.more":
                                        headerField.AssertShowDecimal(fragmentData.MoreFragments);
                                        break;

                                    case "ipv6.framgent.id":
                                        headerField.AssertShowHex(fragmentData.Identification);
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

        private void CompareOptions(XElement field, ref int optionsIndex, IpV6ExtensionHeaderOptions header)
        {
            foreach (var headerField in field.Fields())
            {
                headerField.AssertNoFields();
                switch (headerField.Name())
                {
                    case "":
                        ValidateExtensionHeaderUnnamedField(header, headerField, ref optionsIndex);
                        break;

                    case "ipv6.opt.pad1":
                        Assert.AreEqual(IpV6OptionType.Pad1, header.Options[optionsIndex++].OptionType);
                        break;

                    case "ipv6.opt.padn":
                        Assert.AreEqual(IpV6OptionType.PadN, header.Options[optionsIndex].OptionType);
                        headerField.AssertShowDecimal(header.Options[optionsIndex++].Length);
                        break;

                    case "ipv6.mipv6_type":
                        Assert.AreEqual(IpV6OptionType.HomeAddress, header.Options[optionsIndex].OptionType);
                        break;

                    case "ipv6.mipv6_length":
                        headerField.AssertShowDecimal(header.Options[optionsIndex].Length - 2);
                        break;

                    case "ipv6.mipv6_home_address":
                        IpV6OptionHomeAddress homeAddress = (IpV6OptionHomeAddress)header.Options[optionsIndex++];
                        headerField.AssertShow(homeAddress.HomeAddress.ToString("x"));
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
                            MoreAssert.IsInRange((ushort)IpV6RouterAlertType.ActiveNetwork, (ushort)IpV6RouterAlertType.NsisNatfwNslp, (ushort)routerAlert.RouterAlertType);
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
