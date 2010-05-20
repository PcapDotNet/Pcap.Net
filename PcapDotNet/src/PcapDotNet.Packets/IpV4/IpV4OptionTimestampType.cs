namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// The type of the timestamp ip option.
    /// </summary>
    public enum IpV4OptionTimestampType : byte
    {
        /// <summary>
        /// Time stamps only, stored in consecutive 32-bit words.
        /// </summary>
        TimestampOnly = 0,

        /// <summary>
        /// Each timestamp is preceded with internet address of the registering entity.
        /// </summary>
        AddressAndTimestamp = 1,

        /// <summary>
        /// The internet address fields are prespecified.  
        /// An IP module only registers its timestamp if it matches its own address with the next specified internet address.
        /// </summary>
        AddressPrespecified = 3
    }
}