using System;
using System.Collections.Generic;
using System.Linq;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFCs 4034, 5155.
    /// </summary>
    internal class DnsTypeBitmaps : IEquatable<DnsTypeBitmaps>
    {
        private const int MaxTypeBitmapsLength = 256 * (2 + 32);

        public DnsTypeBitmaps(IEnumerable<DnsType> typesExist)
        {
            TypesExist = typesExist.Distinct().ToList();
            TypesExist.Sort();
        }

        public bool Contains(DnsType dnsType)
        {
            return TypesExist.BinarySearch(dnsType) >= 0;
        }

        public List<DnsType> TypesExist { get; private set;}

        public bool Equals(DnsTypeBitmaps other)
        {
            return other != null &&
                   TypesExist.SequenceEqual(other.TypesExist);
        }

        public override bool  Equals(object obj)
        {
            return Equals(obj as DnsTypeBitmaps);
        }

        public int GetLength()
        {
            int length = 0;
            int previousWindow = -1;
            int maxBit = -1;
            foreach (DnsType dnsType in TypesExist)
            {
                byte window = (byte)(((ushort)dnsType) >> 8);
                if (window > previousWindow)
                {
                    if (maxBit != -1)
                        length += 2 + maxBit / 8 + 1;
                    previousWindow = window;
                    maxBit = -1;
                }

                byte bit = (byte)dnsType;
                maxBit = Math.Max(bit, maxBit);
            }

            if (maxBit != -1)
                length += 2 + maxBit / 8 + 1;

            return length;
        }

        public int Write(byte[] buffer, int offset)
        {
            int originalOffset = offset;
            int previousWindow = -1;
            int maxBit = -1;
            byte[] windowBitmap = null;
            foreach (DnsType dnsType in TypesExist)
            {
                byte window = (byte)(((ushort)dnsType) >> 8);
                if (window > previousWindow)
                {
                    if (maxBit != -1)
                        WriteBitmap(buffer, ref offset, (byte)previousWindow, maxBit, windowBitmap);
                    previousWindow = window;
                    windowBitmap = new byte[32];
                    maxBit = -1;
                }

                byte bit = (byte)dnsType;
                maxBit = Math.Max(bit, maxBit);
                windowBitmap[bit / 8] |= (byte)(1 << (7 - bit % 8));
            }

            if (maxBit != -1)
                WriteBitmap(buffer, ref offset, (byte)previousWindow, maxBit, windowBitmap);

            return offset - originalOffset;
        }

        public static DnsTypeBitmaps CreateInstance(byte[] buffer, int offset, int length)
        {
            if (length > MaxTypeBitmapsLength)
                return null;

            List<DnsType> typesExist = new List<DnsType>();
            while (length != 0)
            {
                if (length < 3)
                    return null;
                byte window = buffer[offset++];
                byte bitmapLength = buffer[offset++];
                length -= 2;

                if (bitmapLength < 1 || bitmapLength > 32 || length < bitmapLength)
                    return null;
                for (int i = 0; i != bitmapLength; ++i)
                {
                    byte bits = buffer[offset++];
                    int bitIndex = 0;
                    while (bits != 0)
                    {
                        if ((byte)(bits & 0x80) == 0x80)
                        {
                            typesExist.Add((DnsType)((window << 8) + 8 * i + bitIndex));
                        }
                        bits <<= 1;
                        ++bitIndex;
                    }
                }
                length -= bitmapLength;
            }

            return new DnsTypeBitmaps(typesExist);
        }

        private static void WriteBitmap(byte[] buffer, ref int offset, byte window, int maxBit, byte[] windowBitmap)
        {
            buffer.Write(ref offset, window);
            byte numBytes = (byte)(maxBit / 8 + 1);
            buffer.Write(ref offset, numBytes);
            DataSegment data = new DataSegment(windowBitmap, 0, numBytes);
            data.Write(buffer, ref offset);
        }
    }
}