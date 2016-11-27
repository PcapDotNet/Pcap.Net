using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// DHCP option for undefined options
    /// </summary>
    public class DhcpAnyOption : DhcpOption
    {
        public DhcpAnyOption(DataSegment data, DhcpOptionCode code) : base(code)
        {
            Data = data;
        }

        public override byte Length
        {
            get
            {
                if (Data == null)
                {
                    return 0;
                }
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
            if (Data == null)
                throw new ArgumentNullException(nameof(Data));
            if (Data.Length > byte.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(Data), Data.Length, "Data.Length has to be less than 256");
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Data);
        }

        public DataSegment Data
        {
            get { return _data; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Data));
                if (value.Length > byte.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(Data), value.Length, "Data.Length has to be less than 256");
                _data = value;
            }
        }

        private DataSegment _data;
    }
}