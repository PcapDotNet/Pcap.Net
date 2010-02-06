using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Transport
{
    public class UdpLayer : TransportLayer
    {
        public bool CalculateChecksum { get; set; }

        public override IpV4Protocol PreviousLayerProtocol
        {
            get { return IpV4Protocol.Udp; }
        }

        public override int NextLayerChecksumOffset
        {
            get { return UdpDatagram.Offset.Checksum; }
        }

        public override bool NextLayerIsChecksumOptional
        {
            get { return true; }
        }

        public override bool NextLayerCalculateChecksum
        {
            get { return CalculateChecksum; }
        }

        public override int Length
        {
            get { return UdpDatagram.HeaderLength; }
        }

        public override void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer)
        {
            UdpDatagram.WriteHeader(buffer, offset, SourcePort, DestinationPort, payloadLength);
        }
    }
}