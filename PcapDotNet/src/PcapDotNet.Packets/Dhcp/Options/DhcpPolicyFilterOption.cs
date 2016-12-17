using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option specifies policy filters for non-local source routing.
    /// The filters consist of a list of IP addresses and masks which specify
    /// destination/mask pairs with which to filter incoming source routes.
    /// Any source routed datagram whose next-hop address does not match one
    /// of the filters should be discarded by the client.
    /// <pre>
    ///  Code   Len         Address 1                  Mask 1
    /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+
    /// |  21 |  n  |  a1 |  a2 |  a3 |  a4 |  m1 |  m2 |  m3 |  m4 |
    /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+
    ///         Address 2                  Mask 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+---
    /// |  a1 |  a2 |  a3 |  a4 |  m1 |  m2 |  m3 |  m4 | ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpPolicyFilterOption : DhcpOption
    {
        internal const int MAX_FILTERS = 255 / IpV4AddressWithMask.SizeOf;

        /// <summary>
        /// create new DhcpPolicyFilterOption
        /// </summary>
        /// <param name="filters">Filters</param>
        public DhcpPolicyFilterOption(IList<IpV4AddressWithMask> filters) : base(DhcpOptionCode.PolicyFilter)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));
            if (filters.Count < 1)
                throw new ArgumentOutOfRangeException(nameof(filters), filters.Count, "The minimum items in filters is 1");
            if (filters.Count > MAX_FILTERS)
                throw new ArgumentOutOfRangeException(nameof(filters), filters.Count, "The maximum items in filters is " + MAX_FILTERS);

            Filters = new ReadOnlyCollection<IpV4AddressWithMask>(filters);
        }

        internal static DhcpPolicyFilterOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            if (length % 8 != 0)
                throw new ArgumentException("length has to be a multiple of 8");
            IList<IpV4Address> addresses = DhcpAddressListOption.GetAddresses(data, length, ref offset);

            List<IpV4AddressWithMask> filters = new List<IpV4AddressWithMask>();
            for (int i = 0; i < addresses.Count; i += 2)
            {
                filters.Add(new IpV4AddressWithMask(addresses[i], addresses[i + 1]));
            }
            return new DhcpPolicyFilterOption(filters);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            foreach (IpV4AddressWithMask filter in Filters)
            {
                buffer.Write(ref offset, filter.Address, Endianity.Big);
                buffer.Write(ref offset, filter.Mask, Endianity.Big);
            }
        }

        /// <summary>
        /// Length of the Dhcp-Option
        /// </summary>
        public override byte Length
        {
            get
            {
                return (byte)(Filters.Count * IpV4AddressWithMask.SizeOf);
            }
        }

        /// <summary>
        /// Filters
        /// </summary>
        public IReadOnlyCollection<IpV4AddressWithMask> Filters
        {
            get;
            private set;
        }

        /// <summary>
        /// Represents and IpV4Address with Mask
        /// </summary>
        public struct IpV4AddressWithMask : IEquatable<IpV4AddressWithMask>
        {
            /// <summary>
            /// The number of bytes the IpV4AddressWithMask take.
            /// </summary>
            public const int SizeOf = IpV4Address.SizeOf + IpV4Address.SizeOf;

            /// <summary>
            /// Address
            /// </summary>
            public IpV4Address Address

            {
                get;
                private set;
            }

            /// <summary>
            /// Mask
            /// </summary>
            public IpV4Address Mask
            {
                get;
                private set;
            }

            /// <summary>
            /// create new IpV4AddressWithMask
            /// </summary>
            /// <param name="address">Address</param>
            /// <param name="mask">Mask</param>
            public IpV4AddressWithMask(IpV4Address address, IpV4Address mask)
            {
                Address = address;
                Mask = mask;
            }

            /// <summary>
            /// Determines whether the specified object is equal to the current object.
            /// </summary>
            /// <param name="obj">The object to compare with the current object.</param>
            /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
            public override bool Equals(object obj)
            {
                if (obj is IpV4AddressWithMask)
                {
                    return Equals((IpV4AddressWithMask)obj);
                }
                return false;
            }

            /// <summary>
            /// Determines whether the IpV4AddressWithMask is equal to the current IpV4AddressWithMask.
            /// </summary>
            /// <param name="other">The object to compare with the current object.</param>
            /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
            public bool Equals(IpV4AddressWithMask other)
            {
                return object.Equals(Address, other.Address) &&
                    object.Equals(Mask, other.Mask);
            }

            /// <summary>
            /// Returns the hash code for this instance.
            /// </summary>
            /// <returns>A 32-bit signed integer hash code.</returns>
            public override int GetHashCode()
            {
                return Address.GetHashCode() ^ Mask.GetHashCode();
            }

            /// <summary>
            /// Determines whether the IpV4AddressWithMask is equal to the current IpV4AddressWithMask.
            /// </summary>
            /// <param name="left">left IpV4AddressWithMask</param>
            /// <param name="right">right IpV4AddressWithMask</param>
            /// <returns></returns>
            public static bool operator ==(IpV4AddressWithMask left, IpV4AddressWithMask right)
            {
                return Object.Equals(left, right);
            }

            /// <summary>
            /// Determines whether the IpV4AddressWithMask is not equal to the current IpV4AddressWithMask.
            /// </summary>
            /// <param name="left">left IpV4AddressWithMask</param>
            /// <param name="right">right IpV4AddressWithMask</param>
            /// <returns></returns>
            public static bool operator !=(IpV4AddressWithMask left, IpV4AddressWithMask right)
            {
                return !Object.Equals(left, right);
            }
        }
    }
}