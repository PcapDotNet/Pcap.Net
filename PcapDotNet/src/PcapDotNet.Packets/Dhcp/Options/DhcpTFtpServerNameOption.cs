using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option is used to identify a TFTP server when the 'sname' field
    /// in the DHCP header has been used for DHCP options.
    /// <pre>
    ///  Code Len    TFTP server
    /// +-----+-----+-----+-----+-----+---
    /// | 66  |  n  |  c1 |  c2 |  c3 | ...
    /// +-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpTFtpServerNameOption : DhcpStringOption
    {
        /// <summary>
        /// create new DhcpTFtpServerNameOption.
        /// </summary>
        /// <param name="tfptServer">TFTP server</param>
        public DhcpTFtpServerNameOption(string tfptServer) : base(tfptServer, DhcpOptionCode.TfptServerName)
        {
        }

        internal static DhcpTFtpServerNameOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpTFtpServerNameOption>(data, ref offset, p => new DhcpTFtpServerNameOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// TFTP server.
        /// </summary>
        public string TFtpServer
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}