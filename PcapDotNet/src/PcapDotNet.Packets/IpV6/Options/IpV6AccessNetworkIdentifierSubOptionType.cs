namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6757.
    /// </summary>
    public enum IpV6AccessNetworkIdentifierSubOptionType : byte
    {
        /// <summary>
        /// Invalid value.
        /// </summary>
        None = 0,

        /// <summary>
        /// Network-Identifier sub-option.
        /// </summary>
        NetworkIdentifier = 1,

        /// <summary>
        /// Geo-Location sub-option.
        /// </summary>
        GeoLocation = 2,

        /// <summary>
        /// Operator-Identifier sub-option.
        /// </summary>
        OperatorIdentifier = 3,
    }
}