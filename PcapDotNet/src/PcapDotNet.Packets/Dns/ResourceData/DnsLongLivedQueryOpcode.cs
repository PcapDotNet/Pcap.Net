namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// http://files.dns-sd.org/draft-sekar-dns-llq.txt.
    /// </summary>
    public enum DnsLongLivedQueryOpCode : ushort
    {
        None = 0,
        Setup = 1,
        Refresh = 2,
        Event = 3,
    }
}