namespace PcapDotNet.Packets.IpV6
{
    internal interface IIpV6OptionComplexFactory
    {
        IpV6Option CreateInstance(DataSegment data);
    }
}