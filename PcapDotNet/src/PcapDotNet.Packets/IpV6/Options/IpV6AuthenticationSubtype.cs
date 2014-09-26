namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 4285
    /// </summary>
    public enum IpV6AuthenticationSubtype : byte
    {
        /// <summary>
        /// Invalid value.
        /// </summary>
        None = 0,

        /// <summary>
        /// Used to authenticate the Binding Update and Binding Acknowledgement messages based on the shared-key-based security association 
        /// between the Mobile Node and the Home Agent.
        /// The shared-key-based mobility security association between Mobile Node and Home Agent used within this specification consists of a mobility SPI,
        /// a key, an authentication algorithm, and the replay protection mechanism in use.  
        /// The mobility SPI is a number in the range [0-4294967296], where the range [0-255] is reserved.  
        /// The key consists of an arbitrary value and is 16 octets in length.  
        /// The authentication algorithm is HMAC_SHA1.  
        /// The replay protection mechanism may use the Sequence number as specified in RFC 3775 or the Timestamp option.
        /// If the Timestamp option is used for replay protection, the mobility security association includes a "close enough" field to account 
        /// for clock drift.
        /// A default value of 7 seconds should be used.
        /// This value should be greater than 3 seconds.
        /// The MN-HA mobility message authentication option must be the last option in a message with a mobility header 
        /// if it is the only mobility message authentication option in the message.
        /// The authentication data is calculated on the message starting from the mobility header up to and including the mobility SPI value of this option.
        /// Authentication Data = First(96, HMAC_SHA1(MN-HA Shared key, Mobility Data))
        /// Mobility Data = care-of address | home address | Mobility Header (MH) Data
        /// MH Data is the content of the Mobility Header up to and including the mobility SPI field of this option.
        /// The Checksum field in the Mobility Header must be set to 0 to calculate the Mobility Data.
        /// The first 96 bits from the Message Authentication Code (MAC) result are used as the Authentication Data field.
        /// </summary>
        HomeAgent = 1,

        /// <summary>
        /// The MN-AAA authentication mobility option is used to authenticate the Binding Update message based on the shared mobility security association 
        /// between the Mobile Node and AAA server in Home network (AAAH).
        /// It is not used in Binding Acknowledgement messages.
        /// The corresponding Binding Acknowledgement messages must be authenticated using the MN-HA mobility message authentication option.
        /// The MN-AAA mobility message authentication option must be the last option in a message with a mobility header.
        /// The corresponding response must include the MN-HA mobility message authentication option,
        /// and must not include the MN-AAA mobility message authentication option.
        /// The Mobile Node may use the Mobile Node Identifier option (RFC 4283) to enable the Home Agent to make use of available AAA infrastructure.
        /// The authentication data is calculated on the message starting from the mobility header up to and including the mobility SPI value of this option.
        /// The authentication data shall be calculated as follows:
        /// Authentication data = hash_fn(MN-AAA Shared key, MAC_Mobility Data)
        /// hash_fn() is decided by the value of mobility SPI field in the MN-AAA mobility message authentication option.
        /// SPI = HMAC_SHA1_SPI:
        /// If mobility SPI has the well-known value HMAC_SHA1_SPI, then hash_fn() is HMAC_SHA1.
        /// When HMAC_SHA1_SPI is used, the BU is authenticated by AAA using HMAC_SHA1 authentication.
        /// In that case, MAC_Mobility Data is calculated as follows:
        /// MAC_Mobility Data = SHA1(care-of address | home address | MH Data)
        /// MH Data is the content of the Mobility Header up to and including the mobility SPI field of this option.
        /// </summary>
        AuthenticationAuthorizationAccountingServer = 2,
    }
}