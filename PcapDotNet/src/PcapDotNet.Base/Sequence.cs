namespace PcapDotNet.Base
{
    public static class Sequence
    {
        public static int GetHashCode(object value1, object value2)
        {
            return value1.GetHashCode() ^ value2.GetHashCode();
        }

        public static int GetHashCode(object value1, object value2, object value3)
        {
            return GetHashCode(value1, value2) ^ value3.GetHashCode();
        }

        public static int GetHashCode(object value1, object value2, object value3, object value4)
        {
            return GetHashCode(value1, value2, value3) ^ value4.GetHashCode();
        }

        public static int GetHashCode(object value1, object value2, object value3, object value4, object value5)
        {
            return GetHashCode(value1, value2, value3, value4) ^ value5.GetHashCode();
        }

        public static int GetHashCode(object value1, object value2, object value3, object value4, object value5, object value6)
        {
            return GetHashCode(value1, value2, value3, value4, value5) ^ value6.GetHashCode();
        }

        public static int GetHashCode(object value1, object value2, object value3, object value4, object value5, object value6, object value7)
        {
            return GetHashCode(value1, value2, value3, value4, value5, value6) ^ value7.GetHashCode();
        }
    }
}