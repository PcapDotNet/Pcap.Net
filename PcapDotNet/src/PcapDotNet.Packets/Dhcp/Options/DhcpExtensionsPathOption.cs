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
    /// |  18 |  n  |  n1 |  n2 |  n3 |  n4 | ...
    /// +-----+-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpExtensionsPathOption : DhcpOption
    {
        public DhcpExtensionsPathOption(string extensionsPath) : base(DhcpOptionCode.ExtensionsPath)
        {
            ExtensionsPath = extensionsPath;
        }

        internal static DhcpExtensionsPathOption Read(DataSegment data, ref int offset)
        {
            byte len = data[offset++];
            string hostName = Encoding.ASCII.GetString(data.ReadBytes(offset, len));
            DhcpExtensionsPathOption option = new DhcpExtensionsPathOption(hostName);
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Encoding.ASCII.GetBytes(ExtensionsPath));
        }

        public override byte Length
        {
            get
            {
                return (byte)Encoding.ASCII.GetByteCount(ExtensionsPath);
            }
        }

        public string ExtensionsPath
        {
            get { return _extensionsPath; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(ExtensionsPath));
                if (value.Length < 1)
                    throw new ArgumentOutOfRangeException(nameof(ExtensionsPath), value.Length, "ExtensionsPath has to be at least 1 characters long");
                if (value.Length > byte.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(ExtensionsPath), value.Length, "ExtensionsPath has to be less than 256 characters long");

                _extensionsPath = value;
            }
        }

        private string _extensionsPath;
    }
}