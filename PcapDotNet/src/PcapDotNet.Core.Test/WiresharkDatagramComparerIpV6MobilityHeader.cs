using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;

namespace PcapDotNet.Core.Test
{
    [ExcludeFromCodeCoverage]
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
                            protocol == IpV4Protocol.SunNetworkDisk ||
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
                            protocol == IpV4Protocol.IntermediateSystemToIntermediateSystemOverIpV4 ||
                            protocol == IpV4Protocol.ComputerProtocolNetworkExecutive ||
                            protocol == IpV4Protocol.EncapsulationHeader ||
                            protocol == IpV4Protocol.GatewayToGateway ||
                            protocol == IpV4Protocol.SatMon ||
                            protocol == IpV4Protocol.VersatileMessageTransactionProtocol ||
                            protocol == IpV4Protocol.ReliableDatagramProtocol ||
                            protocol == IpV4Protocol.InternetReliableTransactionProtocol ||
                            protocol == IpV4Protocol.MeritInternodalProtocol ||
                            protocol == IpV4Protocol.Skip ||
                            protocol == IpV4Protocol.BurroughsNetworkArchitecture ||
                            protocol == IpV4Protocol.InterDomainRoutingProtocol ||
                            protocol == IpV4Protocol.ActiveNetworks ||
                            protocol == IpV4Protocol.SpectraLinkRadioProtocol ||
                            protocol == IpV4Protocol.MobileAdHocNetwork ||
                            protocol == IpV4Protocol.DissimilarGatewayProtocol ||
                            protocol == IpV4Protocol.SpriteRpc ||
                            protocol == IpV4Protocol.CombatRadioUserDatagram ||
                            protocol == IpV4Protocol.Gmtp ||
                            protocol == IpV4Protocol.MobileHostRoutingProtocol ||
                            protocol == IpV4Protocol.Shim6 ||  // TODO: Implement Shim6.
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
                    field.AssertShowDecimal(0);
                    field.AssertNoFields();
                    break;

                case "mip6.csum":
                    field.AssertShowDecimal(mobilityHeader.Checksum);
                    break;

                case "mip6.em.data":
                    IpV6ExtensionHeaderMobilityExperimental experimentalHeader = (IpV6ExtensionHeaderMobilityExperimental)mobilityHeader;
                    field.AssertValue(experimentalHeader.MessageData);
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

                                    case "mip6.bri_seqnr":
                                        subfield.AssertShowDecimal(bindingRevocationIndicationMessage.SequenceNumber);
                                        break;

                                    case "mip6.bri_ip":
                                        subfield.AssertShowDecimal(bindingRevocationIndicationMessage.ProxyBinding);
                                        break;

                                    case "mip6.bri_iv":
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

                                    case "mip6.bri_seqnr":
                                        subfield.AssertShowDecimal(bindingRevocationAcknowledgementMessage.SequenceNumber);
                                        break;

                                    case "mip6.bri_ap":
                                        subfield.AssertShowDecimal(bindingRevocationAcknowledgementMessage.ProxyBinding);
                                        break;

                                    case "mip6.bri_av":
                                        subfield.AssertShowDecimal(bindingRevocationAcknowledgementMessage.IpV4HomeAddressBindingOnly);
                                        break;

                                    case "mip6.bri_ag":
                                        subfield.AssertShowDecimal(bindingRevocationAcknowledgementMessage.Global);
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
                                        subfield.AssertShowDecimal(careOfTestInit.CareOfInitCookie);
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
                                        subfield.AssertShowDecimal(careOfTest.CareOfInitCookie);
                                        break;

                                    case "mip6.hot.token":
                                        subfield.AssertShowDecimal(careOfTest.CareOfKeygenToken);
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
                                        subfield.AssertShowDecimal(homeTest.HomeInitCookie);
                                        break;

                                    case "mip6.hot.token":
                                        subfield.AssertShowDecimal(homeTest.HomeKeygenToken);
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
                                        subfield.AssertShowDecimal(homeTestInit.HomeInitCookie);
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
                                        subfield.AssertShowDecimal(bindingUpdate.ProxyRegistration);
                                        break;

                                    case "mip6.bu.f_flag":
                                        subfield.AssertShowDecimal(bindingUpdate.ForcingUdpEncapsulation);
                                        break;

                                    case "mip6.bu.t_flag":
                                        subfield.AssertShowDecimal(bindingUpdate.TypeLengthValueHeaderFormat);
                                        break;

                                    case "mip6.bu.b_flag":
                                        subfield.AssertShowDecimal(bindingUpdate.BulkBindingUpdate);
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
                                        subfield.AssertShowDecimal(bindingAcknowledgement.TypeLengthValueHeaderFormat);
                                        break;

                                    case "mip6.ba.b_flag":
                                        // TODO: Support Bulk Binding Update Support for Proxy Mobile IPv6 (RFC 6602).
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

                        case "Handover Acknowledge ":
                            var handoverAcknowledgeMessage = (IpV6ExtensionHeaderMobilityHandoverAcknowledgeMessage)mobilityHeader;
                            foreach (XElement subfield in field.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "mip6.hack.seqnr":
                                        subfield.AssertShowDecimal(handoverAcknowledgeMessage.SequenceNumber);
                                        break;

                                    case "mip6.hack.code":
                                        subfield.AssertShowDecimal((byte)handoverAcknowledgeMessage.Code);
                                        break;

                                    default:
                                        throw new InvalidOperationException(string.Format("Invalid IPv6 Handover Acknowledge mobility header field {0}", subfield.Name()));
                                }
                            }
                            break;

                        case "Handover Initiate":
                            var handoverInitiateMessage = (IpV6ExtensionHeaderMobilityHandoverInitiateMessage)mobilityHeader;
                            foreach (XElement subfield in field.Fields())
                            {
                                subfield.AssertNoFields();
                                switch (subfield.Name())
                                {
                                    case "mip6.hi.seqnr":
                                        subfield.AssertShowDecimal(handoverInitiateMessage.SequenceNumber);
                                        break;

                                    case "mip6.hi.s_flag":
                                        subfield.AssertShowDecimal(handoverInitiateMessage.AssignedAddressConfiguration);
                                        break;

                                    case "mip6.hi.u_flag":
                                        subfield.AssertShowDecimal(handoverInitiateMessage.Buffer);
                                        break;

                                    case "mip6.hi.code":
                                        subfield.AssertShowDecimal((byte)handoverInitiateMessage.Code);
                                        break;

                                    default:
                                        throw new InvalidOperationException(string.Format("Invalid IPv6 mobility header field {0}", subfield.Name()));
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
                                    case "":
                                        switch (option.OptionType)
                                        {
                                            case IpV6MobilityOptionType.LinkLayerAddress:
                                                optionField.AssertShow("Mobility Header Link-Layer Address");
                                                IpV6MobilityOptionLinkLayerAddress linkLayerAddress = (IpV6MobilityOptionLinkLayerAddress)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "":
                                                            optionSubfield.AssertShow("Mobility Header Link-Layer Address option");
                                                            foreach (XElement optionSubsubfield in optionSubfield.Fields())
                                                            {
                                                                if (HandleCommonMobilityOptionSubfield(optionSubsubfield, option))
                                                                    continue;

                                                                switch (optionSubsubfield.Name())
                                                                {
                                                                    case "mip6.lla.optcode":
                                                                        optionSubsubfield.AssertShowDecimal((byte)linkLayerAddress.Code);
                                                                        break;

                                                                    case "":
                                                                        // TODO: Uncomment when https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10627 is fixed.
//                                                                        optionSubsubfield.AssertValue(linkLayerAddress.LinkLayerAddress);
                                                                        break;

                                                                    default:
                                                                        throw new InvalidOperationException(string.Format(
                                                                            "Invalid IPv6 Link Layer Address option subfield {0}", optionSubsubfield.Name()));
                                                                }
                                                            }
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format(
                                                                "Invalid IPv6 Link Layer Address option field {0}", optionSubfield.Name()));
                                                    }
                                                }
                                                // TODO: Change to break when https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10627 is fixed.
                                                return false;

                                            case IpV6MobilityOptionType.IpV4DefaultRouterAddress:
                                                var ipV4DefaultRouterAddress = (IpV6MobilityOptionIpV4DefaultRouterAddress)option;
                                                optionField.AssertShow("IPv4 Default-Router Address: " + ipV4DefaultRouterAddress.DefaultRouterAddress);
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;
                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.ipv4dra.reserved":
                                                            optionSubfield.AssertShowDecimal(0);
                                                            break;

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
                                                if (optionField.Show() != "PadN" && optionIndex == mobilityHeader.MobilityOptions.Count - 1)
                                                {
                                                    Assert.IsFalse(mobilityHeader.IsValid);
                                                    return true;
                                                }
                                                optionField.AssertShow("PadN");
                                                IpV6MobilityOptionPadN padN = (IpV6MobilityOptionPadN)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;
                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "":
                                                            optionSubfield.AssertShow(string.Format("PadN: {0} bytes", padN.PaddingDataLength));
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.IpV4HomeAddressReply:
                                                IpV6MobilityOptionIpV4HomeAddressReply ipV4HomeAddressReply = (IpV6MobilityOptionIpV4HomeAddressReply)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;
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
                                                var ipV4HomeAddressRequest = (IpV6MobilityOptionIpV4HomeAddressRequest)option;
                                                optionField.AssertShow("IPv4 Home Address Request: " + ipV4HomeAddressRequest.HomeAddress);
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    ValidateIpV6MobilityOptionIpV4HomeAddressField(optionSubfield, ipV4HomeAddressRequest);
                                                }
                                                break;

                                            case IpV6MobilityOptionType.IpV4AddressAcknowledgement:
                                                optionField.AssertShow("IPv4 Address Acknowledgement");
                                                var ipV4AddressAcknowledgement = (IpV6MobilityOptionIpV4AddressAcknowledgement)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "":
                                                            optionSubfield.AssertShow("IPv4 Address Acknowledgement");
                                                            foreach (XElement optionSubsubfield in optionSubfield.Fields())
                                                            {
                                                                optionSubsubfield.AssertNoFields();
                                                                switch (optionSubsubfield.Name())
                                                                {
                                                                    case "mip6.ipv4aa.sts":
                                                                        optionSubsubfield.AssertShowDecimal((byte)ipV4AddressAcknowledgement.Status);
                                                                        break;

                                                                    default:
                                                                        ValidateIpV6MobilityOptionIpV4HomeAddressField(optionSubsubfield, ipV4AddressAcknowledgement);
                                                                        break;
                                                                }
                                                            }
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.MobileNetworkPrefix:
                                                optionField.AssertShow("Mobile Network Prefix");
                                                var mobileNetworkPrefix = (IpV6MobilityOptionMobileNetworkPrefix)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "":
                                                            optionSubfield.AssertShow("Mobile Network Prefix");
                                                            foreach (XElement optionSubsubfield in optionSubfield.Fields())
                                                            {
                                                                optionSubsubfield.AssertNoFields();
                                                                switch (optionSubsubfield.Name())
                                                                {
                                                                    case "mip6.nemo.mnp.mnp":
                                                                        optionSubsubfield.AssertShow(mobileNetworkPrefix.NetworkPrefix.ToString("x"));
                                                                        break;

                                                                    default:
                                                                        throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subsubfield {0}",
                                                                                                                          optionSubsubfield.Name()));
                                                                }
                                                            }
                                                            break;

                                                        case "mip6.nemo.mnp.pfl":
                                                            optionSubfield.AssertNoFields();
                                                            optionSubfield.AssertShowDecimal(mobileNetworkPrefix.PrefixLength);
                                                            break;

                                                        default:
                                                            optionSubfield.AssertNoFields();
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.HomeNetworkPrefix:
                                                optionField.AssertShow("Home Network Prefix");
                                                var homeNetworkPrefix = (IpV6MobilityOptionHomeNetworkPrefix)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "":
                                                            optionSubfield.AssertShow("Home Network Prefix");
                                                            foreach (XElement optionSubsubfield in optionSubfield.Fields())
                                                            {
                                                                switch (optionSubsubfield.Name())
                                                                {
                                                                    case "mip6.nemo.mnp.mnp":
                                                                        optionSubsubfield.AssertShow(homeNetworkPrefix.NetworkPrefix.ToString("x"));
                                                                        break;

                                                                    default:
                                                                        throw new InvalidOperationException(string.Format("Invalid IPv6 Network Prefix option subfield {0}", optionSubsubfield.Name()));
                                                                }
                                                            }
                                                            break;

                                                        case "mip6.nemo.mnp.pfl":
                                                            optionSubfield.AssertShowDecimal(homeNetworkPrefix.PrefixLength);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Network Prefix option field {0}", optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.VendorSpecific:
                                                Assert.IsTrue(optionField.Show().StartsWith("Vendor Specific: "));
                                                IpV6MobilityOptionVendorSpecific vendorSpecific = (IpV6MobilityOptionVendorSpecific)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;
                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.vsm.vendorId":
                                                            optionSubfield.AssertShowDecimal(vendorSpecific.VendorId);
                                                            break;

                                                        case "mip6.vsm.subtype":
                                                            optionSubfield.AssertShowDecimal(vendorSpecific.Subtype);
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

                                            case IpV6MobilityOptionType.NonceIndexes:
                                                optionField.AssertShow("Nonce Indices");
                                                IpV6MobilityOptionNonceIndexes nonceIndexes = (IpV6MobilityOptionNonceIndexes)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "":
                                                            optionSubfield.AssertShow("Nonce Indices");
                                                            foreach (XElement optionSubsubfield in optionSubfield.Fields())
                                                            {
                                                                optionSubsubfield.AssertNoFields();
                                                                switch (optionSubsubfield.Name())
                                                                {
                                                                    case "mip6.ni.hni":
                                                                        optionSubsubfield.AssertShowDecimal(nonceIndexes.HomeNonceIndex);
                                                                        break;

                                                                    case "mip6.ni.cni":
                                                                        optionSubsubfield.AssertShowDecimal(nonceIndexes.CareOfNonceIndex);
                                                                        break;

                                                                    default:
                                                                        throw new InvalidOperationException(string.Format("Invalid IPv6 Nonce Indices option subfield {0}",
                                                                                                                          optionSubsubfield.Name()));
                                                                }
                                                            }
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
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "":
                                                            optionSubfield.AssertShow("Link-local Address");
                                                            foreach (XElement optionSubsubfield in optionSubfield.Fields())
                                                            {
                                                                optionSubsubfield.AssertNoFields();
                                                                switch (optionSubsubfield.Name())
                                                                {
                                                                    case "mip6.lila_lla":
                                                                        optionSubsubfield.AssertShow(linkLocalAddress.LinkLocalAddress.ToString("x"));
                                                                        break;

                                                                    default:
                                                                        throw new InvalidOperationException(string.Format(
                                                                            "Invalid IPv6 Link-local Address option field {0}", optionSubsubfield.Name()));
                                                                }
                                                            }
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format(
                                                                "Invalid IPv6 Link-local Address option field {0}", optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.MobileNodeIdentifier:
                                                Assert.IsTrue(optionField.Show().StartsWith("Mobile Node Identifier"));
                                                IpV6MobilityOptionMobileNodeIdentifier mobileNodeIdentifier = (IpV6MobilityOptionMobileNodeIdentifier)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

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
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "":
                                                            foreach (XElement authSubfield in optionSubfield.Fields())
                                                            {
                                                                switch (authSubfield.Name())
                                                                {
                                                                    case "mip6.bad.auth":
                                                                        authSubfield.AssertValue(authorizationData.Authenticator);
                                                                        break;

                                                                    default:
                                                                        throw new InvalidOperationException(
                                                                            string.Format("Invalid IPv6 Authorization Data option subfield {0}",
                                                                                          authSubfield.Name()));
                                                                }
                                                            }
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
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "":
                                                            foreach (XElement optionSubsubfield in optionSubfield.Fields())
                                                            {
                                                                optionSubsubfield.AssertNoFields();
                                                                switch (optionSubsubfield.Name())
                                                                {
                                                                    case "mip6.ipv4ha.p_flag":
                                                                        optionSubsubfield.AssertShowDecimal(ipV4HomeAddress.RequestPrefix);
                                                                        break;

                                                                    default:
                                                                        ValidateIpV6MobilityOptionIpV4HomeAddressField(optionSubsubfield, ipV4HomeAddress);
                                                                        break;
                                                                }
                                                            }
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format(
                                                                "Invalid IPv6 Authorization Data option field {0}", optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.ServiceSelection:
                                                IpV6MobilityOptionServiceSelection serviceSelection = (IpV6MobilityOptionServiceSelection)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;
                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.ss.identifier":
                                                            optionSubfield.AssertValue(serviceSelection.Identifier);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.RedirectCapability:
                                                IpV6MobilityOptionRedirectCapability redirectCapability = (IpV6MobilityOptionRedirectCapability)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;
                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.recap.reserved":
                                                            optionSubfield.AssertShowDecimal(0);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.BindingIdentifier:
                                                IpV6MobilityOptionBindingIdentifier bindingIdentifier = (IpV6MobilityOptionBindingIdentifier)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;
                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.bi.bid":
                                                            optionSubfield.AssertShowDecimal(bindingIdentifier.BindingId);
                                                            break;

                                                        case "mip6.bi.status":
                                                            optionSubfield.AssertShowDecimal((byte)bindingIdentifier.Status);
                                                            break;

                                                        case "mip6.bi.h_flag":
                                                            optionSubfield.AssertShowDecimal(bindingIdentifier.SimultaneousHomeAndForeignBinding);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.LoadInformation:
                                                IpV6MobilityOptionLoadInformation loadInformation = (IpV6MobilityOptionLoadInformation)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.load_inf.priority":
                                                            optionSubfield.AssertShowDecimal(loadInformation.Priority);
                                                            break;

                                                        case "mip6.load_inf.sessions_in_use":
                                                            optionSubfield.AssertShowDecimal(loadInformation.SessionsInUse);
                                                            break;

                                                        case "mip6.load_inf.maximum_sessions":
                                                            optionSubfield.AssertShowDecimal(loadInformation.MaximumSessions);
                                                            break;

                                                        case "mip6.load_inf.used_capacity":
                                                            optionSubfield.AssertShowDecimal(loadInformation.UsedCapacity);
                                                            break;

                                                        case "mip6.load_inf.maximum_capacity":
                                                            optionSubfield.AssertShowDecimal(loadInformation.MaximumCapacity);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.IpV4CareOfAddress:
                                                IpV6MobilityOptionIpV4CareOfAddress ipV4CareOfAddress = (IpV6MobilityOptionIpV4CareOfAddress)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.ipv4coa.reserved":
                                                            optionSubfield.AssertShowDecimal(0);
                                                            break;

                                                        case "mip6.ipv4coa.addr":
                                                            optionSubfield.AssertShow(ipV4CareOfAddress.CareOfAddress.ToString());
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.CryptographicallyGeneratedAddressParametersRequest:
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.IpV6AddressPrefix:
                                                var ipV6AddressPrefix = (IpV6MobilityOptionIpV6AddressPrefix)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.mhipv6ap.opt_code":
                                                            optionSubfield.AssertShowDecimal((byte)ipV6AddressPrefix.Code);
                                                            break;

                                                        case "mip6.mhipv6ap.len":
                                                            optionSubfield.AssertShowDecimal(ipV6AddressPrefix.PrefixLength);
                                                            break;

                                                        case "mip6.mhipv6ap.ipv6_address":
                                                            optionSubfield.AssertValue(ipV6AddressPrefix.AddressOrPrefix.ToValue());
                                                            break;

                                                        case "mip6.mhipv6ap.ipv6_address_prefix":
                                                            Assert.IsTrue(optionSubfield.Value().EndsWith(ipV6AddressPrefix.AddressOrPrefix.ToValue().ToString("x32")));
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.MobileNodeGroupIdentifier:
                                                var mobileNodeGroupIdentifier = (IpV6MobilityOptionMobileNodeGroupIdentifier)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.mng.sub_type":
                                                            optionSubfield.AssertShowDecimal((byte)mobileNodeGroupIdentifier.Subtype);
                                                            break;

                                                        case "mip6.mng.reserved":
                                                            optionSubfield.AssertShowDecimal(0);
                                                            break;

                                                        case "mip6.mng._mng_id":
                                                            optionSubfield.AssertShowDecimal(mobileNodeGroupIdentifier.MobileNodeGroupIdentifier);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.BindingAuthorizationDataForFmIpV6:
                                                var bindingAuthorizationDataForFmIpV6 = (IpV6MobilityOptionBindingAuthorizationDataForFmIpV6)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.badff.spi":
                                                            optionSubfield.AssertShowDecimal(bindingAuthorizationDataForFmIpV6.SecurityParameterIndex);
                                                            break;

                                                        case "mip6.badff.auth":
                                                            optionSubfield.AssertValue(bindingAuthorizationDataForFmIpV6.Authenticator);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.MobileNodeLinkLayerIdentifier:
                                                var mobileNodeLinkLayerIdentifier = (IpV6MobilityOptionMobileNodeLinkLayerIdentifier)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.mnlli.reserved":
                                                            optionSubfield.AssertShowDecimal(0);
                                                            break;

                                                        case "mip6.mnlli.lli":
                                                            optionSubfield.AssertValue(mobileNodeLinkLayerIdentifier.LinkLayerIdentifier);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.AccessTechnologyType:
                                                var accessTechnologyType = (IpV6MobilityOptionAccessTechnologyType)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.att.reserved":
                                                            optionSubfield.AssertShowDecimal(0);
                                                            break;

                                                        case "mip6.att":
                                                            optionSubfield.AssertShowDecimal((byte)accessTechnologyType.AccessTechnologyType);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.AccessNetworkIdentifier:
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "_ws.expert":
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.BindingRefreshAdvice:
                                                var bindingRefreshAdvice = (IpV6MobilityOptionBindingRefreshAdvice)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.bra.interval":
                                                            optionSubfield.AssertShowDecimal(bindingRefreshAdvice.RefreshInterval);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.PermanentHomeKeygenToken:
                                                var permanentHomeKeygenToken = (IpV6MobilityOptionPermanentHomeKeygenToken)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.phkt.phkt":
                                                            optionSubfield.AssertValue(permanentHomeKeygenToken.PermanentHomeKeygenToken);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.Redirect:
                                                var redirect = (IpV6MobilityOptionRedirect)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.redir.k":
                                                            optionSubfield.AssertShowDecimal(redirect.LocalMobilityAddressIpV6 != null);
                                                            break;

                                                        case "mip6.redir.n":
                                                            optionSubfield.AssertShowDecimal(redirect.LocalMobilityAddressIpV4 != null);
                                                            break;

                                                        case "mip6.redir.reserved":
                                                            optionSubfield.AssertShowDecimal(0);
                                                            break;

                                                        case "mip6.redir.addr_r2lma_ipv4":
                                                            optionSubfield.AssertShow(redirect.LocalMobilityAddressIpV4.ToString());
                                                            break;

                                                        case "mip6.redir.addr_r2lma_ipv6":
                                                            optionSubfield.AssertValue(redirect.LocalMobilityAddressIpV6.Value.ToValue());
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.CareOfTest:
                                                var careOfTestOption = (IpV6MobilityOptionCareOfTest)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.mocot.co_keygen_tok":
                                                            optionSubfield.AssertValue(careOfTestOption.CareOfKeygenToken);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.ReplayProtection:
                                                var replayProtection = (IpV6MobilityOptionReplayProtection)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.mseg_id.timestamp":
                                                            optionSubfield.AssertValue(replayProtection.Timestamp);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.Authentication:
                                                var authentication = (IpV6MobilityOptionAuthentication)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.auth.subtype":
                                                            optionSubfield.AssertShowDecimal((byte)authentication.Subtype);
                                                            break;

                                                        case "mip6.auth.mobility_spi":
                                                            optionSubfield.AssertShowDecimal(authentication.MobilitySecurityParameterIndex);
                                                            break;

                                                        case "mip6.auth.auth_data":
                                                            optionSubfield.AssertValue(authentication.AuthenticationData);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.HandoffIndicator:
                                                var handoffIndicator = (IpV6MobilityOptionHandoffIndicator)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.hi.reserved":
                                                            optionSubfield.AssertShowDecimal(0);
                                                            break;

                                                        case "mip6.hi":
                                                            optionSubfield.AssertShowDecimal((byte)handoffIndicator.HandoffIndicator);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.IpV4DhcpSupportMode:
                                                var ipV4DhcpSupportMode = (IpV6MobilityOptionIpV4DhcpSupportMode)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.ipv4dsm.reserved":
                                                            optionSubfield.AssertShowDecimal(0);
                                                            break;

                                                        case "mip6.ipv4dsm.s_flag":
                                                            optionSubfield.AssertShowDecimal(ipV4DhcpSupportMode.IsServer);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.DnsUpdate:
                                                var dnsUpdate = (IpV6MobilityOptionDnsUpdate)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.dnsu.status":
                                                            optionSubfield.AssertShowDecimal((byte)dnsUpdate.Status);
                                                            break;

                                                        case "mip6.dnsu.flag.r":
                                                            optionSubfield.AssertShowDecimal(dnsUpdate.Remove);
                                                            break;

                                                        case "mip6.dnsu.mn_id":
                                                            optionSubfield.AssertValue(dnsUpdate.MobileNodeIdentity);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.LocalMobilityAnchorAddress:
                                                var localMobilityAnchorAddress = (IpV6MobilityOptionLocalMobilityAnchorAddress)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mmip6.lmaa.opt_code":
                                                            optionSubfield.AssertShowDecimal((byte)localMobilityAnchorAddress.Code);
                                                            break;

                                                        case "mmip6.lmaa.reserved":
                                                            optionSubfield.AssertShowDecimal(0);
                                                            break;

                                                        case "mip6.lmaa.ipv6":
                                                            optionSubfield.AssertValue(localMobilityAnchorAddress.LocalMobilityAnchorAddressIpV6.Value.ToValue());
                                                            break;

                                                        case "mip6.lmaa.ipv4":
                                                            // TODO: Uncomment when https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=10961 is fixed.
//                                                            optionSubfield.AssertShow(localMobilityAnchorAddress.LocalMobilityAnchorAddressIpV4.Value.ToString());
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.ContextRequest:
                                                var contextRequest = (IpV6MobilityOptionContextRequest)option;
                                                optionField.AssertShow("Context Request" +
                                                                       (contextRequest.Requests.Any() ? "" : " (with option length = 2 bytes; should be >= 4)"));
                                                int requestIndex = 0;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.cr.reserved":
                                                            optionSubfield.AssertShowDecimal(0);
                                                            break;

                                                        case "mmip6.cr.req_type":
                                                            optionSubfield.AssertShowDecimal(contextRequest.Requests[requestIndex].RequestType);
                                                            break;

                                                        case "mmip6.cr.req_length":
                                                            optionSubfield.AssertShowDecimal(contextRequest.Requests[requestIndex].OptionLength);
                                                            if (contextRequest.Requests[requestIndex].OptionLength == 0)
                                                                ++requestIndex;
                                                            break;

                                                        case "mip6.vsm.vendorId":
                                                            optionSubfield.AssertValue(contextRequest.Requests[requestIndex].Option.Subsegment(0, 4));
                                                            break;

                                                        case "mip6.vsm.subtype":
                                                            optionSubfield.AssertValue(contextRequest.Requests[requestIndex].Option.Subsegment(4, 1));
                                                            ++requestIndex;
                                                            break;

                                                        case "":
                                                            optionSubfield.AssertValue(contextRequest.Requests[requestIndex].Option);
                                                            ++requestIndex;
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.AlternateIpV4CareOfAddress:
                                                var alternateIpV4CareOfAddress = (IpV6MobilityOptionAlternateIpV4CareOfAddress)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.alt_ip4":
                                                            optionSubfield.AssertShow(alternateIpV4CareOfAddress.AlternateCareOfAddress.ToString());
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.Signature:
                                                var signature = (IpV6MobilityOptionSignature)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.sign.sign":
                                                            optionSubfield.AssertValue(signature.Signature);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.TransientBinding:
                                                optionField.AssertShow("Transient Binding(2 bytes)");
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "_ws.expert":
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.Timestamp:
                                                var timestamp = (IpV6MobilityOptionTimestamp)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.timestamp_tmp":
                                                            optionSubfield.AssertValue(timestamp.Timestamp);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.RestartCounter:
                                                var restartCounter = (IpV6MobilityOptionRestartCounter)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.rc":
                                                            optionSubfield.AssertShowDecimal(restartCounter.RestartCounter);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.CryptographicallyGeneratedAddressParameters:
                                                var cryptographicallyGeneratedAddressParameters = (IpV6MobilityOptionCryptographicallyGeneratedAddressParameters)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.cgar.cga_par":
                                                            optionSubfield.AssertValue(cryptographicallyGeneratedAddressParameters.CryptographicallyGeneratedAddressParameters);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.FlowIdentification:
                                                optionField.AssertShow("Flow Identification(" + (option.Length - 2) + " bytes)");
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "_ws.expert":
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.NatDetection:
                                                optionField.AssertShow("NAT Detection");
                                                var natDetection = (IpV6MobilityOptionNatDetection)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.natd.f_flag":
                                                            optionSubfield.AssertShowDecimal(natDetection.UdpEncapsulationRequired);
                                                            break;

                                                        case "mip6.natd.reserved":
                                                            optionSubfield.AssertShowDecimal(0);
                                                            break;

                                                        case "mip6.natd.refresh_t":
                                                            optionSubfield.AssertShowDecimal(natDetection.RefreshTime);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.FlowSummary:
                                                optionField.AssertShow("Flow Summary(" + (option.Length - 2) + " bytes)");
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "_ws.expert":
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.Experimental:
                                                optionField.AssertShow("Experimental" +
                                                                       (option.Length == 2 ? " (with option length = 0 bytes; should be >= 1)" : ""));
                                                var experimental = (IpV6MobilityOptionExperimental)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.em.data":
                                                            optionSubfield.AssertValue(experimental.Data);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.MobileAccessGatewayIpV6Address:
                                                optionField.AssertShow("MAG IPv6 Address(18 bytes)");

                                                // TODO: Dedup this code.
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "_ws.expert":
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.CareOfTestInit:
                                                optionField.AssertShow("Care-of Test Init");
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    Assert.IsTrue(HandleCommonMobilityOptionSubfield(optionSubfield, option));
                                                }
                                                break;

                                            case IpV6MobilityOptionType.MobileNodeLinkLocalAddressInterfaceIdentifier:
                                                optionField.AssertShow("Mobile Node Link-local Address Interface Identifier(10 bytes)");

                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "_ws.expert":
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.AlternateCareOfAddress:
                                                optionField.AssertShow("Alternate Care-of Address");
                                                var alternateCareOfAddress = (IpV6MobilityOptionAlternateCareOfAddress)option;
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.acoa.acoa":
                                                            optionSubfield.AssertValue(alternateCareOfAddress.AlternateCareOfAddress.ToValue());
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
                                                break;

                                            case IpV6MobilityOptionType.GreKey:
                                                var greKey = (IpV6MobilityOptionGreKey)option;
                                                optionField.AssertShow("GRE Key: " + greKey.GreKeyIdentifier);
                                                foreach (XElement optionSubfield in optionField.Fields())
                                                {
                                                    if (HandleCommonMobilityOptionSubfield(optionSubfield, option))
                                                        continue;

                                                    optionSubfield.AssertNoFields();
                                                    switch (optionSubfield.Name())
                                                    {
                                                        case "mip6.ipv4dra.reserved":
                                                            optionSubfield.AssertShowDecimal(0);
                                                            break;

                                                        case "mip6.gre_key":
                                                            optionSubfield.AssertShowDecimal(greKey.GreKeyIdentifier);
                                                            break;

                                                        default:
                                                            throw new InvalidOperationException(string.Format("Invalid IPv6 Mobility option subfield {0}",
                                                                                                              optionSubfield.Name()));
                                                    }
                                                }
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
                                          mobilityHeader.MobilityHeaderType == IpV6MobilityHeaderType.HomeAgentSwitchMessage ||
                                          mobilityHeader.MobilityHeaderType == IpV6MobilityHeaderType.LocalizedRoutingInitiation ||
                                          mobilityHeader.MobilityHeaderType == IpV6MobilityHeaderType.LocalizedRoutingAcknowledgement);
                            field.AssertNoFields();
                            break;
                    }
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Invalid ipv6 mobility header field {0}", field.Name()));
            }

            return true;
        }

        private bool HandleCommonMobilityOptionSubfield(XElement optionSubfield, IpV6MobilityOption option)
        {
            switch (optionSubfield.Name())
            {
                case "mip6.mobility_opt":
                    optionSubfield.AssertNoFields();
                    optionSubfield.AssertShowDecimal((byte)option.OptionType);
                    return true;

                case "mip6.mobility_opt.len":
                    optionSubfield.AssertNoFields();
                    optionSubfield.AssertShowDecimal(option.Length - 2);
                    return true;

                default:
                    return false;
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