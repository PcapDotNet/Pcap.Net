using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// The base class for route tracking options (loose, strict, record).
    /// </summary>
    public abstract class IpV4OptionRoute : IpV4OptionComplex, IEquatable<IpV4OptionRoute>
    {
        /// <summary>
        /// The minimum option length in bytes (type, length, pointer).
        /// </summary>
        public const int OptionMinimumLength = 3;

        /// <summary>
        /// The minimum option value length in bytes (pointer).
        /// </summary>
        public const int OptionValueMinimumLength = OptionMinimumLength - OptionHeaderLength;

        /// <summary>
        /// The maximum value for the index pointed the route.
        /// </summary>
        public const byte PointedAddressIndexMaxValue = byte.MaxValue / 4 - 1;

        /// <summary>
        /// The pointed index in the route.
        /// </summary>
        public byte PointedAddressIndex
        {
            get { return _pointedAddressIndex; }
        }

        /// <summary>
        /// The route tracked - the collection of addresses written.
        /// </summary>
        public ReadOnlyCollection<IpV4Address> Route
        {
            get { return _route; }
        }

        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public override int Length
        {
            get { return OptionMinimumLength + IpV4Address.SizeOf * Route.Count; }
        }

        /// <summary>
        /// True iff this option may appear at most once in a datagram.
        /// </summary>
        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        /// <summary>
        /// Two routes options are equal iff they have the same type, same pointed address index and same route.
        /// </summary>
        public bool Equals(IpV4OptionRoute other)
        {
            if (other == null)
                return false;

            return Equivalent(other) &&
                   PointedAddressIndex == other.PointedAddressIndex &&
                   Route.SequenceEqual(other.Route);
        }

        /// <summary>
        /// Two routes options are equal iff they have the same type, same pointed address index and same route.
        /// </summary>
        public override bool Equals(IpV4Option other)
        {
            return Equals(other as IpV4OptionRoute);
        }

        /// <summary>
        /// The hash code of the route option is the xor of the following hash codes: base class, pointed address index and all the addresses in the route.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   PointedAddressIndex.GetHashCode() ^
                   Route.SequenceGetHashCode();
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer[offset++] = (byte)(OptionMinimumLength + 1 + PointedAddressIndex * 4);
            foreach (IpV4Address address in Route)
                buffer.Write(ref offset, address, Endianity.Big);
        }

        /// <summary>
        /// Tries to read the value of the route option from the given buffer.
        /// </summary>
        /// <param name="addresses">The route read from the buffer.</param>
        /// <param name="pointedAddressIndex">The index pointed in the route read from the buffer.</param>
        /// <param name="buffer">The buffer to read the value from.</param>
        /// <param name="offset">The offset in the buffer to start reading the value from.</param>
        /// <param name="valueLength">The number of bytes that the value should be.</param>
        /// <returns>True iff the read was successful.</returns>
        protected static bool TryRead(out IpV4Address[] addresses, out byte pointedAddressIndex,
                                      byte[] buffer, ref int offset, byte valueLength)
        {
            addresses = null;
            pointedAddressIndex = 0;

            if (valueLength < OptionValueMinimumLength)
                return false;

            if (valueLength % 4 != 1)
                return false;

            byte pointer = buffer[offset++];
            if (pointer % 4 != 0 || pointer < 4)
                return false;

            pointedAddressIndex = (byte)(pointer / 4 - 1);

            int numAddresses = valueLength / 4;
            addresses = new IpV4Address[numAddresses];
            for (int i = 0; i != numAddresses; ++i)
                addresses[i] = buffer.ReadIpV4Address(ref offset, Endianity.Big);

            return true;
        }

        /// <summary>
        /// Construct a route option from the given values.
        /// </summary>
        protected IpV4OptionRoute(IpV4OptionType optionType, IList<IpV4Address> route, byte pointedAddressIndex)
            : base(optionType)
        {
            if (pointedAddressIndex > PointedAddressIndexMaxValue)
                throw new ArgumentOutOfRangeException("pointedAddressIndex", pointedAddressIndex, "Maximum value is " + PointedAddressIndexMaxValue);

            _route = route.AsReadOnly();
            _pointedAddressIndex = pointedAddressIndex;
        }

        private readonly ReadOnlyCollection<IpV4Address> _route;
        private readonly byte _pointedAddressIndex;
    }
}