namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFCs 1035, 1183, 1348, 1706, 1712, 1876, 1995, 2065, 2163, 2168, 2230, 2535, 2671, 2672, 2782, 2845, 2874, 2915, 2930, 3123, 3226, 3403, 3596, 3658,
    /// 3755, 4025, 4034, 4255, 4398, 4408, 4431, 4701, 5011, 5155, 5205, 5864, 5936.
    /// Other sources: ATMDOC, Barwood, Eastlake, Faltstrom, Hallam-Baker, PATTON, Reid, Weiler, Wijngaards.
    /// Type fields are used in resource records.
    /// </summary>
    public enum DnsType : ushort
    {
        /// <summary>
        /// Undefined value.
        /// </summary>
        None = 0,

        /// <summary>
        /// RFC 1035.
        /// A host address.
        /// Payload type: DnsResourceDataIpV4.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "A")]
        A = 1,

        /// <summary>
        /// RFC 1035.
        /// An authoritative name server.
        /// Payload type: DnsResourceDataDomainName.
        /// </summary>
        Ns = 2,

        /// <summary>
        /// RFC 1035.
        /// A mail destination (Obsolete - use MX).
        /// Payload type: DnsResourceDataDomainName.
        /// </summary>
        Md = 3,

        /// <summary>
        /// RFC 1035.
        /// MF - A mail forwarder (Obsolete - use MX).
        /// Payload type: DnsResourceDataDomainName.
        /// </summary>
        MailForwarder = 4,

        /// <summary>
        /// RFC 1035.
        /// The canonical name for an alias.
        /// Payload type: DnsResourceDataDomainName.
        /// </summary>
        CName = 5,
        
        /// <summary>
        /// RFC 1035.
        /// SOA - Marks the start of a zone of authority.
        /// Payload type: DnsResourceDataStartOfAuthority.
        /// </summary>
        StartOfAuthority = 6,

        /// <summary>
        /// RFC 1035.
        /// MB - A mailbox domain name (EXPERIMENTAL).
        /// Payload type: DnsResourceDataDomainName.
        /// </summary>
        Mailbox = 7,

        /// <summary>
        /// RFC 1035.
        /// MG - A mail group member (EXPERIMENTAL).
        /// Payload type: DnsResourceDataDomainName.
        /// </summary>
        MailGroup = 8,

        /// <summary>
        /// RFC 1035.
        /// MR - A mail rename domain name (EXPERIMENTAL).
        /// Payload type: DnsResourceDataDomainName.
        /// </summary>
        MailRename = 9,

        /// <summary>
        /// RFC 1035.
        /// A null RR (EXPERIMENTAL).
        /// Payload type: DnsResourceDataAnything.
        /// </summary>
        Null = 10,

        /// <summary>
        /// RFC 1035.
        /// WKS.
        /// A well known service description..
        /// Payload type: DnsResourceDataWellKnownService.
        /// </summary>
        WellKnownService = 11,

        /// <summary>
        /// RFC 1035.
        /// A domain name pointer.
        /// Payload type: DnsResourceDataDomainName.
        /// </summary>
        Ptr = 12,

        /// <summary>
        /// RFC 1035.
        /// Host information.
        /// Payload type: DnsResourceDataHostInformation.
        /// </summary>
        HInfo = 13,

        /// <summary>
        /// RFC 1035.
        /// mailbox or mail list information.
        /// Payload type: DnsResourceDataMailingListInfo.
        /// </summary>
        MInfo = 14,

        /// <summary>
        /// RFC 1035.
        /// MX - Mail exchange.
        /// Payload type: DnsResourceDataMailExchange.
        /// </summary>
        MailExchange = 15,

        /// <summary>
        /// RFC 1035.
        /// Text strings.
        /// Payload type: DnsResourceDataText.
        /// </summary>
        Txt = 16,

        /// <summary>
        /// RFC 1183.
        /// RP - For Responsible Person.
        /// Payload type: DnsResourceDataResponsiblePerson.
        /// </summary>
        ResponsiblePerson = 17,

        /// <summary>
        /// RFCs 1183, 5864.
        /// AFSDB - For AFS Data Base location.
        /// Payload type: DnsResourceDataAfsDb.
        /// </summary>
        AfsDatabase = 18,

        /// <summary>
        /// RFC 1183.
        /// For X.25 PSDN address.
        /// Payload type: DnsResourceDataString.
        /// </summary>
        X25 = 19,

        /// <summary>
        /// RFC 1183.
        /// For ISDN address.
        /// Payload type: DnsResourceDataIsdn.
        /// </summary>
        Isdn = 20,

        /// <summary>
        /// RFC 1183.
        /// RT - For Route Through.
        /// Payload type: DnsResourceDataRouteThrough.
        /// </summary>
        RouteThrough = 21,

        /// <summary>
        /// RFC 1706.
        /// NSAP - Network Service Access Point.
        /// For NSAP address, NSAP style A record.
        /// Payload type: DnsResourceDataNetworkServiceAccessPoint.
        /// </summary>
        NetworkServiceAccessPoint = 22,

        /// <summary>
        /// RFC 1348.
        /// NSAPPTR - For domain name pointer, NSAP style.
        /// Payload type: DnsResourceDataDomainName.
        /// </summary>
        NetworkServiceAccessPointPointer = 23,

        /// <summary>
        /// RFCs 2535, 3755, 4034.
        /// SIG - For security signature.
        /// Payload type: DnsResourceDataSignature.
        /// </summary>
        Signature = 24, 

        /// <summary>
        /// RFCs 2065, 2535, 3755, 4034.
        /// For security key.
        /// Payload type: DnsResourceDataKey.
        /// </summary>
        Key = 25,

        /// <summary>
        /// RFC 2163.
        /// PX - X.400 mail mapping information.
        /// Payload type: DnsResourceDataX400Pointer.
        /// </summary>
        PointerX400 = 26, 

        /// <summary>
        /// RFC 1712.
        /// GPos.
        /// Geographical Position.
        /// Payload type: DnsResourceDataGeographicalPosition.
        /// </summary>
        GeographicalPosition = 27,

        /// <summary>
        /// RFC 3596.
        /// IP6 Address.
        /// Payload type: DnsResourceDataIpV6.
        /// </summary>
        Aaaa = 28,

        /// <summary>
        /// RFC 1876.
        /// LOC.
        /// Location Information.
        /// Payload type: DnsResourceDataLocationInformation.
        /// </summary>
        Location = 29,

        /// <summary>
        /// RFC 2535, 3755.
        /// NXT - Next Domain - OBSOLETE.
        /// Payload type: DnsResourceDataNextDomain.
        /// </summary>
        NextDomain = 30,

        /// <summary>
        /// Patton.
        /// Nimrod Endpoint Identifier.
        /// Payload type: DnsResourceDataAnything.
        /// </summary>
        EId = 31,

        /// <summary>
        /// Patton.
        /// NimLoc - Nimrod Locator.
        /// Payload type: DnsResourceDataAnything.
        /// </summary>
        NimrodLocator = 32,

        /// <summary>
        /// RFC 2782.
        /// SRV - Server Selection.
        /// Payload type: DnsResourceDataServerSelection.
        /// </summary>
        ServerSelection = 33,

        /// <summary>
        /// ATMDOC.
        /// ATMA.
        /// ATM Address.
        /// Payload type: DnsResourceDataAsynchronousTransferModeAddress.
        /// </summary>
        AsynchronousTransferModeAddress = 34,

        /// <summary>
        /// RFCs 2168, 2915, 3403.
        /// Naming Authority Pointer.
        /// Payload type: DnsResourceDataNamingAuthorityPointer.
        /// </summary>
        NaPtr = 35,

        /// <summary>
        /// RFC 2230.
        /// KX - Key Exchanger.
        /// Payload type: DnsResourceDataKeyExchanger.
        /// </summary>
        KeyExchanger = 36,

        /// <summary>
        /// RFC 4398.
        /// CERT.
        /// Payload type: DnsResourceDataCertificate.
        /// </summary>
        Cert = 37,

        /// <summary>
        /// RFCs 2874, 3226.
        /// A6 (Experimental).
        /// Payload type: DnsResourceDataA6.
        /// </summary>
        A6 = 38,
        
        /// <summary>
        /// RFC 2672.
        /// DNAME.
        /// Payload type: DnsResourceDataDomainName.
        /// </summary>
        DName = 39,

        /// <summary>
        /// Eastlake.
        /// SINK.
        /// Payload type: DnsResourceDataSink.
        /// </summary>
        Sink = 40,

        /// <summary>
        /// RFC 2671.
        /// OPT.
        /// Payload type: DnsResourceDataOptions.
        /// </summary>
        Opt = 41,

        /// <summary>
        /// RFC 3123.
        /// APL.
        /// Address Prefix List.
        /// Payload type: DnsResourceDataAddressPrefixList.
        /// </summary>
        AddressPrefixList = 42,

        /// <summary>
        /// RFCs 3658, 4034.
        /// DS - Delegation Signer.
        /// Payload type: DnsResourceDataDelegationSigner.
        /// </summary>
        DelegationSigner = 43,

        /// <summary>
        /// RFC 4255.
        /// SSHFP - SSH Key Fingerprint.
        /// Used to store a fingerprint of an SSH public host key that is associated with a Domain Name System (DNS) name.
        /// Payload type: DnsResourceDataSshFingerprint.
        /// </summary>
        SshFingerprint = 44,

        /// <summary>
        /// RFC 4025.
        /// IPSECKEY.
        /// Payload type: DnsResourceDataIpSecKey.
        /// </summary>
        IpSecKey = 45,

        /// <summary>
        /// RFCs 3755, 4034.
        /// RRSIG.
        /// Payload type: DnsResourceDataSignature.
        /// </summary>
        ResourceRecordSignature = 46,

        /// <summary>
        /// RFCs 3755, 4034.
        /// NSEC.
        /// Payload type: DnsResourceDataNextDomainSecure.
        /// </summary>
        NSec = 47,

        /// <summary>
        /// RFCs 3755, 4034, 5011.
        /// DNSKEY.
        /// Represents the public key.
        /// Payload type: DnsResourceDataDnsKey.
        /// </summary>
        DnsKey = 48,

        /// <summary>
        /// RFC 4701.
        /// DHCID.
        /// Dynamic Host Configuration Information.
        /// Payload type: DnsResourceDataAnything.
        /// </summary>
        DynamicHostConfigurationId = 49,

        /// <summary>
        /// RFC 5155.
        /// NSEC3.
        /// Payload type: DnsResourceDataNextDomainSecure3.
        /// </summary>
        NSec3 = 50,

        /// <summary>
        /// RFC 5155.
        /// NSEC3PARAM.
        /// Payload type: DnsResourceDataNextDomainSecure3Parameters.
        /// </summary>
        NSec3Parameters = 51,

        /// <summary>
        /// RFC 5205.
        /// Host Identity Protocol.
        /// Payload type: DnsResourceDataHostIdentityProtocol.
        /// </summary>
        Hip = 55,

        /// <summary>
        /// Reid.
        /// NINFO.
        /// Payload type: DnsResourceDataNInfo.
        /// </summary>
        NInfo = 56,

        /// <summary>
        /// Reid.
        /// RKEY.
        /// Can be used to encrypt NAPTR record.
        /// Payload type: DnsResourceDataRKey.
        /// </summary>
        RKey = 57,

        /// <summary>
        /// Wijngaards.
        /// TALINK - DNSSEC Trust Anchor LINK.
        /// Payload type: DnsResourceDataTrustAnchorLink.
        /// </summary>
        TrustAnchorLink = 58,

        /// <summary>
        /// Barwood.
        /// CDS.
        /// Child DS.
        /// Payload type: DnsResourceDataDelegationSigner.
        /// </summary>
        ChildDelegationSigner = 59,

        /// <summary>
        /// RFC 4408.
        /// SPF.
        /// Sender Policy Framework.
        /// Payload type: DnsResourceDataText.
        /// </summary>
        SenderPolicyFramework = 99,

        /// <summary>
        /// IANA-Reserved.
        /// Not documented.
        /// </summary>
        UInfo = 100,

        /// <summary>
        /// IANA-Reserved.
        /// Not documented.
        /// </summary>
        Uid = 101,

        /// <summary>
        /// IANA-Reserved.
        /// Not documented.
        /// </summary>
        Gid = 102,

        /// <summary>
        /// IANA-Reserved.
        /// UNSPEC - Not documented.
        /// </summary>
        Unspecified = 103,

        /// <summary>
        /// RFC 2930.
        /// Transaction Key.
        /// Payload type: DnsResourceDataTransactionKey.
        /// </summary>
        TKey = 249,

        /// <summary>
        /// RFC 2845.
        /// TSIG - Transaction Signature.
        /// Payload type: DnsResourceDataTransactionSignature.
        /// </summary>
        TransactionSignature = 250,

        /// <summary>
        /// RFC 1995.
        /// Incremental transfer.
        /// Query Type.
        /// </summary>
        Ixfr = 251,

        /// <summary>
        /// RFCs 1035, 5936.
        /// Transfer of an entire zone.
        /// Query Type.
        /// </summary>
        Axfr = 252,

        /// <summary>
        /// RFC 1035.
        /// Mailbox-related RRs (MB, MG or MR).
        /// Query Type.
        /// </summary>
        MailB = 253,

        /// <summary>
        /// RFC 1035.
        /// Mail agent RRs (Obsolete - see MX).
        /// Query Type.
        /// </summary>
        MailA = 254,

        /// <summary>
        /// *.
        /// A request for all records
        /// Query Type.
        /// </summary>
        Any = 255,

        /// <summary>
        /// Faltstrom.
        /// URI.
        /// Payload type: DnsResourceDataUri.
        /// </summary>
        Uri = 256,

        /// <summary>
        /// Hallam-Baker.
        /// CAA - Certification Authority Authorization.
        /// Payload type: DnsResourceDataCertificationAuthorityAuthorization.
        /// </summary>
        CertificationAuthorityAuthorization = 257,

        /// <summary>
        /// Weiler. 2005-12-13.
        /// TA - DNSSEC Trust Authorities or Trust Anchor.
        /// Payload type: DnsResourceDataDelegationSigner.
        /// </summary>
        TrustAnchor = 32768,

        /// <summary>
        /// RFC 4431.
        /// DLV - DNSSEC Lookaside Validation.
        /// Payload type: DnsResourceDataDelegationSigner.
        /// </summary>
        DnsSecLookAsideValidation = 32769,
    }
}