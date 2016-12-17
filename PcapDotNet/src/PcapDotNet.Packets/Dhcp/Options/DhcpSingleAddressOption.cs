using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// Abstract class for all Dhcp-Options with a single Addresses
    /// </summary>
    public abstract class DhcpSingleAddressOption : DhcpOption
    {
        internal DhcpSingleAddressOption(IpV4Address address, DhcpOptionCode code) : base(code)
        {
            InternalValue = address;
        }

        internal static T Read<T>(DataSegment data, ref int offset, Func<IpV4Address, T> ctor) where T : DhcpSingleAddressOption
        {
            if (ctor == null)
                throw new ArgumentNullException(nameof(ctor));
            byte len = data[offset++];
            if (len != IpV4Address.SizeOf)
                throw new ArgumentException("Length of a DHCP SingleAddress Option has to be 4");
            T option = ctor(data.ReadIpV4Address(offset, Endianity.Big));
            offset += IpV4Address.SizeOf;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, InternalValue, Endianity.Big);
        }

        /// <summary>
        /// RFC 2132.
        /// Value of Length-Field
        /// </summary>
        public override byte Length
        {
            get { return IpV4Address.SizeOf; }
        }

        /// <summary>
        /// The real value of the SingleAddressOption
        /// </summary>
        protected IpV4Address InternalValue
        {
            get;
            set;
        }
    }
}