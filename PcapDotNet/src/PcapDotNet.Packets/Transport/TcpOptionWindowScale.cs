using System;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// Window Scale Option (RFC 1323)
    /// The three-byte Window Scale option may be sent in a SYN segment by a TCP.  
    /// It has two purposes: (1) indicate that the TCP is prepared to do both send and receive window scaling, 
    /// and (2) communicate a scale factor to be applied to its receive window.  
    /// Thus, a TCP that is prepared to scale windows should send the option, even if its own scale factor is 1.  
    /// The scale factor is limited to a power of two and encoded logarithmically, so it may be implemented by binary shift operations.
    /// 
    /// <pre>
    /// +---------+---------+---------+
    /// | Kind=3  |Length=3 |shift.cnt|
    /// +---------+---------+---------+
    /// </pre>
    /// 
    /// <para>
    /// This option is an offer, not a promise; both sides must send Window Scale options in their SYN segments to enable window scaling in either direction.
    /// If window scaling is enabled, then the TCP that sent this option will right-shift its true receive-window values by 'shift.cnt' bits 
    /// for transmission in SEG.WND.  
    /// The value 'shift.cnt' may be zero (offering to scale, while applying a scale factor of 1 to the receive window).
    /// </para>
    /// 
    /// <para>
    /// This option may be sent in an initial SYN segment (i.e., a segment with the SYN bit on and the ACK bit off).  
    /// It may also be sent in a SYN,ACK segment, but only if a Window Scale option was received in the initial SYN segment.  
    /// A Window Scale option in a segment without a SYN bit should be ignored.
    /// </para>
    /// 
    /// <para>
    /// The Window field in a SYN (i.e., a SYN or SYN,ACK) segment itself is never scaled.
    /// </para>
    /// </summary>
    [TcpOptionTypeRegistration(TcpOptionType.WindowScale)]
    public sealed class TcpOptionWindowScale: TcpOptionComplex, IOptionComplexFactory, IEquatable<TcpOptionWindowScale>
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 3;

        /// <summary>
        /// The number of bytes this option value take.
        /// </summary>
        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        /// <summary>
        /// Create a scale factor option using the given scale factor log.
        /// </summary>
        public TcpOptionWindowScale(byte scaleFactorLog)
            : base(TcpOptionType.WindowScale)
        {
            ScaleFactorLog = scaleFactorLog;
        }

        /// <summary>
        /// The default scale factor log is 0 (scale factor is 1).
        /// </summary>
        public TcpOptionWindowScale()
            : this(0)
        {
        }

        /// <summary>
        /// The log of the window scale factor.
        /// </summary>
        public byte ScaleFactorLog { get; private set; }

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
        /// Two window scale options are equal if they have the same scale factor.
        /// </summary>
        public bool Equals(TcpOptionWindowScale other)
        {
            if (other == null)
                return false;
            return ScaleFactorLog == other.ScaleFactorLog;
        }

        /// <summary>
        /// Two window scale options are equal if they have the same scale factor.
        /// </summary>
        public override bool Equals(TcpOption other)
        {
            return Equals(other as TcpOptionWindowScale);
        }

        /// <summary>
        /// The hash code of the window scale option is the hash code of the option type xored with the hash code of the scale factor log.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   ScaleFactorLog.GetHashCode();
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

            byte scaleFactorLog = buffer.ReadByte(ref offset);
            return new TcpOptionWindowScale(scaleFactorLog);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, ScaleFactorLog);
        }
    }
}