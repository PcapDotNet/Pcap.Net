using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Gre
{
    /// <summary>
    /// RFC 1702.
    /// Represents a source route entry consisting of a list of IP addresses and indicates an IP source route.
    /// </summary>
    public class GreSourceRouteEntryIp : GreSourceRouteEntry
    {
        /// <summary>
        /// Initializes using the given IP addresses and the next as number index.
        /// </summary>
        /// <param name="addresses">IP addresses of the source route.</param>
        /// <param name="nextAddressIndex">The next IP address index in the source route.</param>
        public GreSourceRouteEntryIp(ReadOnlyCollection<IpV4Address> addresses, int nextAddressIndex)
        {
            _addresses = addresses;
            _nextAddressIndex = nextAddressIndex;
        }

        /// <summary>
        /// The Address Family field contains a two octet value which indicates the syntax and semantics of the Routing Information field.  
        /// </summary>
        public override GreSourceRouteEntryAddressFamily AddressFamily
        {
            get { return GreSourceRouteEntryAddressFamily.IpSourceRoute; }
        }

        /// <summary>
        /// The SRE Length field contains the number of octets in the SRE.  
        /// </summary>
        public override byte PayloadLength
        {
            get { return (byte)(Addresses.Count * IpV4Address.SizeOf); }
        }

        /// <summary>
        /// The SRE Offset field indicates the octet offset from the start of the Routing Information field to the first octet of the active entry in Source Route Entry to be examined.
        /// </summary>
        public override byte PayloadOffset
        {
            get { return (byte)(NextAddressIndex * IpV4Address.SizeOf); }
        }

        /// <summary>
        /// True iff the IP addresses are equal.
        /// </summary>
        protected override bool EqualsPayloads(GreSourceRouteEntry other)
        {
            return Addresses.SequenceEqual(((GreSourceRouteEntryIp)other).Addresses);
        }

        /// <summary>
        /// The xor of the hash code of the IP addresses.
        /// </summary>
        protected override int PayloadHashCode
        {
            get { return Addresses.SequenceGetHashCode(); }
        }

        /// <summary>
        /// Writes the payload to the given buffer in the given offset.
        /// </summary>
        /// <param name="buffer">The buffer to write the payload to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        protected override void WritePayload(byte[] buffer, int offset)
        {
            foreach (IpV4Address address in Addresses)
                buffer.Write(ref offset, address, Endianity.Big);
        }

        /// <summary>
        /// IP addresses of the source route.
        /// </summary>
        public ReadOnlyCollection<IpV4Address> Addresses
        {
            get { return _addresses; }
        }

        /// <summary>
        /// The next IP address index in the source route.
        /// </summary>
        public int NextAddressIndex
        {
            get { return _nextAddressIndex; }
        }

        /// <summary>
        /// The next IP address.
        /// </summary>
        public IpV4Address NextAddress
        {
            get { return Addresses[NextAddressIndex]; }
        }

        internal GreSourceRouteEntryIp(IpV4Address[] addresses, int nextAddressIndex)
            :this(addresses.AsReadOnly(), nextAddressIndex)
        {
        }

        private readonly ReadOnlyCollection<IpV4Address> _addresses;
        private readonly int _nextAddressIndex;
    }
}