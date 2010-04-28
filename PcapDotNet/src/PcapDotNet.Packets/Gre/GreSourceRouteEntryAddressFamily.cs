namespace PcapDotNet.Packets.Gre
{
    public enum GreSourceRouteEntryAddressFamily : ushort
    {
        None = 0x0000,
        IpSourceRoute = 0x0800,
        AsSourceRoute = 0xfffe,
    }
}