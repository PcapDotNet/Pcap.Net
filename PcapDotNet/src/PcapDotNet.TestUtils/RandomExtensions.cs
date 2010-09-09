﻿using System;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.TestUtils
{
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
            return (byte)random.Next(minValue, maxValue);
        }

        public static byte NextByte(this Random random, int maxValue)
        {
            return (byte)random.Next(maxValue);
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
            return (char)random.Next(minValue, maxValue);
        }

        public static ushort NextUShort(this Random random, int maxValue)
        {
            return (ushort)random.Next(maxValue);
        }

        public static ushort NextUShort(this Random random)
        {
            return random.NextUShort(ushort.MaxValue + 1);
        }

        public static UInt24 NextUInt24(this Random random)
        {
            return (UInt24)random.Next(UInt24.MaxValue + 1);
        }

        public static uint NextUInt(this Random random)
        {
            return (uint)random.Next(int.MinValue, int.MaxValue);
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
            if (underlyingType == typeof(ushort))
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
    }
}
