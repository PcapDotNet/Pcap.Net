using System;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Useful functions for a sequence of objects.
    /// </summary>
    public static class Sequence
    {
        /// <summary>
        /// Returns the xor of the hash codes of the given objects.
        /// </summary>
        public static int GetHashCode(object value1, object value2)
        {
            int hashCode = 0;
            if (value1 != null)
                hashCode ^= value1.GetHashCode();
            if (value2 != null)
                hashCode ^= value2.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Returns the xor of the hash codes of the given objects.
        /// </summary>
        public static int GetHashCode(object value1, object value2, object value3)
        {
            int result = GetHashCode(value1, value2);
            if (value3 == null)
                return result;

            return result ^ value3.GetHashCode();
        }

        /// <summary>
        /// Returns the xor of the hash codes of the given objects.
        /// </summary>
        public static int GetHashCode(params object[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            int result = 0;
            foreach (object value in values)
            {
                if (value != null)
                    result ^= value.GetHashCode();
            }

            return result;
        }
    }
}