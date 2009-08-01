using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets
{
    public class IpV4OptionTimestampOnly : IpV4OptionTimestamp
    {
        public IpV4OptionTimestampOnly(byte overflow, byte pointedIndex, IList<uint> timestamps)
            : base(IpV4OptionTimestampType.TimestampOnly, overflow, pointedIndex)
        {
            _timestamps = timestamps.AsReadOnly();
        }

        public IpV4OptionTimestampOnly(byte overflow, byte pointedIndex, params uint[] timestamps)
            : this(overflow, pointedIndex, (IList<uint>)timestamps)
        {
        }

        public ReadOnlyCollection<uint> Timestamps
        {
            get { return _timestamps; }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   _timestamps.Aggregate(0, (value, timestamp) => value ^ timestamp.GetHashCode());
        }

        internal static IpV4OptionTimestampOnly Read(byte overflow, byte pointedIndex, byte[] buffer, ref int offset, int numValues)
        {
            uint[] timestamps = new uint[numValues];
            for (int i = 0; i != numValues; ++i)
                timestamps[i] = buffer.ReadUInt(ref offset, Endianity.Big);

            return new IpV4OptionTimestampOnly(overflow, pointedIndex, timestamps);
        }

        protected override int ValuesLength
        {
            get { return _timestamps.Count * 4; }
        }

        protected override bool EqualValues(IpV4OptionTimestamp other)
        {
            return _timestamps.SequenceEqual(((IpV4OptionTimestampOnly)other)._timestamps);
        }

        protected override void WriteValues(byte[] buffer, ref int offset)
        {
            foreach (uint timestamp in _timestamps)
                buffer.Write(ref offset, timestamp, Endianity.Big);
        }

        private readonly ReadOnlyCollection<uint> _timestamps;
    }
}