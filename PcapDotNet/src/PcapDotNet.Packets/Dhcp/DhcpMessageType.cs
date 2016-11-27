using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp
{
    /// <summary>
    /// RFCs 2131.
    /// </summary>
    public enum DhcpMessageType : byte

    {
        /// <summary>
        /// RFC 2131.
        /// Indicates a Boot Request
        /// </summary>
        BootRequest = 1,

        /// <summary>
        /// RFC 2131.
        /// Indicates a Boot Reply
        /// </summary>
        BootReply = 2
    }
}