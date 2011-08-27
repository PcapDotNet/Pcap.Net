using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets
{
    public class DataSegment : IEquatable<DataSegment>, IEnumerable<byte>
    {
        /// <summary>
        /// Take all the bytes as a segment.
        /// </summary>
        /// <param name="buffer">The buffer to take as a segment.</param>
        public DataSegment(byte[] buffer)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");

            Buffer = buffer;
            StartOffset = 0;
            Length = buffer.Length;
        }

        /// <summary>
        /// Take only part of the bytes as a segment.
        /// </summary>
        /// <param name="buffer">The bytes to take the segment from.</param>
        /// <param name="offset">The offset in the buffer to start taking the bytes from.</param>
        /// <param name="length">The number of bytes to take.</param>
        public DataSegment(byte[] buffer, int offset, int length)
        {
            Buffer = buffer;
            StartOffset = offset;
            Length = length;
        }

        /// <summary>
        /// The number of bytes in this segment.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The value of the byte in the given offset in the segment.
        /// </summary>
        /// <param name="offset">The offset in the segment to take the byte from.</param>
        public byte this[int offset]
        {
            get { return Buffer[StartOffset + offset]; }
        }

        /// <summary>
        /// Returns the Segment's bytes as a read only MemoryStream with a non-public buffer.
        /// </summary>
        /// <returns>A read only MemoryStream containing the bytes of the segment.</returns>
        public MemoryStream ToMemoryStream()
        {
            return new MemoryStream(Buffer, StartOffset, Length, false, false);
        }

        /// <summary>
        /// Iterate through all the bytes in the segment.
        /// </summary>
        public IEnumerator<byte> GetEnumerator()
        {
            for (int i = 0; i != Length; ++i)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Two segments are equal if they have the same data.
        /// </summary>
        public bool Equals(DataSegment other)
        {
            if (other == null || Length != other.Length)
                return false;

            for (int i = 0; i != Length; ++i)
            {
                if (this[i] != other[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Two segments are equal if they have the same data.
        /// </summary>
        public sealed override bool Equals(object obj)
        {
            return Equals(obj as DataSegment);
        }

        /// <summary>
        /// The hash code of a segment is the hash code of its length xored with all its bytes (each byte is xored with the next byte in the integer).
        /// </summary>
        public sealed override int GetHashCode()
        {
            return Length.GetHashCode() ^ this.BytesSequenceGetHashCode();
        }

        /// <summary>
        /// Converts the segment to a hexadecimal string representing every bytes as two hexadecimal digits.
        /// </summary>
        /// <returns>A hexadecimal string representing every bytes as two hexadecimal digits.</returns>
        public sealed override string ToString()
        {
            return Buffer.Range(StartOffset, Length).BytesSequenceToHexadecimalString();
        }

        /// <summary>
        /// Converts the segment to a string using the given encoding.
        /// </summary>
        /// <param name="encoding">The encoding to use to convert the bytes sequence in the segment to a string.</param>
        /// <returns>A string of the bytes in the segment converted using the given encoding.</returns>
        public string ToString(Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            return encoding.GetString(Buffer, StartOffset, Length);
        }

        internal void Write(byte[] buffer, int offset)
        {
            Buffer.BlockCopy(StartOffset, buffer, offset, Length);
        }

        /// <summary>
        /// The original buffer that holds all the data for the segment.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        internal byte[] Buffer { get; private set; }

        /// <summary>
        /// The offset of the first byte of the segment in the buffer.
        /// </summary>
        internal int StartOffset { get; private set; }

        /// <summary>
        /// Reads a requested number of bytes from a specific offset in the segment.
        /// </summary>
        /// <param name="offset">The offset in the segment to start reading.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The bytes read from the segment starting from the given offset and in the given length.</returns>
        protected byte[] ReadBytes(int offset, int length)
        {
            return Buffer.ReadBytes(StartOffset + offset, length);
        }

        internal DataSegment SubSegment(int offset, int length)
        {
            return new DataSegment(Buffer, StartOffset + offset, length);
        }

        internal bool ReadBool(int offset, byte mask)
        {
            return (this[offset] & mask) == mask;
        }

        /// <summary>
        /// Reads 2 bytes from a specific offset in the segment as a ushort with a given endianity.
        /// </summary>
        /// <param name="offset">The offset in the segment to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "ushort")]
        internal ushort ReadUShort(int offset, Endianity endianity)
        {
            return Buffer.ReadUShort(StartOffset + offset, endianity);
        }

        /// <summary>
        /// Reads 4 bytes from a specific offset in the segment as an int with a given endianity.
        /// </summary>
        /// <param name="offset">The offset in the segment to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int")]
        internal int ReadInt(int offset, Endianity endianity)
        {
            return Buffer.ReadInt(StartOffset + offset, endianity);
        }

        /// <summary>
        /// Reads 4 bytes from a specific offset in the segment as a uint with a given endianity.
        /// </summary>
        /// <param name="offset">The offset in the segment to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "uint")]
        protected uint ReadUInt(int offset, Endianity endianity)
        {
            return Buffer.ReadUInt(StartOffset + offset, endianity);
        }

        /// <summary>
        /// Reads 6 bytes from a specific offset in the segment as a MacAddress with a given endianity.
        /// </summary>
        /// <param name="offset">The offset in the segment to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        protected MacAddress ReadMacAddress(int offset, Endianity endianity)
        {
            return Buffer.ReadMacAddress(StartOffset + offset, endianity);
        }

        /// <summary>
        /// Reads 4 bytes from a specific offset in the segment as an IpV4Address with a given endianity.
        /// </summary>
        /// <param name="offset">The offset in the segment to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        internal IpV4Address ReadIpV4Address(int offset, Endianity endianity)
        {
            return Buffer.ReadIpV4Address(StartOffset + offset, endianity);
        }

        /// <summary>
        /// Reads 4 bytes from a specific offset in the segment as an IpV4TimeOfDay with a given endianity.
        /// </summary>
        /// <param name="offset">The offset in the segment to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        protected IpV4TimeOfDay ReadIpV4TimeOfDay(int offset, Endianity endianity)
        {
            return Buffer.ReadIpV4TimeOfDay(StartOffset + offset, endianity);
        }

        /// <summary>
        /// Converts the given 16 bits sum to a checksum.
        /// Sums the two 16 bits in the 32 bits value and if the result is bigger than a 16 bits value repeat.
        /// The result is one's complemented and the least significant 16 bits are taken.
        /// </summary>
        /// <param name="sum"></param>
        /// <returns></returns>
        protected static ushort Sum16BitsToChecksum(uint sum)
        {
            // Take only 16 bits out of the 32 bit sum and add up the carrier.
            // if the results overflows - do it again.
            while (sum > 0xFFFF)
                sum = (sum & 0xFFFF) + (sum >> 16);

            // one's complement the result
            sum = ~sum;

            return (ushort)sum;
        }

        /// <summary>
        /// Sums bytes in a buffer as 16 bits big endian values.
        /// If the number of bytes is odd then a 0x00 value is assumed after the last byte.
        /// Used to calculate checksum.
        /// </summary>
        /// <param name="buffer">The buffer to take the bytes from.</param>
        /// <param name="offset">The offset in the buffer to start reading the bytes.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>A value equals to the sum of all 16 bits big endian values of the given bytes.</returns>
        protected static uint Sum16Bits(byte[] buffer, int offset, int length)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");

            int endOffset = offset + length;
            uint sum = 0;
            while (offset < endOffset - 1)
                sum += buffer.ReadUShort(ref offset, Endianity.Big);
            if (offset < endOffset)
                sum += (ushort)(buffer[offset] << 8);
            return sum;
        }
    }
}