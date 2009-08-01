using System;
using System.Net;
using PcapDotNet.Base;

namespace PcapDotNet.Packets
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "ushort")]
        public static ushort ReadUShort(this byte[] buffer, ref int offset, Endianity endianity)
        {
            ushort result = ReadUShort(buffer, offset, endianity);
            offset += 2;
            return result;
        }

        public static UInt24 ReadUInt24(this byte[] buffer, int offset, Endianity endianity)
        {
            UInt24 value = ReadUInt24(buffer, offset);
            if (IsWrongEndianity(endianity))
                value = HostToNetworkOrder(value);
            return value;
        }

        public static UInt24 ReadUInt24(this byte[] buffer, ref int offset, Endianity endianity)
        {
            UInt24 result = ReadUInt24(buffer, offset, endianity);
            offset += 3;
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "uint")]
        public static uint ReadUInt(this byte[] buffer, int offset, Endianity endianity)
        {
            return (uint)ReadInt(buffer, offset, endianity);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "uint")]
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

        public static void Write(this byte[] buffer, ref int offset, ushort value, Endianity endianity)
        {
            Write(buffer, offset, value, endianity);
            offset += 2;
        }

        public static void Write(this byte[] buffer, int offset, UInt24 value, Endianity endianity)
        {
            if (IsWrongEndianity(endianity))
                value = HostToNetworkOrder(value);
            Write(buffer, offset, value);
        }

        public static void Write(this byte[] buffer, ref int offset, UInt24 value, Endianity endianity)
        {
            Write(buffer, offset, value, endianity);
            offset += 3;
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

        public static void Write(this byte[] buffer, ref int offset, uint value, Endianity endianity)
        {
            Write(buffer, offset, value, endianity);
            offset += 4;
        }

        private static bool IsWrongEndianity(Endianity endianity)
        {
            return (BitConverter.IsLittleEndian == (endianity == Endianity.Big));
        }

        private static UInt24 HostToNetworkOrder(UInt24 value)
        {
            UInt24 result = value;
            unsafe
            {
                UInt24* ptr = &result;
                byte* bytePtr = (byte*)ptr;
                byte tmp = bytePtr[0];
                bytePtr[0] = bytePtr[2];
                bytePtr[2] = tmp;
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
    }
}