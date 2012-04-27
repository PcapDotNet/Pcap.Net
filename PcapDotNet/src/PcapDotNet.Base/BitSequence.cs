using System;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Useful functions for sequences of bits.
    /// </summary>
    public static class BitSequence
    {
        /// <summary>
        /// Casts a bit to a byte.
        /// True becomes 1 and false becomes 0.
        /// </summary>
        /// <param name="value">Bit 7 of the byte.</param>
        /// <returns>1 for true, 0 for false.</returns>
        public static byte ToByte(this bool value)
        {
            return value ? (byte)1 : (byte)0;
        }

        /// <summary>
        /// Merges 2 bits to a byte.
        /// Bits should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bit 6 of the byte.</param>
        /// <param name="value2">Bit 7 of the byte.</param>
        /// <returns>A byte with the 2 least significant bits determined by the input.</returns>
        public static byte Merge(bool value1, bool value2)
        {
            return (byte)((value1.ToByte() << 1) | value2.ToByte());
        }

        /// <summary>
        /// Merges 3 bits to a byte.
        /// Bits should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bit 5 of the byte.</param>
        /// <param name="value2">Bit 6 of the byte.</param>
        /// <param name="value3">Bit 7 of the byte.</param>
        /// <returns>A byte with the 3 least significant bits determined by the input.</returns>
        public static byte Merge(bool value1, bool value2, bool value3)
        {
            return (byte)((Merge(value1, value2) << 1) | value3.ToByte());
        }

        /// <summary>
        /// Merges 4 bits to a byte.
        /// Bits should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bit 4 of the byte.</param>
        /// <param name="value2">Bit 5 of the byte.</param>
        /// <param name="value3">Bit 6 of the byte.</param>
        /// <param name="value4">Bit 7 of the byte.</param>
        /// <returns>A byte with the 4 least significant bits determined by the input.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1025:ReplaceRepetitiveArgumentsWithParamsArray")]
        public static byte Merge(bool value1, bool value2, bool value3, bool value4)
        {
            return (byte)((Merge(value1, value2, value3) << 1) | value4.ToByte());
        }

        /// <summary>
        /// Merges 5 bits to a byte.
        /// Bits should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bit 3 of the byte.</param>
        /// <param name="value2">Bit 4 of the byte.</param>
        /// <param name="value3">Bit 5 of the byte.</param>
        /// <param name="value4">Bit 6 of the byte.</param>
        /// <param name="value5">Bit 7 of the byte.</param>
        /// <returns>A byte with the 5 least significant bits determined by the input.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1025:ReplaceRepetitiveArgumentsWithParamsArray")]
        public static byte Merge(bool value1, bool value2, bool value3, bool value4, bool value5)
        {
            return (byte)((Merge(value1, value2, value3, value4) << 1) | value5.ToByte());
        }

        /// <summary>
        /// Merges 6 bits to a byte.
        /// Bits should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bit 2 of the byte.</param>
        /// <param name="value2">Bit 3 of the byte.</param>
        /// <param name="value3">Bit 4 of the byte.</param>
        /// <param name="value4">Bit 5 of the byte.</param>
        /// <param name="value5">Bit 6 of the byte.</param>
        /// <param name="value6">Bit 7 of the byte.</param>
        /// <returns>A byte with the 6 least significant bits determined by the input.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1025:ReplaceRepetitiveArgumentsWithParamsArray")]
        public static byte Merge(bool value1, bool value2, bool value3, bool value4, bool value5, bool value6)
        {
            return (byte)((Merge(value1, value2, value3, value4, value5) << 1) | value6.ToByte());
        }

        /// <summary>
        /// Merges 7 bits to a byte.
        /// Bits should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bit 1 of the byte.</param>
        /// <param name="value2">Bit 2 of the byte.</param>
        /// <param name="value3">Bit 3 of the byte.</param>
        /// <param name="value4">Bit 4 of the byte.</param>
        /// <param name="value5">Bit 5 of the byte.</param>
        /// <param name="value6">Bit 6 of the byte.</param>
        /// <param name="value7">Bit 7 of the byte.</param>
        /// <returns>A byte with the 7 least significant bits determined by the input.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1025:ReplaceRepetitiveArgumentsWithParamsArray")]
        public static byte Merge(bool value1, bool value2, bool value3, bool value4, bool value5, bool value6, bool value7)
        {
            return (byte)((Merge(value1, value2, value3, value4, value5, value6) << 1) | value7.ToByte());
        }

        /// <summary>
        /// Merges 8 bits to a byte.
        /// Bits should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bit 0 of the byte.</param>
        /// <param name="value2">Bit 1 of the byte.</param>
        /// <param name="value3">Bit 2 of the byte.</param>
        /// <param name="value4">Bit 3 of the byte.</param>
        /// <param name="value5">Bit 4 of the byte.</param>
        /// <param name="value6">Bit 5 of the byte.</param>
        /// <param name="value7">Bit 6 of the byte.</param>
        /// <param name="value8">Bit 7 of the byte.</param>
        /// <returns>A byte whose bits are determined by the input.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1025:ReplaceRepetitiveArgumentsWithParamsArray")]
        public static byte Merge(bool value1, bool value2, bool value3, bool value4, bool value5, bool value6, bool value7, bool value8)
        {
            return (byte)((Merge(value1, value2, value3, value4, value5, value6, value7) << 1) | value8.ToByte());
        }

        /// <summary>
        /// Merges 2 bytes to a ushort.
        /// Bytes should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bits 0-7 of the ushort.</param>
        /// <param name="value2">Bits 8-15 of the ushort.</param>
        /// <returns>A ushort whose bits are determined by the input.</returns>
        public static ushort Merge(byte value1, byte value2)
        {
            return (ushort)((value1 << 8) | value2);
        }

        /// <summary>
        /// Merges 3 bytes to a UInt24.
        /// Bytes should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bits 0-7 of the UInt24.</param>
        /// <param name="value2">Bits 8-15 of the UInt24.</param>
        /// <param name="value3">Bits 16-23 of the UInt24.</param>
        /// <returns>A UInt24 whose bits are determined by the input.</returns>
        public static UInt24 Merge(byte value1, byte value2, byte value3)
        {
            return (UInt24)Merge(0, value1, value2, value3);
        }

        /// <summary>
        /// Merges 4 bytes to a uint.
        /// Bytes should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bits 0-7 of the uint.</param>
        /// <param name="value2">Bits 8-15 of the uint.</param>
        /// <param name="value3">Bits 16-23 of the uint.</param>
        /// <param name="value4">Bits 24-31 of the uint.</param>
        /// <returns>A uint whose bits are determined by the input.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1025:ReplaceRepetitiveArgumentsWithParamsArray")]
        public static uint Merge(byte value1, byte value2, byte value3, byte value4)
        {
            return Merge(Merge(value1, value2), Merge(value3, value4));
        }

        /// <summary>
        /// Merges 6 bytes to a UInt48.
        /// Bytes should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bits 0-7 of the UInt48.</param>
        /// <param name="value2">Bits 8-15 of the UInt48.</param>
        /// <param name="value3">Bits 16-23 of the UInt48.</param>
        /// <param name="value4">Bits 24-31 of the UInt48.</param>
        /// <param name="value5">Bits 32-39 of the UInt48.</param>
        /// <param name="value6">Bits 40-47 of the UInt48.</param>
        /// <returns>A UInt48 whose bits are determined by the input.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1025:ReplaceRepetitiveArgumentsWithParamsArray")]
        public static UInt48 Merge(byte value1, byte value2, byte value3, byte value4, byte value5, byte value6)
        {
            return (UInt48)Merge(0, 0, value1, value2, value3, value4, value5, value6);
        }

        /// <summary>
        /// Merges 8 bytes to a ulong.
        /// Bytes should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bits 0-7 of the ulong.</param>
        /// <param name="value2">Bits 8-15 of the ulong.</param>
        /// <param name="value3">Bits 16-23 of the ulong.</param>
        /// <param name="value4">Bits 24-31 of the ulong.</param>
        /// <param name="value5">Bits 32-39 of the ulong.</param>
        /// <param name="value6">Bits 40-47 of the ulong.</param>
        /// <param name="value7">Bits 48-55 of the ulong.</param>
        /// <param name="value8">Bits 56-63 of the ulong.</param>
        /// <returns>A ulong whose bits are determined by the input.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1025:ReplaceRepetitiveArgumentsWithParamsArray")]
        public static ulong Merge(byte value1, byte value2, byte value3, byte value4, byte value5, byte value6, byte value7, byte value8)
        {
            return Merge(Merge(value1, value2, value3, value4), Merge(value5, value6, value7, value8));
        }

        /// <summary>
        /// Merges 16 bytes to a UInt128.
        /// Bytes should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bits 0-7 of the UInt128.</param>
        /// <param name="value2">Bits 8-15 of the UInt128.</param>
        /// <param name="value3">Bits 16-23 of the UInt128.</param>
        /// <param name="value4">Bits 24-31 of the UInt128.</param>
        /// <param name="value5">Bits 32-39 of the UInt128.</param>
        /// <param name="value6">Bits 40-47 of the UInt128.</param>
        /// <param name="value7">Bits 48-55 of the UInt128.</param>
        /// <param name="value8">Bits 56-63 of the UInt128.</param>
        /// <param name="value9">Bits 64-71 of the UInt128.</param>
        /// <param name="value10">Bits 72-79 of the UInt128.</param>
        /// <param name="value11">Bits 80-87 of the UInt128.</param>
        /// <param name="value12">Bits 88-95 of the UInt128.</param>
        /// <param name="value13">Bits 96-103 of the UInt128.</param>
        /// <param name="value14">Bits 104-111 of the UInt128.</param>
        /// <param name="value15">Bits 112-119 of the UInt128.</param>
        /// <param name="value16">Bits 120-127 of the UInt128.</param>
        /// <returns>A UInt128 whose bits are determined by the input.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1025:ReplaceRepetitiveArgumentsWithParamsArray")]
        public static UInt128 Merge(byte value1, byte value2, byte value3, byte value4, byte value5, byte value6, byte value7, byte value8,
                                    byte value9, byte value10, byte value11, byte value12, byte value13, byte value14, byte value15, byte value16)
        {
            return Merge(Merge(value1, value2, value3, value4, value5, value6, value7, value8),
                         Merge(value9, value10, value11, value12, value13, value14, value15, value16));
        }

        /// <summary>
        /// Merges 2 ushort to a uint.
        /// ushorts should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bits 0-15 of the uint.</param>
        /// <param name="value2">Bits 16-31 of the uint.</param>
        /// <returns>A uint whose bits are determined by the input.</returns>
        public static uint Merge(ushort value1, ushort value2)
        {
            return (uint)((value1 << 16) | value2);
        }

        /// <summary>
        /// Merges a byte and a ushort to a UInt24.
        /// Values should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bits 0-7 of the UInt24.</param>
        /// <param name="value2">Bits 8-23 of the UInt24.</param>
        /// <returns>A UInt24 whose bits are determined by the input.</returns>
        public static UInt24 Merge(byte value1, ushort value2)
        {
            return (UInt24)Merge(0, value1, value2);
        }

        /// <summary>
        /// Merges a ushort and 2 bytes to a uint.
        /// Values should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bits 0-15 of the uint.</param>
        /// <param name="value2">Bits 16-23 of the uint.</param>
        /// <param name="value3">Bits 24-31 of the uint.</param>
        /// <returns>A uint whose bits are determined by the input.</returns>
        public static uint Merge(ushort value1, byte value2, byte value3)
        {
            return Merge(value1, Merge(value2, value3));
        }

        /// <summary>
        /// Merges a ushort and 2 bytes to a uint.
        /// Values should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bits 0-7 of the uint.</param>
        /// <param name="value2">Bits 8-23 of the uint.</param>
        /// <param name="value3">Bits 24-31 of the uint.</param>
        /// <returns>A uint whose bits are determined by the input.</returns>
        public static uint Merge(byte value1, ushort value2, byte value3)
        {
            return (uint)((value1 << 24) | (value2 << 8) | value3);
        }

        /// <summary>
        /// Merges a ushort and 2 bytes to a uint.
        /// Values should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bits 0-7 of the uint.</param>
        /// <param name="value2">Bits 8-15 of the uint.</param>
        /// <param name="value3">Bits 16-31 of the uint.</param>
        /// <returns>A uint whose bits are determined by the input.</returns>
        public static uint Merge(byte value1, byte value2, ushort value3)
        {
            return Merge(Merge(value1, value2), value3);
        }

        /// <summary>
        /// Merges 2 uints to a ulong.
        /// uints should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bits 0-31 of the ulong.</param>
        /// <param name="value2">Bits 32-63 of the ulong.</param>
        /// <returns>A ulong whose bits are determined by the input.</returns>
        public static ulong Merge(uint value1, uint value2)
        {
            return (((ulong)value1) << 32) | value2;
        }

        /// <summary>
        /// Merges 2 ulongs to a UInt128.
        /// ulongs should be arranged from the most significant to the least.
        /// </summary>
        /// <param name="value1">Bits 0-63 of the UInt128.</param>
        /// <param name="value2">Bits 64-127 of the UInt128.</param>
        /// <returns>A UInt128 whose bits are determined by the input.</returns>
        public static UInt128 Merge(ulong value1, ulong value2)
        {
            return new UInt128(value1, value2);
        }
    }
}