namespace PcapDotNet.Packets.Dns
{
    public class DnsResourceDataUnknown : DnsResourceData
    {
        public DnsResourceDataUnknown(DataSegment data)
        {
            Data = data;
        }

        public DataSegment Data { get; private set; }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return Data.Length;
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            Data.Write(buffer, dnsOffset + offsetInDns);
            return Data.Length;
        }
    }
}