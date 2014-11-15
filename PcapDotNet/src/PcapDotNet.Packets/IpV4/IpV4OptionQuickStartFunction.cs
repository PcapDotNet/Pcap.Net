namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// RFC 4782.
    /// Defines the possible quick start functions.
    /// </summary>
    public enum IpV4OptionQuickStartFunction : byte
    {
        /// <summary>
        /// Request for a specific rate.
        /// </summary>
        RateRequest = 0x0,

        /// <summary>
        /// Reports on a specific rate that was agreed (or disagreed).
        /// </summary>
        RateReport = 0x8
    }
}