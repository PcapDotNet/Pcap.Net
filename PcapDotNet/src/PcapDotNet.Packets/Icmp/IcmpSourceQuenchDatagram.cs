namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// <pre>
    /// +-----+------+------+-----------+
    /// | Bit | 0-7  | 8-15 | 16-31     |
    /// +-----+------+------+-----------+
    /// | 0   | Type | Code | Checksum  |
    /// +-----+------+------+-----------+
    /// | 32  | unused                  |
    /// +-----+-------------------------+
    /// | 64  | Internet Header         |
    /// |     | + 64 bits of            |
    /// |     | Original Data Datagram  |
    /// +-----+-------------------------+
    /// </pre>
    /// </summary>
    [IcmpDatagramRegistration(IcmpMessageType.SourceQuench)]
    public class IcmpSourceQuenchDatagram : IcmpIpV4HeaderPlus64BitsPayloadDatagram
    {
        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IcmpSourceQuenchLayer
                       {
                           Checksum = Checksum
                       };
        }

        internal override IcmpDatagram CreateInstance(byte[] buffer, int offset, int length)
        {
            return new IcmpSourceQuenchDatagram(buffer, offset, length);
        }

        private IcmpSourceQuenchDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }
    }
}