namespace PcapDotNet.Packets.IpV6
{
    public enum IpV6MobilityOptionType : byte
    {
        /// <summary>
        /// RFC 6275.
        /// </summary>
        Pad1 = 0x00,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        PadN = 0x01,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        BindingRefreshAdvice = 0x02,

        /// <summary>
        /// RFC 6275.
        /// Alternate Care-of Address.
        /// </summary>
        AlternateCareOfAddress = 0x03,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        NonceIndices = 0x04,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        BindingAuthorizationData = 0x05,

        /// <summary>
        /// RFC 3963.
        /// Mobile Network Prefix Option.
        /// </summary>
        MobileNetworkPrefix = 0x06,

        /// <summary>
        /// RFC 5568.
        /// Mobility Header Link-Layer Address option.
        /// </summary>
        LinkLayerAddress = 0x07,

        /// <summary>
        /// RFC 4283.
        /// MN-ID-OPTION-TYPE.
        /// </summary>
        MobileNodeIdentifier = 0x08,

        /// <summary>
        /// RFC 4285.
        /// AUTH-OPTION-TYPE.
        /// </summary>
        Authentication = 0x09,

        /// <summary>
        /// RFC 4285.
        /// MESG-ID-OPTION-TYPE
        /// </summary>
        ReplayProtection = 0x0A,

        /// <summary>
        /// RFC 4866.
        /// CGA Parameters Request.
        /// </summary>
        CgaParametersRequest = 0x0B,

        /// <summary>
        /// RFC 4866.
        /// CGA Parameters.
        /// </summary>
        CgaParameters = 0x0C,

        /// <summary>
        /// RFC 4866.
        /// </summary>
        Signature = 0x0D,

        /// <summary>
        /// RFC 4866.
        /// </summary>
        PermanentHomeKeygenToken = 0x0E,

        /// <summary>
        /// RFC 4866.
        /// Care-of Test Init
        /// </summary>
        CareOfTestInit = 0x0F,

        /// <summary>
        /// RFC 4866.
        /// Care-of Test.
        /// </summary>
        CareOfTest = 0x10,

        /// <summary>
        /// RFC 5026.
        /// DNS-UPDATE-TYPE.
        /// </summary>
        DnsUpdate = 0x11,

        /// <summary>
        /// RFC 5096.
        /// Experimental Mobility Option.
        /// </summary>
        Experimental = 0x12,
        
        /// <summary>
        /// RFC 5094.
        /// Vendor Specific Mobility Option.
        /// </summary>
        VendorSpecific = 0x13,

        /// <summary>
        /// RFC 5149.
        /// </summary>
        ServiceSelection = 0x14,

        /// <summary>
        /// RFC 5568.
        /// Binding Authorization Data for FMIPv6 (BADF).
        /// </summary>
        BindingAuthorizationDataForFmIpV6 = 0x15,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        HomeNetworkPrefix = 0x16,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        HandoffIndicator = 0x17,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        AccessTechnologyType = 0x18,

        /// <summary>
        /// RFC 5213.
        /// Mobile Node Link-layer Identifier Option
        /// </summary>
        MobileNodeLinkLayerIdentifier = 0x19,

        /// <summary>
        /// RFC 5213.
        /// Link-local Address Option.
        /// </summary>
        LinkLocalAddress = 0x1A,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        Timestamp = 0x1B,

        /// <summary>
        /// RFC 5847.
        /// </summary>
        RestartCounter = 0x1C,

        /// <summary>
        /// RFC 5555.
        /// IPv4 Home Address.
        /// </summary>
        IpV4HomeAddress = 0x1D,

        /// <summary>
        /// RFC 5555.
        /// IPv4 Address Acknowledgement.
        /// </summary>
        IpV4AddressAcknowledgement = 0x1E,

        /// <summary>
        /// RFC 5555.
        /// NAT Detection.
        /// </summary>
        NatDetection = 0x1F,

        /// <summary>
        /// RFC 5555.
        /// IPv4 Care-of Address.
        /// </summary>
        IpV4CareOfAddress = 0x20,

        /// <summary>
        /// RFC 5845.
        /// GRE Key Option.
        /// </summary>
        GreKey = 0x21,

        /// <summary>
        /// RFC 5568.
        /// Mobility Header IPv6 Address/Prefix.
        /// </summary>
        IpV6AddressPrefix = 0x22,

        /// <summary>
        /// RFC 5648.
        /// </summary>
        BindingIdentifier = 0x23,

        /// <summary>
        /// RFC5844.
        /// IPv4 Home Address Request.
        /// </summary>
        IpV4HomeAddressRequest = 0x24,

        /// <summary>
        /// RFC 5844.
        /// IPv4 Home Address Reply.
        /// </summary>
        IpV4HomeAddressReply = 0x25,

        /// <summary>
        /// RFC 5844.
        /// IPv4 Default-Router Address.
        /// </summary>
        IpV4DefaultRouterAddress = 0x26,

        /// <summary>
        /// RFC 5844.
        /// IPv4 DHCP Support Mode.
        /// </summary>
        IpV4DhcpSupportMode = 0x27,

        /// <summary>
        /// RFC 5949.
        /// </summary>
        ContextRequest = 0x28,

        /// <summary>
        /// RFC 5949.
        /// </summary>
        LocalMobilityAnchorAddress = 0x29,

        /// <summary>
        /// RFC 5949.
        /// Mobile Node Link-local Address Interface Identifier Option.
        /// </summary>
        MobileNodeLinkLocalAddressInterfaceIdentifier = 0x2A,

        /// <summary>
        /// RFC 6058.
        /// </summary>
        TransientBinding = 0x2B,

        /// <summary>
        /// RFC 6089
        /// </summary>
        FlowSummary = 0x2C,

        /// <summary>
        /// RFC 6089.
        /// </summary>
        FlowIdentification = 0x2D,

        /// <summary>
        /// RFC 6463.
        /// Redirect-Capability Mobility Option.
        /// </summary>
        RedirectCapability = 0x2E,

        /// <summary>
        /// RFC 6463.
        /// </summary>
        Redirect = 0x2F,

        /// <summary>
        /// RFC 6463.
        /// </summary>
        LoadInformation = 0x30,

        /// <summary>
        /// RFC 6463.
        /// Alternate IPv4 Care-of Address.
        /// </summary>
        AlternateIpV4CareOfAddress = 0x31,

        /// <summary>
        /// RFC 6602.
        /// </summary>
        MobileNodeGroupIdentifier = 0x32,

        /// <summary>
        /// RFC 6705.
        /// MAG IPv6 Address.
        /// </summary>
        MobileAccessGatewayIpV6Address = 0x33,

        /// <summary>
        /// RFC 6757.
        /// </summary>
        AccessNetworkIdentifier = 0x34,
    }
}