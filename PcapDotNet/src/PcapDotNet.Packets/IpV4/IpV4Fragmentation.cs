using System;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Represents IPv4 Fragmentation information.
    /// </summary>
    public struct IpV4Fragmentation : IEquatable<IpV4Fragmentation>
    {
        /// <summary>
        /// No fragmentation.
        /// </summary>
        public static IpV4Fragmentation None
        {
            get { return _none; }
        }

        /// <summary>
        /// Creates fragmentation field value according to the given information.
        /// </summary>
        /// <param name="options">Options for fragmentation (must be one of the values of the enum).</param>
        /// <param name="offset">This field indicates where in the complete datagram this fragment belongs. Measured in bytes but must divide by 8.</param>
        public IpV4Fragmentation(IpV4FragmentationOptions options, ushort offset)
            : this((ushort)((ushort)options | (offset / 8)))
        {
            if (offset % 8 != 0)
                throw new ArgumentException("offset must divide by 8", "offset");

            if (((ushort)options & 0x1FFF) != 0)
                throw new ArgumentException("invalid options " + options);
        }

        /// <summary>
        /// Options for fragmentation.
        /// </summary>
        public IpV4FragmentationOptions Options
        {
            get { return (IpV4FragmentationOptions)(_value & 0xE000); }
        }

        /// <summary>
        /// This field indicates where in the complete datagram this fragment belongs. Measured in bytes but must divide by 8.
        /// </summary>
        public ushort Offset
        {
            get { return (ushort)((_value & 0x1FFF) * 8); }
        }

        /// <summary>
        /// Two framentations are equal if they are exactly the same fragmentation (options and offset).
        /// </summary>
        public bool Equals(IpV4Fragmentation other)
        {
            return _value == other._value;
        }

        /// <summary>
        /// Two framentations are equal if they are exactly the same fragmentation (options and offset).
        /// </summary>
        public override bool Equals(object obj)
        {
            return (obj is IpV4Fragmentation &&
                    Equals((IpV4Fragmentation)obj));
        }

        /// <summary>
        /// Two framentations are equal if they are exactly the same fragmentation (options and offset).
        /// </summary>
        public static bool operator ==(IpV4Fragmentation value1, IpV4Fragmentation value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Two framentations are different if they are different fragmentation (options or offset).
        /// </summary>
        public static bool operator !=(IpV4Fragmentation value1, IpV4Fragmentation value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// The hash code of the fragmentation is the hash code of its combined flags and offset 16 bit field.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        internal IpV4Fragmentation(ushort value)
        {
            _value = value;
        }

        internal void Write(byte[] buffer, int offset)
        {
            buffer.Write(offset, _value, Endianity.Big);
        }

        private static readonly IpV4Fragmentation _none = new IpV4Fragmentation(IpV4FragmentationOptions.None, 0);
        private readonly ushort _value;
    }
}