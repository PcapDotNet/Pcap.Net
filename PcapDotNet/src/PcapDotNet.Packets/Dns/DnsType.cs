namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFCs 1035, 1183, 1348, 1706, 1712, 1876, 1995, 2163, 2168, 2230, 2535, 2672, 2782, 2845, 2874, 2915, 2930, 3123, 3226, 3403, 3596, 3658, 3755, 4025, 
    /// 4034, 4255, 4398, 4408, 4431, 4701, 5155, 5205, 5864, 5936.
    /// Other sources: ATMDOC, Barwood, Eastlake, Faltstrom, Hallam-Baker, PATTON, Reid, Weiler, Wijngaards.
    /// Type fields are used in resource records.
    /// </summary>
    public enum DnsType : ushort
    {
        /// <summary>
        /// RFC 1035.
        /// A host address.
        /// Payload type: DnsResourceDataIpV4.
        /// </summary>
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
        /// A mail forwarder (Obsolete - use MX).
        /// Payload type: DnsResourceDataDomainName.
        /// </summary>
        Mf = 4,

        /// <summary>
        /// RFC 1035.
        /// The canonical name for an alias.
        /// Payload type: DnsResourceDataDomainName.
        /// </summary>
        CName = 5,
        
        /// <summary>
        /// RFC 1035.
        /// Marks the start of a zone of authority.
        /// Payload type: DnsResourceDataStartOfAuthority.
        /// </summary>
        Soa = 6,

        /// <summary>
        /// RFC 1035.
        /// A mailbox domain name (EXPERIMENTAL).
        /// Payload type: DnsResourceDataDomainName.
        /// </summary>
        Mb = 7,

        /// <summary>
        /// RFC 1035.
        /// A mail group member (EXPERIMENTAL).
        /// Payload type: DnsResourceDataDomainName.
        /// </summary>
        Mg = 8,

        /// <summary>
        /// RFC 1035.
        /// A mail rename domain name (EXPERIMENTAL).
        /// Payload type: DnsResourceDataDomainName.
        /// </summary>
        Mr = 9,

        /// <summary>
        /// RFC 1035.
        /// A null RR (EXPERIMENTAL).
        /// Payload type: DnsResourceDataAnything.
        /// </summary>
        Null = 10,

        /// <summary>
        /// RFC 1035.
        /// A well known service description..
        /// Payload type: DnsResourceDataWellKnownService.
        /// </summary>
        Wks = 11,

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
        /// Mail exchange.
        /// Payload type: DnsResourceDataMailExchange.
        /// </summary>
        Mx = 15,

        /// <summary>
        /// RFC 1035.
        /// Text strings.
        /// Payload type: DnsResourceDataText.
        /// </summary>
        Txt = 16,

        /// <summary>
        /// RFC 1183.
        /// For Responsible Person.
        /// Payload type: DnsResourceDataResponsiblePerson.
        /// </summary>
        Rp = 17,

        /// <summary>
        /// RFCs 1183, 5864.
        /// For AFS Data Base location.
        /// Payload type: DnsResourceDataAfsDb.
        /// </summary>
        AfsDb = 18,

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
        /// For Route Through.
        /// Payload type: DnsResourceDataRouteThrough.
        /// </summary>
        Rt = 21,

        /// <summary>
        /// RFC 1706.
        /// Network Service Access Point.
        /// For NSAP address, NSAP style A record.
        /// Payload type: DnsResourceDataNetworkServiceAccessPoint.
        /// </summary>
        Nsap = 22,

        /// <summary>
        /// RFC 1348.
        /// For domain name pointer, NSAP style.
        /// Payload type: DnsResourceDataDomainName.
        /// </summary>
        NsapPtr = 23,

        /// <summary>
        /// RFCs 2535, 3755, 4034.
        /// For security signature.
        /// Payload type: DnsResourceDataSig.
        /// </summary>
        Sig = 24, 

        /// <summary>
        /// RFCs 2535, 3755, 4034.
        /// For security key.
        /// Payload type: DnsResourceDataKey.
        /// </summary>
        Key = 25,

        /// <summary>
        /// RFC 2163.
        /// X.400 mail mapping information.
        /// </summary>
        Px = 26, 

        /// <summary>
        /// RFC 1712.
        /// Geographical Position.
        /// </summary>
        GPos = 27,

        /// <summary>
        /// RFC 3596.
        /// IP6 Address.
        /// </summary>
        Aaaa = 28,

        /// <summary>
        /// RFC 1876.
        /// Location Information.
        /// </summary>
        Loc = 29,

        /// <summary>
        /// RFC 2535, 3755.
        /// Next Domain - OBSOLETE.
        /// </summary>
        Nxt = 30,

        /// <summary>
        /// Patton.
        /// Endpoint Identifier
        /// </summary>
        EId = 31,

        /// <summary>
        /// Patton.
        /// Nimrod Locator.
        /// </summary>
        NimLoc = 32,

        /// <summary>
        /// RFC 2782.
        /// Server Selection.
        /// </summary>
        Srv = 33,

        /// <summary>
        /// ATMDOC.
        /// ATM Address.
        /// </summary>
        AtmA = 34,

        /// <summary>
        /// RFCs 2168, 2915, 3403.
        /// Naming Authority Pointer.
        /// </summary>
        NaPtr = 35,

        /// <summary>
        /// RFC 2230.
        /// Key Exchanger.
        /// </summary>
        Kx = 36,

        /// <summary>
        /// RFC 4398.
        /// CERT.
        /// </summary>
        Cert = 37,

        /// <summary>
        /// RFCs 2874, 3226.
        /// A6 (Experimental).
        /// </summary>
        A6 = 38,
        
        /// <summary>
        /// RFC 2672.
        /// DNAME.
        /// </summary>
        DName = 39,

        /// <summary>
        /// Eastlake.
        /// SINK.
        /// </summary>
        Sink = 40,

        /// <summary>
        /// RFC 2671.
        /// OPT.
        /// </summary>
        Opt = 41,

        /// <summary>
        /// RFC 3123.
        /// APL.
        /// </summary>
        Apl = 42,

        /// <summary>
        /// RFCs 3658, 4034.
        /// Delegation Signer.
        /// </summary>
        Ds = 43,

        /// <summary>
        /// RFC 4255.
        /// SSH Key Fingerprint.
        /// </summary>
        SshFp = 44,

        /// <summary>
        /// RFC 4025.
        /// IPSECKEY.
        /// </summary>
        IpSecKey = 45,

        /// <summary>
        /// RFCs 3755, 4034.
        /// RRSIG.
        /// </summary>
        RrSig = 46,

        /// <summary>
        /// RFCs 3755, 4034.
        /// NSEC.
        /// </summary>
        NSec = 47,

        /// <summary>
        /// RFCs 3755, 4034.
        /// DNSKEY.
        /// </summary>
        DnsKey = 48,

        /// <summary>
        /// RFC 4701.
        /// DHCID.
        /// </summary>
        Dhcid = 49,

        /// <summary>
        /// RFC 5155.
        /// NSEC3.
        /// </summary>
        NSec3 = 50,

        /// <summary>
        /// RFC 5155.
        /// NSEC3PARAM.
        /// </summary>
        NSec3Param = 51,

        /// <summary>
        /// RFC 5205.
        /// Host Identity Protocol.
        /// </summary>
        Hip = 55,

        /// <summary>
        /// Reid.
        /// NINFO.
        /// </summary>
        NInfo = 56,

        /// <summary>
        /// Reid.
        /// RKEY.
        /// </summary>
        RKey = 57,

        /// <summary>
        /// Wijngaards.
        /// Trust Anchor LINK.
        /// </summary>
        TaLink = 58,

        /// <summary>
        /// Barwood.
        /// Child DS.
        /// </summary>
        Cds = 59,

        /// <summary>
        /// RFC 4408.
        /// </summary>
        Spf = 99,

        /// <summary>
        /// IANA-Reserved.
        /// </summary>
        UInfo = 100,

        /// <summary>
        /// IANA-Reserved.
        /// </summary>
        Uid = 101,

        /// <summary>
        /// IANA-Reserved.
        /// </summary>
        Gid = 102,

        /// <summary>
        /// IANA-Reserved.
        /// </summary>
        UnSpec = 103,

        /// <summary>
        /// RFC 2930.
        /// Transaction Key.
        /// </summary>
        TKey = 249,

        /// <summary>
        /// RFC 2845.
        /// Transaction Signature.
        /// </summary>
        TSig = 250,

        /// <summary>
        /// RFC 1995.
        /// Incremental transfer.
        /// </summary>
        Ixfr = 251,

        /// <summary>
        /// RFCs 1035, 5936.
        /// Transfer of an entire zone.
        /// </summary>
        Axft = 252,

        /// <summary>
        /// RFC 1035].
        /// Mailbox-related RRs (MB, MG or MR).
        /// </summary>
        MailB = 253,

        /// <summary>
        /// RFC 1035].
        /// Mail agent RRs (Obsolete - see MX).
        /// </summary>
        MailA = 254,

        /// <summary>
        /// *.
        /// A request for all records
        /// Query Type.
        /// </summary>
        All = 255,

        /// <summary>
        /// Faltstrom.
        /// URI.
        /// </summary>
        Uri = 256,

        /// <summary>
        /// Hallam-Baker.
        /// Certification Authority Authorization.
        /// </summary>
        Caa = 257,

        /// <summary>
        /// Weiler. 2005-12-13.
        /// DNSSEC Trust Authorities.
        /// </summary>
        Ta = 32768,

        /// <summary>
        /// RFC 4431.
        /// DNSSEC Lookaside Validation.
        /// </summary>
        Dlv = 32769,
    }
}