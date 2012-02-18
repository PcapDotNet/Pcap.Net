namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// The ATM address format values.
    /// </summary>
    public enum DnsAtmAddressFormat : byte
    {
        /// <summary>
        /// ATM  End  System Address (AESA) format.
        /// </summary>
        AtmEndSystemAddress = 0,

        /// <summary>
        /// E.164 format.
        /// </summary>
        E164 = 1,
    }
}