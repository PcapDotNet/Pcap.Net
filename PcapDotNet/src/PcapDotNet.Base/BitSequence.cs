namespace PcapDotNet.Base
{
    public static class BitSequence
    {
        public static byte ToByte(this bool value)
        {
            return value ? (byte)1 : (byte)0;
        }

        public static byte Merge(bool value1, bool value2)
        {
            return (byte)((value1.ToByte() << 1) | value2.ToByte());
        }

        public static byte Merge(bool value1, bool value2, bool value3)
        {
            return (byte)((Merge(value1, value2) << 1) | value3.ToByte());
        }

        public static byte Merge(bool value1, bool value2, bool value3, bool value4)
        {
            return (byte)((Merge(value1, value2, value3) << 1) | value4.ToByte());
        }

        public static byte Merge(bool value1, bool value2, bool value3, bool value4, bool value5)
        {
            return (byte)((Merge(value1, value2, value3, value4) << 1) | value5.ToByte());
        }

        public static byte Merge(bool value1, bool value2, bool value3, bool value4, bool value5, bool value6)
        {
            return (byte)((Merge(value1, value2, value3, value4, value5) << 1) | value6.ToByte());
        }

        public static byte Merge(bool value1, bool value2, bool value3, bool value4, bool value5, bool value6, bool value7)
        {
            return (byte)((Merge(value1, value2, value3, value4, value5, value6) << 1) | value7.ToByte());
        }

        public static byte Merge(bool value1, bool value2, bool value3, bool value4, bool value5, bool value6, bool value7, bool value8)
        {
            return (byte)((Merge(value1, value2, value3, value4, value5, value6, value7) << 1) | value8.ToByte());
        }

        public static ushort Merge(byte value1, byte value2)
        {
            return (ushort)((value1 << 8) | value2);
        }

        public static UInt24 Merge(byte value1, byte value2, byte value3)
        {
            return (UInt24)Merge(0, value1, value2, value3);
        }

        public static uint Merge(byte value1, byte value2, byte value3, byte value4)
        {
            return Merge(Merge(value1, value2), Merge(value3, value4));
        }

        public static UInt48 Merge(byte value1, byte value2, byte value3, byte value4, byte value5, byte value6)
        {
            return (UInt48)Merge(0, 0, value1, value2, value3, value4, value5, value6);
        }

        public static ulong Merge(byte value1, byte value2, byte value3, byte value4, byte value5, byte value6, byte value7, byte value8)
        {
            return Merge(Merge(value1, value2, value3, value4), Merge(value5, value6, value7, value8));
        }

        public static uint Merge(ushort value1, ushort value2)
        {
            return (uint)((value1 << 16) | value2);
        }

        public static UInt24 Merge(byte value1, ushort value2)
        {
            return (UInt24)Merge(0, value1, value2);
        }

        public static uint Merge(ushort value1, byte value2, byte value3)
        {
            return Merge(value1, Merge(value2, value3));
        }

        public static uint Merge(byte value1, ushort value2, byte value3)
        {
            return (uint)((value1 << 24) | (value2 << 8) | value3);
        }

        public static uint Merge(byte value1, byte value2, ushort value3)
        {
            return Merge(Merge(value1, value2), value3);
        }

        public static ulong Merge(uint value1, uint value2)
        {
            return (((ulong)value1) << 32) | value2;
        }
    }
}