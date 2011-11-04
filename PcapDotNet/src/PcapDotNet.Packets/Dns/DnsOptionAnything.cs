using System;

namespace PcapDotNet.Packets.Dns
{
    public class DnsOptionAnything : DnsOption
    {
        public DnsOptionAnything(DnsOptionCode code, DataSegment data)
            : base(code)
        {
            Data = data;
        }

        public DataSegment Data { get; private set; }

        public override int DataLength
        {
            get { return Data.Length; }
        }

        internal override bool EqualsData(DnsOption other)
        {
            return Data.Equals(((DnsOptionAnything)other).Data);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            Data.Write(buffer, ref offset);
        }
    }
}