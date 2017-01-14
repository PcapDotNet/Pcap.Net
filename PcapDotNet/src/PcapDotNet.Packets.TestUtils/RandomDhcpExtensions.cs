using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using PcapDotNet.Base;
using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.Dhcp;
using PcapDotNet.Packets.Dhcp.Options;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.TestUtils
{
    [ExcludeFromCodeCoverage]
    public static class RandomDhcpExtensions
    {
        public static DhcpLayer NextDhcpLayer(this Random random)
        {
            const int MaxOptions = 10;

            DhcpLayer dhcpLayer = new DhcpLayer();
            dhcpLayer.MessageType = random.NextEnum<DhcpMessageType>();
            dhcpLayer.HardwareType = random.NextEnum<ArpHardwareType>(Enum.GetValues(typeof(ArpHardwareType)).Cast<ArpHardwareType>().Where(p => ((short)p) > byte.MaxValue));
            dhcpLayer.HardwareAddressLength = random.NextByte(16);
            dhcpLayer.Hops = random.NextByte();
            dhcpLayer.TransactionId = random.NextUInt();
            dhcpLayer.SecondsElapsed = random.NextUShort();
            dhcpLayer.DhcpFlags = random.NextEnum<DhcpFlags>();
            dhcpLayer.ClientIpAddress = random.NextIpV4Address();
            dhcpLayer.YourClientIpAddress = random.NextIpV4Address();
            dhcpLayer.NextServerIpAddress = random.NextIpV4Address();
            dhcpLayer.RelayAgentIpAddress = random.NextIpV4Address();
            dhcpLayer.ClientHardwareAddress = random.NextDataSegment(16);
            if (random.NextBool())
            {
                dhcpLayer.ServerHostName = random.NextCString(1, 64);
            }
            else
            {
                dhcpLayer.ServerHostName = "";
            }
            if (random.NextBool())
            {
                dhcpLayer.BootFileName = random.NextCString(1, 128);
            }
            else
            {
                dhcpLayer.BootFileName = "";
            }
            dhcpLayer.IsDhcp = random.NextBool();
            int numQueries = random.Next(MaxOptions + 1);
            List<DhcpOption> options = new List<DhcpOption>();
            for (int i = 0; i != numQueries; ++i)
                options.Add(random.NextDhcpOption());
            dhcpLayer.Options = options;

            return dhcpLayer;
        }

        public static DhcpOption NextDhcpOption(this Random random)
        {
            switch (random.NextEnum<DhcpOptionCode>())
            {
                #region 3. RFC 1497 Vendor Extensions

                case DhcpOptionCode.Pad:
                    return new DhcpPadOption();

                case DhcpOptionCode.End:
                    return new DhcpEndOption();

                case DhcpOptionCode.SubnetMask:
                    return new DhcpSubnetMaskOption(random.NextIpV4Address());

                case DhcpOptionCode.TimeOffset:
                    return new DhcpTimeOffsetOption(random.NextInt());

                case DhcpOptionCode.Router:
                    return new DhcpRouterOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.TimeServer:
                    return new DhcpTimeServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.NameServer:
                    return new DhcpNameServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.DomainNameServerServer:
                    return new DhcpDomainNameServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.LogServer:
                    return new DhcpLogServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.CookieServer:
                    return new DhcpCookieServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.LprServer:
                    return new DhcpLprServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.ImpressServer:
                    return new DhcpImpressServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.ResourceLocationServer:
                    return new DhcpResourceLocationServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.HostName:
                    return new DhcpHostNameOption(random.NextCString(1, 254));

                case DhcpOptionCode.BootFileSize:
                    return new DhcpBootFileSizeOption(random.NextUShort());

                case DhcpOptionCode.MeritDumpFile:
                    return new DhcpMeritDumpFileOption(random.NextCString(1, 254));

                case DhcpOptionCode.DomainName:
                    return new DhcpDomainNameOption(random.NextCString(1, 254));

                case DhcpOptionCode.SwapServer:
                    return new DhcpSwapServerOption(random.NextIpV4Address());

                case DhcpOptionCode.RootPath:
                    return new DhcpRootPathOption(random.NextCString(1, 254));

                case DhcpOptionCode.ExtensionsPath:
                    return new DhcpExtensionsPathOption(random.NextCString(1, 254));

                #endregion 3. RFC 1497 Vendor Extensions

                #region 4. IP Layer Parameters per Host

                case DhcpOptionCode.IPForwardingEnable:
                    return new DhcpIPForwardingEnableOption(random.NextBool());

                case DhcpOptionCode.NonLocalSourceRoutingEnable:
                    return new DhcpNonLocalSourceRoutingEnableOption(random.NextBool());

                case DhcpOptionCode.PolicyFilter:
                    return new DhcpPolicyFilterOption(Enumerable.Range(0, random.NextByte(DhcpPolicyFilterOption.IpV4AddressWithMask.SizeOf, byte.MaxValue) / DhcpPolicyFilterOption.IpV4AddressWithMask.SizeOf).Select(p => new DhcpPolicyFilterOption.IpV4AddressWithMask(random.NextIpV4Address(), random.NextIpV4Address())).ToList());

                case DhcpOptionCode.MaximumDatagramReassemblySize:
                    return new DhcpMaximumDatagramReassemblySizeOption(random.NextUShort(576, ushort.MaxValue));

                case DhcpOptionCode.DefaultIpTimeToLive:
                    return new DhcpDefaultIPTimeToLiveOption(random.NextByte());

                case DhcpOptionCode.PathMtuAgingTimeout:
                    return new DhcpPathMtuAgingTimeoutOption(random.NextUInt());

                case DhcpOptionCode.PathMtuPlateauTable:
                    return new DhcpPathMtuPlateauTableOption(Enumerable.Range(0, random.NextByte(sizeof(ushort), byte.MaxValue) / sizeof(ushort)).Select(p => random.NextUShort()).ToList());

                #endregion 4. IP Layer Parameters per Host

                #region 5. IP Layer Parameters per Interface

                case DhcpOptionCode.InterfaceMtu:
                    return new DhcpInterfaceMtuOption(random.NextUShort(68, ushort.MaxValue));

                case DhcpOptionCode.AllSubnetsAreLocal:
                    return new DhcpAllSubnetsAreLocalOption(random.NextBool());

                case DhcpOptionCode.BroadcastAddress:
                    return new DhcpBroadcastAddressOption(random.NextIpV4Address());

                case DhcpOptionCode.PerformMaskDiscovery:
                    return new DhcpPerformMaskDiscoveryOption(random.NextBool());

                case DhcpOptionCode.MaskSupplier:
                    return new DhcpMaskSupplierOption(random.NextBool());

                case DhcpOptionCode.PerformRouterDiscovery:
                    return new DhcpPerformRouterDiscoveryOption(random.NextBool());

                case DhcpOptionCode.RouterSolicitationAddress:
                    return new DhcpRouterSolicitationAddressOption(random.NextIpV4Address());

                case DhcpOptionCode.StaticRoute:
                    return new DhcpStaticRouteOption(Enumerable.Range(0, random.NextByte(IpV4Address.SizeOf * 2, byte.MaxValue) / IpV4Address.SizeOf / 2).Select(p => new DhcpStaticRouteOption.IpV4AddressRoute(random.NextIpV4Address(), random.NextIpV4Address())).ToList());

                #endregion 5. IP Layer Parameters per Interface

                #region 6. Link Layer Parameters per Interface

                case DhcpOptionCode.TrailerEncapsulation:
                    return new DhcpTrailerEncapsulationOption(random.NextBool());

                case DhcpOptionCode.ArpCacheTimeout:
                    return new DhcpArpCacheTimeoutOption(random.NextUInt());

                case DhcpOptionCode.EthernetEncapsulation:
                    return new DhcpEthernetEncapsulationOption(random.NextBool());

                #endregion 6. Link Layer Parameters per Interface

                #region 7. TCP Parameters

                case DhcpOptionCode.TcpDefaultTtl:
                    return new DhcpTcpDefaultTtlOption(random.NextByte());

                case DhcpOptionCode.TcpKeepaliveInterval:
                    return new DhcpTcpKeepaliveIntervalOption(random.NextUInt());

                case DhcpOptionCode.TcpKeepaliveGarbage:
                    return new DhcpTcpKeepaliveGarbageOption(random.NextBool());

                #endregion 7. TCP Parameters

                #region 8. Application and Service Parameters

                case DhcpOptionCode.NetworkInformationServiceDomain:
                    return new DhcpNetworkInformationServiceDomainOption(random.NextCString(1, 254));

                case DhcpOptionCode.NetworkInformationServers:
                    return new DhcpNetworkInformationServersOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.NetworkTimeProtocolServers:
                    return new DhcpNetworkTimeProtocolServersOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.VendorSpecificInformation:
                    return new DhcpVendorSpecificInformationOption(random.NextDataSegment(random.NextByte(1, 254)));

                case DhcpOptionCode.NetBiosOverTcpIpNameServer:
                    return new DhcpNetBiosOverTcpIpNameServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf))); ;

                case DhcpOptionCode.NetBiosOverTcpIpDatagramDistributionServer:
                    return new DhcpNetBiosOverTcpIpDatagramDistributionServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.NetBiosOverTcpIpNodeType:
                    DhcpNetBiosOverTcpIpNodeTypeOption.NodeType flag;
                    do
                    {
                        flag = random.NextFlags<DhcpNetBiosOverTcpIpNodeTypeOption.NodeType>();
                    } while (flag == 0);
                    return new DhcpNetBiosOverTcpIpNodeTypeOption(flag);

                case DhcpOptionCode.NetBiosOverTcpIpScope:
                    return new DhcpNetBiosOverTcpIpScopeOption(random.NextDataSegment(random.NextByte(1, 254)));

                case DhcpOptionCode.XWindowSystemFontServer:
                    return new DhcpXWindowSystemFontServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.XWindowSystemDisplayManager:
                    return new DhcpXWindowSystemDisplayManagerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.NetworkInformationServicePlusDomain:
                    return new DhcpNetworkInformationServicePlusDomainOption(random.NextCString(1, 254));

                case DhcpOptionCode.NetworkInformationServicePlusServers:
                    return new DhcpNetworkInformationServicePlusServersOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.MobileIPHomeAgent:
                    return new DhcpMobileIPHomeAgentOption(random.NextIpV4Addresses(random.Next(0, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.SimpleMailTransportProtocolServer:
                    return new DhcpSimpleMailTransportProtocolServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.PostOfficeProtocolServer:
                    return new DhcpPostOfficeProtocolServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.NetworkNewsTransportProtocolServer:
                    return new DhcpNetworkNewsTransportProtocolServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.DefaultWorldWideWebServer:
                    return new DhcpDefaultWorldWideWebServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.DefaultFingerServer:
                    return new DhcpDefaultFingerServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.DefaultInternetRelayChatServer:
                    return new DhcpDefaultInternetRelayChatServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.StreetTalkServer:
                    return new DhcpStreetTalkServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                case DhcpOptionCode.StreetTalkDirectoryAssistanceServer:
                    return new DhcpStreetTalkDirectoryAssistanceServerOption(random.NextIpV4Addresses(random.Next(1, byte.MaxValue / IpV4Address.SizeOf)));

                #endregion 8. Application and Service Parameters

                #region 9.DHCP Extensions

                case DhcpOptionCode.RequestedIPAddress:
                    return new DhcpRequestedIPAddressOption(random.NextIpV4Address());

                case DhcpOptionCode.IPAddressLeaseTime:
                    return new DhcpIPAddressLeaseTimeOption(random.NextUInt());

                case DhcpOptionCode.OptionOverload:
                    return new DhcpOptionOverloadOption(random.NextEnum<DhcpOptionOverloadOption.OptionOverloadValue>());

                case DhcpOptionCode.TfptServerName:
                    return new DhcpTFtpServerNameOption(random.NextCString(1, byte.MaxValue - 1));

                case DhcpOptionCode.BootfileName:
                    return new DhcpBootfileNameOption(random.NextCString(1, byte.MaxValue - 1));

                case DhcpOptionCode.MessageType:
                    return new DhcpMessageTypeOption(random.NextEnum<DhcpMessageTypeOption.MessageType>());

                case DhcpOptionCode.ServerIdentifier:
                    return new DhcpServerIdentifierOption(random.NextIpV4Address());

                case DhcpOptionCode.ParameterRequestList:
                    return new DhcpParameterRequestListOption(Enumerable.Range(0, random.NextByte(sizeof(DhcpOptionCode), byte.MaxValue) / sizeof(DhcpOptionCode)).Select(p => random.NextEnum<DhcpOptionCode>()).ToList());

                case DhcpOptionCode.Message:
                    return new DhcpMessageOption(random.NextCString(1, byte.MaxValue - 1));

                case DhcpOptionCode.MaximumDhcpMessageSize:
                    return new DhcpMaximumDhcpMessageSizeOption(random.NextUShort());

                case DhcpOptionCode.RenewalTimeValue:
                    return new DhcpRenewalTimeValueOption(random.NextUInt());

                case DhcpOptionCode.RebindingTimeValue:
                    return new DhcpRebindingTimeValueOption(random.NextUInt());

                case DhcpOptionCode.VendorClassidentifier:
                    return new DhcpVendorClassidentifierOption(random.NextDataSegment(random.NextByte(1, 254)));

                case DhcpOptionCode.ClientIdentifier:
                    return new DhcpClientIdentifierOption(random.NextByte(), random.NextDataSegment(random.NextByte(1, byte.MaxValue - 1)));

                #endregion 9.DHCP Extensions

                default:
                    throw new NotSupportedException();
            }
        }
    }
}