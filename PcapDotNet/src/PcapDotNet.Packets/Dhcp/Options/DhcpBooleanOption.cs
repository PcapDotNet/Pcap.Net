using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// Abstract class for all Dhcp-Options with a boolean value.
    /// </summary>
    public class DhcpBooleanOption : DhcpOption
    {
        internal DhcpBooleanOption(bool value, DhcpOptionCode code) : base(code)
        {
            InternalValue = value;
        }

        internal static T Read<T>(DataSegment data, ref int offset, Func<bool, T> ctor) where T : DhcpBooleanOption
        {
            if (ctor == null)
                throw new ArgumentNullException(nameof(ctor));
            byte len = data[offset++];
            if (len != 1)
                throw new ArgumentException("Length of a DHCP DhcpBooleanOption Option has to be 1");
            if (data[offset] != 0 && data[offset] != 1)
                throw new ArgumentException("Value of a DHCP DhcpBooleanOption Option has to be 0 or 1");
            T option = ctor(data[offset++] == 1 ? true : false);
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, InternalValue ? (byte)1 : (byte)0);
        }

        /// <summary>
        /// RFC 2132.
        /// Value of Length-Field.
        /// </summary>
        public override byte Length
        {
            get
            {
                return sizeof(bool);
            }
        }

        /// <summary>
        /// The real value of the BooleanOption.
        /// </summary>
        protected bool InternalValue
        {
            get;
            set;
        }
    }
}