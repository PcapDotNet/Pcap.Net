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
    ///  Code   Len      Dump File Pathname
    /// +-----+-----+-----+-----+-----+-----+---
    /// |  14 |  n  |  n1 |  n2 |  n3 |  n4 | ...
    /// +-----+-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpMeritDumpFileOption : DhcpOption
    {
        public DhcpMeritDumpFileOption(string dumpFilePathname) : base(DhcpOptionCode.MeritDumpFile)
        {
            DumpFilePathname = dumpFilePathname;
        }

        internal static DhcpMeritDumpFileOption Read(DataSegment data, ref int offset)
        {
            byte len = data[offset++];
            string hostName = Encoding.ASCII.GetString(data.ReadBytes(offset, len));
            DhcpMeritDumpFileOption option = new DhcpMeritDumpFileOption(hostName);
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Encoding.ASCII.GetBytes(DumpFilePathname));
        }

        public override byte Length
        {
            get
            {
                return (byte)Encoding.ASCII.GetByteCount(DumpFilePathname);
            }
        }

        public string DumpFilePathname
        {
            get { return _dumpFilePathname; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(DumpFilePathname));
                if (value.Length < 1)
                    throw new ArgumentOutOfRangeException(nameof(DumpFilePathname), value.Length, "DumpFilePathname has to be at least 1 characters long");
                if (value.Length > byte.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(DumpFilePathname), value.Length, "DumpFilePathname has to be less than 256 characters long");

                _dumpFilePathname = value;
            }
        }

        private string _dumpFilePathname;
    }
}