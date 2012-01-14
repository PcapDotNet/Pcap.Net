using System.Collections.Generic;

namespace PcapDotNet.Packets.Dns
{
    public abstract class DnsResourceData2DomainNames : DnsResourceDataDomainNames
    {
        private const int NumDomains = 2;

        internal DnsResourceData2DomainNames(DnsDomainName first, DnsDomainName second)
            : base(first, second)
        {
        }

        internal DnsDomainName First { get { return DomainNames[0]; } }

        internal DnsDomainName Second { get { return DomainNames[1]; } }

        internal static bool TryRead(out DnsDomainName first, out DnsDomainName second,
                                     DnsDatagram dns, int offsetInDns, int length)
        {
            List<DnsDomainName> domainNames = ReadDomainNames(dns, offsetInDns, length, NumDomains);
            if (domainNames == null || domainNames.Count != NumDomains)
            {
                first = null;
                second = null;
                return false;
            }

            first = domainNames[0];
            second = domainNames[1];
            return true;
        }
    }
}