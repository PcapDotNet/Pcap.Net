using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 4034.
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
    [DnsTypeRegistration(Type = DnsType.NSec)]
    public sealed class DnsResourceDataNextDomainSecure : DnsResourceDataNoCompression, IEquatable<DnsResourceDataNextDomainSecure>
    {
        public DnsResourceDataNextDomainSecure(DnsDomainName nextDomainName, IEnumerable<DnsType> typesExist)
            : this(nextDomainName, new DnsTypeBitmaps(typesExist))
        {
        }

        /// <summary>
        /// Contains the next owner name (in the canonical ordering of the zone) that has authoritative data or contains a delegation point NS RRset;
        /// The value of the Next Domain Name field in the last NSEC record in the zone is the name of the zone apex (the owner name of the zone's SOA RR).
        /// This indicates that the owner name of the NSEC RR is the last name in the canonical ordering of the zone.
        ///
        /// Owner names of RRsets for which the given zone is not authoritative (such as glue records) must not be listed in the Next Domain Name
        /// unless at least one authoritative RRset exists at the same owner name.
        /// </summary>
        public DnsDomainName NextDomainName { get; private set; }

        /// <summary>
        /// Identifies the RRset types that exist at the NSEC RR's owner name.
        /// Ordered by the DnsType value.
        /// </summary>
        public ReadOnlyCollection<DnsType> TypesExist { get { return _typeBitmaps.TypesExist.AsReadOnly(); } }

        public bool IsTypePresentForOwner(DnsType dnsType)
        {
            return _typeBitmaps.Contains(dnsType);
        }

        public bool Equals(DnsResourceDataNextDomainSecure other)
        {
            return other != null &&
                   NextDomainName.Equals(other.NextDomainName) &&
                   _typeBitmaps.Equals(other._typeBitmaps);
        }

        public override bool Equals(object other)
        {
            return Equals(other as DnsResourceDataNextDomainSecure);
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode(NextDomainName, _typeBitmaps);
        }

        internal DnsResourceDataNextDomainSecure()
            : this(DnsDomainName.Root, new DnsType[0])
        {
        }

        internal override int GetLength()
        {
            return NextDomainName.NonCompressedLength + _typeBitmaps.GetLength();
        }

        internal override int WriteData(byte[] buffer, int offset)
        {
            NextDomainName.WriteUncompressed(buffer, offset);
            int nextDomainNameLength = NextDomainName.NonCompressedLength;
            return nextDomainNameLength + _typeBitmaps.Write(buffer, offset + nextDomainNameLength);
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            DnsDomainName nextDomainName;
            int nextDomainNameLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out nextDomainName, out nextDomainNameLength))
                return null;
            offsetInDns += nextDomainNameLength;
            length -= nextDomainNameLength;

            DnsTypeBitmaps typeBitmaps = DnsTypeBitmaps.CreateInstance(dns.Buffer, dns.StartOffset + offsetInDns, length);
            if (typeBitmaps == null)
                return null;

            return new DnsResourceDataNextDomainSecure(nextDomainName, typeBitmaps);
        }

        private DnsResourceDataNextDomainSecure(DnsDomainName nextDomainName, DnsTypeBitmaps typeBitmaps)
        {
            NextDomainName = nextDomainName;
            _typeBitmaps = typeBitmaps;
        }

        private readonly DnsTypeBitmaps _typeBitmaps;
    }
}