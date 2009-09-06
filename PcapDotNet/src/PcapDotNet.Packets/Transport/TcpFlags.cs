using System;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// TCP Header Format
    /// +-----+-----+----+-----+-----+-----+-----+-----+-----+-----+-----+
    /// | Bit | 0-6 | 7  | 8   | 9   | 10  | 11  | 12  | 13  | 14  | 15  |
    /// +-----+-----+----+-----+-----+-----+-----+-----+-----+-----+-----+
    /// | 0   |     | NS | CWR | ECE | URG | ACK | PSH | RST | SYN | FIN |
    /// +-----+-----+----+-----+-----+-----+-----+-----+-----+-----+-----+
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags"), Flags]
    public enum TcpFlags : ushort
    {
        None = 0x0000,
        Fin = 0x0001,
        Synchronize = 0x0002,
        Reset = 0x0004,
        Push = 0x0008,
        Acknowledgment = 0x0010,
        Urgent = 0x0020,
        ExplicitCongestionNotificationEcho = 0x0040,
        CongestionWindowReduced = 0x0080,
        Ns = 0x0100,
    }
}