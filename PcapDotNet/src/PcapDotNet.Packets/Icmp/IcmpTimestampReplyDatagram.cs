namespace PcapDotNet.Packets.Icmp
{
    public class IcmpTimestampReplyDatagram : IcmpTimestampDatagram
    {
        internal IcmpTimestampReplyDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

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
    }
}