﻿using System;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// Contains a single DNS domain name.
    /// <pre>
    /// +------+
    /// | NAME |
    /// +------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Ns)]
    [DnsTypeRegistration(Type = DnsType.Md)]
    [DnsTypeRegistration(Type = DnsType.Mf)]
    [DnsTypeRegistration(Type = DnsType.CName)]
    [DnsTypeRegistration(Type = DnsType.Mb)]
    [DnsTypeRegistration(Type = DnsType.Mg)]
    [DnsTypeRegistration(Type = DnsType.Mr)]
    [DnsTypeRegistration(Type = DnsType.Ptr)]
    [DnsTypeRegistration(Type = DnsType.NsapPtr)]
    [DnsTypeRegistration(Type = DnsType.DName)]
    public sealed class DnsResourceDataDomainName : DnsResourceData, IEquatable<DnsResourceDataDomainName>
    {
        public DnsResourceDataDomainName(DnsDomainName data)
        {
            Data = data;
        }

        public DnsDomainName Data { get; private set; }

        public bool Equals(DnsResourceDataDomainName other)
        {
            return other != null && Data.Equals(other.Data);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataDomainName);
        }

        internal DnsResourceDataDomainName()
            : this(DnsDomainName.Root)
        {
        }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return Data.GetLength(compressionData, offsetInDns);
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            return Data.Write(buffer, dnsOffset, compressionData, offsetInDns);
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            int numBytesRead;
            DnsDomainName domainName;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out domainName, out numBytesRead))
                return null;
            length -= numBytesRead;

            if (length != 0)
                return null;

            return new DnsResourceDataDomainName(domainName);
        }
    }
}