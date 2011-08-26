using System;

namespace PcapDotNet.Packets.Dns
{
    public class DnsResourceDataUnknown : DnsResourceData, IEquatable<DnsResourceDataUnknown>
    {
        public DnsResourceDataUnknown(DataSegment data)
        {
            Data = data;
        }

        public DataSegment Data { get; private set; }

        public bool Equals(DnsResourceDataUnknown other)
        {
            return other != null && Data.Equals(other.Data);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataUnknown);
        }

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