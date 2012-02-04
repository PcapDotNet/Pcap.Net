using System;
using System.Collections.Generic;
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
    [DnsTypeRegistration(Type = DnsType.Nxt)]
    public sealed class DnsResourceDataNextDomain : DnsResourceData, IEquatable<DnsResourceDataNextDomain>
    {
        public const int MaxTypeBitMapLength = 16;
        public const DnsType MaxTypeBitMapDnsType = (DnsType)(8 * MaxTypeBitMapLength);

        public DnsResourceDataNextDomain(DnsDomainName nextDomainName, DataSegment typeBitMap)
        {
            if (typeBitMap.Length > MaxTypeBitMapLength)
                throw new ArgumentOutOfRangeException("typeBitMap", typeBitMap.Length, string.Format("Cannot be longer than {0} bytes.", MaxTypeBitMapLength));
            if (typeBitMap.Length > 0 && typeBitMap.Last == 0)
                throw new ArgumentOutOfRangeException("typeBitMap", typeBitMap, "Last byte cannot be 0x00");

            NextDomainName = nextDomainName;
            TypeBitMap = typeBitMap;
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
        public DataSegment TypeBitMap { get; private set; }

        public IEnumerable<DnsType> TypesExist
        {
            get
            {
                ushort typeValue = 0;
                for (int byteOffset = 0; byteOffset != TypeBitMap.Length; ++byteOffset)
                {
                    byte mask = 0x80;
                    for (int i = 0; i != 8; ++i)
                    {
                        if (TypeBitMap.ReadBool(byteOffset, mask))
                            yield return (DnsType)typeValue;
                        ++typeValue;
                        mask >>= 1;
                    }
                }
            }
        }

        public bool IsTypePresentForOwner(DnsType dnsType)
        {
            if (dnsType >= MaxTypeBitMapDnsType)
                throw new ArgumentOutOfRangeException("dnsType", dnsType, string.Format("Cannot be bigger than {0}.", MaxTypeBitMapDnsType));

            int byteOffset;
            byte mask;
            DnsTypeToByteOffsetAndMask(out byteOffset, out mask, dnsType);
            if (byteOffset > TypeBitMap.Length)
                return false;

            return TypeBitMap.ReadBool(byteOffset, mask);
        }

        public static DataSegment CreateTypeBitMap(IEnumerable<DnsType> typesPresentForOwner)
        {
            DnsType maxDnsType = typesPresentForOwner.Max();
            int length = (ushort)(maxDnsType + 7) / 8;
            if (length == 0)
                return DataSegment.Empty;

            byte[] typeBitMapBuffer = new byte[length];
            foreach (DnsType dnsType in typesPresentForOwner)
            {
                int byteOffset;
                byte mask;
                DnsTypeToByteOffsetAndMask(out byteOffset, out mask, dnsType);

                typeBitMapBuffer[byteOffset] |= mask;
            }

            return new DataSegment(typeBitMapBuffer);
        }

        public bool Equals(DnsResourceDataNextDomain other)
        {
            return other != null &&
                   NextDomainName.Equals(other.NextDomainName) &&
                   TypeBitMap.Equals(other.TypeBitMap);
        }

        public override bool Equals(object other)
        {
            return Equals(other as DnsResourceDataNextDomain);
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode(NextDomainName, TypeBitMap);
        }

        internal DnsResourceDataNextDomain()
            : this(DnsDomainName.Root, DataSegment.Empty)
        {
        }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return NextDomainName.GetLength(compressionData, offsetInDns) + TypeBitMap.Length;
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            int numBytesWritten = NextDomainName.Write(buffer, dnsOffset, compressionData, offsetInDns);
            TypeBitMap.Write(buffer, dnsOffset + offsetInDns + numBytesWritten);

            return numBytesWritten + TypeBitMap.Length;
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            DnsDomainName nextDomainName;
            int nextDomainNameLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out nextDomainName, out nextDomainNameLength))
                return null;
            offsetInDns += nextDomainNameLength;
            length -= nextDomainNameLength;

            if (length > MaxTypeBitMapLength)
                return null;

            DataSegment typeBitMap = dns.SubSegment(offsetInDns, length);
            if (length != 0 && typeBitMap.Last == 0)
                return null;

            return new DnsResourceDataNextDomain(nextDomainName, typeBitMap);
        }

        private static void DnsTypeToByteOffsetAndMask(out int byteOffset, out byte mask, DnsType dnsType)
        {
            byteOffset = (ushort)dnsType / 8;
            mask = (byte)(1 << ((ushort)dnsType % 8));
        }
    }
}