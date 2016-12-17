using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// Abstract class for all Dhcp-Options with a byte value
    /// </summary>
    public class DhcpByteOption : DhcpOption
    {
        internal DhcpByteOption(byte value, DhcpOptionCode code) : base(code)
        {
            InternalValue = value;
        }

        internal static T Read<T>(DataSegment data, ref int offset, Func<byte, T> ctor) where T : DhcpByteOption
        {
            if (ctor == null)
                throw new ArgumentNullException(nameof(ctor));
            byte len = data[offset++];
            if (len != 1)
                throw new ArgumentException("Length of a DHCP DhcpByteOption Option has to be 1");
            T option = ctor(data[offset++]);
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, InternalValue);
        }

        /// <summary>
        /// RFC 2132.
        /// Value of Length-Field
        /// </summary>
        public override byte Length
        {
            get
            {
                return sizeof(byte);
            }
        }

        /// <summary>
        /// The real value of the ByteOption
        /// </summary>
        protected byte InternalValue
        {
            get;
            set;
        }
    }
}