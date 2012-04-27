namespace PcapDotNet.Packets.Ethernet
{
    /// <summary>
    /// IEEE P802.1p.
    /// Eight different classes of service are available as expressed through the 3-bit PCP field in an IEEE 802.1Q header added to the frame.
    /// The way traffic is treated when assigned to any particular class is undefined and left to the implementation.
    /// </summary>
    public enum ClassOfService : byte
    {
        /// <summary>
        /// BE.
        /// Best Effort.
        /// </summary>
        BestEffort = 0,

        /// <summary>
        /// BK.
        /// Background.
        /// Lowest.
        /// </summary>
        Background = 1,

        /// <summary>
        /// EE.
        /// Excellent Effort.
        /// </summary>
        ExcellentEffort = 2,

        /// <summary>
        /// CA.
        /// Critical Applications.
        /// </summary>
        CriticalApplications = 3,

        /// <summary>
        /// VI.
        /// Video
        /// Under 100 ms latency.
        /// </summary>
        Video = 4,

        /// <summary>
        /// VO.
        /// Voice.
        /// Under 10 ms latency.
        /// </summary>
        Voice = 5,

        /// <summary>
        /// IC.
        /// Internetwork Control.
        /// </summary>
        InternetworkControl = 6,

        /// <summary>
        /// NC.
        /// Network Control.
        /// Highest.
        /// </summary>
        NetworkControl = 7,
    }
}