using System;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// TCP Timestamps Option (TSopt):
    /// +-------+-------+---------------------+---------------------+
    /// |Kind=8 |  10   |   TS Value (TSval)  |TS Echo Reply (TSecr)|
    /// +-------+-------+---------------------+---------------------+
    ///     1       1              4                     4
    /// 
    /// The Timestamps option carries two four-byte timestamp fields.
    /// The Timestamp Value field (TSval) contains the current value of the timestamp clock of the TCP sending the option.
    /// 
    /// The Timestamp Echo Reply field (TSecr) is only valid if the ACK bit is set in the TCP header; 
    /// if it is valid, it echos a timestamp value that was sent by the remote TCP in the TSval field of a Timestamps option.  
    /// When TSecr is not valid, its value must be zero.  
    /// The TSecr value will generally be from the most recent Timestamp option that was received; however, there are exceptions that are explained below.
    /// 
    /// A TCP may send the Timestamps option (TSopt) in an initial <SYN> segment (i.e., segment containing a SYN bit and no ACK bit), 
    /// and may send a TSopt in other segments only if it received a TSopt in the initial <SYN> segment for the connection.
    /// </summary>
    [OptionTypeRegistration(typeof(TcpOptionType), TcpOptionType.TimeStamp)]
    public class TcpOptionTimeStamp : TcpOptionComplex, IOptionComplexFactory, IEquatable<TcpOptionTimeStamp>
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 10;

        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        public TcpOptionTimeStamp(uint timeStampValue, uint timeStampEchoReply)
            : base(TcpOptionType.TimeStamp)
        {
            TimeStampValue = timeStampValue;
            TimeStampEchoReply = timeStampEchoReply;
        }

        public TcpOptionTimeStamp()
            : this(0, 0)
        {
        }

        public uint TimeStampValue { get; private set; }
        public uint TimeStampEchoReply { get; private set; }

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

        public bool Equals(TcpOptionTimeStamp other)
        {
            if (other == null)
                return false;

            return TimeStampValue == other.TimeStampValue &&
                   TimeStampEchoReply == other.TimeStampEchoReply;
        }

        public override bool Equals(TcpOption other)
        {
            return Equals(other as TcpOptionTimeStamp);
        }

        /// <summary>
        /// Tries to read the option from a buffer starting from the option value (after the type and length).
        /// </summary>
        /// <param name="buffer">The buffer to read the option from.</param>
        /// <param name="offset">The offset to the first byte to read the buffer. Will be incremented by the number of bytes read.</param>
        /// <param name="valueLength">The number of bytes the option value should take according to the length field that was already read.</param>
        /// <returns>On success - the complex option read. On failure - null.</returns>
        public Option CreateInstance(byte[] buffer, ref int offset, byte valueLength)
        {
            if (valueLength != OptionValueLength)
                return null;

            uint timeStampValue = buffer.ReadUInt(ref offset, Endianity.Big);
            uint timeStampEchoReply = buffer.ReadUInt(ref offset, Endianity.Big);
            return new TcpOptionTimeStamp(timeStampValue, timeStampEchoReply);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, TimeStampValue, Endianity.Big);
            buffer.Write(ref offset, TimeStampEchoReply, Endianity.Big);
        }
    }
}