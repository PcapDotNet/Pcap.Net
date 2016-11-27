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
    /// <pre>
    ///  Code   Len  Value
    /// +-----+-----+-----+
    /// |  52 |  1  |1/2/3|
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpOptionOverloadOption : DhcpOption
    {
        public DhcpOptionOverloadOption(OptionOverloadValue value) : base(DhcpOptionCode.OptionOverload)
        {
            Value = value;
        }

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

        public override byte Length
        {
            get
            {
                return 1;
            }
        }

        public OptionOverloadValue Value
        {
            get { return _value; }
            set
            {
                if (!Enum.IsDefined(typeof(OptionOverloadValue), value))
                    throw new ArgumentOutOfRangeException(nameof(Value), value, "Not a valid OptionOverloadValue");
                _value = value;
            }
        }

        private OptionOverloadValue _value;

        public enum OptionOverloadValue : byte
        {
            File = 1,
            SName = 2,
            Both = 3
        }
    }
}