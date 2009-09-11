using System;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// TCP Echo Option:
    /// <pre>
    /// +--------+--------+--------+--------+--------+--------+
    /// | Kind=6 | Length |   4 bytes of info to be echoed    |
    /// +--------+--------+--------+--------+--------+--------+
    /// </pre>
    /// 
    /// <para>
    /// This option carries four bytes of information that the receiving TCP may send back in a subsequent TCP Echo Reply option.  
    /// A TCP may send the TCP Echo option in any segment, but only if a TCP Echo option was received in a SYN segment for the connection.
    /// </para>
    /// 
    /// <para>
    /// When the TCP echo option is used for RTT measurement, it will be included in data segments, 
    /// and the four information bytes will define the time at which the data segment was transmitted in any format convenient to the sender.
    /// </para>
    /// </summary>
    [OptionTypeRegistration(typeof(TcpOptionType), TcpOptionType.Echo)]
    public class TcpOptionEcho : TcpOptionComplex, IOptionComplexFactory, IEquatable<TcpOptionEcho>
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
        public TcpOptionEcho(uint info)
            : base(TcpOptionType.Echo)
        {
            Info = info;
        }

        /// <summary>
        /// The default info is 0.
        /// </summary>
        public TcpOptionEcho()
            : this(0)
        {
        }

        /// <summary>
        /// The info value of the option to be echoed.
        /// </summary>
        public uint Info { get; private set;}

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
        /// Two echo options are equal if they have the same info.
        /// </summary>
        public bool Equals(TcpOptionEcho other)
        {
            if (other == null)
                return false;

            return Info == other.Info;
        }

        /// <summary>
        /// Two echo options are equal if they have the same info.
        /// </summary>
        public override bool Equals(TcpOption other)
        {
            return Equals(other as TcpOptionEcho);
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

            uint info = buffer.ReadUInt(ref offset, Endianity.Big);
            return new TcpOptionEcho(info);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Info, Endianity.Big);
        }
    }
}