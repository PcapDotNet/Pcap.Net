namespace PcapDotNet.Packets
{
    public enum IpV4OptionSecurityLevel : ushort
    {
        Unclassified = 0x0000,
        Confidential = 0xF135,
        EncryptedForTransmissionOnly = 0x789A,
        Mmmm = 0xBC4D,
        Prog = 0x5E26,
        Restricted = 0xAF13,
        Secret = 0xD788,
        TopSecret = 0x6BC5
    }
}