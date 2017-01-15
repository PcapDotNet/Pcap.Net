using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// Abstract class for all possible Dhcp-Options.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public abstract class DhcpOption : IEquatable<DhcpOption>
    {
        /// <summary>
        /// Option-Code according RFC 2132.
        /// </summary>
        public DhcpOptionCode OptionCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Length of the Dhcp-Option.
        /// </summary>
        public abstract byte Length
        {
            get;
        }

        /// <summary>
        /// create new Option.
        /// </summary>
        /// <param name="code">Option-Code</param>
        protected DhcpOption(DhcpOptionCode code)
        {
            OptionCode = code;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        internal static DhcpOption CreateInstance(DataSegment data, ref int offset)
        {
            switch ((DhcpOptionCode)data[offset++])
            {
                #region 3. RFC 1497 Vendor Extensions

                case DhcpOptionCode.Pad:
                    return DhcpPadOption.Read(data, ref offset);

                case DhcpOptionCode.End:
                    return DhcpEndOption.Read(data, ref offset);

                case DhcpOptionCode.SubnetMask:
                    return DhcpSubnetMaskOption.Read(data, ref offset);

                case DhcpOptionCode.TimeOffset:
                    return DhcpTimeOffsetOption.Read(data, ref offset);

                case DhcpOptionCode.Router:
                    return DhcpRouterOption.Read(data, ref offset);

                case DhcpOptionCode.TimeServer:
                    return DhcpTimeServerOption.Read(data, ref offset);

                case DhcpOptionCode.NameServer:
                    return DhcpNameServerOption.Read(data, ref offset);

                case DhcpOptionCode.DomainNameServerServer:
                    return DhcpDomainNameServerOption.Read(data, ref offset);

                case DhcpOptionCode.LogServer:
                    return DhcpLogServerOption.Read(data, ref offset);

                case DhcpOptionCode.CookieServer:
                    return DhcpCookieServerOption.Read(data, ref offset);

                case DhcpOptionCode.LprServer:
                    return DhcpLprServerOption.Read(data, ref offset);

                case DhcpOptionCode.ImpressServer:
                    return DhcpImpressServerOption.Read(data, ref offset);

                case DhcpOptionCode.ResourceLocationServer:
                    return DhcpResourceLocationServerOption.Read(data, ref offset);

                case DhcpOptionCode.HostName:
                    return DhcpHostNameOption.Read(data, ref offset);

                case DhcpOptionCode.BootFileSize:
                    return DhcpBootFileSizeOption.Read(data, ref offset);

                case DhcpOptionCode.MeritDumpFile:
                    return DhcpMeritDumpFileOption.Read(data, ref offset);

                case DhcpOptionCode.DomainName:
                    return DhcpDomainNameOption.Read(data, ref offset);

                case DhcpOptionCode.SwapServer:
                    return DhcpSwapServerOption.Read(data, ref offset);

                case DhcpOptionCode.RootPath:
                    return DhcpRootPathOption.Read(data, ref offset);

                case DhcpOptionCode.ExtensionsPath:
                    return DhcpExtensionsPathOption.Read(data, ref offset);

                #endregion 3. RFC 1497 Vendor Extensions

                #region 4. IP Layer Parameters per Host

                case DhcpOptionCode.IPForwardingEnable:
                    return DhcpIPForwardingEnableOption.Read(data, ref offset);

                case DhcpOptionCode.NonLocalSourceRoutingEnable:
                    return DhcpNonLocalSourceRoutingEnableOption.Read(data, ref offset);

                case DhcpOptionCode.PolicyFilter:
                    return DhcpPolicyFilterOption.Read(data, ref offset);

                case DhcpOptionCode.MaximumDatagramReassemblySize:
                    return DhcpMaximumDatagramReassemblySizeOption.Read(data, ref offset);

                case DhcpOptionCode.DefaultIpTimeToLive:
                    return DhcpDefaultIPTimeToLiveOption.Read(data, ref offset);

                case DhcpOptionCode.PathMtuAgingTimeout:
                    return DhcpPathMtuAgingTimeoutOption.Read(data, ref offset);

                case DhcpOptionCode.PathMtuPlateauTable:
                    return DhcpPathMtuPlateauTableOption.Read(data, ref offset);

                #endregion 4. IP Layer Parameters per Host

                #region 5. IP Layer Parameters per Interface

                case DhcpOptionCode.InterfaceMtu:
                    return DhcpInterfaceMtuOption.Read(data, ref offset);

                case DhcpOptionCode.AllSubnetsAreLocal:
                    return DhcpAllSubnetsAreLocalOption.Read(data, ref offset);

                case DhcpOptionCode.BroadcastAddress:
                    return DhcpBroadcastAddressOption.Read(data, ref offset);

                case DhcpOptionCode.PerformMaskDiscovery:
                    return DhcpPerformMaskDiscoveryOption.Read(data, ref offset);

                case DhcpOptionCode.MaskSupplier:
                    return DhcpMaskSupplierOption.Read(data, ref offset);

                case DhcpOptionCode.PerformRouterDiscovery:
                    return DhcpPerformRouterDiscoveryOption.Read(data, ref offset);

                case DhcpOptionCode.RouterSolicitationAddress:
                    return DhcpRouterSolicitationAddressOption.Read(data, ref offset);

                case DhcpOptionCode.StaticRoute:
                    return DhcpStaticRouteOption.Read(data, ref offset);

                #endregion 5. IP Layer Parameters per Interface

                #region 6. Link Layer Parameters per Interface

                case DhcpOptionCode.TrailerEncapsulation:
                    return DhcpTrailerEncapsulationOption.Read(data, ref offset);

                case DhcpOptionCode.ArpCacheTimeout:
                    return DhcpArpCacheTimeoutOption.Read(data, ref offset);

                case DhcpOptionCode.EthernetEncapsulation:
                    return DhcpEthernetEncapsulationOption.Read(data, ref offset);

                #endregion 6. Link Layer Parameters per Interface

                #region 7. TCP Parameters

                case DhcpOptionCode.TcpDefaultTtl:
                    return DhcpTcpDefaultTtlOption.Read(data, ref offset);

                case DhcpOptionCode.TcpKeepaliveInterval:
                    return DhcpTcpKeepaliveIntervalOption.Read(data, ref offset);

                case DhcpOptionCode.TcpKeepaliveGarbage:
                    return DhcpTcpKeepaliveGarbageOption.Read(data, ref offset);

                #endregion 7. TCP Parameters

                #region 8. Application and Service Parameters

                case DhcpOptionCode.NetworkInformationServiceDomain:
                    return DhcpNetworkInformationServiceDomainOption.Read(data, ref offset);

                case DhcpOptionCode.NetworkInformationServers:
                    return DhcpNetworkInformationServersOption.Read(data, ref offset);

                case DhcpOptionCode.NetworkTimeProtocolServers:
                    return DhcpNetworkTimeProtocolServersOption.Read(data, ref offset);

                case DhcpOptionCode.VendorSpecificInformation:
                    return DhcpVendorSpecificInformationOption.Read(data, ref offset);

                case DhcpOptionCode.NetBiosOverTcpIpNameServer:
                    return DhcpNetBiosOverTcpIpNameServerOption.Read(data, ref offset);

                case DhcpOptionCode.NetBiosOverTcpIpDatagramDistributionServer:
                    return DhcpNetBiosOverTcpIpDatagramDistributionServerOption.Read(data, ref offset);

                case DhcpOptionCode.NetBiosOverTcpIpNodeType:
                    return DhcpNetBiosOverTcpIpNodeTypeOption.Read(data, ref offset);

                case DhcpOptionCode.NetBiosOverTcpIpScope:
                    return DhcpNetBiosOverTcpIpScopeOption.Read(data, ref offset);

                case DhcpOptionCode.XWindowSystemFontServer:
                    return DhcpXWindowSystemFontServerOption.Read(data, ref offset);

                case DhcpOptionCode.XWindowSystemDisplayManager:
                    return DhcpXWindowSystemDisplayManagerOption.Read(data, ref offset);

                case DhcpOptionCode.NetworkInformationServicePlusDomain:
                    return DhcpNetworkInformationServicePlusDomainOption.Read(data, ref offset);

                case DhcpOptionCode.NetworkInformationServicePlusServers:
                    return DhcpNetworkInformationServicePlusServersOption.Read(data, ref offset);

                case DhcpOptionCode.MobileIPHomeAgent:
                    return DhcpMobileIPHomeAgentOption.Read(data, ref offset);

                case DhcpOptionCode.SimpleMailTransportProtocolServer:
                    return DhcpSimpleMailTransportProtocolServerOption.Read(data, ref offset);

                case DhcpOptionCode.PostOfficeProtocolServer:
                    return DhcpPostOfficeProtocolServerOption.Read(data, ref offset);

                case DhcpOptionCode.NetworkNewsTransportProtocolServer:
                    return DhcpNetworkNewsTransportProtocolServerOption.Read(data, ref offset);

                case DhcpOptionCode.DefaultWorldWideWebServer:
                    return DhcpDefaultWorldWideWebServerOption.Read(data, ref offset);

                case DhcpOptionCode.DefaultFingerServer:
                    return DhcpDefaultFingerServerOption.Read(data, ref offset);

                case DhcpOptionCode.DefaultInternetRelayChatServer:
                    return DhcpDefaultInternetRelayChatServerOption.Read(data, ref offset);

                case DhcpOptionCode.StreetTalkServer:
                    return DhcpStreetTalkServerOption.Read(data, ref offset);

                case DhcpOptionCode.StreetTalkDirectoryAssistanceServer:
                    return DhcpStreetTalkDirectoryAssistanceServerOption.Read(data, ref offset);

                #endregion 8. Application and Service Parameters

                #region 9.DHCP Extensions

                case DhcpOptionCode.RequestedIPAddress:
                    return DhcpRequestedIPAddressOption.Read(data, ref offset);

                case DhcpOptionCode.IPAddressLeaseTime:
                    return DhcpIPAddressLeaseTimeOption.Read(data, ref offset);

                case DhcpOptionCode.OptionOverload:
                    return DhcpOptionOverloadOption.Read(data, ref offset);

                case DhcpOptionCode.TfptServerName:
                    return DhcpTFtpServerNameOption.Read(data, ref offset);

                case DhcpOptionCode.BootfileName:
                    return DhcpBootfileNameOption.Read(data, ref offset);

                case DhcpOptionCode.MessageType:
                    return DhcpMessageTypeOption.Read(data, ref offset);

                case DhcpOptionCode.ServerIdentifier:
                    return DhcpServerIdentifierOption.Read(data, ref offset);

                case DhcpOptionCode.ParameterRequestList:
                    return DhcpParameterRequestListOption.Read(data, ref offset);

                case DhcpOptionCode.Message:
                    return DhcpMessageOption.Read(data, ref offset);

                case DhcpOptionCode.MaximumDhcpMessageSize:
                    return DhcpMaximumDhcpMessageSizeOption.Read(data, ref offset);

                case DhcpOptionCode.RenewalTimeValue:
                    return DhcpRenewalTimeValueOption.Read(data, ref offset);

                case DhcpOptionCode.RebindingTimeValue:
                    return DhcpRebindingTimeValueOption.Read(data, ref offset);

                case DhcpOptionCode.VendorClassidentifier:
                    return DhcpVendorClassidentifierOption.Read(data, ref offset);

                case DhcpOptionCode.ClientIdentifier:
                    return DhcpClientIdentifierOption.Read(data, ref offset);

                #endregion 9.DHCP Extensions

                default:
                    return DhcpAnyOption.Read(data, ref offset);
            }
        }

        internal virtual void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(offset++, (byte)OptionCode);
            buffer.Write(offset++, Length);
        }

        /// <summary>
        /// Two options objects are equal if they have the same parameters.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DhcpOption);
        }

        /// <summary>
        /// Two options objects are equal if they have the same parameters.
        /// </summary>
        public bool Equals(DhcpOption other)
        {
            if (other == null)
                return false;

            if (OptionCode != other.OptionCode)
                return false;

            if (Length != other.Length)
                return false;

            //we compare the output of write

            byte[] selfData = new byte[2 + Length];
            byte[] otherData = new byte[2 + other.Length];
            int offset = 0;
            Write(selfData, ref offset);
            offset = 0;
            other.Write(otherData, ref offset);

            return selfData.SequenceEqual(otherData);
        }

        /// <summary>
        /// calculate a hash of the option.
        /// </summary>
        /// <returns>a hash representing this instance</returns>
        public override int GetHashCode()
        {
            byte[] selfData = new byte[2 + Length];
            int offset = 0;
            Write(selfData, ref offset);

            return Sequence.GetHashCode(selfData.Cast<object>().ToArray());
        }
    }
}