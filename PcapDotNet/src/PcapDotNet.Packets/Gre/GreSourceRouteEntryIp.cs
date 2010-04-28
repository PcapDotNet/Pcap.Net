using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Gre
{
    public class GreSourceRouteEntryIp : GreSourceRouteEntry
    {
        public GreSourceRouteEntryIp(ReadOnlyCollection<IpV4Address> addresses, int nextAddressIndex)
        {
            _addresses = addresses;
            _nextAddressIndex = nextAddressIndex;
        }

        public override GreSourceRouteEntryAddressFamily AddressFamily
        {
            get { return GreSourceRouteEntryAddressFamily.IpSourceRoute; }
        }

        public override byte PayloadLength
        {
            get { return (byte)(Addresses.Count * IpV4Address.SizeOf); }
        }

        public override byte PayloadOffset
        {
            get { return (byte)(NextAddressIndex * IpV4Address.SizeOf); }
        }

        protected override bool EqualsPayloads(GreSourceRouteEntry other)
        {
            return Addresses.SequenceEqual(((GreSourceRouteEntryIp)other).Addresses);
        }

        protected override void WritePayload(byte[] buffer, int offset)
        {
            foreach (IpV4Address address in Addresses)
                buffer.Write(ref offset, address, Endianity.Big);
        }

        public ReadOnlyCollection<IpV4Address> Addresses
        {
            get { return _addresses; }
        }

        public int NextAddressIndex
        {
            get { return _nextAddressIndex; }
        }

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