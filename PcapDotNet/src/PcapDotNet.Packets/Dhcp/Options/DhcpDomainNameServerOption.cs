using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// The domain name server option specifies a list of Domain Name System
    /// (STD 13, RFC 1035 [8]) name servers available to the client. Servers
    /// SHOULD be listed in order of preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// |  6  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpDomainNameServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpDomainNameServerOption.
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpDomainNameServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.DomainNameServerServer)
        {
        }

        internal static DhcpDomainNameServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpDomainNameServerOption>(data, ref offset, p => new DhcpDomainNameServerOption(p));
        }
    }
}