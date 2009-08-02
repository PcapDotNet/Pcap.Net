namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Security (S field):  16 bits
    /// </summary>
    public enum IpV4OptionSecurityLevel : ushort
    {
        /// <summary>
        /// Unclassified
        /// </summary>
        Unclassified = 0x0000,
        /// <summary>
        /// Confidential
        /// </summary>
        Confidential = 0xF135,
        /// <summary>
        /// Encrypted For Transmission Only (EFTO)
        /// </summary>
        EncryptedForTransmissionOnly = 0x789A,
        /// <summary>
        /// MMMM
        /// </summary>
        Mmmm = 0xBC4D,
        /// <summary>
        /// PROG
        /// </summary>
        Prog = 0x5E26,
        /// <summary>
        /// Restricted
        /// </summary>
        Restricted = 0xAF13,
        /// <summary>
        /// Secret
        /// </summary>
        Secret = 0xD788,
        /// <summary>
        /// Top Secret
        /// </summary>
        TopSecret = 0x6BC5
    }
}