using System;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Represents the time passed since midnight UT.
    /// </summary>
    public struct IpV4TimeOfDay : IEquatable<IpV4TimeOfDay>
    {
        /// <summary>
        /// Create the time from milliseconds since midnight UT.
        /// </summary>
        public IpV4TimeOfDay(uint millisecondsSinceMidnightUniversalTime)
        {
            _millisecondsSinceMidnightUniversalTime = millisecondsSinceMidnightUniversalTime;
        }

        /// <summary>
        /// Create the time from TimeSpan since midnight UT.
        /// </summary>
        public IpV4TimeOfDay(TimeSpan timeOfDaySinceMidnightUniversalTime)
            : this((uint)timeOfDaySinceMidnightUniversalTime.TotalMilliseconds)
        {
        }

        /// <summary>
        /// Number of milliseconds passed since midnight UT.
        /// </summary>
        public uint MillisecondsSinceMidnightUniversalTime
        {
            get { return _millisecondsSinceMidnightUniversalTime; }
        }

        /// <summary>
        /// Time passed since midnight UT.
        /// </summary>
        public TimeSpan TimeSinceMidnightUniversalTime
        {
            get { return TimeSpan.FromMilliseconds(MillisecondsSinceMidnightUniversalTime); }
        }

        /// <summary>
        /// Two times are equal if the have the exact same value.
        /// </summary>
        public bool Equals(IpV4TimeOfDay other)
        {
            return MillisecondsSinceMidnightUniversalTime == other.MillisecondsSinceMidnightUniversalTime;
        }

        /// <summary>
        /// Two times are equal if the have the exact same value.
        /// </summary>
        public override bool Equals(object obj)
        {
            return (obj is IpV4TimeOfDay &&
                    Equals((IpV4TimeOfDay)obj));
        }

        /// <summary>
        /// Two times are equal if the have the exact same value.
        /// </summary>
        public static bool operator ==(IpV4TimeOfDay value1, IpV4TimeOfDay value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Two times are different if the have different values.
        /// </summary>
        public static bool operator !=(IpV4TimeOfDay value1, IpV4TimeOfDay value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// The hash code of a time is the hash code of the milliseconds since midnight UT value.
        /// </summary>
        public override int GetHashCode()
        {
            return MillisecondsSinceMidnightUniversalTime.GetHashCode();
        }

        private readonly uint _millisecondsSinceMidnightUniversalTime;
    }
}