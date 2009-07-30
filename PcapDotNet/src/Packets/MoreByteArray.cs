using System;
using System.Net;

namespace Packets
{
    public enum Endianity : byte
    {
        Small,
        Big
    }

    public static class MoreByteArray
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "short")]
        public static short ReadShort(this byte[] buffer, int offset, Endianity endianity)
        {
            short value = ReadShort(buffer, offset);
            if (IsWrongEndianity(endianity))
                value = IPAddress.HostToNetworkOrder(value);
            return value;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "ushort")]
        public static ushort ReadUShort(this byte[] buffer, int offset, Endianity endianity)
        {
            return (ushort)ReadShort(buffer, offset, endianity);
        }

        public static ushort ReadUShort(this byte[] buffer, ref int offset, Endianity endianity)
        {
            ushort result = ReadUShort(buffer, offset, endianity);
            offset += 2;
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "uint")]
        public static uint ReadUInt(this byte[] buffer, int offset, Endianity endianity)
        {
            return (uint)ReadInt(buffer, offset, endianity);
        }

        public static uint ReadUInt(this byte[] buffer, ref int offset, Endianity endianity)
        {
            uint result = ReadUInt(buffer, offset, endianity);
            offset += 4;
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int")]
        public static int ReadInt(this byte[] buffer, int offset, Endianity endianity)
        {
            int value = ReadInt(buffer, offset);
            if (IsWrongEndianity(endianity))
                value = IPAddress.HostToNetworkOrder(value);
            return value;
        }

        public static void Write(this byte[] buffer, int offset, short value, Endianity endianity)
        {
            if (IsWrongEndianity(endianity))
                value = IPAddress.HostToNetworkOrder(value);
            Write(buffer, offset, value);
        }

        public static void Write(this byte[] buffer, int offset, ushort value, Endianity endianity)
        {
            Write(buffer, offset, (short)value, endianity);
        }

        public static void Write(this byte[] buffer, int offset, int value, Endianity endianity)
        {
            if (IsWrongEndianity(endianity))
                value = IPAddress.HostToNetworkOrder(value);
            Write(buffer, offset, value);
        }

        public static void Write(this byte[] buffer, int offset, uint value, Endianity endianity)
        {
            Write(buffer, offset, (int)value, endianity);
        }

        private static bool IsWrongEndianity(Endianity endianity)
        {
            return (BitConverter.IsLittleEndian == (endianity == Endianity.Big));
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
    }
}