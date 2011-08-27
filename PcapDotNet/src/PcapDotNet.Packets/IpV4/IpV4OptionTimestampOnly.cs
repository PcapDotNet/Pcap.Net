using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Represents a timestamp IPv4 option with only the timestamps.
    /// </summary>
    public sealed class IpV4OptionTimestampOnly : IpV4OptionTimestamp
    {
        /// <summary>
        /// Create the option by giving it all the data.
        /// </summary>
        /// <param name="overflow">The number of IP modules that cannot register timestamps due to lack of space. Maximum value is 15.</param>
        /// <param name="pointedIndex">The index in the timestamp that points to the for next timestamp.</param>
        /// <param name="timestamps">The timestamps as time passed since midnight UT.</param>
        public IpV4OptionTimestampOnly(byte overflow, byte pointedIndex, IList<IpV4TimeOfDay> timestamps)
            : base(IpV4OptionTimestampType.TimestampOnly, overflow, pointedIndex)
        {
            _timestamps = timestamps.AsReadOnly();
        }

        /// <summary>
        /// Create the option by giving it all the data.
        /// </summary>
        /// <param name="overflow">The number of IP modules that cannot register timestamps due to lack of space. Maximum value is 15.</param>
        /// <param name="pointedIndex">The index in the timestamp that points to the for next timestamp.</param>
        /// <param name="timestamps">The timestamps as time passed since midnight UT.</param>
        public IpV4OptionTimestampOnly(byte overflow, byte pointedIndex, params IpV4TimeOfDay[] timestamps)
            : this(overflow, pointedIndex, (IList<IpV4TimeOfDay>)timestamps)
        {
        }

        /// <summary>
        /// The number of timestamps this option holds (or can hold).
        /// </summary>
        public override int CountTimestamps
        {
            get { return Timestamps.Count(); }
        }

        /// <summary>
        /// The timestamps as time passed since midnight UT.
        /// </summary>
        public ReadOnlyCollection<IpV4TimeOfDay> Timestamps
        {
            get { return _timestamps; }
        }

        /// <summary>
        /// The hash code of this options is the hash code of the base class xored with the hash code of the timestamps.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Timestamps.SequenceGetHashCode();
        }

        internal static IpV4OptionTimestampOnly Read(byte overflow, byte pointedIndex, byte[] buffer, ref int offset, int numValues)
        {
            IpV4TimeOfDay[] timestamps = new IpV4TimeOfDay[numValues];
            for (int i = 0; i != numValues; ++i)
                timestamps[i] = buffer.ReadIpV4TimeOfDay(ref offset, Endianity.Big);

            return new IpV4OptionTimestampOnly(overflow, pointedIndex, timestamps);
        }

        /// <summary>
        /// The number of bytes the value of the option take.
        /// </summary>
        protected override int ValuesLength
        {
            get { return Timestamps.Count * sizeof(uint); }
        }

        /// <summary>
        /// Compares the values of the options.
        /// </summary>
        protected override bool EqualValues(IpV4OptionTimestamp other)
        {
            return Timestamps.SequenceEqual(((IpV4OptionTimestampOnly)other).Timestamps);
        }

        /// <summary>
        /// Writes the value of the option to the buffer.
        /// </summary>
        protected override void WriteValues(byte[] buffer, ref int offset)
        {
            foreach (IpV4TimeOfDay timestamp in Timestamps)
                buffer.Write(ref offset, timestamp.MillisecondsSinceMidnightUniversalTime, Endianity.Big);
        }

        private readonly ReadOnlyCollection<IpV4TimeOfDay> _timestamps;
    }
}