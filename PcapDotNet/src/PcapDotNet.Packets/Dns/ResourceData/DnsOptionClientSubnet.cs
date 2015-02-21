using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// https://tools.ietf.org/html/draft-ietf-dnsop-edns-client-subnet
    /// <pre>
    /// +-----+----------------+---------------+
    /// | bit | 0-7            | 8-15          |
    /// +-----+----------------+---------------+
    /// | 0   | FAMILY                         |
    /// +-----+----------------+---------------+
    /// | 16  | SOURCE NETMASK | SCOPE NETMASK |
    /// +-----+----------------+---------------+
    /// | 32  | ADDRESS                        |
    /// | ... |                                |
    /// +-----+--------------------------------+
    /// </pre>
    /// </summary>
    public sealed class DnsOptionClientSubnet : DnsOption
    {
        private static class Offset
        {
            public const int Family = 0;
            public const int SourceNetmask = Family + sizeof(ushort);
            public const int ScopeNetmask = SourceNetmask + sizeof(byte);
            public const int Address = ScopeNetmask + sizeof(byte);
        }


        /// <summary>
        /// The minimum number of bytes this option data can take.
        /// </summary>
        public const int MininmumDataLength = Offset.Address;

        public DnsOptionClientSubnet(AddressFamily family, byte sourceMask, byte scopeNetmask, DataSegment address)
            : base(DnsOptionCode.ClientSubnet)
        {
            Family = family;
            SourceNetmask = sourceMask;
            ScopeNetmask = scopeNetmask;
            Address = address;
        }

        /// <summary>
        /// Indicates the family of the address contained in the option.
        /// </summary>
        public AddressFamily Family { get; private set; }

        /// <summary>
        /// Representing the length of the netmask pertaining to the query.
        /// In replies, it mirrors the same value as in the requests.
        /// It can be set to 0 to disable client-based lookups, in which case the Address field must be absent.
        /// </summary>
        public byte SourceNetmask { get; private set; }

        /// <summary>
        /// Representing the length of the netmask pertaining to the reply.
        /// In requests, it should be set to the longest cacheable length supported by the Intermediate Nameserver.
        /// In requests it may be set to 0 to have the Authoritative Nameserver treat the longest cacheable length as the SourceNetmask length.
        /// In responses, this field is set by the Authoritative Nameserver to indicate the coverage of the response.
        /// It might or might not match SourceNetmask; it could be shorter or longer.
        /// </summary>
        public byte ScopeNetmask { get; private set; }

        /// <summary>
        /// Contains either an IPv4 or IPv6 address, depending on Family, truncated in the request to the number of bits indicated by the Source Netmask field,
        /// with bits set to 0 to pad up to the end of the last octet used. (This need not be as many octets as a complete address would take.)
        /// In the reply, if the ScopeNetmask of the request was 0 then Address must contain the same octets as in the request.
        /// Otherwise, the bits for Address will be significant through the maximum of the SouceNetmask or ScopeNetmask, and 0 filled to the end of an octet.
        /// </summary>
        public DataSegment Address { get; private set; }

        /// <summary>
        /// The number of bytes the option data takes.
        /// </summary>
        public override int DataLength
        {
            get { return MininmumDataLength + Address.Length; }
        }

        internal override bool EqualsData(DnsOption other)
        {
            return EqualsData(other as DnsOptionClientSubnet);
        }

        internal override int DataGetHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge((ushort)Family, SourceNetmask, ScopeNetmask), Address);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, (ushort)Family, Endianity.Big);
            buffer.Write(ref offset, SourceNetmask);
            buffer.Write(ref offset, ScopeNetmask);
            buffer.Write(ref offset, Address);
        }

        internal static DnsOptionClientSubnet Read(DataSegment data)
        {
            if (data.Length < MininmumDataLength)
                return null;

            AddressFamily family = (AddressFamily)data.ReadUShort(Offset.Family, Endianity.Big);
            byte sourceNetmask = data[Offset.SourceNetmask];
            byte scopeNetmask = data[Offset.ScopeNetmask];
            DataSegment address = data.Subsegment(Offset.Address, data.Length - Offset.Address);

            return new DnsOptionClientSubnet(family, sourceNetmask, scopeNetmask, address);
        }

        private bool EqualsData(DnsOptionClientSubnet other)
        {
            return other != null &&
                   Family.Equals(other.Family) &&
                   SourceNetmask.Equals(other.SourceNetmask) &&
                   ScopeNetmask.Equals(other.ScopeNetmask) &&
                   Address.Equals(other.Address);
        }
    }
}