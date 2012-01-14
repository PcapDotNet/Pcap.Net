namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// Eastlake.
    /// </summary>
    public enum DnsSinkCoding : byte
    {
        /// <summary>
        /// The SNMP subset of ASN.1.
        /// </summary>
        Asn1Snmp = 0x01,

        /// <summary>
        /// OSI ASN.1 1990 [ASN.1].
        /// </summary>
        Asn1Osi1990 = 0x02,

        /// <summary>
        /// OSI ASN.1 1994.
        /// </summary>
        Asn1Osi1994 = 0x03,

        /// <summary>
        /// Private abstract syntax notations.
        /// This coding value will not be assigned to a standard abstract syntax notation.
        /// An OSI Object Identifier (OID) preceded by a one byte unsigned length appears at the beginning of the data area to indicate which private abstract syntax is being used.
        /// </summary>
        AsnPrivate = 0x3F,

        /// <summary>
        /// DNS RRs.
        /// The data portion consists of DNS resource records as they would be transmitted in a DNS response section.
        /// The subcoding octet is the number of RRs in the data area as an unsigned integer.
        /// Domain names may be compressed via pointers as in DNS replies.
        /// The origin for the pointers is the beginning of the RDATA section of the SINK RR.
        /// Thus the SINK RR is safe to cache since only code that knows how to parse the data portion need know of and can expand these compressions.
        /// </summary>
        DnsResourceRecords = 0x40,

        /// <summary>
        /// MIME structured data [RFC 2045, 2046].
        /// The data portion is a MIME structured message.
        /// The "MIME-Version:" header line may be omitted unless the version is other than "1.0".
        /// The top level Content-Transfer-Encoding may be encoded into the subcoding octet.
        /// Note that, to some extent, the size limitations of DNS RRs may be overcome in the MIME case by using the "Content-Type: message/external-body" mechanism.
        /// </summary>
        Mime = 0x41,

        /// <summary>
        /// Text tagged data.
        /// The data potion consists of text formated as specified in the TXT RR except that the first and every subsequent odd numbered text item 
        /// is considered to be a tag labeling the immediately following text item.
        /// If there are an odd number of text items overall, then the last is considered to label a null text item.
        /// Syntax of the tags is as specified in RFC 1738 for the "Common Internet Scheme Syntax" without the two leading slashes ("//").
        /// Thus any organization with a domain name can assign tags without fear of conflict.
        /// </summary>
        TextTaggedData = 0x42,

        /// <summary>
        /// Private formats indicated by a URL.
        /// The format of the data portion is indicated by an initial URL [RFC 1738] which is terminated by a zero valued octet
        /// followed by the data with that format.
        /// The subcoding octet is available for whatever use the private formating wishes to make of it.
        /// The manner in which the URL specifies the format is not defined but presumably the retriever will recognize the URL or the data it points to.
        /// </summary>
        PrivateByUrl = 0xFE
    }
}