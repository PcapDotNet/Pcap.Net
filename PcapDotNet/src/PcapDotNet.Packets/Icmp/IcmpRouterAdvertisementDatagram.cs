using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 1256.
    /// <pre>
    /// +-----+-----------+-----------------+----------+
    /// | Bit | 0-7       | 8-15            | 16-31    |
    /// +-----+-----------+-----------------+----------+
    /// | 0   | Type      | Code            | Checksum |
    /// +-----+-----------+-----------------+----------+
    /// | 32  | Num Addrs | Addr Entry Size | Lifetime |
    /// +-----+-----------+-----------------+----------+
    /// | 64  | Router Address[1]                      |
    /// +-----+----------------------------------------+
    /// | 96  | Preference Level[1]                    |
    /// +-----+----------------------------------------+
    /// | 128 | Router Address[2]                      |
    /// +-----+----------------------------------------+
    /// | 160 | Preference Level[2]                    |
    /// +-----+----------------------------------------+
    /// |  .  |                   .                    |
    /// |  .  |                   .                    |
    /// |  .  |                   .                    |
    /// </pre>
    /// </summary>
    [IcmpDatagramRegistration(IcmpMessageType.RouterAdvertisement)]
    public sealed class IcmpRouterAdvertisementDatagram : IcmpDatagram
    {
        /// <summary>
        /// The default number of 32-bit words of information per each router address.
        /// </summary>
        public const int DefaultAddressEntrySize = 2;

        private static class Offset
        {
            public const int NumberOfAddresses = 4;
            public const int AddressEntrySize = 5;
            public const int Lifetime = 6;
            public const int Addresses = 8;
        }

        /// <summary>
        /// The number of router addresses advertised in this message.
        /// </summary>
        public byte NumberOfAddresses
        {
            get { return this[Offset.NumberOfAddresses]; }
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
        /// The pairs of sending router's IP address(es) on the interface from which this message is sent
        /// and the preferability of each Router Address[i] as a default router address, relative to other router addresses on the same subnet.
        /// A signed, twos-complement value; higher values mean more preferable.
        /// </summary>
        public ReadOnlyCollection<IcmpRouterAdvertisementEntry> Entries
        {
            get
            {
                if (_entries == null)
                {
                    IcmpRouterAdvertisementEntry[] entries = new IcmpRouterAdvertisementEntry[NumberOfAddresses];
                    int currentOffset = Offset.Addresses;
                    for (int i = 0; i != entries.Length && currentOffset + IpV4Address.SizeOf <= Length; ++i)
                    {
                        entries[i] = new IcmpRouterAdvertisementEntry(ReadIpV4Address(currentOffset, Endianity.Big),
                                                                      ReadInt(currentOffset + IpV4Address.SizeOf, Endianity.Big));
                        currentOffset += AddressEntrySize * IpV4Address.SizeOf;
                    }
                    _entries = new ReadOnlyCollection<IcmpRouterAdvertisementEntry>(entries);
                }

                return _entries;
            }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IcmpRouterAdvertisementLayer
                   {
                       Entries = Entries.ToList().AsReadOnly(),
                       Checksum = Checksum,
                       Lifetime = Lifetime,
                   };
        }

        /// <summary>
        /// Valid if the datagram's length is OK, the checksum is correct, the code is in the expected range
        /// and the address entry size is the default address entry size.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            return base.CalculateIsValid() &&
                   AddressEntrySize == DefaultAddressEntrySize &&
                   Length == HeaderLength + NumberOfAddresses * AddressEntrySize * IpV4Address.SizeOf;
        }

        internal override IcmpDatagram CreateInstance(byte[] buffer, int offset, int length)
        {
            return new IcmpRouterAdvertisementDatagram(buffer, offset, length);
        }

        internal static int GetPayloadLength(int numEntries)
        {
            return numEntries * DefaultAddressEntrySize * IpV4Address.SizeOf;
        }

        internal static void WriteHeaderAdditional(byte[] buffer, int offset,
                                                   IEnumerable<IcmpRouterAdvertisementEntry> entries)
        {
            foreach (IcmpRouterAdvertisementEntry entry in entries)
            {
                buffer.Write(ref offset, entry.RouterAddress, Endianity.Big);
                buffer.Write(ref offset, entry.RouterAddressPreference, Endianity.Big);
            }
        }

        private IcmpRouterAdvertisementDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        private ReadOnlyCollection<IcmpRouterAdvertisementEntry> _entries;
    }
}