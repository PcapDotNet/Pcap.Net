using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 2535.
    /// <pre>
    /// +------------------+
    /// | next domain name |
    /// |                  |
    /// +------------------+
    /// | type bit map     |
    /// |                  |
    /// +------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.NextDomain)]
    public sealed class DnsResourceDataNextDomain : DnsResourceData, IEquatable<DnsResourceDataNextDomain>
    {
        public const int MaxTypeBitmapLength = 16;
        public const DnsType MaxTypeBitmapDnsType = (DnsType)(8 * MaxTypeBitmapLength);

        public DnsResourceDataNextDomain(DnsDomainName nextDomainName, DataSegment typeBitmap)
        {
            if (typeBitmap == null)
                throw new ArgumentNullException("typeBitmap");

            if (typeBitmap.Length > MaxTypeBitmapLength)
                throw new ArgumentOutOfRangeException("typeBitmap", typeBitmap.Length,
                                                      string.Format(CultureInfo.InvariantCulture, "Cannot be longer than {0} bytes.", MaxTypeBitmapLength));
            if (typeBitmap.Length > 0 && typeBitmap.Last == 0)
                throw new ArgumentOutOfRangeException("typeBitmap", typeBitmap, "Last byte cannot be 0x00");

            NextDomainName = nextDomainName;
            TypeBitmap = typeBitmap;
        }

        /// <summary>
        /// The next domain name according to the canonical DNS name order.
        /// </summary>
        public DnsDomainName NextDomainName { get; private set; }

        /// <summary>
        /// One bit per RR type present for the owner name.
        /// A one bit indicates that at least one RR of that type is present for the owner name.
        /// A zero indicates that no such RR is present.
        /// All bits not specified because they are beyond the end of the bit map are assumed to be zero.
        /// Note that bit 30, for NXT, will always be on so the minimum bit map length is actually four octets.
        /// Trailing zero octets are prohibited in this format.
        /// The first bit represents RR type zero (an illegal type which can not be present) and so will be zero in this format.
        /// This format is not used if there exists an RR with a type number greater than 127.
        /// If the zero bit of the type bit map is a one, it indicates that a different format is being used which will always be the case if a type number greater than 127 is present.
        /// </summary>
        public DataSegment TypeBitmap { get; private set; }

        public IEnumerable<DnsType> TypesExist
        {
            get
            {
                ushort typeValue = 0;
                for (int byteOffset = 0; byteOffset != TypeBitmap.Length; ++byteOffset)
                {
                    byte mask = 0x80;
                    for (int i = 0; i != 8; ++i)
                    {
                        if (TypeBitmap.ReadBool(byteOffset, mask))
                            yield return (DnsType)typeValue;
                        ++typeValue;
                        mask >>= 1;
                    }
                }
            }
        }

        public bool IsTypePresentForOwner(DnsType dnsType)
        {
            if (dnsType >= MaxTypeBitmapDnsType)
                throw new ArgumentOutOfRangeException("dnsType", dnsType,
                                                      string.Format(CultureInfo.InvariantCulture, "Cannot be bigger than {0}.", MaxTypeBitmapDnsType));

            int byteOffset;
            byte mask;
            DnsTypeToByteOffsetAndMask(out byteOffset, out mask, dnsType);
            if (byteOffset > TypeBitmap.Length)
                return false;

            return TypeBitmap.ReadBool(byteOffset, mask);
        }

        public static DataSegment CreateTypeBitmap(IEnumerable<DnsType> typesPresentForOwner)
        {
            if (typesPresentForOwner == null)
                throw new ArgumentNullException("typesPresentForOwner");

            DnsType maxDnsType = typesPresentForOwner.Max();
            int length = (ushort)(maxDnsType + 7) / 8;
            if (length == 0)
                return DataSegment.Empty;

            byte[] typeBitmapBuffer = new byte[length];
            foreach (DnsType dnsType in typesPresentForOwner)
            {
                int byteOffset;
                byte mask;
                DnsTypeToByteOffsetAndMask(out byteOffset, out mask, dnsType);

                typeBitmapBuffer[byteOffset] |= mask;
            }

            return new DataSegment(typeBitmapBuffer);
        }

        public bool Equals(DnsResourceDataNextDomain other)
        {
            return other != null &&
                   NextDomainName.Equals(other.NextDomainName) &&
                   TypeBitmap.Equals(other.TypeBitmap);
        }

        public override bool Equals(object other)
        {
            return Equals(other as DnsResourceDataNextDomain);
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode(NextDomainName, TypeBitmap);
        }

        internal DnsResourceDataNextDomain()
            : this(DnsDomainName.Root, DataSegment.Empty)
        {
        }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return NextDomainName.GetLength(compressionData, offsetInDns) + TypeBitmap.Length;
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            int numBytesWritten = NextDomainName.Write(buffer, dnsOffset, compressionData, offsetInDns);
            TypeBitmap.Write(buffer, dnsOffset + offsetInDns + numBytesWritten);

            return numBytesWritten + TypeBitmap.Length;
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            DnsDomainName nextDomainName;
            int nextDomainNameLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out nextDomainName, out nextDomainNameLength))
                return null;
            offsetInDns += nextDomainNameLength;
            length -= nextDomainNameLength;

            if (length > MaxTypeBitmapLength)
                return null;

            DataSegment typeBitmap = dns.Subsegment(offsetInDns, length);
            if (length != 0 && typeBitmap.Last == 0)
                return null;

            return new DnsResourceDataNextDomain(nextDomainName, typeBitmap);
        }

        private static void DnsTypeToByteOffsetAndMask(out int byteOffset, out byte mask, DnsType dnsType)
        {
            byteOffset = (ushort)dnsType / 8;
            mask = (byte)(1 << ((ushort)dnsType % 8));
        }
    }
}