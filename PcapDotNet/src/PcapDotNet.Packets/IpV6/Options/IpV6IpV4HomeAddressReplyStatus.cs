namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5844.
    /// </summary>
    public enum IpV6IpV4HomeAddressReplyStatus : byte
    {
        /// <summary>
        /// Success.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Failure, reason unspecified.
        /// </summary>
        FailureReasonUnspecified = 128,

        /// <summary>
        /// Administratively prohibited.
        /// </summary>
        AdministrativelyProhibited = 129,

        /// <summary>
        /// Incorrect IPv4 home address.
        /// </summary>
        IncorrectIpV4HomeAddress = 130,

        /// <summary>
        /// Invalid IPv4 address.
        /// </summary>
        InvalidIpV4Address = 131,

        /// <summary>
        /// Dynamic IPv4 home address assignment not available.
        /// </summary>
        DynamicIpV4HomeAddressAssignmentNotAvailable = 132,
    }
}