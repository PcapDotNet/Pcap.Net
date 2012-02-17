using System;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Useful functions for a sequence of objects.
    /// </summary>
    public static class Sequence
    {
        public static int GetHashCode(object value1, object value2)
        {
            if (value1 == null) 
                throw new ArgumentNullException("value1");
            if (value2 == null)
                throw new ArgumentNullException("value2");

            return value1.GetHashCode() ^ value2.GetHashCode();
        }

        public static int GetHashCode(object value1, object value2, object value3)
        {
            if (value3 == null)
                throw new ArgumentNullException("value3");

            return GetHashCode(value1, value2) ^ value3.GetHashCode();
        }

        public static int GetHashCode(params object[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            int result = 0;
            for (int i = 0; i != values.Length; ++i)
                result ^= values[i].GetHashCode();

            return result;
        }
    }
}