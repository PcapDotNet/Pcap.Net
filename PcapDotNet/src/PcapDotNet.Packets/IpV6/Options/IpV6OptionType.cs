namespace PcapDotNet.Packets.IpV6
{
    public enum IpV6OptionType :  byte
    {
        /// <summary>
        /// RFC 2460.
        /// </summary>
        Pad1 = 0x00,

        /// <summary>
        /// RFC 2460.
        /// </summary>
        PadN = 0x01,

        /// <summary>
        /// RFC 2675.
        /// </summary>
        JumboPayload = 0xC2,

        /// <summary>
        /// RFC 2473.
        /// </summary>
        TunnelEncapsulationLimit = 0x04,

        /// <summary>
        /// RFC 2711.
        /// </summary>
        RouterAlert = 0x05,

        /// <summary>
        /// RFC 4782, Errata 2034.
        /// </summary>
        QuickStart = 0x26,

        /// <summary>
        /// RFC 5570.
        /// </summary>
        Calipso = 0x07,

        /// <summary>
        /// RFC 6621.
        /// </summary>
        SmfDpd = 0x08,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        HomeAddress = 0xC9,

        /// <summary>
        /// Charles Lynn.
        /// </summary>
        EndpointIdentification = 0x8A,

        /// <summary>
        /// RFC 6553.
        /// </summary>
        RplOption = 0x63,

        /// <summary>
        /// RFC irtf-rrg-ilnp-noncev6-06.
        /// </summary>
        IlnpNonce = 0x8B,
        
        /// <summary>
        /// RFC ietf-6man-lineid-08.
        /// </summary>
        LineIdentification = 0x8C,
    }
}
