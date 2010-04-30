namespace PcapDotNet.Packets.Gre
{
    /// <summary>
    /// Represents a source route entry consisting of an unknown data.
    /// </summary>
    public class GreSourceRouteEntryUnknown : GreSourceRouteEntry
    {
        /// <summary>
        /// Initializes using an address family, data, and offset to the first octet of the active entry in Source Route Entry to be examined.
        /// </summary>
        /// <param name="addressFamily">The Address Family field contains a two octet value which indicates the syntax and semantics of the Routing Information field.</param>
        /// <param name="data">The data of the entry source route.</param>
        /// <param name="offset">The SRE Offset field indicates the octet offset from the start of the Routing Information field to the first octet of the active entry in Source Route Entry to be examined.</param>
        public GreSourceRouteEntryUnknown(GreSourceRouteEntryAddressFamily addressFamily, Datagram data, int offset)
        {
            _addressFamily = addressFamily;
            _data = data;
            _offset = offset;
        }

        /// <summary>
        /// The Address Family field contains a two octet value which indicates the syntax and semantics of the Routing Information field.
        /// </summary>
        public override GreSourceRouteEntryAddressFamily AddressFamily
        {
            get { return _addressFamily; }
        }

        /// <summary>
        /// The SRE Length field contains the number of octets in the SRE.  
        /// </summary>
        public override byte PayloadLength
        {
            get { return (byte)Data.Length; }
        }

        /// <summary>
        /// The SRE Offset field indicates the octet offset from the start of the Routing Information field to the first octet of the active entry in Source Route Entry to be examined.
        /// </summary>
        public override byte PayloadOffset
        {
            get { return (byte)_offset; }
        }

        /// <summary>
        /// The data of the entry source route.
        /// </summary>
        public Datagram Data
        {
            get { return _data; }
        }

        /// <summary>
        /// True iff the payloads a are equal.
        /// </summary>
        protected override bool EqualsPayloads(GreSourceRouteEntry other)
        {
            return Data.Equals(((GreSourceRouteEntryUnknown)other).Data);
        }

        /// <summary>
        /// The hash code of the payload.
        /// </summary>
        protected override int PayloadHashCode
        {
            get { return Data.GetHashCode(); }
        }

        /// <summary>
        /// Writes the payload to the given buffer in the given offset.
        /// </summary>
        /// <param name="buffer">The buffer to write the payload to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        protected override void WritePayload(byte[] buffer, int offset)
        {
            buffer.Write(offset, Data);
        }

        private readonly GreSourceRouteEntryAddressFamily _addressFamily;
        private readonly Datagram _data;
        private readonly int _offset;
    }
}