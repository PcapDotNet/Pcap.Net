using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcapDotNet.Base;

namespace PcapDotNet.TestUtils
{
    public static class MoreRandom
    {
        public static bool NextBool(this Random random)
        {
            return random.Next() % 2 == 0;
        }

        public static byte NextByte(this Random random, int maxValue)
        {
            return (byte)random.Next(maxValue);
        }

        public static byte NextByte(this Random random)
        {
            return random.NextByte(byte.MaxValue + 1);
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
            return (uint)random.Next();
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

        public static DateTime NextDateTime(this Random random)
        {
            return new DateTime(random.NextLong(DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks + 1));
        }

        public static T NextEnum<T>(this Random random, params T[] valuesToIgnore)
        {
            Type type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException("T must be an Enum");

            IList<T> enumValues = new List<T>(((IEnumerable<T>)Enum.GetValues(type)).Except(valuesToIgnore));
            if (enumValues.Count == 0)
                throw new ArgumentException("T is an enum with no values", "T");

            return (T)random.NextValue(enumValues);
        }

        public static object NextValue<T>(this Random random, IList<T> values)
        {
            return values[random.Next(values.Count)];
        }
    }
}
