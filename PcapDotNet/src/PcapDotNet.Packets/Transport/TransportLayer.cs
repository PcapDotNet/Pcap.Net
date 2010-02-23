using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Transport
{
    public abstract class TransportLayer : Layer, IIpV4NextTransportLayer
    {
        public ushort? Checksum { get; set; }

        public ushort SourcePort { get; set; }

        public ushort DestinationPort { get; set; }

        public abstract IpV4Protocol PreviousLayerProtocol { get; }
        public virtual bool CalculateChecksum
        {
            get { return true; }
        }

        public abstract int NextLayerChecksumOffset { get; }
        public abstract bool NextLayerIsChecksumOptional { get; }
        public abstract bool NextLayerCalculateChecksum { get; }

        public virtual bool Equals(TransportLayer other)
        {
            return other != null &&
                   PreviousLayerProtocol == other.PreviousLayerProtocol &&
                   Checksum == other.Checksum &&
                   SourcePort == other.SourcePort && DestinationPort == other.DestinationPort;
        }

        public override sealed bool Equals(Layer other)
        {
            return base.Equals(other) && Equals(other as TransportLayer);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   Checksum.GetHashCode() ^
                   ((SourcePort << 16) + DestinationPort);
        }
    }
}