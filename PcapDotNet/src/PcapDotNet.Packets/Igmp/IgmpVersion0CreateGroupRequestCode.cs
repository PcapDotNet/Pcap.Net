namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 988.
    /// In an IGMP version 0 Create Group Request message, the code field indicates if the new host group is to be public or private.
    /// </summary>
    public enum IgmpVersion0CreateGroupRequestCode : byte
    {
        /// <summary>
        /// The new host group is to be public.
        /// </summary>
        Public = 0,

        /// <summary>
        /// The new host group is to be private.
        /// </summary>
        Private = 1,
    }
}