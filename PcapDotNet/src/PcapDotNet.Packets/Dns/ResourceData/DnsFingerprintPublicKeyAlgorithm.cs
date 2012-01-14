namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 4255.
    /// Describes the algorithm of the public key.
    /// </summary>
    public enum DnsFingerprintPublicKeyAlgorithm : byte
    {
        Rsa = 1,
        Dss = 2,
    }
}