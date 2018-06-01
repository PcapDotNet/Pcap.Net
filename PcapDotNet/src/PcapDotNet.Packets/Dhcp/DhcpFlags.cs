using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp
{
    /// <summary>
    /// RFC 2131.
    /// <pre>
    ///                     1 1 1 1 1 1
    /// 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |B|             MBZ             |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///
    /// B:  BROADCAST flag
    /// MBZ:  MUST BE ZERO(reserved for future use)
    /// </pre>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
    public enum DhcpFlags : ushort
    {
        /// <summary>
        /// RFC 2131.
        /// Response as Unicast.
        /// </summary>
        Unicast = 0 << 15,

        /// <summary>
        /// RFC 2131.
        /// Response as Broadcast.
        /// </summary>
        Broadcast = 1 << 15
    }
}