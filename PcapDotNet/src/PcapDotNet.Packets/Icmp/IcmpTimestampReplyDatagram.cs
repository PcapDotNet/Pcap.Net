namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// <pre>
    /// +-----+------+------+-----------------+
    /// | Bit | 0-7  | 8-15 | 16-31           |
    /// +-----+------+------+-----------------+
    /// | 0   | Type | Code | Checksum        |
    /// +-----+------+------+-----------------+
    /// | 32  | Identifier  | Sequence Number |
    /// +-----+-------------+-----------------+
    /// | 64  | Originate Timestamp           |
    /// +-----+-------------------------------+
    /// | 96  | Receive Timestamp             |
    /// +-----+-------------------------------+
    /// | 128 | Transmit Timestamp            |
    /// +-----+-------------------------------+
    /// </pre>
    /// </summary>
    [IcmpDatagramRegistration(IcmpMessageType.TimestampReply)]
    public class IcmpTimestampReplyDatagram : IcmpTimestampDatagram
    {
        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IcmpTimestampReplyLayer
                       {
                           Checksum = Checksum,
                           Identifier = Identifier,
                           SequenceNumber = SequenceNumber,
                           OriginateTimestamp = OriginateTimestamp,
                           ReceiveTimestamp = ReceiveTimestamp,
                           TransmitTimestamp = TransmitTimestamp
                       };
        }

        internal override IcmpDatagram CreateInstance(byte[] buffer, int offset, int length)
        {
            return new IcmpTimestampReplyDatagram(buffer, offset, length);
        }

        private IcmpTimestampReplyDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }
    }
}