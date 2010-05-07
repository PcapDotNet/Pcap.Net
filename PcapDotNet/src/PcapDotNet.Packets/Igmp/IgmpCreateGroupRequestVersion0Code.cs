namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// The code of the IGMP message for Create Group Request (RFC988).
    /// </summary>
    public enum IgmpCreateGroupRequestVersion0Code : byte
    {
        /// <summary>
        /// Public.
        /// </summary>
        Public = 0x00,

        /// <summary>
        /// Private.
        /// </summary>
        Private = 0x01
    }
}