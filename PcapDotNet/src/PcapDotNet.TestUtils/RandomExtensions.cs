using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using PcapDotNet.Base;

namespace PcapDotNet.TestUtils
{
    [ExcludeFromCodeCoverage]
    public static class RandomExtensions
    {
        public static bool NextBool(this Random random, int chance = 2)
        {
            if (chance < 1)
                throw new ArgumentOutOfRangeException("chance", chance, "must be at least 1");
            return random.Next() % chance == 0;
        }

        public static byte NextByte(this Random random, int minValue, int maxValue)
        {
            return (byte)random.NextInt(minValue, maxValue);
        }

        public static byte NextByte(this Random random, int maxValue)
        {
            return (byte)random.NextInt(0, maxValue);
        }

        public static byte NextByte(this Random random)
        {
            return random.NextByte(byte.MaxValue + 1);
        }

        public static byte[] NextBytes(this Random random, int length)
        {
            byte[] bytes = new byte[length];
            random.NextBytes(bytes);
            return bytes;
        }

        public static char NextChar(this Random random, char minValue, char maxValue)
        {
            return (char)random.NextInt(minValue, maxValue);
        }

        public static ushort NextUShort(this Random random, int maxValue)
        {
            return (ushort)random.NextInt(0, maxValue);
        }

        public static ushort NextUShort(this Random random, ushort minValue, int maxValue)
        {
            return (ushort)random.NextInt(minValue, maxValue);
        }

        public static ushort NextUShort(this Random random)
        {
            return random.NextUShort(ushort.MaxValue + 1);
        }

        public static UInt24 NextUInt24(this Random random)
        {
            return (UInt24)random.NextInt(0, UInt24.MaxValue + 1);
        }

        public static int NextInt(this Random random)
        {
            return random.Next(int.MinValue, int.MaxValue);
        }

        public static int NextInt(this Random random, int maxValue)
        {
            return random.Next(int.MinValue, maxValue);
        }

        public static int NextInt(this Random random, int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }

        public static uint NextUInt(this Random random)
        {
            return (uint)random.NextInt();
        }

        public static uint NextUInt(this Random random, uint maxValue)
        {
            return random.NextUInt() % maxValue;
        }

        public static uint NextUInt(this Random random, uint minvalue, uint maxValue)
        {
            return random.NextUInt() % (maxValue - minvalue + 1) + minvalue;
        }

        public static UInt48 NextUInt48(this Random random)
        {
            return (UInt48)random.NextLong(UInt48.MaxValue + 1);
        }

        public static long NextLong(this Random random, long minValue, long maxValue)
        {
            return minValue + (long)random.NextULong((ulong)(maxValue - minValue));
        }

        public static long NextLong(this Random random, long maxValue)
        {
            return random.NextLong(0, maxValue);
        }

        public static long NextLong(this Random random)
        {
            return (long)random.NextULong();
        }

        public static ulong NextULong(this Random random, ulong maxValue)
        {
            return random.NextULong() % maxValue;
        }

        public static ulong NextULong(this Random random)
        {
            return ((((ulong)random.NextUInt()) << 32) + random.NextUInt());
        }

        public static UInt128 NextUInt128(this Random random)
        {
            return new UInt128(random.NextULong(), random.NextULong());
        }

        public static UInt128 NextUInt128(this Random random, UInt128 maximumValue)
        {
            if (maximumValue <= ulong.MaxValue)
                return random.NextULong((ulong)maximumValue);
            return new UInt128(random.NextULong((ulong)(maximumValue >> 64)), random.NextULong());
        }

        public static UInt128 NextUInt128(this Random random, UInt128 minimumValue, UInt128 maximumValue)
        {
            if (minimumValue == 0)
                return random.NextUInt128(maximumValue);

            return minimumValue + random.NextUInt128(maximumValue - minimumValue);
        }

        public static DateTime NextDateTime(this Random random, DateTime minimumValue, DateTime maximumValue)
        {
            return new DateTime(random.NextLong(minimumValue.ToUniversalTime().Ticks,
                                                maximumValue.ToUniversalTime().Ticks + 1), DateTimeKind.Utc).ToLocalTime();
        }

        public static DateTime NextDateTime(this Random random)
        {
            return random.NextDateTime(DateTime.MinValue, DateTime.MaxValue);
        }

        public static TimeSpan NextTimeSpan(this Random random, TimeSpan minimumValue, TimeSpan maximumValue)
        {
            return TimeSpan.FromTicks(random.NextLong(minimumValue.Ticks, maximumValue.Ticks + 1));
        }

        public static T NextEnum<T>(this Random random, IEnumerable<T> valuesToIgnore)
        {
            Type type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException("T must be an Enum");

            IList<T> enumValues = new List<T>(type.GetEnumValues<T>().Except(valuesToIgnore));
            if (enumValues.Count == 0)
                throw new ArgumentException("T is an enum with no values", "T");

            return random.NextValue(enumValues);
        }

        public static T NextEnum<T>(this Random random, params T[] valuesToIgnore)
        {
            return random.NextEnum((IEnumerable<T>)valuesToIgnore);
        }

        public static T NextValue<T>(this Random random, IList<T> values)
        {
            return values[random.Next(values.Count)];
        }

        public static T NextFlags<T>(this Random random)
        {
            Type type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException("T must be an Enum");

            IEnumerable<T> enumValues = (IEnumerable<T>)Enum.GetValues(type);
            Type underlyingType = Enum.GetUnderlyingType(type);

            List<object> enumValuesAsUnderlyingType = new List<object>(enumValues.Select(value => Convert.ChangeType(value, underlyingType)));
            List<ulong> enumValuesAsULong;
            if (underlyingType == typeof(byte))
                enumValuesAsULong = new List<ulong>(enumValuesAsUnderlyingType.Cast<byte>().Select(value => (ulong)value));
            else if (underlyingType == typeof(ushort))
                enumValuesAsULong = new List<ulong>(enumValuesAsUnderlyingType.Cast<ushort>().Select(value => (ulong)value));
            else
                throw new ArgumentException("Type " + underlyingType + " is not supported");

            ulong resultValue = enumValuesAsULong.Aggregate(default(ulong),
                                                            (result, value) => random.NextBool()
                                                                                   ? result | value
                                                                                   : result);
            T resultEnum = (T)Convert.ChangeType(resultValue, underlyingType);
            return resultEnum;
        }

        public static string NextCString(this Random random, int minLength, int maxLength)
        {
            if (minLength < 0)
                throw new ArgumentOutOfRangeException(nameof(minLength), minLength, $"{nameof(minLength)} has to be greater than {nameof(maxLength)}");
            if (minLength > maxLength)
                throw new ArgumentOutOfRangeException(nameof(minLength), minLength, $"{nameof(minLength)} has to be greater or equal than {nameof(maxLength)}");

            return random.NextCString(random.NextInt(minLength, maxLength));
        }

        public static string NextCString(this Random random, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, $"{nameof(length)} has to be greater than 0");

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                builder.Append(random.NextChar((char)1, (char)127));
            }
            return builder.ToString();
        }
    }
}