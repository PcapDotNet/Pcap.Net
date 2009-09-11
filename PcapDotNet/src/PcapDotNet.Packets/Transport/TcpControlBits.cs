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
    [Flags]
    public enum TcpControlBits : ushort
    {
        /// <summary>
        /// No control bits are turned on.
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// No more data from sender.
        /// </summary>
        Fin = 0x0001,

        /// <summary>
        /// Synchronize sequence numbers.
        /// </summary>
        Synchronize = 0x0002,

        /// <summary>
        /// Reset the connection.
        /// </summary>
        Reset = 0x0004,

        /// <summary>
        /// Push Function.
        /// </summary>
        Push = 0x0008,
        /// <summary>
        /// Acknowledgment field significant.
        /// </summary>
        Acknowledgment = 0x0010,

        /// <summary>
        /// Urgent Pointer field significant.
        /// </summary>
        Urgent = 0x0020,

        /// <summary>
        /// RFC 3168.
        /// </summary>
        ExplicitCongestionNotificationEcho = 0x0040,

        /// <summary>
        /// RFC 3168.
        /// </summary>
        CongestionWindowReduced = 0x0080,

        /// <summary>
        /// RFC 3540.
        /// </summary>
        NonceSum = 0x0100,
    }
}