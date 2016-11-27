using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// <pre>
    ///  Code   Len     Text
    /// +-----+-----+-----+-----+---
    /// |  56 |  n  |  c1 |  c2 | ...
    /// +-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpMessageOption : DhcpOption
    {
        public DhcpMessageOption(string text) : base(DhcpOptionCode.Message)
        {
            Text = text;
        }

        internal static DhcpMessageOption Read(DataSegment data, ref int offset)
        {
            byte len = data[offset++];
            string text = Encoding.ASCII.GetString(data.ReadBytes(offset, len));
            DhcpMessageOption option = new DhcpMessageOption(text);
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Encoding.ASCII.GetBytes(Text));
        }

        public override byte Length
        {
            get
            {
                return (byte)Encoding.ASCII.GetByteCount(Text);
            }
        }

        public string Text
        {
            get { return _rootPath; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Text));
                if (value.Length < 1)
                    throw new ArgumentOutOfRangeException(nameof(Text), value.Length, "Text has to be at least 1 characters long");
                if (value.Length > byte.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(Text), value.Length, "Text has to be less than 256 characters long");

                _rootPath = value;
            }
        }

        private string _rootPath;
    }
}