namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5568.
    /// Code of a Handover Initiate Message IPv6 Mobility Extension Header.
    /// </summary>
    public enum IpV6HandoverInitiateMessageCode : byte
    {
        /// <summary>
        /// The Previous Access Router uses this code value when it processes a Fast Binding Update with the Previous Care of Address as source IP address.
        /// </summary>
        SourceIpAddressIsPreviousCareOfAddress = 0x00,

        /// <summary>
        /// The Previous Access Router uses this Code value when it processes a Fast Binidng Update whose source IP address is not the Previous Care of Address.
        /// </summary>
        SourceIpAddressIsNotPreviousCareOfAddress = 0x01,
    }
}