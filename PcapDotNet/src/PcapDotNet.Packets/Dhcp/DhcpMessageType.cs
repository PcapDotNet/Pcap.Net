using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp
{
    /// <summary>
    /// RFC 2131.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum DhcpMessageOPCode : byte
    {
        /// <summary>
        /// RFC 2131.
        /// Indicates a Boot Request.
        /// </summary>
        BootRequest = 1,

        /// <summary>
        /// RFC 2131.
        /// Indicates a Boot Reply.
        /// </summary>
        BootReply = 2
    }
}