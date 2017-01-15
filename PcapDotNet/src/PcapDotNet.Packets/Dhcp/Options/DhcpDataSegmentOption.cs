using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// Abstract class for all Dhcp-Options with a DataSegment value.
    /// </summary>
    public class DhcpDataSegmentOption : DhcpOption
    {
        internal DhcpDataSegmentOption(DataSegment value, DhcpOptionCode code) : base(code)
        {
            InternalValue = value;
        }

        internal static T Read<T>(DataSegment data, ref int offset, Func<DataSegment, T> ctor) where T : DhcpDataSegmentOption
        {
            if (ctor == null)
                throw new ArgumentNullException(nameof(ctor));
            byte len = data[offset++];
            T option = ctor(data.Subsegment(offset, len));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, InternalValue);
        }

        /// <summary>
        /// RFC 2132.
        /// Value of Length-Field.
        /// </summary>
        public override byte Length
        {
            get
            {
                return (byte)InternalValue.Length;
            }
        }

        /// <summary>
        /// The real value of the UIntOption.
        /// </summary>
        protected DataSegment InternalValue
        {
            get { return _internalValue; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (value.Length < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value.Length, "DataSegment.Length has to be greater than 0");
                }
                if (value.Length > byte.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value.Length, "DataSegment.Length has to be less than 256");
                }
                _internalValue = value;
            }
        }

        private DataSegment _internalValue;
    }
}