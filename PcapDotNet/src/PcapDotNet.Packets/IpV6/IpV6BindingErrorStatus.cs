namespace PcapDotNet.Packets.IpV6
{
    public enum IpV6BindingErrorStatus : byte
    {
        /// <summary>
        /// RFC 6275.
        /// </summary>
        UnknownBindingForHomeAddressDestinationOption = 1,

        /// <summary>
        /// RFC 6275.
        /// </summary>
        UnrecognizedMhTypeValue = 2,
    }
}