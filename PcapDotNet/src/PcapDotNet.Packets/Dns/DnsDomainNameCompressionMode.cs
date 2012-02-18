namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// Defines when and how to compress DNS domain names when creating a DNS datagram.
    /// </summary>
    public enum DnsDomainNameCompressionMode
    {
        /// <summary>
        /// Compress any domain name if possible.
        /// </summary>
        All,

        /// <summary>
        /// Never compress domain names.
        /// </summary>
        Nothing
    }
}