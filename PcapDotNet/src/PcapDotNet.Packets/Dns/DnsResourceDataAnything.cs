using System;

namespace PcapDotNet.Packets.Dns
{
    [DnsTypeRegistration(Type = DnsType.Null)]
    [DnsTypeRegistration(Type = DnsType.EId)]
    [DnsTypeRegistration(Type = DnsType.NimLoc)]
    public sealed class DnsResourceDataAnything : DnsResourceDataSimple, IEquatable<DnsResourceDataAnything>
    {
        public DnsResourceDataAnything()
        {
            Data = DataSegment.Empty;
        }

        public DnsResourceDataAnything(DataSegment data)
        {
            Data = data;
        }

        public DataSegment Data { get; private set; }

        public bool Equals(DnsResourceDataAnything other)
        {
            return other != null && Data.Equals(other.Data);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataAnything);
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
            return new DnsResourceDataAnything(data);
        }
    }
}