namespace PcapDotNet.Packets.Dns
{
    public abstract class DnsResourceDataSimple : DnsResourceDataNoCompression
    {
        internal sealed override int WriteData(byte[] buffer, int offset)
        {
            WriteDataSimple(buffer, offset);
            return GetLength();
        }

        internal abstract void WriteDataSimple(byte[] buffer, int offset);

        internal sealed override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            return CreateInstance(dns.SubSegment(offsetInDns, length));
        }

        internal abstract DnsResourceData CreateInstance(DataSegment data);
    }
}