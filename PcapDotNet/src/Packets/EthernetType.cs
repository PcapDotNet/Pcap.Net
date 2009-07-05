namespace Packets
{
    public enum EthernetType : ushort
    {
        IpV4 = 0x0800,
        Arp = 0x0806,
        Ieee802_1Q = 0x8100,
        IpV6 = 0x86DD
    }
}