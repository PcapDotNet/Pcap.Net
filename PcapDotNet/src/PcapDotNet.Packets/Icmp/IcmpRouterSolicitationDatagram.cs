namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 1256.
    /// <pre>
    /// +-----+------+------+-----------+
    /// | Bit | 0-7  | 8-15 | 16-31     |
    /// +-----+------+------+-----------+
    /// | 0   | Type | Code | Checksum  |
    /// +-----+------+------+-----------+
    /// | 32  | reserved                |
    /// +-----+-------------------------+
    /// </pre>
    /// </summary>
    public class IcmpRouterSolicitationDatagram : IcmpDatagram
    {
        internal IcmpRouterSolicitationDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        public override ILayer ExtractLayer()
        {
            return new IcmpRouterSolicitationLayer();
        }
    }
}