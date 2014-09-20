using System;
using System.Globalization;
using System.Text;
using PcapDotNet.Base;

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
        /// The zero address (0.0.0.0).
        /// </summary>
        public static IpV4Address Zero
        {
            get { return _zero; }
        }

        /// <summary>
        /// The all-hosts group. 
        /// If you ping that group, all multicast capable hosts on the network should answer, 
        /// as every multicast capable host must join that group at start-up on all it's multicast capable interfaces. 
        /// </summary>
        public static IpV4Address AllHostsGroupAddress
        {
            get { return _allHostsGroupAddress; }
        }

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
            if (value == null) 
                throw new ArgumentNullException("value");

            string[] values = value.Split('.');
            _value = BitSequence.Merge(byte.Parse(values[0], CultureInfo.InvariantCulture),
                                       byte.Parse(values[1], CultureInfo.InvariantCulture),
                                       byte.Parse(values[2], CultureInfo.InvariantCulture),
                                       byte.Parse(values[3], CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Converts the string representation of an IPv4 address (1.2.3.4) to its IPv4 address equivalent.
        /// A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="ipV6AddressString">A string containing the IPv4 address to convert (1.2.3.4).</param>
        /// <param name="result">
        /// When this method returns, contains the IPv4 address value equivalent of the IPv4 address contained in s, if the conversion succeeded,
        /// or zero IPv4 address if the conversion failed.
        /// The conversion fails if the s parameter is null or String.Empty or is not of the correct format. This parameter is passed uninitialized.
        /// </param>
        /// <returns>True iff parsing was successful.</returns>
        public static bool TryParse(string ipV6AddressString, out IpV4Address result)
        {
            if (ipV6AddressString == null)
            {
                result = Zero;
                return false;
            }

            string[] valuesStrings = ipV6AddressString.Split('.');
            if (valuesStrings.Length != 4)
            {
                result = Zero;
                return false;
            }

            byte[] values = new byte[4];
            for (int i = 0; i != 4; ++i)
            {
                if (!byte.TryParse(valuesStrings[i], NumberStyles.None, CultureInfo.InvariantCulture, out values[i]))
                {
                    result = Zero;
                    return false;
                }
            }

            result = new IpV4Address(BitSequence.Merge(values[0], values[1], values[2], values[3]));
            return true;
        }

        /// <summary>
        /// Gets the address value as a 32 bit integer.
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
        private static readonly IpV4Address _allHostsGroupAddress = new IpV4Address("224.0.0.1");
    }
}