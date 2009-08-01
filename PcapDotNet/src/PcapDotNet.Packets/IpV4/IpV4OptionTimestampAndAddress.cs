using System;
using System.Collections.Generic;
using System.Linq;

namespace PcapDotNet.Packets
{
    public class IpV4OptionTimestampAndAddress : IpV4OptionTimestamp
    {
        public IpV4OptionTimestampAndAddress(IpV4OptionTimestampType timestampType, byte overflow, byte pointedIndex, KeyValuePair<IpV4Address, TimeSpan>[] addressesAndTimestamps)
            : base(timestampType, overflow, pointedIndex)
        {
            _addressesAndTimestamps = addressesAndTimestamps;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   _addressesAndTimestamps.Aggregate(0, (value, pair) => value ^
                                                                         pair.Key.GetHashCode() ^
                                                                         pair.Value.GetHashCode());
        }

        internal static IpV4OptionTimestampAndAddress Read(IpV4OptionTimestampType timestampType, byte overflow, byte pointedIndex, byte[] buffer, ref int offset, int numValues)
        {
            if (numValues % 2 != 0)
                return null;

            KeyValuePair<IpV4Address, TimeSpan>[] addressesAndTimestamps = new KeyValuePair<IpV4Address, TimeSpan>[numValues / 2];
            for (int i = 0; i != numValues / 2; ++i)
            {
                addressesAndTimestamps[i] = new KeyValuePair<IpV4Address, TimeSpan>(new IpV4Address(buffer.ReadUInt(ref offset, Endianity.Big)),
                                                                                    ReadTimeOfDay(buffer, ref offset));
            }

            return new IpV4OptionTimestampAndAddress(timestampType, overflow, pointedIndex, addressesAndTimestamps);
        }

        protected override int ValuesLength
        {
            get { return _addressesAndTimestamps.Length * 2 * 4; }
        }

        protected override bool EqualValues(IpV4OptionTimestamp other)
        {
            return _addressesAndTimestamps.SequenceEqual(((IpV4OptionTimestampAndAddress)other)._addressesAndTimestamps);
        }

        protected override void WriteValues(byte[] buffer, ref int offset)
        {
            foreach (KeyValuePair<IpV4Address, TimeSpan> addressAndTimestamp in _addressesAndTimestamps)
            {
                buffer.Write(ref offset, addressAndTimestamp.Key.ToValue(), Endianity.Big);
                buffer.Write(ref offset, (uint)addressAndTimestamp.Value.TotalMilliseconds, Endianity.Big);
            }
        }

        private readonly KeyValuePair<IpV4Address, TimeSpan>[] _addressesAndTimestamps;
    }
}