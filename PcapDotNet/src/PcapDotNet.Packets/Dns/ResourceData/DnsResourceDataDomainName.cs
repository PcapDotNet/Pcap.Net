using System;

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
    [DnsTypeRegistration(Type = DnsType.MailForwarder)]
    [DnsTypeRegistration(Type = DnsType.CName)]
    [DnsTypeRegistration(Type = DnsType.Mailbox)]
    [DnsTypeRegistration(Type = DnsType.MailGroup)]
    [DnsTypeRegistration(Type = DnsType.MailRename)]
    [DnsTypeRegistration(Type = DnsType.Ptr)]
    [DnsTypeRegistration(Type = DnsType.NetworkServiceAccessPointPointer)]
    [DnsTypeRegistration(Type = DnsType.DName)]
    public sealed class DnsResourceDataDomainName : DnsResourceData, IEquatable<DnsResourceDataDomainName>
    {
        /// <summary>
        /// Constructs an instance from the domain name data.
        /// </summary>
        public DnsResourceDataDomainName(DnsDomainName data)
        {
            Data = data;
        }

        /// <summary>
        /// The domain name value.
        /// </summary>
        public DnsDomainName Data { get; private set; }

        /// <summary>
        /// Two DnsResourceDataDomainName are equal iff their domain name values are equal.
        /// </summary>
        public bool Equals(DnsResourceDataDomainName other)
        {
            return other != null && Data.Equals(other.Data);
        }

        /// <summary>
        /// Two DnsResourceDataDomainName are equal iff their domain name values are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataDomainName);
        }

        /// <summary>
        /// The hash code of the domain name value.
        /// </summary>
        public override int GetHashCode()
        {
            return Data.GetHashCode();
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