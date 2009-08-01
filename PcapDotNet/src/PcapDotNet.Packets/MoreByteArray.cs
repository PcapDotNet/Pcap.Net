using System;
using System.Net;
using PcapDotNet.Base;

namespace PcapDotNet.Packets
{
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
            offset += sizeof(ushort);
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
            offset += UInt24.SizeOf;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "uint")]
        public static uint ReadUInt(this byte[] buffer, int offset, Endianity endianity)
        {
            return (uint)ReadInt(buffer, offset, endianity);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "uint")]
        public static uint ReadUInt(this byte[] buffer, ref int offset, Endianity endianity)
        {
            uint result = ReadUInt(buffer, offset, endianity);
            offset += sizeof(int);
            return result;
        }

        public static UInt48 ReadUInt48(this byte[] buffer, int offset, Endianity endianity)
        {
            UInt48 value = ReadUInt48(buffer, offset);
            if (IsWrongEndianity(endianity))
                value = HostToNetworkOrder(value);
            return value;
        }

        public static UInt48 ReadUInt48(this byte[] buffer, ref int offset, Endianity endianity)
        {
            UInt48 result = buffer.ReadUInt48(offset, endianity);
            offset += UInt48.SizeOf;
            return result;
        }

        public static MacAddress ReadMacAddress(this byte[] buffer, int offset, Endianity endianity)
        {
            return new MacAddress(buffer.ReadUInt48(offset, endianity));
        }

        public static MacAddress ReadMacAddress(this byte[] buffer, ref int offset, Endianity endianity)
        {
            return new MacAddress(buffer.ReadUInt48(ref offset, endianity));
        }

        public static IpV4Address ReadIpV4Address(this byte[] buffer, int offset, Endianity endianity)
        {
            return new IpV4Address(buffer.ReadUInt(offset, endianity));
        }

        public static IpV4Address ReadIpV4Address(this byte[] buffer, ref int offset, Endianity endianity)
        {
            return new IpV4Address(buffer.ReadUInt(ref offset, endianity));
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
            offset += sizeof(ushort);
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
            offset += UInt24.SizeOf;
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
            offset += sizeof(uint);
        }

        public static void Write(this byte[] buffer, int offset, UInt48 value, Endianity endianity)
        {
            if (IsWrongEndianity(endianity))
                value = HostToNetworkOrder(value);
            Write(buffer, offset, value);
        }

        public static void Write(this byte[] buffer, ref int offset, UInt48 value, Endianity endianity)
        {
            Write(buffer, offset, value, endianity);
            offset += UInt48.SizeOf;
        }

        public static void Write(this byte[] buffer, int offset, MacAddress value, Endianity endianity)
        {
            buffer.Write(offset, value.ToValue(), endianity);
        }

        public static void Write(this byte[] buffer, ref int offset, MacAddress value, Endianity endianity)
        {
            buffer.Write(ref offset, value.ToValue(), endianity);
        }

        public static void Write(this byte[] buffer, int offset, IpV4Address value, Endianity endianity)
        {
            buffer.Write(offset, value.ToValue(), endianity);
        }

        public static void Write(this byte[] buffer, ref int offset, IpV4Address value, Endianity endianity)
        {
            buffer.Write(ref offset, value.ToValue(), endianity);
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