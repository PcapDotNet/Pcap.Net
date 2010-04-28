using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Gre
{
    public class GreSourceRouteEntryAs : GreSourceRouteEntry
    {
        public GreSourceRouteEntryAs(ReadOnlyCollection<ushort> asNumbers, int nextAsNumberIndex)
        {
            _asNumbers = asNumbers;
            _nextAsNumberIndex = nextAsNumberIndex;
        }

        public override GreSourceRouteEntryAddressFamily AddressFamily
        {
            get { return GreSourceRouteEntryAddressFamily.AsSourceRoute; }
        }

        public override byte PayloadLength
        {
            get { return (byte)(AsNumbers.Count * sizeof(ushort)); }
        }

        public override byte PayloadOffset
        {
            get { return (byte)(NextAsNumberIndex * sizeof(ushort)); }
        }

        protected override bool EqualsPayloads(GreSourceRouteEntry other)
        {
            return AsNumbers.SequenceEqual(((GreSourceRouteEntryAs)other).AsNumbers);
        }

        public ReadOnlyCollection<ushort> AsNumbers
        {
            get { return _asNumbers; }
        }

        public int NextAsNumberIndex
        {
            get { return _nextAsNumberIndex; }
        }

        public ushort NextAsNumber
        {
            get { return AsNumbers[NextAsNumberIndex]; }
        }

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