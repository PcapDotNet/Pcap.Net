using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// DHCP option for any options.
    /// </summary>
    public class DhcpAnyOption : DhcpOption
    {
        /// <summary>
        /// create new Any-Option.
        /// </summary>
        /// <param name="data">data represented by the option</param>
        /// <param name="code">the OptionCode</param>
        public DhcpAnyOption(DataSegment data, DhcpOptionCode code) : base(code)
        {
            Data = data;
        }

        /// <summary>
        /// Length of the Dhcp-Option.
        /// </summary>
        public override byte Length
        {
            get
            {
                return (byte)Data.Length;
            }
        }

        internal static DhcpAnyOption Read(DataSegment data, ref int offset)
        {
            DhcpOptionCode code = (DhcpOptionCode)data[offset - 1];
            byte length = data[offset++];
            DhcpAnyOption option = new DhcpAnyOption(data.Subsegment(offset, length), code);
            offset += length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            if (Data.Length > byte.MaxValue)
                throw new InvalidOperationException("Data.Length has to be less than 256 but is " + Data.Length);
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Data);
        }

        /// <summary>
        /// Data of the Option.
        /// </summary>
        public DataSegment Data
        {
            get { return _data; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value.Length > byte.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value), value.Length, "Data.Length has to be less than 256");
                _data = value;
            }
        }

        private DataSegment _data;
    }
}