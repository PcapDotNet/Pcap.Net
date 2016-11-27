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

        public DhcpPolicyFilterOption(IList<IpV4AddressWithMask> filters) : base(DhcpOptionCode.PolicyFilter)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));
            if (filters.Count > MAX_FILTERS)
                throw new ArgumentOutOfRangeException(nameof(filters), filters.Count, $"The maximum items in filters is {MAX_FILTERS}");

            Filters = new ReadOnlyCollection<IpV4AddressWithMask>(filters);
        }

        internal static DhcpPolicyFilterOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            if (length % 8 != 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, "length has to be a multiple of 8");
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

        public override byte Length
        {
            get
            {
                if (Filters.Count > MAX_FILTERS)
                    throw new ArgumentOutOfRangeException(nameof(Filters), Filters.Count, $"The maximum items in Filter is {MAX_FILTERS}");

                return (byte)(Filters.Count * IpV4AddressWithMask.SizeOf);
            }
        }

        public IReadOnlyCollection<IpV4AddressWithMask> Filters
        {
            get;
            private set;
        }

        public struct IpV4AddressWithMask
        {
            /// <summary>
            /// The number of bytes the IpV4AddressWithMask take.
            /// </summary>
            public const int SizeOf = IpV4Address.SizeOf + IpV4Address.SizeOf;

            public IpV4Address Address

            {
                get;
                private set;
            }

            public IpV4Address Mask
            {
                get;
                private set;
            }

            public IpV4AddressWithMask(IpV4Address address, IpV4Address mask)
            {
                Address = address;
                Mask = mask;
            }
        }
    }
}