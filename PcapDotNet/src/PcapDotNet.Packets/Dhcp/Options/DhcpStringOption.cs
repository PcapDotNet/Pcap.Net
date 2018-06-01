using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// Abstract class for all Dhcp-Options with a string property
    /// </summary>
    public abstract class DhcpStringOption : DhcpOption
    {
        internal DhcpStringOption(string value, DhcpOptionCode code) : base(code)
        {
            InternalValue = value;
        }

        internal static T Read<T>(DataSegment data, ref int offset, Func<string, T> ctor) where T : DhcpStringOption
        {
            byte len = data[offset++];
            T option = ctor(Encoding.ASCII.GetString(data.ReadBytes(offset, len)));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Encoding.ASCII.GetBytes(InternalValue));
        }

        /// <summary>
        /// RFC 2132.
        /// Value of Length-Field.
        /// </summary>
        public override byte Length
        {
            get
            {
                return (byte)Encoding.ASCII.GetByteCount(InternalValue);
            }
        }

        /// <summary>
        /// The real value of the StringOption.
        /// </summary>
        protected string InternalValue
        {
            get { return _value; }

            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value.Length < 1)
                    throw new ArgumentOutOfRangeException(nameof(value), value.Length, "Value has to be at least 1 characters long");
                if (value.Length >= byte.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value), value.Length, "Value has to be less than 255 characters long");

                _value = value;
            }
        }

        private string _value;
    }
}