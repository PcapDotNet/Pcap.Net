namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// Eastlake.
    /// </summary>
    public enum DnsSinkCodingSubCoding : ushort
    {
        /// <summary>
        /// Undefined value.
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// The SNMP subset of ASN.1.
        /// Basic Encoding Rules.
        /// </summary>
        Asn1SimpleNetworkManagementProtocolBasicEncodingRules = 0x0101,

        /// <summary>
        /// The SNMP subset of ASN.1.
        /// Distinguished Encoding Rules.
        /// </summary>
        Asn1SimpleNetworkManagementProtocolDistinguishedEncodingRules = 0x0102,

        /// <summary>
        /// The SNMP subset of ASN.1.
        /// Packed Encoding Rules Aligned.
        /// </summary>
        Asn1SimpleNetworkManagementProtocolPer = 0x0103,

        /// <summary>
        /// The SNMP subset of ASN.1.
        /// Packed Encoding Rules Unaligned.
        /// </summary>
        Asn1SimpleNetworkManagementProtocolPerUnaligned = 0x0104,

        /// <summary>
        /// The SNMP subset of ASN.1.
        /// Canonical Encoding Rules.
        /// </summary>
        Asn1SimpleNetworkManagementProtocolCanonicalEncodingRules = 0x0105,

        /// <summary>
        /// The SNMP subset of ASN.1.
        /// An OID preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private encoding.
        /// </summary>
        Asn1SimpleNetworkManagementProtocolPrivate = 0x01FE,

        /// <summary>
        /// OSI ASN.1 1990 [ASN.1].
        /// Basic Encoding Rules.
        /// </summary>
        Asn1Osi1990BasicEncodingRules = 0x0201,

        /// <summary>
        /// OSI ASN.1 1990 [ASN.1].
        /// Distinguished Encoding Rules.
        /// </summary>
        Asn1Osi1990DistinguishedEncodingRules = 0x0202,

        /// <summary>
        /// OSI ASN.1 1990 [ASN.1].
        /// Packed Encoding Rules Aligned.
        /// </summary>
        Asn1Osi1990Per = 0x0203,

        /// <summary>
        /// OSI ASN.1 1990 [ASN.1].
        /// Packed Encoding Rules Unaligned.
        /// </summary>
        Asn1Osi1990PerUnaligned = 0x0204,

        /// <summary>
        /// OSI ASN.1 1990 [ASN.1].
        /// Canonical Encoding Rules.
        /// </summary>
        Asn1Osi1990CanonicalEncodingRules = 0x0205,

        /// <summary>
        /// OSI ASN.1 1990 [ASN.1].
        /// An OID preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private encoding.
        /// </summary>
        Asn1Osi1990Private = 0x02FE,

        /// <summary>
        /// OSI ASN.1 1994.
        /// Basic Encoding Rules.
        /// </summary>
        Asn1Osi1994BasicEncodingRules = 0x0301,

        /// <summary>
        /// OSI ASN.1 1994.
        /// Distinguished Encoding Rules.
        /// </summary>
        Asn1Osi1994DistinguishedEncodingRules = 0x0302,

        /// <summary>
        /// OSI ASN.1 1994.
        /// Packed Encoding Rules Aligned.
        /// </summary>
        Asn1Osi1994Per = 0x0303,

        /// <summary>
        /// OSI ASN.1 1994.
        /// Packed Encoding Rules Unaligned.
        /// </summary>
        Asn1Osi1994PerUnaligned = 0x0304,

        /// <summary>
        /// OSI ASN.1 1994.
        /// Canonical Encoding Rules.
        /// </summary>
        Asn1Osi1994CanonicalEncodingRules = 0x0305,

        /// <summary>
        /// OSI ASN.1 1994.
        /// An OID preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private encoding.
        /// </summary>
        Asn1Osi1994Private = 0x03FE,

        /// <summary>
        /// Private abstract syntax notations.
        /// This coding value will not be assigned to a standard abstract syntax notation.
        /// An OSI Object Identifier (OID) preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private abstract syntax is being used.
        /// Basic Encoding Rules.
        /// </summary>
        AsnPrivateBasicEncodingRules = 0x3F01,

        /// <summary>
        /// Private abstract syntax notations.
        /// This coding value will not be assigned to a standard abstract syntax notation.
        /// An OSI Object Identifier (OID) preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private abstract syntax is being used.
        /// Distinguished Encoding Rules.
        /// </summary>
        AsnPrivateDistinguishedEncodingRules = 0x3F02,

        /// <summary>
        /// Private abstract syntax notations.
        /// This coding value will not be assigned to a standard abstract syntax notation.
        /// An OSI Object Identifier (OID) preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private abstract syntax is being used.
        /// Packed Encoding Rules Aligned.
        /// </summary>
        AsnPrivatePer = 0x3F03,

        /// <summary>
        /// Private abstract syntax notations.
        /// This coding value will not be assigned to a standard abstract syntax notation.
        /// An OSI Object Identifier (OID) preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private abstract syntax is being used.
        /// Packed Encoding Rules Unaligned.
        /// </summary>
        AsnPrivatePerUnaligned = 0x3F04,

        /// <summary>
        /// Private abstract syntax notations.
        /// This coding value will not be assigned to a standard abstract syntax notation.
        /// An OSI Object Identifier (OID) preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private abstract syntax is being used.
        /// Canonical Encoding Rules.
        /// </summary>
        AsnPrivateCanonicalEncodingRules = 0x3F05,

        /// <summary>
        /// Private abstract syntax notations.
        /// This coding value will not be assigned to a standard abstract syntax notation.
        /// An OSI Object Identifier (OID) preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private abstract syntax is being used.
        /// An OID preceded by a one byte unsigned length appears in the data area just after the coding OID.
        /// </summary>
        AsnPrivatePrivate = 0x3FFE,

        /// <summary>
        /// MIME structured data [RFC 2045, 2046].
        /// The data portion is a MIME structured message.
        /// The "MIME-Version:" header line may be omitted unless the version is other than "1.0".
        /// Note that, to some extent, the size limitations of DNS RRs may be overcome in the MIME case by using the "Content-Type: message/external-body" mechanism.
        /// 7 bit.
        /// </summary>
        Mime7Bit = 0x4101,

        /// <summary>
        /// MIME structured data [RFC 2045, 2046].
        /// The data portion is a MIME structured message.
        /// The "MIME-Version:" header line may be omitted unless the version is other than "1.0".
        /// Note that, to some extent, the size limitations of DNS RRs may be overcome in the MIME case by using the "Content-Type: message/external-body" mechanism.
        /// 8 bit.
        /// </summary>
        Mime8Bit = 0x4102,

        /// <summary>
        /// MIME structured data [RFC 2045, 2046].
        /// The data portion is a MIME structured message.
        /// The "MIME-Version:" header line may be omitted unless the version is other than "1.0".
        /// Note that, to some extent, the size limitations of DNS RRs may be overcome in the MIME case by using the "Content-Type: message/external-body" mechanism.
        /// Binary.
        /// </summary>
        MimeBinary = 0x4103,

        /// <summary>
        /// MIME structured data [RFC 2045, 2046].
        /// The data portion is a MIME structured message.
        /// The "MIME-Version:" header line may be omitted unless the version is other than "1.0".
        /// Note that, to some extent, the size limitations of DNS RRs may be overcome in the MIME case by using the "Content-Type: message/external-body" mechanism.
        /// Quoted-printable.
        /// </summary>
        MimeQuotedPrintable = 0x4104,

        /// <summary>
        /// MIME structured data [RFC 2045, 2046].
        /// The data portion is a MIME structured message.
        /// The "MIME-Version:" header line may be omitted unless the version is other than "1.0".
        /// Note that, to some extent, the size limitations of DNS RRs may be overcome in the MIME case by using the "Content-Type: message/external-body" mechanism.
        /// Base 64.
        /// </summary>
        MimeBase64 = 0x4105,

        /// <summary>
        /// MIME structured data [RFC 2045, 2046].
        /// The data portion is a MIME structured message.
        /// The "MIME-Version:" header line may be omitted unless the version is other than "1.0".
        /// Note that, to some extent, the size limitations of DNS RRs may be overcome in the MIME case by using the "Content-Type: message/external-body" mechanism.
        /// The data portion must start with an "x-" token denoting the private content-transfer-encoding immediately followed by one null (zero) octet 
        /// followed by the remainder of the MIME object.
        /// </summary>
        MimePrivate = 0x41FE,

        /// <summary>
        /// Text tagged data.
        /// The data potion consists of text formated as specified in the TXT RR except that the first and every subsequent odd numbered text item 
        /// is considered to be a tag labeling the immediately following text item.
        /// If there are an odd number of text items overall, then the last is considered to label a null text item.
        /// Syntax of the tags is as specified in RFC 1738 for the "Common Internet Scheme Syntax" without the two leading slashes ("//").
        /// Thus any organization with a domain name can assign tags without fear of conflict.
        /// ASCII.
        /// </summary>
        TextTaggedDataAscii = 0x4201,

        /// <summary>
        /// Text tagged data.
        /// The data potion consists of text formated as specified in the TXT RR except that the first and every subsequent odd numbered text item 
        /// is considered to be a tag labeling the immediately following text item.
        /// If there are an odd number of text items overall, then the last is considered to label a null text item.
        /// Syntax of the tags is as specified in RFC 1738 for the "Common Internet Scheme Syntax" without the two leading slashes ("//").
        /// Thus any organization with a domain name can assign tags without fear of conflict.
        /// UTF-7 [RFC 1642].
        /// </summary>
        TextTaggedDataUtf7 = 0x4202,

        /// <summary>
        /// Text tagged data.
        /// The data potion consists of text formated as specified in the TXT RR except that the first and every subsequent odd numbered text item 
        /// is considered to be a tag labeling the immediately following text item.
        /// If there are an odd number of text items overall, then the last is considered to label a null text item.
        /// Syntax of the tags is as specified in RFC 1738 for the "Common Internet Scheme Syntax" without the two leading slashes ("//").
        /// Thus any organization with a domain name can assign tags without fear of conflict.
        /// UTF-8 [RFC 2044].
        /// </summary>
        TextTaggedDataUtf8 = 0x4203,

        /// <summary>
        /// Text tagged data.
        /// The data potion consists of text formated as specified in the TXT RR except that the first and every subsequent odd numbered text item 
        /// is considered to be a tag labeling the immediately following text item.
        /// If there are an odd number of text items overall, then the last is considered to label a null text item.
        /// Syntax of the tags is as specified in RFC 1738 for the "Common Internet Scheme Syntax" without the two leading slashes ("//").
        /// Thus any organization with a domain name can assign tags without fear of conflict.
        /// ASCII with MIME header escapes [RFC 2047].
        /// </summary>
        TextTaggedDataAsciiMimeHeaderEscapes = 0x4204,

        /// <summary>
        /// Text tagged data.
        /// The data potion consists of text formated as specified in the TXT RR except that the first and every subsequent odd numbered text item 
        /// is considered to be a tag labeling the immediately following text item.
        /// If there are an odd number of text items overall, then the last is considered to label a null text item.
        /// Syntax of the tags is as specified in RFC 1738 for the "Common Internet Scheme Syntax" without the two leading slashes ("//").
        /// Thus any organization with a domain name can assign tags without fear of conflict.
        /// Each text item must start with a domain name [RFC 1034] denoting the private text encoding immediately followed by one null (zero) octet
        /// followed by the remainder of the text item.
        /// </summary>
        TextTaggedDataPrivate = 0x42FE,
    }
}