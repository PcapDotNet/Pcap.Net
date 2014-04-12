namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 4283.
    /// </summary>
    public enum IpV6MobileNodeIdentifierSubtype : byte
    {
        /// <summary>
        /// RFC 4283.
        /// Uses an identifier of the form user@realm (RFC 4282).
        /// </summary>
        NetworkAccessIdentifier = 1,
    }
}