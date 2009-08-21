using System;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// TCP POC-service-profile Option (RFC 1693).
    /// 
    ///                           1 bit        1 bit    6 bits
    /// +----------+----------+------------+----------+--------+
    /// |  Kind=10 | Length=3 | Start_flag | End_flag | Filler |
    /// +----------+----------+------------+----------+--------+
    /// 
    /// Contains two 1-bit flags necessary to handle the case where the service profile does not fit in a single TCP segment.  
    /// The "Start_flag" indicates that the information in the data section represents the beginning of the service profile 
    /// and the "End_flag" represents the converse.  
    /// For service profiles which fit completely in a single segment, both flags will be set to 1. 
    /// Otherwise, the Start_flag is set in the initial segment and the End_flag in the final segment 
    /// allowing the peer entity to reconstrcut the entire service profile (using the normal sequence numbers in the segment header).  
    /// The "Filler" field serves merely to complete the third byte of the option.
    /// </summary>
    [OptionTypeRegistration(typeof(TcpOptionType), TcpOptionType.PartialOrderServiceProfile)]
    public class TcpOptionPartialOrderServiceProfile : TcpOptionComplex, IOptionComplexFactory, IEquatable<TcpOptionPartialOrderServiceProfile>
    {
        private const byte NoFlags = 0x00;
        private const byte StartFlag = 0x80;
        private const byte EndFlag = 0x40;

        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 3;

        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        public TcpOptionPartialOrderServiceProfile(bool isStart, bool isEnd)
            : base(TcpOptionType.PartialOrderServiceProfile)
        {
            IsStart = isStart;
            IsEnd = isEnd;
        }

        public TcpOptionPartialOrderServiceProfile()
            : this(true, true)
        {
        }

        public bool IsStart { get; private set; }
        public bool IsEnd { get; private set; }

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

        public bool Equals(TcpOptionPartialOrderServiceProfile other)
        {
            if (other == null)
                return false;

            return (IsStart == other.IsStart) &&
                   (IsEnd == other.IsEnd);
        }

        public override bool Equals(TcpOption other)
        {
            return Equals(other as TcpOptionPartialOrderServiceProfile);
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

            byte data = buffer.ReadByte(ref offset);
            return new TcpOptionPartialOrderServiceProfile((data & StartFlag) == StartFlag, (data & EndFlag) == EndFlag);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            byte data = (byte)((IsStart ? StartFlag : NoFlags) | (IsEnd ? EndFlag : NoFlags));
            buffer.Write(ref offset, data);
        }
    }
}