using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV4
{
    public class IpV4OptionTimestampAndAddress : IpV4OptionTimestamp
    {
        public IpV4OptionTimestampAndAddress(IpV4OptionTimestampType timestampType, byte overflow, byte pointedIndex, IList<IpV4OptionTimedAddress> addressesAndTimestamps)
            : base(timestampType, overflow, pointedIndex)
        {
            if (timestampType != IpV4OptionTimestampType.AddressAndTimestamp &&
                timestampType != IpV4OptionTimestampType.AddressPrespecified)
            {
                throw new ArgumentException("Illegal timestamp type " + timestampType, "timestampType");
            }

            _addressesAndTimestamps = addressesAndTimestamps.AsReadOnly();
        }

        public ReadOnlyCollection<IpV4OptionTimedAddress> TimedRoute
        {
            get { return _addressesAndTimestamps; }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ TimedRoute.SequenceGetHashCode();
        }

        internal static IpV4OptionTimestampAndAddress Read(IpV4OptionTimestampType timestampType, byte overflow, byte pointedIndex, byte[] buffer, ref int offset, int numValues)
        {
            if (numValues % 2 != 0)
                return null;

            IpV4OptionTimedAddress[] addressesAndTimestamps = new IpV4OptionTimedAddress[numValues / 2];
            for (int i = 0; i != numValues / 2; ++i)
            {
                addressesAndTimestamps[i] = new IpV4OptionTimedAddress(buffer.ReadIpV4Address(ref offset, Endianity.Big),
                                                                       buffer.ReadUInt(ref offset, Endianity.Big));
            }

            return new IpV4OptionTimestampAndAddress(timestampType, overflow, pointedIndex, addressesAndTimestamps);
        }

        protected override int ValuesLength
        {
            get { return TimedRoute.Count * (IpV4Address.SizeOf + sizeof(uint)); }
        }

        protected override bool EqualValues(IpV4OptionTimestamp other)
        {
            return TimedRoute.SequenceEqual(((IpV4OptionTimestampAndAddress)other).TimedRoute);
        }

        protected override void WriteValues(byte[] buffer, ref int offset)
        {
            foreach (IpV4OptionTimedAddress addressAndTimestamp in TimedRoute)
            {
                buffer.Write(ref offset, addressAndTimestamp.Address, Endianity.Big);
                buffer.Write(ref offset, addressAndTimestamp.MillisecondsSinceMidnight, Endianity.Big);
            }
        }

        private readonly ReadOnlyCollection<IpV4OptionTimedAddress> _addressesAndTimestamps;
    }
}