using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 1256.
    /// Represents an entry in Router Advertisement ICMP message.
    /// </summary>
    public class IcmpRouterAdvertisementEntry : IEquatable<IcmpRouterAdvertisementEntry>
    {
        /// <summary>
        /// Creates an instance using the given router address and preference.
        /// </summary>
        /// <param name="routerAddress">The sending router's IP address(es) on the interface from which this message is sent.</param>
        /// <param name="routerAddressPreference">The preferability of each Router Address[i] as a default router address, relative to other router addresses on the same subnet. A signed, twos-complement value; higher values mean more preferable.</param>
        public IcmpRouterAdvertisementEntry(IpV4Address routerAddress, int routerAddressPreference)
        {
            _routerAddress = routerAddress;
            _routerAddressPreference = routerAddressPreference;
        }

        /// <summary>
        /// The sending router's IP address(es) on the interface from which this message is sent.
        /// </summary>
        public IpV4Address RouterAddress
        {
            get { return _routerAddress;}
        }

        /// <summary>
        /// The preferability of each Router Address[i] as a default router address, relative to other router addresses on the same subnet. A signed, twos-complement value; higher values mean more preferable.
        /// </summary>
        public int RouterAddressPreference
        {
            get {return _routerAddressPreference; }
        }

        /// <summary>
        /// Two entries are equal if they have the same router address and preference.
        /// </summary>
        public bool Equals(IcmpRouterAdvertisementEntry other)
        {
            return other != null &&
                   RouterAddress == other.RouterAddress &&
                   RouterAddressPreference == other.RouterAddressPreference;
        }

        /// <summary>
        /// Two entries are equal if they have the same router address and preference.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as IcmpRouterAdvertisementEntry);
        }

        /// <summary>
        /// A xor of the hash codes of the router address and preference.
        /// </summary>
        public override int GetHashCode()
        {
            return RouterAddress.GetHashCode() ^ RouterAddressPreference.GetHashCode();
        }

        private readonly IpV4Address _routerAddress;
        private readonly int _routerAddressPreference;
    }
}