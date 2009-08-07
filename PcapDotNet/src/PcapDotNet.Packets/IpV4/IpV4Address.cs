using System.Globalization;
using System.Text;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Represents an IPv4 address.
    /// </summary>
    public struct IpV4Address
    {
        /// <summary>
        /// The number of bytes the address take.
        /// </summary>
        public const int SizeOf = sizeof(uint);

        /// <summary>
        /// Create an address from a 32 bit integer.
        /// 0 -> 0.0.0.0
        /// 1 -> 0.0.0.1
        /// 256 -> 0.0.1.0
        /// </summary>
        public IpV4Address(uint value)
        {
            _value = value;
        }

        /// <summary>
        /// Creates an address from an address string (1.2.3.4).
        /// </summary>
        public IpV4Address(string value)
        {
            string[] values = value.Split('.');
            _value = (uint)((byte.Parse(values[0], CultureInfo.InvariantCulture) << 24) +
                            (byte.Parse(values[1], CultureInfo.InvariantCulture) << 16) +
                            (byte.Parse(values[2], CultureInfo.InvariantCulture) << 8) +
                            (byte.Parse(values[3], CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// The zero address (0.0.0.0).
        /// </summary>
        public static IpV4Address Zero
        {
            get { return _zero; }
        }

        /// <summary>
        /// Gets the address valud as a 32 bit integer.
        /// </summary>
        public uint ToValue()
        {
            return _value;
        }

        /// <summary>
        /// Two addresses are equal if the have the exact same value.
        /// </summary>
        public bool Equals(IpV4Address other)
        {
            return ToValue() == other.ToValue();
        }

        /// <summary>
        /// Two addresses are equal if the have the exact same value.
        /// </summary>
        public override bool Equals(object obj)
        {
            return (obj is IpV4Address &&
                    Equals((IpV4Address)obj));
        }

        /// <summary>
        /// Two addresses are equal if the have the exact same value.
        /// </summary>
        public static bool operator ==(IpV4Address value1, IpV4Address value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Two addresses are different if the have different values.
        /// </summary>
        public static bool operator !=(IpV4Address value1, IpV4Address value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// The hash code of an address is the hash code of its 32 bit integer value.
        /// </summary>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <summary>
        /// Translates the address to a string (1.2.3.4).
        /// </summary>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(15);

            stringBuilder.Append(_value >> 24);
            stringBuilder.Append('.');
            stringBuilder.Append((_value >> 16) % 256);
            stringBuilder.Append('.');
            stringBuilder.Append((_value >> 8) % 256);
            stringBuilder.Append('.');
            stringBuilder.Append(_value % 256);

            return stringBuilder.ToString();
        }

        private readonly uint _value;
        private static readonly IpV4Address _zero = new IpV4Address(0);
    }
}