using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;

namespace PcapDotNet.Core.Test
{
    internal class WiresharkDatagramComparerIpV6MobilityHeader : WiresharkDatagramComparerSimple
    {
        public WiresharkDatagramComparerIpV6MobilityHeader()
        {
        }

        protected override string PropertyName
        {
            get { return ""; }
        }

        protected override bool CompareField(XElement field, Datagram datagram)
        {
            IpV6Datagram ipV6Datagram = datagram as IpV6Datagram;
            if (ipV6Datagram == null)
                return true;
            if (ipV6Datagram.NextHeader == IpV4Protocol.Cftp ||
                ipV6Datagram.ExtensionHeaders.Any(extensionHeader => extensionHeader.NextHeader == IpV4Protocol.Cftp))
                return false;
            // TODO: Remove after https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=9996 is fixed.
            if (ipV6Datagram.ExtensionHeaders.Select(extensionHeader => extensionHeader.NextHeader).Concat(ipV6Datagram.NextHeader).Any(
                protocol => protocol == IpV4Protocol.WidebandMonitoring ||
                            protocol == IpV4Protocol.SunNd ||
                            protocol == IpV4Protocol.Swipe ||
                            protocol == IpV4Protocol.AnyHostInternal ||
                            protocol == IpV4Protocol.SourceDemandRoutingProtocol ||
                            protocol == IpV4Protocol.MobileInternetworkingControlProtocol ||
                            protocol == IpV4Protocol.IsoIp ||
                            protocol == IpV4Protocol.Kryptolan ||
                            protocol == IpV4Protocol.LArp ||
                            protocol == IpV4Protocol.SecureVersatileMessageTransactionProtocol ||
                            protocol == IpV4Protocol.WangSpanNetwork ||
                            protocol == IpV4Protocol.Cbt ||
                            protocol == IpV4Protocol.Visa ||
                            protocol == IpV4Protocol.SimpleMessageProtocol ||
                            protocol == IpV4Protocol.InternetPacketCoreUtility ||
                            protocol == IpV4Protocol.BbnRccMonitoring ||
                            protocol == IpV4Protocol.IpIp ||
                            protocol == IpV4Protocol.FibreChannel ||
                            protocol == IpV4Protocol.ServiceSpecificConnectionOrientedProtocolInAMultilinkAndConnectionlessEnvironment ||
                            protocol == IpV4Protocol.SitaraNetworksProtocol ||
                            protocol == IpV4Protocol.Fire ||
                            protocol == IpV4Protocol.Leaf1 ||
                            protocol == IpV4Protocol.IpsilonFlowManagementProtocol ||
                            protocol == IpV4Protocol.CompaqPeer ||
                            protocol == IpV4Protocol.InterDomainPolicyRoutingProtocolControlMessageTransportProtocol ||
                            protocol == IpV4Protocol.BulkDataTransferProtocol ||
                            protocol == IpV4Protocol.SemaphoreCommunicationsSecondProtocol ||
                            protocol == IpV4Protocol.Mobile ||
                            protocol == IpV4Protocol.HostMonitoringProtocol ||
                            protocol == IpV4Protocol.Chaos ||
                            protocol == IpV4Protocol.DiiDataExchange ||
                            protocol == IpV4Protocol.Emcon ||
                            protocol == IpV4Protocol.ThirdPartyConnect ||
                            protocol == IpV4Protocol.Aris ||
                            protocol == IpV4Protocol.NetworkVoice ||
                            protocol == IpV4Protocol.AnyPrivateEncryptionScheme ||
                            protocol == IpV4Protocol.PacketVideoProtocol ||
                            protocol == IpV4Protocol.PacketRadioMeasurement ||
                            protocol == IpV4Protocol.AnyLocalNetwork ||
                            protocol == IpV4Protocol.Qnx ||
                            protocol == IpV4Protocol.Tcf ||
                            protocol == IpV4Protocol.Ttp ||
                            protocol == IpV4Protocol.ScheduleTransferProtocol ||
                            protocol == IpV4Protocol.TransportLayerSecurityProtocol ||
                            protocol == IpV4Protocol.Ax25 ||
                            protocol == IpV4Protocol.CombatRadioTransportProtocol ||
                            protocol == IpV4Protocol.PerformanceTransparencyProtocol ||
                            protocol == IpV4Protocol.IntegratedNetLayerSecurityProtocol ||
                            protocol == IpV4Protocol.DatagramDeliveryProtocol ||
                            protocol == IpV4Protocol.PrivateNetworkToNetworkInterface ||
                            protocol == IpV4Protocol.Pipe ||
                            protocol == IpV4Protocol.BackroomSatMon ||
                            protocol == IpV4Protocol.Iplt ||
                            protocol == IpV4Protocol.Any0HopProtocol ||
                            protocol == IpV4Protocol.Leaf2 ||
                            protocol == IpV4Protocol.InterDomainPolicyRoutingProtocol ||
                            protocol == IpV4Protocol.NationalScienceFoundationNetworkInteriorGatewayProtocol ||
                            protocol == IpV4Protocol.WidebandExpak ||
                            protocol == IpV4Protocol.Uti ||
                            protocol == IpV4Protocol.Multiplexing ||
                            protocol == IpV4Protocol.Il ||
                            protocol == IpV4Protocol.MulticastTransportProtocol ||
                            protocol == IpV4Protocol.AnyDistributedFileSystem ||
                            protocol == IpV4Protocol.InteractiveAgentTransferProtocol ||
                            protocol == IpV4Protocol.InternetPluribusPacketCore ||
                            protocol == IpV4Protocol.InternetworkPacketExchangeInIp ||
                            protocol == IpV4Protocol.IsIsOverIpV4 ||
                            protocol == IpV4Protocol.ComputerProtocolNetworkExecutive ||
                            protocol == IpV4Protocol.EncapsulationHeader ||
                            protocol == IpV4Protocol.RemoteVirtualDiskProtocol))
                return false;

            int currentExtensionHeaderIndex = ipV6Datagram.ExtensionHeaders.TakeWhile(extensionHeader => extensionHeader.Protocol != IpV4Protocol.MobilityHeader).Count();
            if (currentExtensionHeaderIndex >= ipV6Datagram.ExtensionHeaders.Headers.Count && !ipV6Datagram.IsValid)
                return false;
            IpV6ExtensionHeaderMobility mobilityHeader = (IpV6ExtensionHeaderMobility)ipV6Datagram.ExtensionHeaders[currentExtensionHeaderIndex];

            switch (field.Name())
            {
                case "mip6.proto":
                    field.AssertShowDecimal((byte)mobilityHeader.NextHeader);
                    field.AssertNoFields();
                    break;

                case "mip6.hlen":
                    if (mobilityHeader.IsValid)
                        field.AssertShowDecimal(mobilityHeader.Length / 8 - 1);
                    field.AssertNoFields();
                    break;

                case "mip6.mhtype":
                    field.AssertShowDecimal((byte)mobilityHeader.MobilityHeaderType);
                    break;

                case "mip6.reserved":
                    field.AssertShowHex((byte)0);
                    field.AssertNoFields();
                    break;

                case "mip6.csum":
                    field.AssertShowHex(mobilityHeader.Checksum);
                    break;

                case "":
                    switch (field.Show())
                    {
                        case "Binding Refresh Request":
                            Assert.AreEqual(IpV6MobilityHeaderType.BindingRefreshRequest, mobilityHeader.MobilityHeaderType);
                            field.AssertNoFields();
                            break;

                        case "Heartbeat":
                            IpV6ExtensionHeaderMobilityHeartbeatMessage heartbeatMessage = (IpV6ExtensionHeaderMobilityHeartbeatMessage)mobilityHeader;
                            foreach (XElement subfield in field.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "mip6.hb.u_flag":
                                        subfield.AssertShowDecimal(heartbeatMessage.IsUnsolicitedHeartbeatResponse);
                                        break;

                                    case "mip6.hb.r_flag":
                                        subfield.AssertShowDecimal(heartbeatMessage.IsResponse);
                                        break;

                                    case "mip6.hb.seqnr":
                                        subfield.AssertShowDecimal(heartbeatMessage.SequenceNumber);
                                        break;

                                    default:
                                        throw new InvalidOperationException(string.Format("Invalid IPv6 Heartbeat mobility header field {0}", subfield.Name()));
                                }
                            }
                            break;

                        case "Binding Revocation Indication":
                            IpV6ExtensionHeaderMobilityBindingRevocationIndicationMessage bindingRevocationIndicationMessage = (IpV6ExtensionHeaderMobilityBindingRevocationIndicationMessage)mobilityHeader;
                            foreach (XElement subfield in field.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "mip6.bri_br.type":
                                        subfield.AssertShowDecimal((byte)bindingRevocationIndicationMessage.BindingRevocationType);
                                        break;

                                    case "mip6.bri_r.trigger":
                                        subfield.AssertShowDecimal((byte)bindingRevocationIndicationMessage.RevocationTrigger);
                                        break;

                                    case "mip6._bri_seqnr":
                                        subfield.AssertShowDecimal(bindingRevocationIndicationMessage.SequenceNumber);
                                        break;

                                    case "mip6.bri_ip":
                                        subfield.AssertShowDecimal(bindingRevocationIndicationMessage.ProxyBinding);
                                        break;

                                    case "mip6.bri_ia":
                                        // TODO: Should be named differently. See https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10015
                                        subfield.AssertShowDecimal(bindingRevocationIndicationMessage.IpV4HomeAddressBindingOnly);
                                        break;

                                    case "mip6.bri_ig":
                                        subfield.AssertShowDecimal(bindingRevocationIndicationMessage.Global);
                                        break;

                                    case "mip6.bri_res":
                                        break;

                                    default:
                                        throw new InvalidOperationException(string.Format("Invalid IPv6 Binding Revocation Acknowledgement Message mobility header field {0}", subfield.Name()));
                                }
                            }
                            break;

                        case "Binding Revocation Acknowledge":
                            IpV6ExtensionHeaderMobilityBindingRevocationAcknowledgementMessage bindingRevocationAcknowledgementMessage = (IpV6ExtensionHeaderMobilityBindingRevocationAcknowledgementMessage)mobilityHeader;
                            foreach (XElement subfield in field.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "mip6.bri_br.type":
                                        subfield.AssertShowDecimal((byte)bindingRevocationAcknowledgementMessage.BindingRevocationType);
                                        break;

                                    case "mip6.bri_status":
                                        subfield.AssertShowDecimal((byte)bindingRevocationAcknowledgementMessage.Status);
                                        break;

                                    case "mip6._bri_seqnr":
                                        subfield.AssertShowDecimal(bindingRevocationAcknowledgementMessage.SequenceNumber);
                                        break;

                                    case "mip6.bri_ap":
                                        subfield.AssertShowDecimal(bindingRevocationAcknowledgementMessage.ProxyBinding);
                                        break;

                                    case "mip6.bri_ag":
                                        // TODO: Fix after https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10007 is fixed.
                                        subfield.AssertShowDecimal(bindingRevocationAcknowledgementMessage.IpV4HomeAddressBindingOnly);
                                        break;

                                    case "mip6.bri_res":
                                        break;

                                    default:
                                        throw new InvalidOperationException(string.Format("Invalid IPv6 Binding Revocation Acknowledgement Message mobility header field {0}", subfield.Name()));
                                }
                            }
                            break;

                        case "Care-of Test Init":
                            IpV6ExtensionHeaderMobilityCareOfTestInit careOfTestInit = (IpV6ExtensionHeaderMobilityCareOfTestInit)mobilityHeader;
                            foreach (XElement subfield in field.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "mip6.coti.cookie":
                                        subfield.AssertShowHex(careOfTestInit.CareOfInitCookie);
                                        break;

                                    default:
                                        throw new InvalidOperationException(string.Format("Invalid IPv6 Care Of Test Init mobility header field {0}", subfield.Name()));
                                }
                            }
                            break;

                        case "Care-of Test":
                            IpV6ExtensionHeaderMobilityCareOfTest careOfTest = (IpV6ExtensionHeaderMobilityCareOfTest)mobilityHeader;
                            foreach (XElement subfield in field.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "mip6.cot.nindex":
                                        subfield.AssertShowDecimal(careOfTest.CareOfNonceIndex);
                                        break;

                                    case "mip6.cot.cookie":
                                        subfield.AssertShowHex(careOfTest.CareOfInitCookie);
                                        break;

                                    case "mip6.hot.token":
                                        subfield.AssertShowHex(careOfTest.CareOfKeygenToken);
                                        break;

                                    default:
                                        throw new InvalidOperationException(string.Format("Invalid IPv6 Care Of Test mobility header field {0}", subfield.Name()));
                                }
                            }
                            break;

                        case "Fast Binding Acknowledgement":
                            IpV6ExtensionHeaderMobilityFastBindingAcknowledgement fastBindingAcknowledgement = (IpV6ExtensionHeaderMobilityFastBindingAcknowledgement)mobilityHeader;
                            foreach (XElement subfield in field.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "fmip6.fback.status":
                                        subfield.AssertShowDecimal((byte)fastBindingAcknowledgement.Status);
                                        break;

                                    case "fmip6.fback.k_flag":
                                        subfield.AssertShowDecimal(fastBindingAcknowledgement.KeyManagementMobilityCapability);
                                        break;

                                    case "fmip6.fback.seqnr":
                                        subfield.AssertShowDecimal(fastBindingAcknowledgement.SequenceNumber);
                                        break;

                                    case "fmip6.fback.lifetime":
                                        subfield.AssertShowDecimal(fastBindingAcknowledgement.Lifetime);
                                        break;

                                    default:
                                        throw new InvalidOperationException(string.Format("Invalid IPv6 Fast Binding Acknowledgement mobility header field {0}", subfield.Name()));
                                }
                            }
                            break;

                        case "Binding Error":
                            IpV6ExtensionHeaderMobilityBindingError bindingError = (IpV6ExtensionHeaderMobilityBindingError)mobilityHeader;
                            foreach (XElement subfield in field.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "mip6.be.status":
                                        subfield.AssertShowDecimal((byte)bindingError.Status);
                                        break;

                                    case "mip6.be.haddr":
                                        subfield.AssertShow(bindingError.HomeAddress.ToString("x"));
                                        break;

                                    default:
                                        throw new InvalidOperationException(string.Format("Invalid IPv6 Binding Error mobility header field {0}", subfield.Name()));
                                }
                            }
                            break;

                        case "Fast Neighbor Advertisement":
                            Assert.AreEqual(IpV6MobilityHeaderType.FastNeighborAdvertisement, mobilityHeader.MobilityHeaderType);
                            field.AssertNoFields();
                            break;

                        case "Home Test":
                            IpV6ExtensionHeaderMobilityHomeTest homeTest = (IpV6ExtensionHeaderMobilityHomeTest)mobilityHeader;
                            foreach (XElement subfield in field.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "mip6.hot.nindex":
                                        subfield.AssertShowDecimal(homeTest.HomeNonceIndex);
                                        break;

                                    case "mip6.hot.cookie":
                                        subfield.AssertShowHex(homeTest.HomeInitCookie);
                                        break;

                                    case "mip6.hot.token":
                                        subfield.AssertShowHex(homeTest.HomeKeygenToken);
                                        break;

                                    default:
                                        throw new InvalidOperationException(string.Format("Invalid IPv6 Home Test mobility header field {0}", subfield.Name()));
                                }
                            }
                            break;

                        case "Home Test Init":
                            IpV6ExtensionHeaderMobilityHomeTestInit homeTestInit = (IpV6ExtensionHeaderMobilityHomeTestInit)mobilityHeader;
                            foreach (XElement subfield in field.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "mip6.hoti.cookie":
                                        subfield.AssertShowHex(homeTestInit.HomeInitCookie);
                                        break;

                                    default:
                                        throw new InvalidOperationException(string.Format("Invalid IPv6 Home Test Init mobility header field {0}", subfield.Name()));
                                }
                            }
                            break;

                        case "Binding Update":
                            IpV6ExtensionHeaderMobilityBindingUpdate bindingUpdate = (IpV6ExtensionHeaderMobilityBindingUpdate)mobilityHeader;
                            foreach (XElement subfield in field.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "mip6.bu.seqnr":
                                        subfield.AssertShowDecimal(bindingUpdate.SequenceNumber);
                                        break;

                                    case "mip6.bu.a_flag":
                                        subfield.AssertShowDecimal(bindingUpdate.Acknowledge);
                                        break;

                                    case "mip6.bu.h_flag":
                                        subfield.AssertShowDecimal(bindingUpdate.HomeRegistration);
                                        break;

                                    case "mip6.bu.l_flag":
                                        subfield.AssertShowDecimal(bindingUpdate.LinkLocalAddressCompatibility);
                                        break;

                                    case "mip6.bu.k_flag":
                                        subfield.AssertShowDecimal(bindingUpdate.KeyManagementMobilityCapability);
                                        break;

                                    case "mip6.bu.m_flag":
                                        subfield.AssertShowDecimal(bindingUpdate.MapRegistration);
                                        break;

                                    case "mip6.nemo.bu.r_flag":
                                        subfield.AssertShowDecimal(bindingUpdate.MobileRouter);
                                        break;

                                    case "mip6.bu.p_flag":
                                        subfield.AssertShowDecimal(bindingUpdate.ProxyRegistrationFlag);
                                        break;

                                    case "mip6.bu.f_flag":
                                        subfield.AssertShowDecimal(bindingUpdate.ForcingUdpEncapsulation);
                                        break;

                                    case "mip6.bu.t_flag":
                                        subfield.AssertShowDecimal(bindingUpdate.TlvHeaderFormat);
                                        break;

                                    case "mip6.bu.lifetime":
                                        subfield.AssertShowDecimal(bindingUpdate.Lifetime);
                                        break;

                                    default:
                                        throw new InvalidOperationException(string.Format("Invalid IPv6 Binding Update mobility header field {0}", subfield.Name()));
                                }
                            }
                            break;

                        case "Binding Acknowledgement":
                            IpV6ExtensionHeaderMobilityBindingAcknowledgement bindingAcknowledgement = (IpV6ExtensionHeaderMobilityBindingAcknowledgement)mobilityHeader;
                            foreach (XElement subfield in field.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "mip6.ba.status":
                                        subfield.AssertShowDecimal((byte)bindingAcknowledgement.Status);
                                        break;

                                    case "mip6.ba.k_flag":
                                        subfield.AssertShowDecimal(bindingAcknowledgement.KeyManagementMobilityCapability);
                                        break;

                                    case "mip6.nemo.ba.r_flag":
                                        subfield.AssertShowDecimal(bindingAcknowledgement.MobileRouter);
                                        break;

                                    case "mip6.ba.p_flag":
                                        subfield.AssertShowDecimal(bindingAcknowledgement.ProxyRegistration);
                                        break;

                                    case "mip6.ba.t_flag":
                                        subfield.AssertShowDecimal(bindingAcknowledgement.TlvHeaderFormat);
                                        break;

                                    case "mip6.ba.seqnr":
                                        subfield.AssertShowDecimal(bindingAcknowledgement.SequenceNumber);
                                        break;

                                    case "mip6.ba.lifetime":
                                        subfield.AssertShowDecimal(bindingAcknowledgement.Lifetime);
                                        break;

                                    default:
                                        throw new InvalidOperationException(string.Format("Invalid IPv6 Binding Acknowledgement mobility header field {0}", subfield.Name()));
                                }
                            }
                            break;

                        case "Fast Binding Update":
                            IpV6ExtensionHeaderMobilityFastBindingUpdate fastBindingUpdate = (IpV6ExtensionHeaderMobilityFastBindingUpdate)mobilityHeader;
                            foreach (XElement subfield in field.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "fmip6.fbu.seqnr":
                                        subfield.AssertShowDecimal(fastBindingUpdate.SequenceNumber);
                                        break;

                                    case "fmip6.fbu.a_flag":
                                        subfield.AssertShowDecimal(fastBindingUpdate.Acknowledge);
                                        break;

                                    case "fmip6.fbu.h_flag":
                                        subfield.AssertShowDecimal(fastBindingUpdate.HomeRegistration);
                                        break;

                                    case "fmip6.fbu.l_flag":
                                        subfield.AssertShowDecimal(fastBindingUpdate.LinkLocalAddressCompatibility);
                                        break;

                                    case "fmip6.fbu.k_flag":
                                        subfield.AssertShowDecimal(fastBindingUpdate.KeyManagementMobilityCapability);
                                        break;

                                    case "fmip6.fbu.lifetime":
                                        subfield.AssertShowDecimal(fastBindingUpdate.Lifetime);
                                        break;

                                    default:
                                        throw new InvalidOperationException(string.Format("Invalid IPv6 Fast Binding Update mobility header field {0}", subfield.Name()));
                                }
                            }
                            break;

                        case "Mobility Options":
                            int optionIndex = 0;
                            foreach (XElement optionField in field.Fields())
                            {
                                IpV6MobilityOption option = mobilityHeader.MobilityOptions[optionIndex];
                                switch (optionField.Name())
                                {
                                    case "mip6..mobility_opt":
                                        optionField.AssertShowDecimal((byte)option.OptionType);
                                        optionField.AssertNoFields();
                                        break;

                                    case "mip6.bra.interval":
                                        optionField.AssertShowDecimal(((IpV6MobilityOptionBindingRefreshAdvice)option).RefreshInterval);
                                        optionField.AssertNoFields();
                                        ++optionIndex;
                                        break;

                                    case "mip6.gre_key":
                                        optionField.AssertShowDecimal(((IpV6MobilityOptionGreKey)option).GreKeyIdentifier);
                                        optionField.AssertNoFields();
                                        ++optionIndex;
                                        break;

                                    case "mip6.acoa.acoa":
                                        optionField.AssertShow(((IpV6MobilityOptionAlternateCareOfAddress)option).AlternateCareOfAddress.ToString("x"));
                                        optionField.AssertNoFields();
                                        ++optionIndex;
                                        break;

                                    case "mip6.rc":
                                        optionField.AssertShowDecimal(((IpV6MobilityOptionRestartCounter)option).RestartCounter);
                                        optionField.AssertNoFields();
                                        ++optionIndex;
                                        break;

                                    case "mip6.timestamp":
                                        IpV6MobilityOptionTimestamp timestamp = (IpV6MobilityOptionTimestamp)option;
                                        optionField.AssertValue(timestamp.Timestamp);
                                        Assert.AreEqual(IpV6MobilityOptionType.Timestamp, option.OptionType);
                                        // TODO: Fix this after https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10008 is fixed.
//                                        if (optionField.Show() != "Not representable")
//                                            optionField.AssertShow(timestamp.TimestampDateTime.ToString());
                                        optionField.AssertNoFields();
                                        ++optionIndex;
                                        break;

                                    case "mip6.att":
                                        optionField.AssertShowDecimal((byte)((IpV6MobilityOptionAccessTechnologyType)option).AccessTechnologyType);
                                        optionField.AssertNoFields();
                                        ++optionIndex;
                                        break;

                                    case "mip6.hi":
                                        optionField.AssertShowDecimal((byte)((IpV6MobilityOptionHandoffIndicator)option).HandoffIndicator);
                                        optionField.AssertNoFields();
                                        ++optionIndex;
                                        break;

                                    case "":
                                        switch (option.OptionType)
                                        {
                                            case IpV6MobilityOptionType.LinkLayerAddress:
                                                optionField.AssertShow("Mobility Header Link-Layer Address option");
                                                IpV6MobilityOptionLinkLayerAddress linkLayerAddress = (IpV6MobilityOptionLinkLayerAddress)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.lla.optcode":
                                                            optionSubfield.AssertShowDecimal((byte)linkLayerAddress.Code);
                                                            break;

                                                        case "":
                                                            // TODO: Fix when https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10006 is fixed.
                                                            // optionSubfield.AssertValue(linkLayerAddress.LinkLayerAddress);
                                                            return false;

                                                        default:
                                                            throw new InvalidOperationException(string.Format(
                                                                "Invalid IPv6 Link Layer Address option field {0}", optionSubfield.Name()));
                                                    }
                                                }
                                                // TODO: Remove once https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10043 is fixed.
                                                if (linkLayerAddress.LinkLayerAddress.Length == 0)
                                                    return false;
                                                break;

                                            case IpV6MobilityOptionType.IpV4DefaultRouterAddress:
                                                optionField.AssertShow("IPv4 Default-Router Address");
                                                IpV6MobilityOptionIpV4DefaultRouterAddress ipV4DefaultRouterAddress =
                                                    (IpV6MobilityOptionIpV4DefaultRouterAddress)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.ipv4dra.dra":
                                                            optionSubfield.AssertShow(ipV4DefaultRouterAddress.DefaultRouterAddress.ToString());
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(
                                                                string.Format("Invalid IPv6 IPv4 Default Router Address option field {0}", optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.Pad1:
                                                optionField.AssertShow("Pad1");
                                                optionField.AssertNoFields();
                                                break;

                                            case IpV6MobilityOptionType.PadN:
                                                optionField.AssertShow(string.Format("PadN: {0} bytes", option.Length));
                                                optionField.AssertNoFields();
                                                break;

                                            case IpV6MobilityOptionType.IpV4HomeAddressReply:
                                                optionField.AssertShow("IPv4 Home Address Reply");
                                                IpV6MobilityOptionIpV4HomeAddressReply ipV4HomeAddressReply = (IpV6MobilityOptionIpV4HomeAddressReply)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.ipv4aa.sts":
                                                            optionSubfield.AssertShowDecimal((byte)ipV4HomeAddressReply.Status);
                                                            break;

                                                        default:
                                                            ValidateIpV6MobilityOptionIpV4HomeAddressField(optionSubfield, ipV4HomeAddressReply);
                                                            break;
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.IpV4HomeAddressRequest:
                                                optionField.AssertShow("IPv4 Home Address Request");
                                                IpV6MobilityOptionIpV4HomeAddressRequest ipV4HomeAddressRequest =
                                                    (IpV6MobilityOptionIpV4HomeAddressRequest)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    optionSubfield.AssertNoFields();
                                                    ValidateIpV6MobilityOptionIpV4HomeAddressField(optionSubfield, ipV4HomeAddressRequest);
                                                }
                                                break;

                                            case IpV6MobilityOptionType.IpV4AddressAcknowledgement:
                                                optionField.AssertShow("IPv4 Address Acknowledgement");
                                                IpV6MobilityOptionIpV4AddressAcknowledgement ipV4AddressAcknowledgement =
                                                    (IpV6MobilityOptionIpV4AddressAcknowledgement)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.ipv4aa.sts":
                                                            optionSubfield.AssertShowDecimal((byte)ipV4AddressAcknowledgement.Status);
                                                            break;

                                                        default:
                                                            ValidateIpV6MobilityOptionIpV4HomeAddressField(optionSubfield, ipV4AddressAcknowledgement);
                                                            break;
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.MobileNetworkPrefix:
                                                optionField.AssertShow("Mobile Network Prefix");
                                                IpV6MobilityOptionMobileNetworkPrefix mobileNetworkPrefix = (IpV6MobilityOptionMobileNetworkPrefix)option;
                                                ValidateNetworkPrefixOption(mobileNetworkPrefix, optionField);
                                                break;

                                            case IpV6MobilityOptionType.HomeNetworkPrefix:
                                                optionField.AssertShow("Home Network Prefix");
                                                IpV6MobilityOptionHomeNetworkPrefix homeNetworkPrefix = (IpV6MobilityOptionHomeNetworkPrefix)option;
                                                ValidateNetworkPrefixOption(homeNetworkPrefix, optionField);
                                                break;

                                            case IpV6MobilityOptionType.VendorSpecific:
                                                optionField.AssertShow("Vendor Specific Mobility");
                                                IpV6MobilityOptionVendorSpecific vendorSpecific = (IpV6MobilityOptionVendorSpecific)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.vsm.vendorId":
                                                            optionSubfield.AssertShowDecimal(vendorSpecific.VendorId);
                                                            break;

                                                        case "mip6.vsm.subtype":
                                                            optionSubfield.AssertShowDecimal(vendorSpecific.SubType);
                                                            break;

                                                        case "":
                                                            optionSubfield.AssertValue(vendorSpecific.Data);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Vendor Specific option field {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.NonceIndices:
                                                optionField.AssertShow("Nonce Indices");
                                                IpV6MobilityOptionNonceIndices nonceIndices = (IpV6MobilityOptionNonceIndices)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.ni.hni":
                                                            optionSubfield.AssertShowDecimal(nonceIndices.HomeNonceIndex);
                                                            break;

                                                        case "mip6.ni.cni":
                                                            optionSubfield.AssertShowDecimal(nonceIndices.CareOfNonceIndex);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Nonce Indices option field {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.LinkLocalAddress:
                                                optionField.AssertShow("Link-local Address");
                                                IpV6MobilityOptionLinkLocalAddress linkLocalAddress = (IpV6MobilityOptionLinkLocalAddress)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.lila_lla":
                                                            optionSubfield.AssertShow(linkLocalAddress.LinkLocalAddress.ToString("x"));
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format(
                                                                "Invalid IPv6 Link-local Address option field {0}", optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.MobileNodeIdentifier:
                                                optionField.AssertShow("Mobile Node Identifier");
                                                IpV6MobilityOptionMobileNodeIdentifier mobileNodeIdentifier = (IpV6MobilityOptionMobileNodeIdentifier)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.mnid.subtype":
                                                            optionSubfield.AssertShowDecimal((byte)mobileNodeIdentifier.Subtype);
                                                            break;

                                                        case "":
                                                            optionSubfield.AssertValue(mobileNodeIdentifier.Identifier);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(
                                                                string.Format("Invalid IPv6 Mobile Node Identifier option field {0}", optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.BindingAuthorizationData:
                                                optionField.AssertShow("Authorization Data");
                                                IpV6MobilityOptionBindingAuthorizationData authorizationData =
                                                    (IpV6MobilityOptionBindingAuthorizationData)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.bad.auth":
                                                            optionSubfield.AssertValue(authorizationData.Authenticator);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format(
                                                                "Invalid IPv6 Authorization Data option field {0}", optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.IpV4HomeAddress:
                                                optionField.AssertShow("IPv4 Home Address");
                                                IpV6MobilityOptionIpV4HomeAddress ipV4HomeAddress = (IpV6MobilityOptionIpV4HomeAddress)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.ipv4ha.p_flag":
                                                            optionSubfield.AssertShowDecimal(ipV4HomeAddress.RequestPrefix);
                                                            break;

                                                        default:
                                                            ValidateIpV6MobilityOptionIpV4HomeAddressField(optionSubfield, ipV4HomeAddress);
                                                            break;
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.ServiceSelection:
                                                IpV6MobilityOptionServiceSelection serviceSelection = (IpV6MobilityOptionServiceSelection)option;
                                                // TODO: Get rid of that when https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10045 is fixed.
                                                if (serviceSelection.Identifier.Length == 1)
                                                {
                                                    optionField.AssertShow("Service Selection Mobility (with option length = 1 byte; should be >= 2)");
                                                    break;
                                                }
                                                optionField.AssertValue(serviceSelection.Identifier);
                                                optionField.AssertNoFields();
                                                break;

                                            case IpV6MobilityOptionType.FlowSummary:
                                            case IpV6MobilityOptionType.CgaParametersRequest:
                                            case IpV6MobilityOptionType.Redirect:
                                            case IpV6MobilityOptionType.IpV4CareOfAddress:
                                            case IpV6MobilityOptionType.Signature:
                                            case IpV6MobilityOptionType.MobileNodeGroupIdentifier:
                                            case IpV6MobilityOptionType.MobileNodeLinkLayerIdentifier:
                                            case IpV6MobilityOptionType.Authentication:
                                            case IpV6MobilityOptionType.RedirectCapability:
                                            case IpV6MobilityOptionType.CgaParameters:
                                            case IpV6MobilityOptionType.ContextRequest:
                                            case IpV6MobilityOptionType.IpV6AddressPrefix:
                                            case IpV6MobilityOptionType.FlowIdentification:
                                            case IpV6MobilityOptionType.TransientBinding:
                                            case IpV6MobilityOptionType.LocalMobilityAnchorAddress:
                                            case IpV6MobilityOptionType.PermanentHomeKeygenToken:
                                            case IpV6MobilityOptionType.AccessNetworkIdentifier:
                                            case IpV6MobilityOptionType.BindingIdentifier:
                                            case IpV6MobilityOptionType.DnsUpdate:
                                            case IpV6MobilityOptionType.CareOfTest:
                                            case IpV6MobilityOptionType.IpV4DhcpSupportMode:
                                            case IpV6MobilityOptionType.AlternateIpV4CareOfAddress:
                                            case IpV6MobilityOptionType.MobileNodeLinkLocalAddressInterfaceIdentifier:
                                            case IpV6MobilityOptionType.LoadInformation:
                                            case IpV6MobilityOptionType.BindingAuthorizationDataForFmIpV6:
                                            case IpV6MobilityOptionType.NatDetection:
                                            case IpV6MobilityOptionType.MobileAccessGatewayIpV6Address:
                                            case IpV6MobilityOptionType.ReplayProtection:
                                            case IpV6MobilityOptionType.CareOfTestInit:
                                            case IpV6MobilityOptionType.Experimental:
                                                optionField.AssertShow("IE data not dissected yet");
                                                optionField.AssertNoFields();
                                                break;

                                            default:
                                                throw new InvalidOperationException(string.Format("Unsupported IPv6 mobility option type {0}", option.OptionType));
                                        }
                                        ++optionIndex;
                                        break;

                                    default:
                                        throw new InvalidOperationException(string.Format("Invalid ipv6 mobility header option field {0}", optionField.Name()));
                                }
                            }
                            break;

                        default:
                            field.AssertShow("Unknown MH Type");
                            Assert.IsTrue(mobilityHeader.MobilityHeaderType == IpV6MobilityHeaderType.Experimental ||
                                          mobilityHeader.MobilityHeaderType == IpV6MobilityHeaderType.HandoverAcknowledgeMessage ||
                                          mobilityHeader.MobilityHeaderType == IpV6MobilityHeaderType.HomeAgentSwitchMessage ||
                                          mobilityHeader.MobilityHeaderType == IpV6MobilityHeaderType.LocalizedRoutingInitiation ||
                                          mobilityHeader.MobilityHeaderType == IpV6MobilityHeaderType.LocalizedRoutingAcknowledgement ||
                                          mobilityHeader.MobilityHeaderType == IpV6MobilityHeaderType.HandoverInitiateMessage);
                            field.AssertNoFields();
                            break;
                    }
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Invalid ipv6 mobility header field {0}", field.Name()));
            }

            return true;
        }

        private static void ValidateNetworkPrefixOption(IpV6MobilityOptionNetworkPrefix networkPrefix, XElement field)
        {
            foreach (XElement subfield in field.Fields())
            {
                subfield.AssertNoFields();
                switch (subfield.Name())
                {
                    case "mip6.nemo.mnp.pfl":
                        subfield.AssertShowDecimal(networkPrefix.PrefixLength);
                        break;

                    case "mip6.nemo.mnp.mnp":
                        subfield.AssertShow(networkPrefix.NetworkPrefix.ToString("x"));
                        break;

                    default:
                        throw new InvalidOperationException(string.Format("Invalid IPv6 Network Prefix option field {0}", subfield.Name()));
                }
            }
        }

        private void ValidateIpV6MobilityOptionIpV4HomeAddressField(XElement field, IIpV6MobilityOptionIpV4HomeAddress ipV4HomeAddress)
        {
            switch (field.Name())
            {
                case "mip6.ipv4ha.preflen":
                    field.AssertShowDecimal(ipV4HomeAddress.PrefixLength);
                    break;

                case "mip6.ipv4ha.ha":
                    field.AssertShow(ipV4HomeAddress.HomeAddress.ToString());
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Invalid IpV6 IpV4 Home Address option field {0}", field.Name()));
            }
        }
    }
}