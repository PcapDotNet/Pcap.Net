namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// <pre>
    /// +-----+------+------+-----------+
    /// | Bit | 0-7  | 8-15 | 16-31     |
    /// +-----+------+------+-----------+
    /// | 0   | Type | Code | Checksum  |
    /// +-----+------+------+-----------+
    /// | 32  | unused                  |
    /// +-----+-------------------------+
    /// | 64  | Internet Header         |
    /// |     | + 64 bits of            |
    /// |     | Original Data Datagram  |
    /// +-----+-------------------------+
    /// </pre>
    /// </summary>
    public abstract class IcmpIpV4HeaderPlus64BitsPayloadDatagram : IcmpIpV4PayloadDatagram
    {
        internal IcmpIpV4HeaderPlus64BitsPayloadDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }
    }
}