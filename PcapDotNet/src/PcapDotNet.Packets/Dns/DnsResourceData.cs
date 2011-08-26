namespace PcapDotNet.Packets.Dns
{
    public abstract class DnsResourceData
    {
        internal static DnsResourceData Read(DnsDatagram dns, int offsetInDns, int dataLength)
        {
            return new DnsResourceDataUnknown(dns.SubSegment(offsetInDns, dataLength));
        }

        internal abstract int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns);

        internal int Write(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            int length = WriteData(buffer, dnsOffset, offsetInDns + sizeof(ushort), compressionData);
            buffer.Write(dnsOffset + offsetInDns, (ushort)length, Endianity.Big);
            length += sizeof(ushort);
            return length;
        }

        internal abstract int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData);
    }
}