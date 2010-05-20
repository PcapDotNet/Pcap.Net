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
    [IcmpDatagramRegistration(IcmpMessageType.RouterSolicitation)]
    public class IcmpRouterSolicitationDatagram : IcmpDatagram
    {
        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IcmpRouterSolicitationLayer();
        }

        private IcmpRouterSolicitationDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal override IcmpDatagram CreateInstance(byte[] buffer, int offset, int length)
        {
            return new IcmpRouterSolicitationDatagram(buffer, offset, length);
        }
    }
}