namespace PcapDotNet.Packets.Ethernet
{
    public interface IEthernetNextLayer : ILayer
    {
        EthernetType PreviousLayerEtherType { get; }
        MacAddress? PreviousLayerDefaultDestination { get; }
    }
}