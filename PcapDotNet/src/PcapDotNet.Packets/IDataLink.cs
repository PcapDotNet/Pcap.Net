namespace PcapDotNet.Packets
{
    public interface IDataLink
    {
        DataLinkKind Kind { get; }
    }
}