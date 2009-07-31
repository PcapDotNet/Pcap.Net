namespace PcapDotNet.Packets
{
    public enum IpV4OptionSecurityLevel : ushort
    {
        Unclassified = 0x0000,
        Confidential = 0xF135,
        EFTO = 0x789A,
        MMMM = 0xBC4D,
        PROG = 0x5E26,
        Restricted = 0xAF13,
        Secret = 0xD788,
        TopSecret = 0x6BC5
    }
}