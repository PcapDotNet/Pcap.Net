using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Gre
{
    /// <summary>
    /// RFC 1702.
    /// Represents a source route entry consisting of a list of Autonomous System numbers and indicates an AS source route.
    /// </summary>
    public sealed class GreSourceRouteEntryAs : GreSourceRouteEntry
    {
        /// <summary>
        /// Initializes using the given AS numbers and the next as number index.
        /// </summary>
        /// <param name="asNumbers">Autonomous System numbers of the source route.</param>
        /// <param name="nextAsNumberIndex">The next AS number index in the source route.</param>
        public GreSourceRouteEntryAs(ReadOnlyCollection<ushort> asNumbers, int nextAsNumberIndex)
        {
            _asNumbers = asNumbers;
            _nextAsNumberIndex = nextAsNumberIndex;
        }

        /// <summary>
        /// The Address Family field contains a two octet value which indicates the syntax and semantics of the Routing Information field.
        /// </summary>
        public override GreSourceRouteEntryAddressFamily AddressFamily
        {
            get { return GreSourceRouteEntryAddressFamily.AsSourceRoute; }
        }

        /// <summary>
        /// The SRE Length field contains the number of octets in the SRE.  
        /// </summary>
        public override byte PayloadLength
        {
            get { return (byte)(AsNumbers.Count * sizeof(ushort)); }
        }

        /// <summary>
        /// The SRE Offset field indicates the octet offset from the start of the Routing Information field to the first octet of the active entry in Source Route Entry to be examined.
        /// </summary>
        public override byte PayloadOffset
        {
            get { return (byte)(NextAsNumberIndex * sizeof(ushort)); }
        }

        /// <summary>
        /// True iff the AS numbers are equal.
        /// </summary>
        protected override bool EqualsPayloads(GreSourceRouteEntry other)
        {
            return AsNumbers.SequenceEqual(((GreSourceRouteEntryAs)other).AsNumbers);
        }

        /// <summary>
        /// The xor of the hash code of the AS numbers.
        /// </summary>
        protected override int PayloadHashCode
        {
            get { return AsNumbers.UShortsSequenceGetHashCode(); }
        }

        /// <summary>
        /// Autonomous System numbers of the source route.
        /// </summary>
        public ReadOnlyCollection<ushort> AsNumbers
        {
            get { return _asNumbers; }
        }

        /// <summary>
        /// The next AS number index in the source route.
        /// </summary>
        public int NextAsNumberIndex
        {
            get { return _nextAsNumberIndex; }
        }

        /// <summary>
        /// The next AS number.
        /// </summary>
        public ushort NextAsNumber
        {
            get { return AsNumbers[NextAsNumberIndex]; }
        }

        /// <summary>
        /// Writes the payload to the given buffer in the given offset.
        /// </summary>
        /// <param name="buffer">The buffer to write the payload to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        protected override void WritePayload(byte[] buffer, int offset)
        {
            foreach (ushort asNumber in AsNumbers)
                buffer.Write(ref offset, asNumber, Endianity.Big);
        }

        internal GreSourceRouteEntryAs(ushort[] asNumbers, int nextAsNumberIndex)
            : this(asNumbers.AsReadOnly(), nextAsNumberIndex)
        {
        }

        private readonly ReadOnlyCollection<ushort> _asNumbers;
        private readonly int _nextAsNumberIndex;
    }
}