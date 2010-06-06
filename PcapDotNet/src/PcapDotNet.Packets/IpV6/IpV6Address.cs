using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// Represents an IPv6 address.
    /// </summary>
    public struct IpV6Address
    {
        /// <summary>
        /// The number of bytes the address take.
        /// </summary>
        public const int SizeOf = UInt128.SizeOf;

        /// <summary>
        /// The zero address (::).
        /// </summary>
        public static IpV6Address Zero
        {
            get { return _zero; }
        }

        /// <summary>
        /// Create an address from a 128 bit integer.
        /// 0 -> ::
        /// 1 -> ::1
        /// 256 -> ::100
        /// </summary>
        public IpV6Address(UInt128 value)
        {
            _value = value;
        }

        /// <summary>
        /// Creates an address from an address string ("2001:0db8:0::22:1.2.3.4").
        /// </summary>
        public IpV6Address(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            string cannonizedValue = value;
            
            // Handle ...:1.2.3.4
            int lastColonIndex = cannonizedValue.LastIndexOf(':');
            if (lastColonIndex == -1)
                throw new ArgumentException("Invalid IPv6 address format " + value);

            string lastPart = value.Substring(lastColonIndex + 1, cannonizedValue.Length - lastColonIndex - 1);
            if (lastPart.IndexOf('.') != -1)
            {
                uint lastPartValue = new IpV4Address(lastPart).ToValue();
                cannonizedValue = cannonizedValue.Substring(0, lastColonIndex + 1) +
                                  (lastPartValue >> 16).ToString("x", CultureInfo.InvariantCulture) + ":" + (lastPartValue & 0x0000FFFF).ToString("x", CultureInfo.InvariantCulture);
            }

            // Handle ...::...
            int doubleColonIndex = cannonizedValue.IndexOf("::", StringComparison.Ordinal);
            if (doubleColonIndex != -1)
            {
                int numMissingColons = 7 - cannonizedValue.Count(':');
                if (numMissingColons < 0)
                    throw new ArgumentException("Invalid IPv6 address format " + value);
                cannonizedValue = cannonizedValue.Substring(0, doubleColonIndex + 2) +
                                  new string(':', numMissingColons) +
                                  cannonizedValue.Substring(doubleColonIndex + 2);
            }

            IEnumerable<ushort> values =
                cannonizedValue.Split(':').Select(part => string.IsNullOrEmpty(part) ? (ushort)0 : ushort.Parse(part, NumberStyles.HexNumber, CultureInfo.InvariantCulture));

            ulong mostSignificant = values.Take(4).Aggregate((ulong)0, (sum, element) => (sum << 16) + element);
            ulong leastSignificant = values.Skip(4).Take(4).Aggregate((ulong)0, (sum, element) => (sum << 16) + element);

            _value = new UInt128(mostSignificant, leastSignificant);
        }

        /// <summary>
        /// Gets the address value as a 128 bit integer.
        /// </summary>
        public UInt128 ToValue()
        {
            return _value;
        }

        /// <summary>
        /// Two addresses are equal if the have the exact same value.
        /// </summary>
        public bool Equals(IpV6Address other)
        {
            return _value == other._value;
        }

        /// <summary>
        /// Two addresses are equal if the have the exact same value.
        /// </summary>
        public override bool Equals(object obj)
        {
            return (obj is IpV6Address &&
                    Equals((IpV6Address)obj));
        }

        /// <summary>
        /// Two addresses are equal if the have the exact same value.
        /// </summary>
        public static bool operator ==(IpV6Address value1, IpV6Address value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Two addresses are different if the have different values.
        /// </summary>
        public static bool operator !=(IpV6Address value1, IpV6Address value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// The hash code of an address is the hash code of its 128 bit integer value.
        /// </summary>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <summary>
        /// Translates the address to a string (0ABC:1234:5678:0443:0ABC:1234:5678:0443).
        /// </summary>
        public override string ToString()
        {
            string valueString = _value.ToString("X33", CultureInfo.InvariantCulture).Substring(1);
            StringBuilder stringBuilder = new StringBuilder(39);
            for (int i = 0; i != 8; ++i)
            {
                if (i != 0)
                    stringBuilder.Append(':');
                stringBuilder.Append(valueString.Substring(i * 4, 4));
            }

            return stringBuilder.ToString();
        }

        private readonly UInt128 _value;
        private static readonly IpV6Address _zero = new IpV6Address(0);
    }
}