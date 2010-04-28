namespace PcapDotNet.Packets.Gre
{
    public class GreSourceRouteEntryUnknown : GreSourceRouteEntry
    {
        public GreSourceRouteEntryUnknown(GreSourceRouteEntryAddressFamily addressFamily, Datagram data, int offset)
        {
            _addressFamily = addressFamily;
            _data = data;
            _offset = offset;
        }

        public override GreSourceRouteEntryAddressFamily AddressFamily
        {
            get { return _addressFamily; }
        }

        public override byte PayloadLength
        {
            get { return (byte)Data.Length; }
        }

        public override byte PayloadOffset
        {
            get { return (byte)_offset; }
        }

        public Datagram Data
        {
            get { return _data; }
        }

        protected override bool EqualsPayloads(GreSourceRouteEntry other)
        {
            return Data.Equals(((GreSourceRouteEntryUnknown)other).Data);
        }

        protected override void WritePayload(byte[] buffer, int offset)
        {
            buffer.Write(offset, Data);
        }

        private readonly GreSourceRouteEntryAddressFamily _addressFamily;
        private readonly Datagram _data;
        private readonly int _offset;
    }
}