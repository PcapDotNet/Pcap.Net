namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1183.
    /// </summary>
    public enum DnsAfsDatabaseSubtype : ushort
    {
        /// <summary>
        /// An invalid value.
        /// </summary>
        None = 0,

        /// <summary>
        /// The host has an AFS version 3.0 Volume Location Server for the named AFS cell.
        /// </summary>
        AfsCell = 1,

        /// <summary>
        /// The host has an authenticated name server holding the cell-root directory node for the named DCE/NCA cell.
        /// </summary>
        DceNcaCell = 2,
    }
}