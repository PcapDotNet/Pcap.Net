namespace PcapDotNet.Packets.IpV6
{
    public enum IpV6BindingAcknowledgementStatus : byte
    {
        /// <summary>
        /// RFCs 5213, 5568, 6275.
        /// </summary>
        BindingUpdateAccepted = 0,

        /// <summary>
        /// RFCs 5568, 6275.
        /// Binding Acknowledgement: Prefix discovery necessary.
        /// Fast Binding Acknowledgement: NCoA is invalid. Use NCoA supplied in "alternate" CoA.
        /// </summary>
        AcceptedBut = 1,

        /// <summary>
        /// RFC 5845.
        /// </summary>
        GreKeyOptionNotRequired = 2,

        /// <summary>
        /// RFC 5845.
        /// </summary>
        GreTunnelingButTlvHeaderNotSupported = 3,

        /// <summary>
        /// RFC 5648.
        /// </summary>
        McoaNotComplete = 4,

        /// <summary>
        /// RFC 5648.
        /// </summary>
        McoaReturnHomeWoNdp = 5,

        /// <summary>
        /// RFC 6058.
        /// </summary>
        PbuAcceptedTbIgnoredSettingsMistmatch = 6,

        /// <summary>
        /// RFCs 5568, 6275.
        /// </summary>
        ReasonUnspecified = 128,

        /// <summary>
        /// RFCs 5568, 6275.
        /// </summary>
        AdministrativelyProhibited = 129,

        /// <summary>
        /// RFCs 5568, 6275.
        /// </summary>
        InsufficientResources = 130,

        /// <summary>
        /// RFCs 5568, 6275.
        /// Binding Acknowledgement: Home registration not supported.
        /// Fast Binding Acknowledgement: Incorrect interface identifier length.
        /// </summary>
        HomeRegistrationNotSupportedOrIncorrectInterfaceIdentifierLength = 131,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        NotHomeSubnet = 132,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        NotHomeAgentForThisMobileNode = 133,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        DuplicateAddressDetectionFailed = 134,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        SequenceNumberOutOfWindow = 135,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        ExpiredHomeNonceIndex = 136,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        ExpiredCareOfNonceIndex = 137,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        ExpiredNonces = 138,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        RegistrationTypeChangeDisallowed = 139,

        /// <summary>
        /// RFC 3963.
        /// </summary>
        MobileRouterOperationNotPermitted = 140,

        /// <summary>
        /// RFC 3963.
        /// </summary>
        InvalidPrefix = 141,

        /// <summary>
        /// RFC 3963.
        /// </summary>
        NotAuthorizedForPrefix = 142,

        /// <summary>
        /// RFC 3963.
        /// </summary>
        ForwardingSetupFailed = 143,

        /// <summary>
        /// RFC 4285.
        /// </summary>
        MIpV6IdMismatch = 144,

        /// <summary>
        /// RFC 4285.
        /// </summary>
        MIpV6MesgIdReqd = 145,

        /// <summary>
        /// RFC 4285.
        /// </summary>
        MIpV6AuthFail = 146,

        /// <summary>
        /// RFC 4866.
        /// </summary>
        PermanentHomeKeygenTokenUnavailable = 147,

        /// <summary>
        /// RFC 4866.
        /// </summary>
        CgaAndSignatureVerificationFailed = 148,

        /// <summary>
        /// RFC 4866.
        /// </summary>
        PermanentHomeKeygenTokenExists = 149,

        /// <summary>
        /// RFC 4866.
        /// </summary>
        NonNullHomeNonceIndexExpected = 150,

        /// <summary>
        /// RFC 5149.
        /// </summary>
        ServiceAuthorizationFailed = 151,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        ProxyRegNotEnabled = 152,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        NotLmaForThisMobileNode = 153,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        MagNotAuthorizedForProxyReg = 154,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        NotAuthorizedForHomeNetworkPrefix = 155,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        TimestampMismatch = 156,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        TimestampLowerThanPrevAccepted = 157,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        MissingHomeNetworkPrefixOption = 158,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        BcePbuPrefixSetDoNotMatch = 159,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        MissingMnIdentifierOption = 160,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        MissingHandoffIndicatorOption = 161,

        /// <summary>
        /// RFC 5213.
        /// </summary>
        MissingAccessTechTypeOption = 162,

        /// <summary>
        /// RFC 5845.
        /// </summary>
        GreKeyOptionRequired = 163,

        /// <summary>
        /// RFC 5648.
        /// </summary>
        McoaMalformed = 164,

        /// <summary>
        /// RFC 5648.
        /// </summary>
        McoaNonMcoaBindingExists = 165,

        /// <summary>
        /// RFC 5648.
        /// </summary>
        McoaProhibited = 166,

        /// <summary>
        /// RFC 5648.
        /// </summary>
        McoaUnknownCoa = 167,

        /// <summary>
        /// RFC 5648.
        /// </summary>
        McoaBulkRegistarationProhiited = 168,

        /// <summary>
        /// RFC 5648.
        /// </summary>
        McoaSimultaneousHomeAndForeignProhibited = 169,

        /// <summary>
        /// RFC 5844.
        /// </summary>
        NotAuthorizedForIpV4MobilityService = 170,

        /// <summary>
        /// RFC 5844.
        /// </summary>
        NotAuthorizedForIpV4HomeAddress = 171,

        /// <summary>
        /// RFC 5844.
        /// </summary>
        NotAuthorizedForIpV6MobilityService = 172,

        /// <summary>
        /// RFC 5844.
        /// </summary>
        MultipleIpV4HomeAddressAssignmentNotSupported = 173,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        InvalidCareOfAddress = 174,

        /// <summary>
        /// RFC 6602.
        /// </summary>
        InvalidMobileNodeGroupIdentifier = 175,

        /// <summary>
        /// RFC 6618.
        /// </summary>
        ReinitSaWithHac = 176,
    }
}