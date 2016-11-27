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
    ///  Code   Len                 Host Name
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// |  12 |  n  |  h1 |  h2 |  h3 |  h4 |  h5 |  h6 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpHostNameOption : DhcpOption
    {
        public DhcpHostNameOption(string hostName) : base(DhcpOptionCode.HostName)
        {
            HostName = hostName;
        }

        internal static DhcpHostNameOption Read(DataSegment data, ref int offset)
        {
            byte len = data[offset++];
            string hostName = Encoding.ASCII.GetString(data.ReadBytes(offset, len));
            DhcpHostNameOption option = new DhcpHostNameOption(hostName);
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Encoding.ASCII.GetBytes(HostName));
        }

        public override byte Length
        {
            get
            {
                return (byte)Encoding.ASCII.GetByteCount(HostName);
            }
        }

        public string HostName
        {
            get { return _hostName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(HostName));
                if (value.Length < 1)
                    throw new ArgumentOutOfRangeException(nameof(HostName), value.Length, "HostName has to be at least 1 characters long");
                if (value.Length > byte.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(HostName), value.Length, "HostName has to be less than 256 characters long");

                _hostName = value;
            }
        }

        private string _hostName;
    }
}