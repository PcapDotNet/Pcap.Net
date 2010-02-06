using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    public interface IIgmpLayerWithGroupAddress
    {
        IpV4Address GroupAddress { get; set; }
    }
}