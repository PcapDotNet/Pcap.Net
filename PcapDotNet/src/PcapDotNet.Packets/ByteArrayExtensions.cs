using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Http;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;

namespace PcapDotNet.Packets
{
    /// <summary>
    /// Extension methods for byte[].
    /// </summary>
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Compares all the bytes in the two ranges of the arrays.
        /// Returns the first non-zero compare value of the bytes in the ranges or zero if the ranges have the same byte values.
        /// </summary>
        /// <param name="array">The first array to compare.</param>
        /// <param name="offset">The offset of the first byte to compare in the first array.</param>
        /// <param name="other">The second array to compare.</param>
        /// <param name="otherOffset">The offset of the first byte to compare in the second array.</param>
        /// <param name="count">The number of bytes to compare.</param>
        /// <returns>The first non-zero compare value of the bytes in the ranges or zero if the ranges have the same byte values.</returns>
        public static int Compare(this byte[] array, int offset, byte[] other, int otherOffset, int count)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (other == null)
                throw new ArgumentNullException("other");

            for (int i = 0; i != count; ++i)
            {
                int compare = array[offset + i].CompareTo(other[otherOffset + i]);
                if (compare != 0)
                    return compare;
            }
            return 0;
        }

        /// <summary>
        /// Compares all the bytes in the two ranges of the arrays.
        /// Returns true iff the ranges have the same byte values.
        /// </summary>
        /// <param name="array">The first array to compare.</param>
        /// <param name="offset">The offset of the first byte to compare in the first array.</param>
        /// <param name="other">The second array to compare.</param>
        /// <param name="otherOffset">The offset of the first byte to compare in the second array.</param>
        /// <param name="count">The number of bytes to compare.</param>
        /// <returns>True iff the ranges have the same byte values.</returns>
        public static bool SequenceEqual(this byte[] array, int offset, byte[] other, int otherOffset, int count)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            return array.Compare(offset, other, otherOffset, count) == 0;
        }

        public static DataSegment SubSegment(this byte[] array, int offset, int length)
        {
            return new DataSegment(array, offset, length);
        }

        /// <summary>
        /// Returns the first offset in the array where the other array's range sequence of bytes can be found or the length of the array if no match exists.
        /// </summary>
        /// <param name="array">The array to search for the sequence of bytes.</param>
        /// <param name="offset">The offset of the first byte in the array that should be compared to the sequence to find.</param>
        /// <param name="count">The number of bytes in the array that the sequence can be searched in.</param>
        /// <param name="other">The array that contains the sequence of bytes to search.</param>
        /// <param name="otherOffset">The offset in the array containing the sequence of the first byte of the sequence.</param>
        /// <param name="otherCount">The number of bytes of the sequence.</param>
        /// <returns>The first offset in the array where the other array's range sequence of bytes can be found or the length of the array if no match exists.</returns>
        public static int Find(this byte[] array, int offset, int count, byte[] other, int otherOffset, int otherCount)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (otherCount > count)
                return array.Length;

            int maxOffset = offset + count - otherCount;
            while (offset < maxOffset)
            {
                if (array.SequenceEqual(offset, other, otherOffset, otherCount))
                    return offset;
                ++offset;
            }

            return array.Length;
        }

        /// <summary>
        /// Returns the first offset in the array where the other array sequence of bytes can be found or the length of the array if no match exists.
        /// </summary>
        /// <param name="array">The array to search for the sequence of bytes.</param>
        /// <param name="offset">The offset of the first byte in the array that should be compared to the sequence to find.</param>
        /// <param name="count">The number of bytes in the array that the sequence can be searched in.</param>
        /// <param name="other">The array that contains the sequence of bytes to search.</param>
        /// <returns>The first offset in the array where the other array sequence of bytes can be found or the length of the array if no match exists.</returns>
        public static int Find(this byte[] array, int offset, int count, byte[] other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            return array.Find(offset, count, other, 0, other.Length);
        }

        /// <summary>
        /// Copies a specified number of bytes from a source array starting at a particular offset to a destination array starting at a particular offset.
        /// </summary>
        /// <param name="source">The source buffer.</param>
        /// <param name="sourceOffset">The byte offset into source.</param>
        /// <param name="destination">The destination buffer.</param>
        /// <param name="destinationOffset">The byte offset into destination.</param>
        /// <param name="count">The number of bytes to copy.</param>
        public static void BlockCopy(this byte[] source, int sourceOffset, byte[] destination, int destinationOffset, int count)
        {
            Buffer.BlockCopy(source, sourceOffset, destination, destinationOffset, count);
        }

        /// <summary>
        /// Reads a byte from a specific offset.
        /// </summary>
        /// <param name="buffer">The buffer to read the byte from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <returns>The value read from the buffer.</returns>
        public static byte ReadByte(this byte[] buffer, int offset)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");
            return buffer[offset];
        }

        /// <summary>
        /// Reads a byte from a specific offset and increments the offset by 1.
        /// </summary>
        /// <param name="buffer">The buffer to read the byte from.</param>
        /// <param name="offset">The offset in the buffer to start reading and to increment.</param>
        /// <returns>The value read from the buffer.</returns>
        public static byte ReadByte(this byte[] buffer, ref int offset)
        {
            byte result = ReadByte(buffer, offset);
            offset += sizeof(byte);
            return result;
        }

        /// <summary>
        /// Reads bytes from a specific offset.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The value read from the buffer.</returns>
        public static byte[] ReadBytes(this byte[] buffer, int offset, int length)
        {
            byte[] bytes = new byte[length];
            buffer.BlockCopy(offset, bytes, 0, length);
            return bytes;
        }

        /// <summary>
        /// Reads bytes from a specific offset and increments the offset by the number of bytes read.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading and to increment.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The value read from the buffer.</returns>
        public static byte[] ReadBytes(this byte[] buffer, ref int offset, int length)
        {
            byte[] result = buffer.ReadBytes(offset, length);
            offset += length;
            return result;
        }

        /// <summary>
        /// Reads 2 bytes from a specific offset as a short with a given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "short")]
        public static short ReadShort(this byte[] buffer, int offset, Endianity endianity)
        {
            short value = ReadShort(buffer, offset);
            if (IsWrongEndianity(endianity))
                value = value.ReverseEndianity();
            return value;
        }

        /// <summary>
        /// Reads 2 bytes from a specific offset as a ushort with a given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "ushort")]
        public static ushort ReadUShort(this byte[] buffer, int offset, Endianity endianity)
        {
            return (ushort)ReadShort(buffer, offset, endianity);
        }

        /// <summary>
        /// Reads 2 bytes from a specific offset as a ushort with a given endianity and increments the offset by the number of bytes read.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "ushort")]
        public static ushort ReadUShort(this byte[] buffer, ref int offset, Endianity endianity)
        {
            ushort result = ReadUShort(buffer, offset, endianity);
            offset += sizeof(ushort);
            return result;
        }

        /// <summary>
        /// Reads 3 bytes from a specific offset as a UInt24 with a given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        public static UInt24 ReadUInt24(this byte[] buffer, int offset, Endianity endianity)
        {
            UInt24 value = ReadUInt24(buffer, offset);
            if (IsWrongEndianity(endianity))
                value = HostToNetworkOrder(value);
            return value;
        }

        /// <summary>
        /// Reads 3 bytes from a specific offset as a UInt24 with a given endianity and increments the offset by the number of bytes read.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        public static UInt24 ReadUInt24(this byte[] buffer, ref int offset, Endianity endianity)
        {
            UInt24 result = ReadUInt24(buffer, offset, endianity);
            offset += UInt24.SizeOf;
            return result;
        }

        /// <summary>
        /// Reads 4 bytes from a specific offset as an int with a given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int")]
        public static int ReadInt(this byte[] buffer, int offset, Endianity endianity)
        {
            int value = ReadInt(buffer, offset);
            if (IsWrongEndianity(endianity))
                value = IPAddress.HostToNetworkOrder(value);
            return value;
        }

        /// <summary>
        /// Reads 4 bytes from a specific offset as a uint with a given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "uint")]
        public static uint ReadUInt(this byte[] buffer, int offset, Endianity endianity)
        {
            return (uint)ReadInt(buffer, offset, endianity);
        }

        /// <summary>
        /// Reads 4 bytes from a specific offset as a uint with a given endianity and increments the offset by the number of bytes read.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "uint")]
        public static uint ReadUInt(this byte[] buffer, ref int offset, Endianity endianity)
        {
            uint result = ReadUInt(buffer, offset, endianity);
            offset += sizeof(int);
            return result;
        }

        /// <summary>
        /// Reads 6 bytes from a specific offset as a UInt48 with a given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        public static UInt48 ReadUInt48(this byte[] buffer, int offset, Endianity endianity)
        {
            UInt48 value = ReadUInt48(buffer, offset);
            if (IsWrongEndianity(endianity))
                value = HostToNetworkOrder(value);
            return value;
        }

        /// <summary>
        /// Reads 6 bytes from a specific offset as a UInt48 with a given endianity and increments the offset by the number of bytes read.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        public static UInt48 ReadUInt48(this byte[] buffer, ref int offset, Endianity endianity)
        {
            UInt48 result = buffer.ReadUInt48(offset, endianity);
            offset += UInt48.SizeOf;
            return result;
        }

        /// <summary>
        /// Reads 8 bytes from a specific offset as a long with a given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "long")]
        public static long ReadLong(this byte[] buffer, int offset, Endianity endianity)
        {
            long value = ReadLong(buffer, offset);
            if (IsWrongEndianity(endianity))
                value = IPAddress.HostToNetworkOrder(value);
            return value;
        }

        /// <summary>
        /// Reads 8 bytes from a specific offset as a ulong with a given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "ulong")]
        public static ulong ReadULong(this byte[] buffer, int offset, Endianity endianity)
        {
            return (ulong)buffer.ReadLong(offset, endianity);
        }

        /// <summary>
        /// Reads 16 bytes from a specific offset as a UInt128 with a given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        public static UInt128 ReadUInt128(this byte[] buffer, int offset, Endianity endianity)
        {
            UInt128 value = ReadUInt128(buffer, offset);
            if (IsWrongEndianity(endianity))
                value = HostToNetworkOrder(value);
            return value;
        }

        /// <summary>
        /// Reads a given amount of bytes from a specific offset as an unsigned BigInteger with a given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        public static BigInteger ReadUnsignedBigInteger(this byte[] buffer, int offset, int length, Endianity endianity)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");

            BigInteger value = BigInteger.Zero;
            for (int i = 0; i != length; ++i)
            {
                value <<= 8;
                if (endianity == Endianity.Big)
                    value += buffer[offset + i];
                else
                    value += buffer[offset + length - i - 1];
            }
            return value;
        }

        /// <summary>
        /// Reads 6 bytes from a specific offset as a MacAddress with a given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        public static MacAddress ReadMacAddress(this byte[] buffer, int offset, Endianity endianity)
        {
            return new MacAddress(buffer.ReadUInt48(offset, endianity));
        }

        /// <summary>
        /// Reads 6 bytes from a specific offset as a MacAddress with a given endianity and increments the offset by the number of bytes read.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        public static MacAddress ReadMacAddress(this byte[] buffer, ref int offset, Endianity endianity)
        {
            return new MacAddress(buffer.ReadUInt48(ref offset, endianity));
        }

        /// <summary>
        /// Reads 4 bytes from a specific offset as an IPv4 address with a given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        public static IpV4Address ReadIpV4Address(this byte[] buffer, int offset, Endianity endianity)
        {
            return new IpV4Address(buffer.ReadUInt(offset, endianity));
        }

        /// <summary>
        /// Reads 4 bytes from a specific offset as an IPv4 address with a given endianity and increments the offset by the number of bytes read.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        public static IpV4Address ReadIpV4Address(this byte[] buffer, ref int offset, Endianity endianity)
        {
            return new IpV4Address(buffer.ReadUInt(ref offset, endianity));
        }

        /// <summary>
        /// Reads 4 bytes from a specific offset as an IPv4 time of day with a given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        public static IpV4TimeOfDay ReadIpV4TimeOfDay(this byte[] buffer, int offset, Endianity endianity)
        {
            return new IpV4TimeOfDay(buffer.ReadUInt(offset, endianity));
        }

        /// <summary>
        /// Reads 4 bytes from a specific offset as an IPv4 time of day with a given endianity and increments the offset by the number of bytes read.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        public static IpV4TimeOfDay ReadIpV4TimeOfDay(this byte[] buffer, ref int offset, Endianity endianity)
        {
            return new IpV4TimeOfDay(buffer.ReadUInt(ref offset, endianity));
        }

        /// <summary>
        /// Reads 16 bytes from a specific offset as an IPv6 address with a given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to read the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        public static IpV6Address ReadIpV6Address(this byte[] buffer, int offset, Endianity endianity)
        {
            return new IpV6Address(buffer.ReadUInt128(offset, endianity));
        }

        /// <summary>
        /// Writes the given value to the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        public static void Write(this byte[] buffer, int offset, byte value)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");

            buffer[offset] = value;
        }

        /// <summary>
        /// Writes the given value to the buffer and increments the offset by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        public static void Write(this byte[] buffer, ref int offset, byte value)
        {
            Write(buffer, offset, value);
            offset += sizeof(byte);
        }

        /// <summary>
        /// Writes the given value to the buffer and increments the offset by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        public static void Write(this byte[] buffer, ref int offset, IEnumerable<byte> value)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");
            if (value == null) 
                throw new ArgumentNullException("value");

            foreach (byte b in value)
                buffer.Write(offset++, b);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, int offset, short value, Endianity endianity)
        {
            if (IsWrongEndianity(endianity))
                value = value.ReverseEndianity();
            Write(buffer, offset, value);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, int offset, ushort value, Endianity endianity)
        {
            Write(buffer, offset, (short)value, endianity);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity and increments the offset by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, ref int offset, ushort value, Endianity endianity)
        {
            Write(buffer, offset, value, endianity);
            offset += sizeof(ushort);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, int offset, UInt24 value, Endianity endianity)
        {
            if (IsWrongEndianity(endianity))
                value = HostToNetworkOrder(value);
            Write(buffer, offset, value);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity and increments the offset by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, ref int offset, UInt24 value, Endianity endianity)
        {
            Write(buffer, offset, value, endianity);
            offset += UInt24.SizeOf;
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, int offset, int value, Endianity endianity)
        {
            if (IsWrongEndianity(endianity))
                value = IPAddress.HostToNetworkOrder(value);
            Write(buffer, offset, value);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity and increments the offset by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, ref int offset, int value, Endianity endianity)
        {
            Write(buffer, offset, value, endianity);
            offset += sizeof(int);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, int offset, uint value, Endianity endianity)
        {
            Write(buffer, offset, (int)value, endianity);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity and increments the offset by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, ref int offset, uint value, Endianity endianity)
        {
            Write(buffer, offset, value, endianity);
            offset += sizeof(uint);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, int offset, UInt48 value, Endianity endianity)
        {
            if (IsWrongEndianity(endianity))
                value = HostToNetworkOrder(value);
            Write(buffer, offset, value);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity and increments the offset by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, ref int offset, UInt48 value, Endianity endianity)
        {
            buffer.Write(offset, value, endianity);
            offset += UInt48.SizeOf;
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, int offset, long value, Endianity endianity)
        {
            if (IsWrongEndianity(endianity))
                value = IPAddress.HostToNetworkOrder(value);
            Write(buffer, offset, value);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, int offset, ulong value, Endianity endianity)
        {
            buffer.Write(offset, (long)value, endianity);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity and increments the offset by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, ref int offset, ulong value, Endianity endianity)
        {
            buffer.Write(offset, value, endianity);
            offset += sizeof(ulong);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, int offset, UInt128 value, Endianity endianity)
        {
            if (IsWrongEndianity(endianity))
                value = HostToNetworkOrder(value);
            Write(buffer, offset, value);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity and increments the offset by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, ref int offset, UInt128 value, Endianity endianity)
        {
            buffer.Write(offset, value, endianity);
            offset += UInt128.SizeOf;
        }

        /// <summary>
        /// Writes the given amount of least significant bytes of the value to the buffer using the given endianity.
        /// Doesn't write leading zero bytes.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="length">The maximum amount of bytes to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void WriteUnsigned(this byte[] buffer, int offset, BigInteger value, int length, Endianity endianity)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");
            
            if (value.Sign < 0)
                throw new ArgumentOutOfRangeException("value", value, "Must be non-negative.");
            for (int i = 0; i != length && value != BigInteger.Zero; ++i, value >>= 8)
            {
                byte byteValue = (byte)(value & 0xFF);
                if (endianity == Endianity.Small)
                    buffer[offset + i] = byteValue;
                else
                    buffer[offset + length - i - 1] = byteValue;
            }
        }

        /// <summary>
        /// Writes a string to a byte array in a specific offset using the given encoding.
        /// Increments the offset by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write the string in.</param>
        /// <param name="offset">The offset in the buffer to start writing the string in. Incremented by the number of bytes written.</param>
        /// <param name="value">The string to write in the buffer.</param>
        /// <param name="encoding">The encoding to use to translate the string into a sequence of bytes.</param>
        public static void Write(this byte[] buffer, ref int offset, string value, Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            buffer.Write(ref offset, encoding.GetBytes(value));
        }

        /// <summary>
        /// Writes the given value to the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        public static void Write(this byte[] buffer, int offset, DataSegment value)
        {
            buffer.Write(ref offset, value);
        }

        /// <summary>
        /// Writes the given value to the buffer and increments the offset by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        public static void Write(this byte[] buffer, ref int offset, DataSegment value)
        {
            if (value == null) 
                throw new ArgumentNullException("value");

            value.Write(buffer, offset);
            offset += value.Length;
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, int offset, MacAddress value, Endianity endianity)
        {
            buffer.Write(offset, value.ToValue(), endianity);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity and increments the offset by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, ref int offset, MacAddress value, Endianity endianity)
        {
            buffer.Write(ref offset, value.ToValue(), endianity);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, int offset, IpV4Address value, Endianity endianity)
        {
            buffer.Write(offset, value.ToValue(), endianity);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity and increments the offset by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, ref int offset, IpV4Address value, Endianity endianity)
        {
            buffer.Write(ref offset, value.ToValue(), endianity);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, int offset, IpV4TimeOfDay value, Endianity endianity)
        {
            buffer.Write(offset, value.MillisecondsSinceMidnightUniversalTime, endianity);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity and increments the offset by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, ref int offset, IpV4TimeOfDay value, Endianity endianity)
        {
            buffer.Write(ref offset, value.MillisecondsSinceMidnightUniversalTime, endianity);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, int offset, IpV6Address value, Endianity endianity)
        {
            buffer.Write(offset, value.ToValue(), endianity);
        }

        /// <summary>
        /// Writes the given value to the buffer using the given endianity and increments the offset by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="endianity">The endianity to use when converting the value to bytes.</param>
        public static void Write(this byte[] buffer, ref int offset, IpV6Address value, Endianity endianity)
        {
            buffer.Write(ref offset, value.ToValue(), endianity);
        }

//        public static void WriteCarriageReturnLinefeed(this byte[] buffer, int offset)
//        {
//            buffer.Write(ref offset, AsciiBytes.CarriageReturn);
//            buffer.Write(offset, AsciiBytes.LineFeed);
//        }
//
        /// <summary>
        /// Writes the endline bytes (CRLF) in the buffer in the given offset.
        /// Increments the offset by the number of bytes written (2).
        /// </summary>
        /// <param name="buffer">The buffer to write the CRLF in.</param>
        /// <param name="offset">The offset to start writing the CRLF in. Incremented by the number of bytes written (2).</param>
        public static void WriteCarriageReturnLinefeed(this byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, AsciiBytes.CarriageReturn);
            buffer.Write(ref offset, AsciiBytes.Linefeed);
        }

        /// <summary>
        /// Writes an integer as a decimal string in ASCII encoding to a buffer of bytes in a specific offset.
        /// The offset is incremented by the number of bytes (digits) written.
        /// </summary>
        /// <param name="buffer">The buffer to write the integer in.</param>
        /// <param name="offset">The offset in the buffer to start writing the integer. Incremented by the number of bytes (digits) written.</param>
        /// <param name="value">The integer value to write in the buffer.</param>
        public static void WriteDecimal(this byte[] buffer, ref int offset, uint value)
        {
            buffer.Write(ref offset, value.ToString(CultureInfo.InvariantCulture), Encoding.ASCII);
        }

        private static bool IsWrongEndianity(Endianity endianity)
        {
            return (BitConverter.IsLittleEndian == (endianity == Endianity.Big));
        }

        private static UInt24 HostToNetworkOrder(UInt24 value)
        {
            UInt24 result;

            unsafe
            {
                UInt24* resultPtr = &result;
                byte* resultBytePtr = (byte*)resultPtr;

                UInt24* valuePtr = &value;
                byte* valueBytePtr = (byte*)valuePtr;

                resultBytePtr[0] = valueBytePtr[2];
                resultBytePtr[1] = valueBytePtr[1];
                resultBytePtr[2] = valueBytePtr[0];
            }

            return result;
        }

        private static UInt48 HostToNetworkOrder(UInt48 value)
        {
            UInt48 result;

            unsafe
            {
                UInt48* resultPtr = &result;
                byte* resultBytePtr = (byte*)resultPtr;
                
                UInt48* valuePtr = &value;
                byte* valueBytePtr = (byte*)valuePtr;

                resultBytePtr[0] = valueBytePtr[5];
                resultBytePtr[1] = valueBytePtr[4];
                resultBytePtr[2] = valueBytePtr[3];
                resultBytePtr[3] = valueBytePtr[2];
                resultBytePtr[4] = valueBytePtr[1];
                resultBytePtr[5] = valueBytePtr[0];
            }

            return result;
        }

        private static UInt128 HostToNetworkOrder(UInt128 value)
        {
            UInt128 result;

            unsafe
            {
                UInt128* resultPtr = &result;
                byte* resultBytePtr = (byte*)resultPtr;

                UInt128* valuePtr = &value;
                byte* valueBytePtr = (byte*)valuePtr;

                for (int i = 0; i != UInt128.SizeOf; ++i)
                    resultBytePtr[i] = valueBytePtr[UInt128.SizeOf - i - 1];
            }

            return result;
        }

        private static short ReadShort(byte[] buffer, int offset)
        {
            unsafe
            {
                fixed (byte* ptr = &buffer[offset])
                {
                    return *((short*)ptr);
                }
            }
        }

        private static UInt24 ReadUInt24(byte[] buffer, int offset)
        {
            unsafe
            {
                fixed (byte* ptr = &buffer[offset])
                {
                    return *((UInt24*)ptr);
                }
            }
        }

        private static int ReadInt(byte[] buffer, int offset)
        {
            unsafe
            {
                fixed (byte* ptr = &buffer[offset])
                {
                    return *((int*)ptr);
                }
            }
        }

        private static UInt48 ReadUInt48(byte[] buffer, int offset)
        {
            unsafe
            {
                fixed (byte* ptr = &buffer[offset])
                {
                    return *((UInt48*)ptr);
                }
            }
        }

        private static long ReadLong(byte[] buffer, int offset)
        {
            unsafe
            {
                fixed (byte* ptr = &buffer[offset])
                {
                    return *((long*)ptr);
                }
            }
        }

        private static UInt128 ReadUInt128(byte[] buffer, int offset)
        {
            unsafe
            {
                fixed (byte* ptr = &buffer[offset])
                {
                    return *((UInt128*)ptr);
                }
            }
        }

        private static void Write(byte[] buffer, int offset, short value)
        {
            unsafe
            {
                fixed (byte* ptr = &buffer[offset])
                {
                    *((short*)ptr) = value;
                }
            }
        }

        private static void Write(byte[] buffer, int offset, UInt24 value)
        {
            unsafe
            {
                fixed (byte* ptr = &buffer[offset])
                {
                    *((UInt24*)ptr) = value;
                }
            }
        }
        
        private static void Write(byte[] buffer, int offset, int value)
        {
            unsafe
            {
                fixed (byte* ptr = &buffer[offset])
                {
                    *((int*)ptr) = value;
                }
            }
        }

        private static void Write(byte[] buffer, int offset, UInt48 value)
        {
            unsafe
            {
                fixed (byte* ptr = &buffer[offset])
                {
                    *((UInt48*)ptr) = value;
                }
            }
        }

        private static void Write(byte[] buffer, int offset, long value)
        {
            unsafe
            {
                fixed (byte* ptr = &buffer[offset])
                {
                    *((long*)ptr) = value;
                }
            }
        }

        private static void Write(byte[] buffer, int offset, UInt128 value)
        {
            unsafe
            {
                fixed (byte* ptr = &buffer[offset])
                {
                    *((UInt128*)ptr) = value;
                }
            }
        }
    }
}