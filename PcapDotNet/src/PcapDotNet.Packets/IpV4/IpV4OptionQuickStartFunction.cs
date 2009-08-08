namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Defines the possible quick start functions.
    /// </summary>
    public enum IpV4OptionQuickStartFunction : byte
    {
        /// <summary>
        /// Request for a specific rate.
        /// </summary>
        RateRequest = 0x00,

        /// <summary>
        /// Reports on a specific rate that was agreed (or disagreed).
        /// </summary>
        RateReport = 0x80

    }
}