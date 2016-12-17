using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option specifies the domain name that client should use when
    /// resolving hostnames via the Domain Name System.
    /// <pre>
    ///  Code   Len        Domain Name
    /// +-----+-----+-----+-----+-----+-----+---
    /// |  15 |  n  |  d1 |  d2 |  d3 |  d4 | ...
    /// +-----+-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpDomainNameOption : DhcpStringOption
    {
        /// <summary>
        /// create new DhcpDomainNameOption
        /// </summary>
        /// <param name="domainName">Domain Name</param>
        public DhcpDomainNameOption(string domainName) : base(domainName, DhcpOptionCode.DomainName)
        {
        }

        internal static DhcpDomainNameOption Read(DataSegment data, ref int offset)
        {
            return DhcpStringOption.Read(data, ref offset, p => new DhcpDomainNameOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Domain Name
        /// </summary>
        public string DomainName
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}