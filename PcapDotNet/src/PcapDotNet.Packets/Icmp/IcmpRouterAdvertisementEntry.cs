using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    public class IcmpRouterAdvertisementEntry
    {
        public IcmpRouterAdvertisementEntry(IpV4Address routerAddress, int routerAddressPreference)
        {
            _routerAddress = routerAddress;
            _routerAddressPreference = routerAddressPreference;
        }

        public IpV4Address RouterAddress
        {
            get { return _routerAddress;}
        }

        public int RouterAddressPreference
        {
            get {return _routerAddressPreference; }
        }

        public bool Equals(IcmpRouterAdvertisementEntry other)
        {
            return other != null &&
                   RouterAddress == other.RouterAddress &&
                   RouterAddressPreference == other.RouterAddressPreference;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IcmpRouterAdvertisementEntry);
        }

        public override int GetHashCode()
        {
            return RouterAddress.GetHashCode() ^ RouterAddressPreference.GetHashCode();
        }

        private readonly IpV4Address _routerAddress;
        private readonly int _routerAddressPreference;
    }
}