using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Transport
{
    public class TcpLayer : TransportLayer
    {
        public uint SequenceNumber { get; set; }

        public uint AcknowledgmentNumber { get; set; }

        public TcpControlBits ControlBits { get; set; }

        public ushort Window { get; set; }

        public ushort UrgentPointer { get; set; }

        public TcpOptions Options { get; set; }

        public override IpV4Protocol PreviousLayerProtocol
        {
            get { return IpV4Protocol.Tcp; }
        }

        public override int NextLayerChecksumOffset
        {
            get { return TcpDatagram.Offset.Checksum; }
        }

        public override bool NextLayerIsChecksumOptional
        {
            get { return false; }
        }

        public override bool NextLayerCalculateChecksum
        {
            get { return true; }
        }

        public override int Length
        {
            get { return TcpDatagram.HeaderMinimumLength + Options.BytesLength; }
        }

        public override void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer)
        {
            TcpDatagram.WriteHeader(buffer, offset,
                                    SourcePort, DestinationPort,
                                    SequenceNumber, AcknowledgmentNumber,
                                    ControlBits, Window, UrgentPointer,
                                    Options);
        }

        public bool Equals(TcpLayer other)
        {
            return other != null &&
                   SequenceNumber == other.SequenceNumber && AcknowledgmentNumber == other.AcknowledgmentNumber &&
                   ControlBits == other.ControlBits &&
                   Window == other.Window && UrgentPointer == other.UrgentPointer &&
                   Options.Equals(other.Options);
        }

        public override sealed bool Equals(TransportLayer other)
        {
            return base.Equals(other) && Equals(other as TcpLayer);
        }
    }
}