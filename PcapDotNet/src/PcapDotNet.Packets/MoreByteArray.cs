using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets
{
    /// <summary>
    /// Extension methods for byte[].
    /// </summary>
    public static class MoreByteArray
    {
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
                value = IPAddress.HostToNetworkOrder(value);
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
        /// Writes the given value to the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        public static void Write(this byte[] buffer, int offset, byte value)
        {
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
            Write(buffer, offset, value, endianity);
            offset += UInt48.SizeOf;
        }

        /// <summary>
        /// Writes the given value to the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        public static void Write(this byte[] buffer, int offset, Datagram value)
        {
            value.Write(buffer, offset);
        }

        /// <summary>
        /// Writes the given value to the buffer and increments the offset by the number of bytes written.
        /// </summary>
        /// <param name="buffer">The buffer to write the value to.</param>
        /// <param name="offset">The offset in the buffer to start writing.</param>
        /// <param name="value">The value to write.</param>
        public static void Write(this byte[] buffer, ref int offset, Datagram value)
        {
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
    }
}