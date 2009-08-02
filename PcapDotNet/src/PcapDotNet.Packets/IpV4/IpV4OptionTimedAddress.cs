using System;

namespace PcapDotNet.Packets
{
    public struct IpV4OptionTimedAddress : IEquatable<IpV4OptionTimedAddress>
    {
        public IpV4OptionTimedAddress(IpV4Address address, uint timestamp)
        {
            _address = address;
            _millisecondsSinceMidnightUt = timestamp;
        }

        public IpV4Address Address
        {
            get { return _address; }
        }

        public uint MillisecondsSinceMidnight
        {
            get { return _millisecondsSinceMidnightUt; }
        }

        public TimeSpan TimeOfDay
        {
            get { return TimeSpan.FromMilliseconds(MillisecondsSinceMidnight); }
        }

        public bool Equals(IpV4OptionTimedAddress other)
        {
            return Address == other.Address &&
                   MillisecondsSinceMidnight == other.MillisecondsSinceMidnight;
        }

        public override bool Equals(object obj)
        {
            return (obj is IpV4OptionTimedAddress) &&
                   Equals((IpV4OptionTimedAddress)obj);
        }

        public static bool operator ==(IpV4OptionTimedAddress value1, IpV4OptionTimedAddress value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(IpV4OptionTimedAddress value1, IpV4OptionTimedAddress value2)
        {
            return !(value1 == value2);
        }

        public override int GetHashCode()
        {
            return _address.GetHashCode() ^
                   (int)_millisecondsSinceMidnightUt;
        }

        private readonly IpV4Address _address;
        private readonly uint _millisecondsSinceMidnightUt;
    }
}