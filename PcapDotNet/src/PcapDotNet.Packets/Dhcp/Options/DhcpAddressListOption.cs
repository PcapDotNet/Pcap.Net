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
    /// Abstract class for all Dhcp-Options with a list of Addresses.
    /// </summary>
    public abstract class DhcpAddressListOption : DhcpOption
    {
        internal const int MAX_ADDRESSES = byte.MaxValue / IpV4Address.SizeOf;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        internal DhcpAddressListOption(IList<IpV4Address> addresses, DhcpOptionCode code) : base(code)
        {
            if (addresses == null)
                throw new ArgumentNullException(nameof(addresses));
            if (addresses.Count == 0 && !AllowEmptyAddresses)
                throw new ArgumentOutOfRangeException(nameof(addresses), addresses.Count, "The minimum items in addresses is 1");
            if (addresses.Count > MAX_ADDRESSES)
                throw new ArgumentOutOfRangeException(nameof(addresses), addresses.Count, "The maximum items in addresses is " + MAX_ADDRESSES);
            Addresses = new ReadOnlyCollection<IpV4Address>(addresses);
        }

        internal static T Read<T>(DataSegment data, ref int offset, Func<IList<IpV4Address>, T> ctor) where T : DhcpAddressListOption
        {
            return ctor(GetAddresses(data, ref offset));
        }

        internal static IList<IpV4Address> GetAddresses(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            return GetAddresses(data, length, ref offset);
        }

        internal static IList<IpV4Address> GetAddresses(DataSegment data, byte length, ref int offset)
        {
            if (length % IpV4Address.SizeOf != 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, "length has to be multiple of " + IpV4Address.SizeOf);
            List<IpV4Address> addresses = new List<IpV4Address>();
            for (int i = 0; i < length; i += IpV4Address.SizeOf)
            {
                addresses.Add(data.ReadIpV4Address(offset + i, Endianity.Big));
            }
            offset += length;
            return addresses;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            foreach (IpV4Address address in Addresses)
            {
                buffer.Write(ref offset, address, Endianity.Big);
            }
        }

        /// <summary>
        /// true if Addresses-List is allowed to be empty (Default false).
        /// </summary>
        protected virtual bool AllowEmptyAddresses
        {
            get { return false; }
        }

        /// <summary>
        /// RFC 2132.
        /// Value of Length-Field.
        /// </summary>
        public override byte Length
        {
            get
            {
                return (byte)(Addresses.Count * IpV4Address.SizeOf);
            }
        }

        /// <summary>
        /// RFC 2132.
        /// collection of all addresses of this option.
        /// </summary>
        public IReadOnlyCollection<IpV4Address> Addresses
        {
            get;
            private set;
        }
    }
}