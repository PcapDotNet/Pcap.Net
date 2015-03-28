namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// The ATM address format values.
    /// </summary>
    public enum DnsAsynchronousTransferModeAddressFormat : byte
    {
        /// <summary>
        /// ATM End System Address (AESA) format.
        /// </summary>
        AsynchronousTransferModeEndSystemAddress = 0,

        /// <summary>
        /// E.164 format.
        /// </summary>
        E164 = 1,
    }
}