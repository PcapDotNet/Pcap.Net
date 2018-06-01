using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// </summary>
    public enum DhcpOptionCode : byte
    {
        #region 3. RFC 1497 Vendor Extensions

        /// <summary>
        /// RFC 2132.
        /// The pad option can be used to cause subsequent fields to align on
        /// word boundaries.
        /// </summary>
        Pad = 0,

        /// <summary>
        /// RFC 2132.
        /// The end option marks the end of valid information in the vendor field.
        /// </summary>
        End = 255,

        /// <summary>
        /// RFC 2132.
        /// The subnet mask option specifies the client's subnet mask as per RFC
        /// 950 [5].
        /// If both the subnet mask and the router option are specified in a DHCP
        /// reply, the subnet mask option MUST be first.
        /// </summary>
        SubnetMask = 1,

        /// <summary>
        /// RFC 2132.
        /// The time offset field specifies the offset of the client's subnet in
        /// seconds from Coordinated Universal Time(UTC).
        /// </summary>
        TimeOffset = 2,

        /// <summary>
        /// RFC 2132.
        /// The router option specifies a list of IP addresses for routers on the
        /// client's subnet. Routers SHOULD be listed in order of preference.
        /// </summary>
        Router = 3,

        /// <summary>
        /// RFC 2132.
        /// The time server option specifies a list of RFC 868 [6] time servers
        /// available to the client.Servers SHOULD be listed in order of
        /// preference.
        /// </summary>
        TimeServer = 4,

        /// <summary>
        /// RFC 2132.
        /// The name server option specifies a list of IEN 116 [7] name servers
        /// available to the client.Servers SHOULD be listed in order of preference.
        /// </summary>
        NameServer = 5,

        /// <summary>
        /// RFC 2132.
        /// The domain name server option specifies a list of Domain Name System
        /// (STD 13, RFC 1035 [8]) name servers available to the client. Servers
        /// SHOULD be listed in order of preference.
        /// </summary>
        DomainNameServerServer = 6,

        /// <summary>
        /// RFC 2132.
        /// The log server option specifies a list of MIT-LCS UDP log servers
        /// available to the client
        /// </summary>
        LogServer = 7,

        /// <summary>
        /// RFC 2132.
        /// The cookie server option specifies a list of RFC 865 [9] cookie
        /// servers available to the client. Servers SHOULD be listed in order
        /// of preference.
        /// </summary>
        CookieServer = 8,

        /// <summary>
        /// RFC 2132.
        /// The LPR server option specifies a list of RFC 1179 [10] line printer
        /// servers available to the client. Servers SHOULD be listed in order
        /// of preference.
        /// </summary>
        LprServer = 9,

        /// <summary>
        /// RFC 2132.
        /// The Impress server option specifies a list of Imagen Impress servers
        /// available to the client.Servers SHOULD be listed in order of
        /// preference.
        /// </summary>
        ImpressServer = 10,

        /// <summary>
        /// RFC 2132.
        /// This option specifies a list of RFC 887 [11] Resource Location
        /// servers available to the client.Servers SHOULD be listed in order
        /// of preference.
        /// </summary>
        ResourceLocationServer = 11,

        /// <summary>
        /// RFC 2132.
        /// This option specifies the name of the client. The name may or may
        /// not be qualified with the local domain name(see section 3.17 for the
        /// preferred way to retrieve the domain name). See RFC 1035 for
        /// character set restrictions.
        /// </summary>
        HostName = 12,

        /// <summary>
        /// RFC 2132.
        /// This option specifies the length in 512-octet blocks of the default
        /// boot image for the client.
        /// </summary>
        BootFileSize = 13,

        /// <summary>
        /// RFC 2132.
        /// This option specifies the path-name of a file to which the client's
        /// core image should be dumped in the event the client crashes.
        /// </summary>
        MeritDumpFile = 14,

        /// <summary>
        /// RFC 2132.
        /// This option specifies the domain name that client should use when
        /// resolving hostnames via the Domain Name System.
        /// </summary>
        DomainName = 15,

        /// <summary>
        /// RFC 2132.
        /// This specifies the IP address of the client's swap server.
        /// </summary>
        SwapServer = 16,

        /// <summary>
        /// RFC 2132.
        /// This option specifies the path-name that contains the client's root
        /// disk.
        /// </summary>
        RootPath = 17,

        /// <summary>
        /// RFC 2132.
        /// A string to specify a file, retrievable via TFTP, which contains
        /// information which can be interpreted in the same way as the 64-octet
        /// vendor-extension field within the BOOTP response, with the following
        /// exceptions:
        /// - the length of the file is unconstrained;
        /// - all references to Tag 18 (i.e., instances of the BOOTP Extensions Path field) within the file are ignored.
        /// </summary>
        ExtensionsPath = 18,

        #endregion 3. RFC 1497 Vendor Extensions

        #region 4. IP Layer Parameters per Host

        /// <summary>
        /// RFC 2132.
        /// This option specifies whether the client should configure its IP
        /// layer for packet forwarding.
        /// </summary>
        IPForwardingEnable = 19,

        /// <summary>
        /// RFC 2132.
        /// This option specifies whether the client should configure its IP
        /// layer to allow forwarding of datagrams with non-local source routes.
        /// </summary>
        NonLocalSourceRoutingEnable = 20,

        /// <summary>
        /// RFC 2132.
        /// This option specifies policy filters for non-local source routing.
        /// The filters consist of a list of IP addresses and masks which specify
        /// destination/mask pairs with which to filter incoming source routes.
        /// Any source routed datagram whose next-hop address does not match one
        /// of the filters should be discarded by the client.
        /// </summary>
        PolicyFilter = 21,

        /// <summary>
        /// RFC 2132.
        /// This option specifies the maximum size datagram that the client
        /// should be prepared to reassemble.unsigned integer.
        /// </summary>
        MaximumDatagramReassemblySize = 22,

        /// <summary>
        /// RFC 2132.
        /// This option specifies the default time-to-live that the client should
        /// use on outgoing datagrams.
        /// </summary>
        DefaultIpTimeToLive = 23,

        /// <summary>
        /// RFC 2132.
        /// This option specifies the timeout (in seconds) to use when aging Path
        /// MTU values discovered by the mechanism defined in RFC 1191 [12].
        /// </summary>
        PathMtuAgingTimeout = 24,

        /// <summary>
        /// RFC 2132.
        /// This option specifies a table of MTU sizes to use when performing
        /// Path MTU Discovery as defined in RFC 1191.  The table is formatted as
        /// a list of 16-bit unsigned integers, ordered from smallest to largest.
        /// </summary>
        PathMtuPlateauTable = 25,

        #endregion 4. IP Layer Parameters per Host

        #region 5. IP Layer Parameters per Interface

        /// <summary>
        /// RFC 2132.
        /// This option specifies the MTU to use on this interface.
        /// </summary>
        InterfaceMtu = 26,

        /// <summary>
        /// RFC 2132.
        /// This option specifies whether or not the client may assume that all
        /// subnets of the IP network to which the client is connected use the
        /// same MTU as the subnet of that network to which the client is
        /// directly connected.
        /// </summary>
        AllSubnetsAreLocal = 27,

        /// <summary>
        /// RFC 2132.
        /// This option specifies the broadcast address in use on the client's
        /// subnet.
        /// </summary>
        BroadcastAddress = 28,

        /// <summary>
        /// RFC 2132.
        /// This option specifies whether or not the client should perform subnet
        /// mask discovery using ICMP.
        /// </summary>
        PerformMaskDiscovery = 29,

        /// <summary>
        /// RFC 2132.
        /// This option specifies whether or not the client should respond to
        /// subnet mask requests using ICMP.
        /// </summary>
        MaskSupplier = 30,

        /// <summary>
        /// RFC 2132.
        /// This option specifies whether or not the client should solicit
        /// routers using the Router Discovery mechanism defined in RFC 1256.
        /// </summary>
        PerformRouterDiscovery = 31,

        /// <summary>
        /// RFC 2132.
        /// This option specifies the address to which the client should transmit
        /// router solicitation requests.
        /// </summary>
        RouterSolicitationAddress = 32,

        /// <summary>
        /// RFC 2132.
        /// This option specifies a list of static routes that the client should
        /// install in its routing cache.If multiple routes to the same
        /// destination are specified, they are listed in descending order of
        /// priority.
        /// </summary>
        StaticRoute = 33,

        #endregion 5. IP Layer Parameters per Interface

        #region 6. Link Layer Parameters per Interface

        /// <summary>
        /// RFC 2132.
        /// This option specifies whether or not the client should negotiate the
        /// use of trailers(RFC 893 [14]) when using the ARP protocol.
        /// </summary>
        TrailerEncapsulation = 34,

        /// <summary>
        /// RFC 2132.
        /// This option specifies the timeout in seconds for ARP cache entries.
        /// </summary>
        ArpCacheTimeout = 35,

        /// <summary>
        /// RFC 2132.
        /// This option specifies whether or not the client should use Ethernet
        /// Version 2 (RFC 894 [15]) or IEEE 802.3 (RFC 1042 [16]) encapsulation
        /// if the interface is an Ethernet.
        /// </summary>
        EthernetEncapsulation = 36,

        #endregion 6. Link Layer Parameters per Interface

        #region 7. TCP Parameters

        /// <summary>
        /// RFC 2132.
        /// This option specifies the default TTL that the client should use when
        /// sending TCP segments.
        /// </summary>
        TcpDefaultTtl = 37,

        /// <summary>
        /// RFC 2132.
        /// This option specifies the interval (in seconds) that the client TCP
        /// should wait before sending a keepalive message on a TCP connection.
        /// </summary>
        TcpKeepaliveInterval = 38,

        /// <summary>
        /// RFC 2132.
        /// This option specifies the whether or not the client should send TCP
        /// keepalive messages with a octet of garbage for compatibility with
        /// older implementations.
        /// </summary>
        TcpKeepaliveGarbage = 39,

        #endregion 7. TCP Parameters

        #region 8. Application and Service Parameters

        /// <summary>
        /// RFC 2132.
        /// This option specifies the name of the client's NIS [17] domain.
        /// </summary>
        NetworkInformationServiceDomain = 40,

        /// <summary>
        /// RFC 2132.
        /// This option specifies a list of IP addresses indicating NIS servers
        /// available to the client.Servers SHOULD be listed in order of
        /// </summary>
        NetworkInformationServers = 41,

        /// <summary>
        /// RFC 2132.
        /// This option specifies a list of IP addresses indicating NTP [18]
        /// servers available to the client.Servers SHOULD be listed in order
        /// of preference.
        /// </summary>
        NetworkTimeProtocolServers = 42,

        /// <summary>
        /// RFC 2132.
        /// This option is used by clients and servers to exchange vendor-
        /// specific information. The information is an opaque object of n
        /// octets, presumably interpreted by vendor-specific code on the clients
        /// and servers. The definition of this information is vendor specific.
        /// The vendor is indicated in the vendor class identifier option.
        /// Servers not equipped to interpret the vendor-specific information
        /// sent by a client MUST ignore it(although it may be reported).
        /// Clients which do not receive desired vendor-specific information
        /// SHOULD make an attempt to operate without it, although they may do so
        /// (and announce they are doing so) in a degraded mode.
        /// </summary>
        VendorSpecificInformation = 43,

        /// <summary>
        /// RFC 2132.
        /// The NetBIOS name server (NBNS) option specifies a list of RFC 1001/1002 [19] [20]
        /// NBNS name servers listed in order of preference.
        /// </summary>
        NetBiosOverTcpIpNameServer = 44,

        /// <summary>
        /// RFC 2132.
        /// The NetBIOS datagram distribution server (NBDD) option specifies a
        /// list of RFC 1001/1002 NBDD servers listed in order of preference.
        /// </summary>
        NetBiosOverTcpIpDatagramDistributionServer = 45,

        /// <summary>
        /// RFC 2132.
        /// The NetBIOS node type option allows NetBIOS over TCP/IP clients which
        /// are configurable to be configured as described in RFC 1001/1002.
        /// </summary>
        NetBiosOverTcpIpNodeType = 46,

        /// <summary>
        /// RFC 2132.
        /// The NetBIOS scope option specifies the NetBIOS over TCP/IP scope
        /// parameter for the client as specified in RFC 1001/1002.
        /// </summary>
        NetBiosOverTcpIpScope = 47,

        /// <summary>
        /// RFC 2132.
        /// This option specifies a list of X Window System [21] Font servers
        /// available to the client. Servers SHOULD be listed in order of
        /// preference.
        /// </summary>
        XWindowSystemFontServer = 48,

        /// <summary>
        /// RFC 2132.
        /// This option specifies a list of IP addresses of systems that are
        /// running the X Window System Display Manager and are available to the
        /// client.
        /// </summary>
        XWindowSystemDisplayManager = 49,

        /// <summary>
        /// RFC 2132.
        /// This option specifies the name of the client's NIS+ [17] domain.
        /// </summary>
        NetworkInformationServicePlusDomain = 64,

        /// <summary>
        /// RFC 2132.
        /// This option specifies a list of IP addresses indicating NIS+ servers
        /// available to the client.Servers SHOULD be listed in order of
        /// preference.
        /// </summary>
        NetworkInformationServicePlusServers = 65,

        /// <summary>
        /// RFC 2132.
        /// This option specifies a list of IP addresses indicating mobile IP
        /// home agents available to the client. Agents SHOULD be listed in
        /// order of preference.
        /// </summary>
        MobileIPHomeAgent = 68,

        /// <summary>
        /// RFC 2132.
        ///  The SMTP server option specifies a list of SMTP servers available to
        ///  the client. Servers SHOULD be listed in order of preference.
        /// </summary>
        SimpleMailTransportProtocolServer = 69,

        /// <summary>
        /// RFC 2132.
        /// The POP3 server option specifies a list of POP3 available to the
        /// client.Servers SHOULD be listed in order of preference.
        /// </summary>
        PostOfficeProtocolServer = 70,

        /// <summary>
        /// RFC 2132.
        /// The NNTP server option specifies a list of NNTP available to the
        /// client.Servers SHOULD be listed in order of preference.
        /// </summary>
        NetworkNewsTransportProtocolServer = 71,

        /// <summary>
        /// RFC 2132.
        /// The WWW server option specifies a list of WWW available to the
        /// client.Servers SHOULD be listed in order of preference.
        /// </summary>
        DefaultWorldWideWebServer = 72,

        /// <summary>
        /// RFC 2132.
        /// The Finger server option specifies a list of Finger available to the
        /// client.Servers SHOULD be listed in order of preference.
        /// </summary>
        DefaultFingerServer = 73,

        /// <summary>
        /// RFC 2132.
        /// The IRC server option specifies a list of IRC available to the
        /// client.Servers SHOULD be listed in order of preference.
        /// </summary>
        DefaultInternetRelayChatServer = 74,

        /// <summary>
        /// RFC 2132.
        /// The StreetTalk server option specifies a list of StreetTalk servers
        /// available to the client.Servers SHOULD be listed in order of
        /// preference.
        /// </summary>
        StreetTalkServer = 75,

        /// <summary>
        /// RFC 2132.
        /// The StreetTalk Directory Assistance (STDA) server option specifies a
        /// list of STDA servers available to the client.Servers SHOULD be
        /// listed in order of preference.
        /// </summary>
        StreetTalkDirectoryAssistanceServer = 76,

        #endregion 8. Application and Service Parameters

        #region 9. DHCP Extensions

        /// <summary>
        /// RFC 2132.
        /// This option is used in a client request (DHCPDISCOVER) to allow the
        /// client to request that a particular IP address be assigned.
        /// </summary>
        RequestedIPAddress = 50,

        /// <summary>
        /// RFC 2132.
        /// This option is used in a client request (DHCPDISCOVER or DHCPREQUEST)
        /// to allow the client to request a lease time for the IP address. In a
        /// server reply(DHCPOFFER), a DHCP server uses this option to specify
        /// the lease time it is willing to offer.
        /// </summary>
        IPAddressLeaseTime = 51,

        /// <summary>
        /// RFC 2132.
        /// This option is used to indicate that the DHCP 'sname' or 'file'
        /// fields are being overloaded by using them to carry DHCP options.A
        /// DHCP server inserts this option if the returned parameters will
        /// exceed the usual space allotted for options.
        /// If this option is present, the client interprets the specified
        /// additional fields after it concludes interpretation of the standard
        /// option fields.
        /// </summary>
        OptionOverload = 52,

        /// <summary>
        /// RFC 2132.
        /// This option is used to identify a TFTP server when the 'sname' field
        /// in the DHCP header has been used for DHCP options.
        /// </summary>
        TfptServerName = 66,

        /// <summary>
        /// RFC 2132.
        /// This option is used to identify a bootfile when the 'file' field in
        /// the DHCP header has been used for DHCP options.
        /// </summary>
        BootfileName = 67,

        /// <summary>
        /// RFC 2132.
        /// This option is used to convey the type of the DHCP message.
        /// </summary>
        MessageType = 53,

        /// <summary>
        /// RFC 2132.
        /// This option is used in DHCPOFFER and DHCPREQUEST messages, and may
        /// optionally be included in the DHCPACK and DHCPNAK messages.DHCP
        /// servers include this option in the DHCPOFFER in order to allow the
        /// client to distinguish between lease offers.  DHCP clients use the
        /// contents of the 'server identifier' field as the destination address
        /// for any DHCP messages unicast to the DHCP server.  DHCP clients also
        /// indicate which of several lease offers is being accepted by including
        /// this option in a DHCPREQUEST message.
        /// </summary>
        ServerIdentifier = 54,

        /// <summary>
        /// RFC 2132.
        /// This option is used by a DHCP client to request values for specified
        /// configuration parameters.The list of requested parameters is
        /// specified as n octets, where each octet is a valid DHCP option code
        /// as defined in this document.
        /// The client MAY list the options in order of preference.The DHCP
        /// server is not required to return the options in the requested order,
        /// but MUST try to insert the requested options in the order requested
        /// by the client.
        /// </summary>
        ParameterRequestList = 55,

        /// <summary>
        /// RFC 2132.
        /// This option is used by a DHCP server to provide an error message to a
        /// DHCP client in a DHCPNAK message in the event of a failure. A client
        /// may use this option in a DHCPDECLINE message to indicate the why the
        /// client declined the offered parameters.
        /// </summary>
        Message = 56,

        /// <summary>
        /// RFC 2132.
        /// This option specifies the maximum length DHCP message that it is
        /// willing to accept. A client may use the maximum DHCP message size option in
        /// DHCPDISCOVER or DHCPREQUEST messages, but should not use the option
        /// in DHCPDECLINE messages.
        /// </summary>
        MaximumDhcpMessageSize = 57,

        /// <summary>
        /// RFC 2132.
        /// This option specifies the time interval from address assignment until
        /// the client transitions to the RENEWING state.
        /// </summary>
        RenewalTimeValue = 58,

        /// <summary>
        /// RFC 2132.
        /// This option specifies the time interval from address assignment until
        /// the client transitions to the REBINDING state.
        /// </summary>
        RebindingTimeValue = 59,

        /// <summary>
        /// RFC 2132.
        /// This option is used by DHCP clients to optionally identify the vendor
        /// type and configuration of a DHCP client.
        /// </summary>
        VendorClassidentifier = 60,

        /// <summary>
        /// RFC 2132.
        /// This option is used by DHCP clients to specify their unique
        /// identifier.DHCP servers use this value to index their database of
        /// address bindings.  This value is expected to be unique for all
        /// clients in an administrative domain.
        /// </summary>
        ClientIdentifier = 61,

        #endregion 9. DHCP Extensions
    }
}