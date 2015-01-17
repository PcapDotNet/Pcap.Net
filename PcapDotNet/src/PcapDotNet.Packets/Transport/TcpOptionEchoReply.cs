using System;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// TCP Echo Reply Option:
    /// <pre>
    /// +-----+------+--------+
    /// | Bit | 0-7  | 8-15   |
    /// +-----+------+--------+
    /// | 0   | Kind | Length |
    /// +-----+------+--------+
    /// | 16  | 4 bytes of    |
    /// |     | echoed info   |
    /// +-----+---------------+
    /// </pre>
    /// 
    /// <para>
    /// A TCP that receives a TCP Echo option containing four information bytes will return these same bytes in a TCP Echo Reply option.
    /// </para>
    /// 
    /// <para>
    /// This TCP Echo Reply option must be returned in the next segment (e.g., an ACK segment) that is sent.
    /// If more than one Echo option is received before a reply segment is sent, the TCP must choose only one of the options to echo, 
    /// ignoring the others; specifically, it must choose the newest segment with the oldest sequence number.
    /// </para>
    /// 
    /// <para>
    /// To use the TCP Echo and Echo Reply options, a TCP must send a TCP Echo option in its own SYN segment 
    /// and receive a TCP Echo option in a SYN segment from the other TCP.  
    /// A TCP that does not implement the TCP Echo or Echo Reply options must simply ignore any TCP Echo options it receives.  
    /// However, a TCP should not receive one of these options in a non-SYN segment unless it included a TCP Echo option in its own SYN segment.
    /// </para>
    /// </summary>
    [TcpOptionTypeRegistration(TcpOptionType.EchoReply)]
    public sealed class TcpOptionEchoReply : TcpOptionComplex, IOptionComplexFactory
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 6;

        /// <summary>
        /// The number of bytes this option value take.
        /// </summary>
        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        /// <summary>
        /// Creates the option using the given echo info.
        /// </summary>
        public TcpOptionEchoReply(uint info)
            : base(TcpOptionType.EchoReply)
        {
            Info = info;
        }

        /// <summary>
        /// The default echo info is 0.
        /// </summary>
        public TcpOptionEchoReply()
            : this(0)
        {
        }

        /// <summary>
        /// The echoed info.
        /// </summary>
        public uint Info { get; private set; }

        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public override int Length
        {
            get { return OptionLength; }
        }

        /// <summary>
        /// True iff this option may appear at most once in a datagram.
        /// </summary>
        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
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
            if (valueLength != OptionValueLength)
                return null;

            uint info = buffer.ReadUInt(ref offset, Endianity.Big);
            return new TcpOptionEchoReply(info);
        }

        internal override bool EqualsData(TcpOption other)
        {
            return EqualsData(other as TcpOptionEchoReply);
        }

        internal override int GetDataHashCode()
        {
            return Info.GetHashCode();
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Info, Endianity.Big);
        }

        private bool EqualsData(TcpOptionEchoReply other)
        {
            return other != null &&
                   Info == other.Info;
        }
    }
}