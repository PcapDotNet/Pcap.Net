namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// CC.ECHO Option (RFC 1644).
    /// <pre>
    /// +--------+--------+--------+--------+--------+--------+
    /// |00001101|00000110|    Connection Count:  SEG.CC      |
    /// +--------+--------+--------+--------+--------+--------+
    ///  Kind=13  Length=6
    /// </pre>
    /// 
    /// <para>
    /// This option must be sent (in addition to a CC option) in a segment containing both a SYN and an ACK bit, 
    /// if the initial SYN segment contained a CC or CC.NEW option.  
    /// Its SEG.CC value is the SEG.CC value from the initial SYN.
    /// </para>
    /// 
    /// <para>
    /// A CC.ECHO option should be sent only in a &lt;SYN,ACK&gt; segment and should be ignored if it is received in any other segment.
    /// </para>
    /// </summary>
    [OptionTypeRegistration(typeof(TcpOptionType), TcpOptionType.ConnectionCountEcho)]
    public class TcpOptionConnectionCountEcho : TcpOptionConnectionCountBase, IOptionComplexFactory
    {
        /// <summary>
        /// Creates the option using the given connection count value.
        /// </summary>
        public TcpOptionConnectionCountEcho(uint connectionCount)
            : base(TcpOptionType.ConnectionCountEcho, connectionCount)
        {
        }

        /// <summary>
        /// The default connection count value is 0.
        /// </summary>
        public TcpOptionConnectionCountEcho()
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

            return new TcpOptionConnectionCountEcho(connectionCount);
        }
    }
}