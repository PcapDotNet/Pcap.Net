namespace PcapDotNet.Packets.Icmp
{
    public enum IcmpCodeSecurityFailures : byte
    {
        /// <summary>
        /// RFC 2521.
        /// Indicates that a received datagram includes a Security Parameters Index (SPI) that is invalid or has expired.
        /// </summary>
        BadSpi = 0x00,

        /// <summary>
        /// RFC 2521.
        /// Indicates that a received datagram failed the authenticity or integrity check for a given SPI.
        /// 
        /// <para>
        /// Note that the SPI may indicate an outer Encapsulating Security Protocol when a separate Authentication Header SPI is hidden inside.
        /// </para>
        /// </summary>
        AuthenticationFailed = 0x01,

        /// <summary>
        /// RFC 2521.
        /// Indicates that a received datagram failed a decompression check for a given SPI.
        /// </summary>
        DecompressionFailed = 0x02,

        /// <summary>
        /// RFC 2521.
        /// Indicates that a received datagram failed a decryption check for a given SPI.
        /// </summary>
        DecryptionFailed = 0x03,

        /// <summary>
        /// RFC 2521.
        /// Indicates that a received datagram will not be accepted without additional authentication.
        /// 
        /// <para>
        /// In this case, either no SPI is present, or an unsuitable SPI is present.  
        /// For example, an encryption SPI without integrity arrives from a secure operating system with mutually suspicious users.
        /// </para>
        /// </summary>
        NeedAuthentication = 0x04,

        /// <summary>
        /// RFC 2521.
        /// Indicates that a received datagram will not be accepted because it has insufficient authorization.
        /// 
        /// <para>
        /// In this case, an authentication SPI is present that is inappropriate for the target transport or application.  
        /// The principle party denoted by the SPI does not have proper authorization for the facilities used by the datagram.  
        /// For example, the party is authorized for Telnet access, but not for FTP access.
        /// </para>
        /// </summary>
        NeedAuthorization = 0x05,
    }
}