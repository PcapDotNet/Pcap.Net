namespace Packets
{
    public enum EthernetType : ushort
    {
        None,
        IpV4 = 0x0800,
        Arp = 0x0806,
        Ieee8021Q = 0x8100,
        IpV6 = 0x86DD
    }
}