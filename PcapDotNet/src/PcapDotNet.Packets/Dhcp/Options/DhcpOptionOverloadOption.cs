using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option is used to indicate that the DHCP 'sname' or 'file'
    /// fields are being overloaded by using them to carry DHCP options.A
    /// DHCP server inserts this option if the returned parameters will
    /// exceed the usual space allotted for options.
    /// If this option is present, the client interprets the specified
    /// additional fields after it concludes interpretation of the standard
    /// option fields.
    /// <pre>
    ///  Code   Len  Value
    /// +-----+-----+-----+
    /// |  52 |  1  |1/2/3|
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpOptionOverloadOption : DhcpOption
    {
        /// <summary>
        /// create new DhcpOptionOverloadOption.
        /// </summary>
        /// <param name="value">Value</param>
        public DhcpOptionOverloadOption(OptionOverloadValue value) : base(DhcpOptionCode.OptionOverload)
        {
            Value = value;
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.OptionOverload)]
        internal static DhcpOptionOverloadOption Read(DataSegment data, ref int offset)
        {
            byte len = data[offset++];
            if (len != 1)
                throw new ArgumentException("Length of a DHCP OptionOverload Option has to be 1");
            DhcpOptionOverloadOption option = new DhcpOptionOverloadOption((OptionOverloadValue)data[offset]);
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, (byte)Value);
        }

        /// <summary>
        /// Length of the Dhcp-Option.
        /// </summary>
        public override byte Length
        {
            get
            {
                return sizeof(OptionOverloadValue);
            }
        }

        /// <summary>
        /// RFC 2132.
        /// Value.
        /// </summary>
        public OptionOverloadValue Value
        {
            get { return _value; }
            set
            {
                if (!Enum.IsDefined(typeof(OptionOverloadValue), value))
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Not a valid OptionOverloadValue");
                _value = value;
            }
        }

        private OptionOverloadValue _value;

        /// <summary>
        /// RFC 2132.
        /// Option Overload Value
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
        public enum OptionOverloadValue : byte
        {
            /// <summary>
            /// RFC 2132.
            ///  the 'file' field is used to hold options
            /// </summary>
            File = 1,

            /// <summary>
            /// RFC 2132.
            /// the 'sname' field is used to hold options
            /// </summary>
            SName = 2,

            /// <summary>
            /// RFC 2132.
            /// both fields are used to hold options
            /// </summary>
            Both = 3
        }
    }
}