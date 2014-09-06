using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// A base class for resource record datas that contain DNS domain names.
    /// </summary>
    public abstract class DnsResourceDataDomainNames : DnsResourceData, IEquatable<DnsResourceDataDomainNames>
    {
        /// <summary>
        /// Two domain names resource data are equal if they are of the same type and contain the same domain names in the same order.
        /// </summary>
        public bool Equals(DnsResourceDataDomainNames other)
        {
            return other != null &&
                   GetType() == other.GetType() &&
                   DomainNames.SequenceEqual(other.DomainNames);
        }

        /// <summary>
        /// Two domain names resource data are equal if they are of the same type and contain the same domain names in the same order.
        /// </summary>
        public sealed override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataDomainNames);
        }

        /// <summary>
        /// The hash code of the resource data is based on the concrete type and domain names.
        /// </summary>
        public sealed override int GetHashCode()
        {
            return GetType().GetHashCode() ^ DomainNames.SequenceGetHashCode();
        }

        internal DnsResourceDataDomainNames(ReadOnlyCollection<DnsDomainName> domainNames)
        {
            DomainNames = domainNames;
        }

        internal DnsResourceDataDomainNames(params DnsDomainName[] domainNames)
            : this(domainNames.AsReadOnly())
        {
        }

        internal ReadOnlyCollection<DnsDomainName> DomainNames { get; private set; }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            int totalLength = 0;
            foreach (DnsDomainName domainName in DomainNames)
            {
                int length = domainName.GetLength(compressionData, offsetInDns);
                offsetInDns += length;
                totalLength += length;
            }

            return totalLength;
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            int numBytesWritten = 0;
            foreach (DnsDomainName domainName in DomainNames)
                numBytesWritten += domainName.Write(buffer, dnsOffset, compressionData, offsetInDns + numBytesWritten);
            return numBytesWritten;
        }

        internal static List<DnsDomainName> ReadDomainNames(DnsDatagram dns, int offsetInDns, int length, int numExpected = 0)
        {
            List<DnsDomainName> domainNames = new List<DnsDomainName>(numExpected);
            while (length != 0)
            {
                DnsDomainName domainName;
                int domainNameLength;
                if (!DnsDomainName.TryParse(dns, offsetInDns, length, out domainName, out domainNameLength))
                    return null;
                offsetInDns += domainNameLength;
                length -= domainNameLength;
                domainNames.Add(domainName);
            }

            return domainNames;
        }
    }
}