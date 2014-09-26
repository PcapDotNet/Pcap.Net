namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// Binding Error Status for Mobility Binding Error IPv6 Extension Header.
    /// </summary>
    public enum IpV6BindingErrorStatus : byte
    {
        /// <summary>
        /// Invalid value.
        /// </summary>
        None = 0,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        UnknownBindingForHomeAddressDestinationOption = 1,

        /// <summary>
        /// RFC 6275.
        /// Unrecognized MH Type value.
        /// </summary>
        UnrecognizedMobilityHeaderTypeValue = 2,
    }
}