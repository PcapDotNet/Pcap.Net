using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// Abstract class for all Dhcp-Options with a ushort value
    /// </summary>
    public class DhcpUShortOption : DhcpOption
    {
        internal DhcpUShortOption(ushort value, DhcpOptionCode code) : base(code)
        {
            InternalValue = value;
        }

        internal static T Read<T>(DataSegment data, ref int offset, Func<ushort, T> ctor) where T : DhcpUShortOption
        {
            if (ctor == null)
                throw new ArgumentNullException(nameof(ctor));
            byte len = data[offset++];
            if (len != sizeof(ushort))
                throw new ArgumentException("Length of a DHCP UShortOption Option has to be 2");
            T option = ctor(data.ReadUShort(offset, Endianity.Big));
            offset += option.Length;
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
            get
            {
                return sizeof(ushort);
            }
        }

        /// <summary>
        /// The real value of the UIntOption
        /// </summary>
        protected ushort InternalValue
        {
            get;
            set;
        }
    }
}