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
    public enum TcpFlags : ushort
    {
        None = 0x0000,
        Fin = 0x0001,
        Syn = 0x0002,
        Rst = 0x0004,
        Psh = 0x0008,
        Ack = 0x0010,
        Urg = 0x0020,
        Ece = 0x0040,
        Cwr = 0x0080,
        Ns = 0x0100,
    }
}