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
    ///  Code Len   Bootfile name
    /// +-----+-----+-----+-----+-----+---
    /// | 67  |  n  |  c1 |  c2 |  c3 | ...
    /// +-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpBootfileNameOption : DhcpOption
    {
        public DhcpBootfileNameOption(string domainName) : base(DhcpOptionCode.BootfileName)
        {
            BootfileName = domainName;
        }

        internal static DhcpBootfileNameOption Read(DataSegment data, ref int offset)
        {
            byte len = data[offset++];
            DhcpBootfileNameOption option = new DhcpBootfileNameOption(Encoding.ASCII.GetString(data.ReadBytes(offset, len)));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Encoding.ASCII.GetBytes(BootfileName));
        }

        public override byte Length
        {
            get
            {
                return (byte)Encoding.ASCII.GetByteCount(BootfileName);
            }
        }

        public string BootfileName
        {
            get { return bootfileName; }

            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(BootfileName));
                if (value.Length < 1)
                    throw new ArgumentOutOfRangeException(nameof(BootfileName), value.Length, "BootfileName has to be at least 1 characters long");
                if (value.Length > byte.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(BootfileName), value.Length, "BootfileName has to be less than 256 characters long");

                bootfileName = value;
            }
        }

        private string bootfileName;
    }
}