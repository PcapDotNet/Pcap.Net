using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 1256.
    /// <pre>
    /// +-----+-----------+-----------------+----------+
    /// | Bit | 0-7       | 8-15            | 16-31    |
    /// +-----+-----------+-----------------+----------+
    /// | 0   | Num Addrs | Addr Entry Size | Lifetime |
    /// +-----+-----------+-----------------+----------+
    /// | 32  | Router Address[1]                      |
    /// +-----+----------------------------------------+
    /// | 64  | Preference Level[1]                    |
    /// +-----+----------------------------------------+
    /// | 96  | Router Address[2]                      |
    /// +-----+----------------------------------------+
    /// | 128 | Preference Level[2]                    |
    /// +-----+----------------------------------------+
    /// |  .  |                   .                    |
    /// |  .  |                   .                    |
    /// |  .  |                   .                    |
    /// </pre>
    /// </summary>
    public class IcmpRouterAdvertisementDatagram : IcmpTypedDatagram
    {
        private class Offset
        {
            public const int NumAddresses = 0;
            public const int AddressEntrySize = 1;
            public const int Lifetime = 2;
            public const int Addresses = 4;
        }

        /// <summary>
        /// The number of router addresses advertised in this message.
        /// </summary>
        public byte NumAddresses
        {
            get { return this[Offset.NumAddresses]; }
        }

        /// <summary>
        /// The number of 32-bit words of information per each router address (2, in the version of the protocol described here).
        /// </summary>
        public byte AddressEntrySize
        {
            get { return this[Offset.AddressEntrySize]; }
        }

        /// <summary>
        /// The maximum number of seconds that the router addresses may be considered valid.
        /// </summary>
        public ushort LifetimeSeconds
        {
            get { return ReadUShort(Offset.Lifetime, Endianity.Big); }
        }

        /// <summary>
        /// The maximum time that the router addresses may be considered valid.
        /// </summary>
        public TimeSpan Lifetime
        {
            get { return TimeSpan.FromSeconds(LifetimeSeconds); }
        }

        /// <summary>
        /// The sending router's IP address(es) on the interface from which this message is sent.
        /// </summary>
        public ReadOnlyCollection<IpV4Address> RouterAddresses
        {
            get
            {
                if (_routerAddresses == null)
                {
                    IpV4Address[] addresses = new IpV4Address[NumAddresses];
                    int currentOffset = Offset.Addresses;
                    for (int i = 0; i != addresses.Length && currentOffset + IpV4Address.SizeOf <= Length; ++i)
                    {
                        addresses[i] = ReadIpV4Address(currentOffset, Endianity.Big);
                        currentOffset += AddressEntrySize * IpV4Address.SizeOf;
                    }
                    _routerAddresses = new ReadOnlyCollection<IpV4Address>(addresses);
                }

                return _routerAddresses;
            }
        }

        /// <summary>
        /// The preferability of each Router Address[i] as a default router address, relative to other router addresses on the same subnet.
        /// A signed, twos-complement value; higher values mean more preferable.
        /// </summary>
        public ReadOnlyCollection<int> RouterAddressesPreferences
        {
            get
            {
                if (_routerAddressesPreferences == null)
                {
                    int[] addressesPreferences = new int[NumAddresses];
                    int currentOffset = Offset.Addresses;
                    for (int i = 0; i != addressesPreferences.Length && currentOffset + sizeof(int) <= Length; ++i)
                    {
                        addressesPreferences[i] = ReadInt(currentOffset, Endianity.Big);
                        currentOffset += AddressEntrySize * IpV4Address.SizeOf;
                    }
                    _routerAddressesPreferences = new ReadOnlyCollection<int>(addressesPreferences);
                }

                return _routerAddressesPreferences;
            }
        }

        internal IcmpRouterAdvertisementDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        private ReadOnlyCollection<IpV4Address> _routerAddresses;
        private ReadOnlyCollection<int> _routerAddressesPreferences;
    }
}