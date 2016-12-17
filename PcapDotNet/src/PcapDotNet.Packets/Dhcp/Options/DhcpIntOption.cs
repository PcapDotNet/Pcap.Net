using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// Abstract class for all Dhcp-Options with a int value
    /// </summary>
    public class DhcpIntOption : DhcpOption
    {
        internal DhcpIntOption(int value, DhcpOptionCode code) : base(code)
        {
            InternalValue = value;
        }

        internal static T Read<T>(DataSegment data, ref int offset, Func<int, T> ctor) where T : DhcpIntOption
        {
            if (ctor == null)
                throw new ArgumentNullException(nameof(ctor));
            byte len = data[offset++];
            if (len != sizeof(int))
                throw new ArgumentException("Length of a DHCP UIntOption Option has to be 4");
            T option = ctor(data.ReadInt(offset, Endianity.Big));
            offset += sizeof(uint);
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
                return sizeof(int);
            }
        }

        /// <summary>
        /// The real value of the IntOption
        /// </summary>
        protected int InternalValue
        {
            get;
            set;
        }
    }
}