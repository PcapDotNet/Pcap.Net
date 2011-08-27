using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV4
{
    ///<summary>
    /// Represents a timestamp IPv4 option with each timestamp preceded with internet address of the registering entity or the internet address fields are prespecified.
    ///</summary>
    public sealed class IpV4OptionTimestampAndAddress : IpV4OptionTimestamp
    {
        /// <summary>
        /// Create the option by giving it all the data.
        /// </summary>
        /// <param name="timestampType">The timestamp option type.</param>
        /// <param name="overflow">The number of IP modules that cannot register timestamps due to lack of space. Maximum value is 15.</param>
        /// <param name="pointedIndex">The index in the timestamp that points to the for next timestamp.</param>
        /// <param name="timedRoute">The pairs of addresses and timestamps where each timestamp time passed since midnight UT.</param>
        public IpV4OptionTimestampAndAddress(IpV4OptionTimestampType timestampType, byte overflow, byte pointedIndex, IList<IpV4OptionTimedAddress> timedRoute)
            : base(timestampType, overflow, pointedIndex)
        {
            if (timestampType != IpV4OptionTimestampType.AddressAndTimestamp &&
                timestampType != IpV4OptionTimestampType.AddressPrespecified)
            {
                throw new ArgumentException("Illegal timestamp type " + timestampType, "timestampType");
            }

            _addressesAndTimestamps = timedRoute.AsReadOnly();
        }

        /// <summary>
        /// Create the option by giving it all the data.
        /// </summary>
        /// <param name="timestampType">The timestamp option type.</param>
        /// <param name="overflow">The number of IP modules that cannot register timestamps due to lack of space. Maximum value is 15.</param>
        /// <param name="pointedIndex">The index in the timestamp that points to the for next timestamp.</param>
        /// <param name="timedRoute">The pairs of addresses and timestamps where each timestamp time passed since midnight UT.</param>
        public IpV4OptionTimestampAndAddress(IpV4OptionTimestampType timestampType, byte overflow, byte pointedIndex, params IpV4OptionTimedAddress[] timedRoute)
            : this(timestampType, overflow, pointedIndex, (IList<IpV4OptionTimedAddress>)timedRoute)
        {
        }

        /// <summary>
        /// The number of timestamps this option holds (or can hold).
        /// </summary>
        public override int CountTimestamps
        {
            get { return TimedRoute.Count(); }
        }

        /// <summary>
        /// The pairs of addresses and timestamps where each timestamp time passed since midnight UT.
        /// </summary>
        public ReadOnlyCollection<IpV4OptionTimedAddress> TimedRoute
        {
            get { return _addressesAndTimestamps; }
        }

        /// <summary>
        /// The hash of this option is the base class hash xored with the hash of each timestamp.
        /// </summary>
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
                                                                       buffer.ReadIpV4TimeOfDay(ref offset, Endianity.Big));
            }

            return new IpV4OptionTimestampAndAddress(timestampType, overflow, pointedIndex, addressesAndTimestamps);
        }

        /// <summary>
        /// The number of bytes the value of the option take.
        /// </summary>
        protected override int ValuesLength
        {
            get { return TimedRoute.Count * (IpV4Address.SizeOf + sizeof(uint)); }
        }

        /// <summary>
        /// True iff the options values is equal.
        /// </summary>
        protected override bool EqualValues(IpV4OptionTimestamp other)
        {
            return TimedRoute.SequenceEqual(((IpV4OptionTimestampAndAddress)other).TimedRoute);
        }

        /// <summary>
        /// Writes the value of the option to the buffer.
        /// </summary>
        protected override void WriteValues(byte[] buffer, ref int offset)
        {
            foreach (IpV4OptionTimedAddress addressAndTimestamp in TimedRoute)
            {
                buffer.Write(ref offset, addressAndTimestamp.Address, Endianity.Big);
                buffer.Write(ref offset, addressAndTimestamp.TimeOfDay, Endianity.Big);
            }
        }

        private readonly ReadOnlyCollection<IpV4OptionTimedAddress> _addressesAndTimestamps;
    }
}