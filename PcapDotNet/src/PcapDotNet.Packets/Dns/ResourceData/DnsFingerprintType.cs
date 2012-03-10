namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 4255.
    /// </summary>
    public enum DnsFingerprintType : byte
    {
        /// <summary>
        /// Undefined value.
        /// </summary>
        None = 0,

        /// <summary>
        /// RFC 4255.
        /// </summary>
        Sha1 = 1,
    }
}