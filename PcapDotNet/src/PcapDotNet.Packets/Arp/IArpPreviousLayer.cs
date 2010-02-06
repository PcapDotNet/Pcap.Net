namespace PcapDotNet.Packets.Arp
{
    public interface IArpPreviousLayer : ILayer
    {
        ArpHardwareType PreviousLayerHardwareType { get; }   
    }
}