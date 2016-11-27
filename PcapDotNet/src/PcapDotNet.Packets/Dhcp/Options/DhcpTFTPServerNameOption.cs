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
    ///  Code Len    TFTP server
    /// +-----+-----+-----+-----+-----+---
    /// | 66  |  n  |  c1 |  c2 |  c3 | ...
    /// +-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpTFTPServerNameOption : DhcpOption
    {
        public DhcpTFTPServerNameOption(string tfptServer) : base(DhcpOptionCode.TFTPServerName)
        {
            TFPTServer = tfptServer;
        }

        internal static DhcpTFTPServerNameOption Read(DataSegment data, ref int offset)
        {
            byte len = data[offset++];
            DhcpTFTPServerNameOption option = new DhcpTFTPServerNameOption(Encoding.ASCII.GetString(data.ReadBytes(offset, len)));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Encoding.ASCII.GetBytes(TFPTServer));
        }

        public override byte Length
        {
            get
            {
                return (byte)Encoding.ASCII.GetByteCount(TFPTServer);
            }
        }

        public string TFPTServer
        {
            get { return bootfileName; }

            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(TFPTServer));
                if (value.Length < 1)
                    throw new ArgumentOutOfRangeException(nameof(TFPTServer), value.Length, "BootfileName has to be at least 1 characters long");
                if (value.Length > byte.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(TFPTServer), value.Length, "BootfileName has to be less than 256 characters long");

                bootfileName = value;
            }
        }

        private string bootfileName;
    }
}