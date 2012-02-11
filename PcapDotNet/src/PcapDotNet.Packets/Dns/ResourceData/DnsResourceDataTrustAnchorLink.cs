using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// Wijngaards.
    /// <pre>
    /// +----------------------+
    /// | Previous Domain Name |
    /// +----------------------+
    /// | Next Domain Name     |
    /// +----------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.TrustAnchorLink)]
    public sealed class DnsResourceDataTrustAnchorLink : DnsResourceDataNoCompression, IEquatable<DnsResourceDataTrustAnchorLink>
    {
        private const int MinimumLength = 2 * DnsDomainName.RootLength;

        public DnsResourceDataTrustAnchorLink(DnsDomainName previous, DnsDomainName next)
        {
            Previous = previous;
            Next = next;
        }

        /// <summary>
        /// The start, or previous name.
        /// </summary>
        public DnsDomainName Previous { get; private set; }

        /// <summary>
        /// End or next name in the list.
        /// </summary>
        public DnsDomainName Next { get; private set; }

        public bool Equals(DnsResourceDataTrustAnchorLink other)
        {
            return other != null &&
                   Previous.Equals(other.Previous) &&
                   Next.Equals(other.Next);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataTrustAnchorLink);
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode(Previous, Next);
        }

        internal DnsResourceDataTrustAnchorLink()
            : this(DnsDomainName.Root, DnsDomainName.Root)
        {
        }

        internal override int GetLength()
        {
            return Previous.NonCompressedLength + Next.NonCompressedLength;
        }

        internal override int WriteData(byte[] buffer, int offset)
        {
            Previous.WriteUncompressed(buffer, offset);
            int previousLength = Previous.NonCompressedLength;
            Next.WriteUncompressed(buffer, offset + previousLength);

            return previousLength + Next.NonCompressedLength;
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < MinimumLength)
                return null;

            DnsDomainName previous;
            int previousLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length - DnsDomainName.RootLength, out previous, out previousLength))
                return null;
            offsetInDns += previousLength;
            length -= previousLength;

            DnsDomainName next;
            int nextLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out next, out nextLength))
                return null;

            return new DnsResourceDataTrustAnchorLink(previous, next);
        }
    }
}