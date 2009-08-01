using System;

namespace PcapDotNet.Packets
{
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