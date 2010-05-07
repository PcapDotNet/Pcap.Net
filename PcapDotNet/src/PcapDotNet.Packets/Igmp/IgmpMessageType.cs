namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// The type of the IGMP message.
    /// </summary>
    public enum IgmpMessageType : byte
    {
        /// <summary>
        /// Illegal type.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Create Group Request (RFC988).
        /// </summary>
        CreateGroupRequestVersion0 = 0x01,

        /// <summary>
        /// Create Group Reply (RFC988).
        /// </summary>
        CreateGroupReplyVersion0 = 0x02,

        /// <summary>
        /// Join Group Request (RFC988).
        /// </summary>
        JoinGroupRequestVersion0 = 0x03,

        /// <summary>
        /// Join Group Reply (RFC988).
        /// </summary>
        JoinGroupReplyVersion0 = 0x04,

        /// <summary>
        /// Leave Group Request (RFC988).
        /// </summary>
        LeaveGroupRequestVersion0 = 0x05,

        /// <summary>
        /// Leave Group Reply (RFC988).
        /// </summary>
        LeaveGroupReplyVersion0 = 0x06,

        /// <summary>
        /// Confirm Group Request (RFC988).
        /// </summary>
        ConfirmGroupRequestVersion0 = 0x07,

        /// <summary>
        /// Confirm Group Reply (RFC988).
        /// </summary>
        ConfirmGroupReplyVersion0 = 0x08,

        /// <summary>
        /// Membership Query (RFC3376).
        /// </summary>
        MembershipQuery = 0x11,

        /// <summary>
        /// Version 3 Membership Report (RFC3376).
        /// </summary>
        MembershipReportVersion3 = 0x22,

        /// <summary>
        /// Version 1 Membership Report (RFC1112).
        /// </summary>
        MembershipReportVersion1 = 0x12,

        /// <summary>
        /// Version 2 Membership Report (RFC2236).
        /// </summary>
        MembershipReportVersion2 = 0x16,

        /// <summary>
        /// Version 2 Leave Group (RFC2236).
        /// </summary>
        LeaveGroupVersion2 = 0x17
    }
}