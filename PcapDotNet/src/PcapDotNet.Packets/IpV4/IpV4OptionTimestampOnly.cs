using System;
using System.Linq;

namespace PcapDotNet.Packets
{
    public class IpV4OptionTimestampOnly : IpV4OptionTimestamp
    {
        public IpV4OptionTimestampOnly(byte overflow, byte pointedIndex, params TimeSpan[] timestamps)
            : base(IpV4OptionTimestampType.TimestampOnly, overflow, pointedIndex)
        {
            _timestamps = timestamps;
        }

        internal static IpV4OptionTimestampOnly Read(byte overflow, byte pointedIndex, byte[] buffer, ref int offset, int numValues)
        {
            TimeSpan[] timestamps = new TimeSpan[numValues];
            for (int i = 0; i != numValues; ++i)
                timestamps[i] = ReadTimeOfDay(buffer, ref offset);

            return new IpV4OptionTimestampOnly(overflow, pointedIndex, timestamps);
        }

        protected override int ValuesLength
        {
            get { return _timestamps.Length * 4; }
        }

        protected override bool EqualValues(IpV4OptionTimestamp other)
        {
            return _timestamps.SequenceEqual(((IpV4OptionTimestampOnly)other)._timestamps);
        }

        protected override void WriteValues(byte[] buffer, ref int offset)
        {
            foreach (TimeSpan timestamp in _timestamps)
                buffer.Write(ref offset, (uint)timestamp.TotalMilliseconds, Endianity.Big);
        }

        private readonly TimeSpan[] _timestamps;
    }
}