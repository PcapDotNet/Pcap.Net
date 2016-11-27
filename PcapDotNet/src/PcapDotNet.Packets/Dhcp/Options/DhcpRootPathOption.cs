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
    ///  Code   Len      Root Disk Pathname
    /// +-----+-----+-----+-----+-----+-----+---
    /// |  17 |  n  |  n1 |  n2 |  n3 |  n4 | ...
    /// +-----+-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpRootPathOption : DhcpOption
    {
        public DhcpRootPathOption(string rootPath) : base(DhcpOptionCode.RootPath)
        {
            RootPath = rootPath;
        }

        internal static DhcpRootPathOption Read(DataSegment data, ref int offset)
        {
            byte len = data[offset++];
            string hostName = Encoding.ASCII.GetString(data.ReadBytes(offset, len));
            DhcpRootPathOption option = new DhcpRootPathOption(hostName);
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Encoding.ASCII.GetBytes(RootPath));
        }

        public override byte Length
        {
            get
            {
                return (byte)Encoding.ASCII.GetByteCount(RootPath);
            }
        }

        public string RootPath
        {
            get { return _rootPath; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(RootPath));
                if (value.Length < 1)
                    throw new ArgumentOutOfRangeException(nameof(RootPath), value.Length, "RootPath has to be at least 1 characters long");
                if (value.Length > byte.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(RootPath), value.Length, "RootPath has to be less than 256 characters long");

                _rootPath = value;
            }
        }

        private string _rootPath;
    }
}