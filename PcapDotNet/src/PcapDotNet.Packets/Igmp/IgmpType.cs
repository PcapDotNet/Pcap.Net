namespace PcapDotNet.Packets.Igmp
{
    public enum IgmpType : byte
    {
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