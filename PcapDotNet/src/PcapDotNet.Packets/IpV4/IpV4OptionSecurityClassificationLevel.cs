namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// This field specifies the (U.S.) classification level at which the datagram must be protected.  
    /// The information in the datagram must be protected at this level.  
    /// The bit string values in this table were chosen to achieve a minimum Hamming distance of four (4) between any two valid values.  
    /// This specific assignment of classification level names to values has been defined for compatibility 
    /// with security devices which have already been developed and deployed.
   /// </summary>
    public enum IpV4OptionSecurityClassificationLevel : byte
    {
        /// <summary>
        /// An invalid value for a classification level.
        /// </summary>
        None,

        /// <summary>
        /// Top Secret
        /// </summary>
        TopSecret = 0x3D,

        /// <summary>
        /// Secret
        /// </summary>
        Secret = 0x5A,

        /// <summary>
        /// Confidential
        /// </summary>
        Confidential = 0x96,

        /// <summary>
        /// Unclassified
        /// </summary>
        Unclassified = 0xAB
    }
}