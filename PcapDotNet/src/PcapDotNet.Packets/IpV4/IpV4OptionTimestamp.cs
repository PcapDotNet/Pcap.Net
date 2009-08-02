using System;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Internet Timestamp
    /// +--------+--------+--------+--------+
    /// |01000100| length | pointer|oflw|flg|
    /// +--------+--------+--------+--------+
    /// |         internet address          |
    /// +--------+--------+--------+--------+
    /// |             timestamp             |
    /// +--------+--------+--------+--------+
    /// |                 .                 |
    ///                   .
    ///                   .
    ///  Type = 68
    /// 
    /// The Option Length is the number of octets in the option counting the type, length, pointer, and overflow/flag octets (maximum length 40).
    /// 
    /// The Pointer is the number of octets from the beginning of this option to the end of timestamps plus one 
    /// (i.e., it points to the octet beginning the space for next timestamp).  
    /// The smallest legal value is 5.  
    /// The timestamp area is full when the pointer is greater than the length.
    /// 
    /// The Overflow (oflw) [4 bits] is the number of IP modules that cannot register timestamps due to lack of space.
    /// 
    /// The Flag (flg) [4 bits] values are 
    /// 0 -- time stamps only, stored in consecutive 32-bit words,
    /// 1 -- each timestamp is preceded with internet address of the registering entity,
    /// 3 -- the internet address fields are prespecified.  
    ///      An IP module only registers its timestamp if it matches its own address with the next specified internet address.
    /// 
    /// The Timestamp is a right-justified, 32-bit timestamp in milliseconds since midnight UT.  
    /// If the time is not available in milliseconds or cannot be provided with respect to midnight UT 
    /// then any time may be inserted as a timestamp provided the high order bit of the timestamp field is set to one 
    /// to indicate the use of a non-standard value.
    /// 
    /// The originating host must compose this option with a large enough timestamp data area to hold all the timestamp information expected.  
    /// The size of the option does not change due to adding timestamps.  
    /// The intitial contents of the timestamp data area must be zero or internet address/zero pairs.
    /// 
    /// If the timestamp data area is already full (the pointer exceeds the length) the datagram is forwarded without inserting the timestamp, 
    /// but the overflow count is incremented by one.
    /// If there is some room but not enough room for a full timestamp to be inserted, or the overflow count itself overflows, 
    /// the original datagram is considered to be in error and is discarded.
    /// In either case an ICMP parameter problem message may be sent to the source host.
    /// 
    /// The timestamp option is not copied upon fragmentation.  
    /// It is carried in the first fragment.  
    /// Appears at most once in a datagram.
    /// </summary>
    public abstract class IpV4OptionTimestamp : IpV4OptionComplex, IEquatable<IpV4OptionTimestamp>
    {
        public const int OptionMinimumLength = 4;
        public const int OptionValueMinimumLength = OptionMinimumLength - OptionHeaderLength;
        public const int OverflowMaxValue = 15;
        public const int PointedIndexMaxValue = byte.MaxValue / 4 - 1;

        public IpV4OptionTimestampType TimestampType
        {
            get { return _timestampType; }
        }

        public byte Overflow
        {
            get { return _overflow; }
        }

        public byte PointedIndex
        {
            get { return _pointedIndex; }
        }

        public override int Length
        {
            get
            {
                return OptionMinimumLength +
                       ValuesLength;
            }
        }

        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }


        public bool Equals(IpV4OptionTimestamp other)
        {
            if (other == null)
                return false;

            return TimestampType == other.TimestampType &&
                   Overflow == other.Overflow &&
                   PointedIndex == other.PointedIndex &&
                   EqualValues(other);
        }

        public override bool Equals(IpV4Option other)
        {
            return Equals(other as IpV4OptionTimestamp);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   ((byte)TimestampType << 16) ^
                   (Overflow << 8) ^
                   PointedIndex;
        }

        internal static IpV4OptionTimestamp ReadOptionTimestamp(byte[] buffer, ref int offset, int valueLength)
        {
            if (valueLength < OptionValueMinimumLength || valueLength % 4 != 2)
                return null;

            byte pointer = buffer[offset++];
            if (pointer % 4 != 1 || pointer < 5)
                return null;

            byte pointedIndex = (byte)(pointer / 4 - 1);

            byte overflow = buffer[offset++];
            IpV4OptionTimestampType timestampType = (IpV4OptionTimestampType)(overflow & 0x0F);
            overflow >>= 4;

            int numValues = valueLength / 4;

            switch (timestampType)
            {
                case IpV4OptionTimestampType.TimestampOnly:
                    return IpV4OptionTimestampOnly.Read(overflow, pointedIndex, buffer, ref offset, numValues);

                case IpV4OptionTimestampType.AddressAndTimestamp:
                case IpV4OptionTimestampType.AddressPrespecified:
                    return IpV4OptionTimestampAndAddress.Read(timestampType, overflow, pointedIndex, buffer, ref offset, numValues);

                default:
                    return null;
            }
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer[offset++] = (byte)Length;
            buffer[offset++] = (byte)(OptionMinimumLength + 1 + PointedIndex * 4);
            buffer[offset++] = (byte)(((byte)(Overflow << 4)) | (byte)TimestampType);
            WriteValues(buffer, ref offset);
        }

        protected IpV4OptionTimestamp(IpV4OptionTimestampType timestampType, byte overflow, byte pointedIndex)
            : base(IpV4OptionType.InternetTimestamp)
        {
            if (overflow > OverflowMaxValue)
                throw new ArgumentOutOfRangeException("overflow", overflow, "Maximum value is " + OverflowMaxValue);

            if (pointedIndex > PointedIndexMaxValue)
                throw new ArgumentOutOfRangeException("pointedIndex", pointedIndex, "Maximum value is " + PointedIndexMaxValue);

            _timestampType = timestampType;
            _overflow = overflow;
            _pointedIndex = pointedIndex;
        }

        protected abstract int ValuesLength { get; }

        protected abstract bool EqualValues(IpV4OptionTimestamp other);

        protected abstract void WriteValues(byte[] buffer, ref int offset);

        private readonly IpV4OptionTimestampType _timestampType;
        private readonly byte _overflow;
        private readonly byte _pointedIndex;
    }
}