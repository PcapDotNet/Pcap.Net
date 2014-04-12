namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6089.
    /// </summary>
    public enum IpV6FlowIdentificationSubOptionType : byte
    {
        /// <summary>
        /// RFC 6089.
        /// </summary>
        Pad1 = 0,

        /// <summary>
        /// RFC 6089.
        /// </summary>
        PadN = 1,

        /// <summary>
        /// RFC 6089.
        /// </summary>
        BindingReference = 2,

        /// <summary>
        /// RFC 6089.
        /// </summary>
        TrafficSelector = 3,
    }
}