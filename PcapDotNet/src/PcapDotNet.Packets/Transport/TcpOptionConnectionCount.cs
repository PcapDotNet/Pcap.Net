using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// CC Option (RFC 1644).
    /// <pre>
    /// +--------+--------+--------+--------+--------+--------+
    /// |00001011|00000110|    Connection Count:  SEG.CC      |
    /// +--------+--------+--------+--------+--------+--------+
    ///  Kind=11  Length=6
    /// </pre>
    /// 
    /// <para>
    /// This option may be sent in an initial SYN segment, and it may be sent in other segments if a CC or CC.NEW option 
    /// has been received for this incarnation of the connection.  
    /// Its SEG.CC value is the TCB.CCsend value from the sender's TCB.
    /// </para>
    /// </summary>
    [TcpOptionTypeRegistration(TcpOptionType.ConnectionCount)]
    public class TcpOptionConnectionCount : TcpOptionConnectionCountBase, IOptionComplexFactory
    {
        /// <summary>
        /// Create a connection count tcp option by a given connection count.
        /// </summary>
        public TcpOptionConnectionCount(uint connectionCount)
            : base(TcpOptionType.ConnectionCount, connectionCount)
        {
        }

        /// <summary>
        /// The default connection count is 0.
        /// </summary>
        public TcpOptionConnectionCount()
            : this(0)
        {
        }

        /// <summary>
        /// Tries to read the option from a buffer starting from the option value (after the type and length).
        /// </summary>
        /// <param name="buffer">The buffer to read the option from.</param>
        /// <param name="offset">The offset to the first byte to read the buffer. Will be incremented by the number of bytes read.</param>
        /// <param name="valueLength">The number of bytes the option value should take according to the length field that was already read.</param>
        /// <returns>On success - the complex option read. On failure - null.</returns>
        Option IOptionComplexFactory.CreateInstance(byte[] buffer, ref int offset, byte valueLength)
        {
            uint connectionCount;
            if (!TryRead(out connectionCount, buffer, ref offset, valueLength))
                return null;

            return new TcpOptionConnectionCount(connectionCount);
        }
    }
}