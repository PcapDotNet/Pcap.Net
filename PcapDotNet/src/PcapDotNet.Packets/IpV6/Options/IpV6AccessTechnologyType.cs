namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5213.
    /// </summary>
    public enum IpV6AccessTechnologyType : byte
    {
        /// <summary>
        /// Reserved.
        /// </summary>
        None = 0,

        /// <summary>
        /// Virtual.
        /// Logical Network Interface.
        /// </summary>
        LogicalNetworkInterface = 1,

        /// <summary>
        /// Point-to-Point Protocol.
        /// </summary>
        PointToPointProtocol = 2,

        /// <summary>
        /// IEEE 802.3.
        /// Ethernet.
        /// </summary>
        Ethernet = 3,

        /// <summary>
        /// IEEE 802.11a/b/g.
        /// Wireless LAN.
        /// </summary>
        WirelessLan = 4,

        /// <summary>
        /// IEEE 802.16e.
        /// WIMAX.
        /// </summary>
        WiMax = 5,
    }
}