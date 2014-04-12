namespace PcapDotNet.Packets.IpV6
{
    public enum IpV6HandoverInitiateMessageCode : byte
    {
        SourceIpAddressIsPreviousCareOfAddress = 0x00,
        SourceIpAddressIsNotPreviousCareOfAddress = 0x01,
    }
}