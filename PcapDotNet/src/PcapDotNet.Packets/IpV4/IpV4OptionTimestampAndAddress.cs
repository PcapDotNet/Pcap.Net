using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets
{
    public class IpV4OptionTimestampAndAddress : IpV4OptionTimestamp
    {
        public IpV4OptionTimestampAndAddress(IpV4OptionTimestampType timestampType, byte overflow, byte pointedIndex, IList<KeyValuePair<IpV4Address, uint>> addressesAndTimestamps)
            : base(timestampType, overflow, pointedIndex)
        {
            if (timestampType != IpV4OptionTimestampType.AddressAndTimestamp &&
                timestampType != IpV4OptionTimestampType.AddressPrespecified)
            {
                throw new ArgumentException("Illegal timestamp type " + timestampType, "timestampType");
            }

            _addressesAndTimestamps = addressesAndTimestamps.AsReadOnly();
        }

        public ReadOnlyCollection<KeyValuePair<IpV4Address, uint>> TimedRoute
        {
            get { return _addressesAndTimestamps; }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   _addressesAndTimestamps.Aggregate(0, (value, pair) => value ^
                                                                         pair.Key.GetHashCode() ^
                                                                         (int)pair.Value);
        }

        internal static IpV4OptionTimestampAndAddress Read(IpV4OptionTimestampType timestampType, byte overflow, byte pointedIndex, byte[] buffer, ref int offset, int numValues)
        {
            if (numValues % 2 != 0)
                return null;

            KeyValuePair<IpV4Address, uint>[] addressesAndTimestamps = new KeyValuePair<IpV4Address, uint>[numValues / 2];
            for (int i = 0; i != numValues / 2; ++i)
            {
                addressesAndTimestamps[i] = new KeyValuePair<IpV4Address, uint>(buffer.ReadIpV4Address(ref offset, Endianity.Big),
                                                                                buffer.ReadUInt(ref offset, Endianity.Big));
            }

            return new IpV4OptionTimestampAndAddress(timestampType, overflow, pointedIndex, addressesAndTimestamps);
        }

        protected override int ValuesLength
        {
            get { return _addressesAndTimestamps.Count * 2 * 4; }
        }

        protected override bool EqualValues(IpV4OptionTimestamp other)
        {
            return _addressesAndTimestamps.SequenceEqual(((IpV4OptionTimestampAndAddress)other)._addressesAndTimestamps);
        }

        protected override void WriteValues(byte[] buffer, ref int offset)
        {
            foreach (KeyValuePair<IpV4Address, uint> addressAndTimestamp in _addressesAndTimestamps)
            {
                buffer.Write(ref offset, addressAndTimestamp.Key, Endianity.Big);
                buffer.Write(ref offset, addressAndTimestamp.Value, Endianity.Big);
            }
        }

        private readonly ReadOnlyCollection<KeyValuePair<IpV4Address, uint>> _addressesAndTimestamps;
    }
}