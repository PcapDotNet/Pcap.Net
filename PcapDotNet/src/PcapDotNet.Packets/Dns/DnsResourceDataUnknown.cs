using System;

namespace PcapDotNet.Packets.Dns
{
    public sealed class DnsResourceDataUnknown : DnsResourceDataSimple, IEquatable<DnsResourceDataUnknown>
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

        public sealed override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataUnknown);
        }

        internal override int GetLength()
        {
            return Data.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            Data.Write(buffer, offset);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            return new DnsResourceDataUnknown(data);
        }
    }
}