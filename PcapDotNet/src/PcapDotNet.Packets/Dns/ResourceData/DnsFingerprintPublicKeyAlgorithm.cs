namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 4255.
    /// Describes the algorithm of the public key.
    /// </summary>
    public enum DnsFingerprintPublicKeyAlgorithm : byte
    {
        /// <summary>
        /// Undefined value.
        /// </summary>
        None = 0,

        /// <summary>
        /// RSA algorithm.
        /// </summary>
        Rsa = 1,

        /// <summary>
        /// DSS algorithm.
        /// </summary>
        Dss = 2,
    }
}