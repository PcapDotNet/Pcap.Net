using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option specifies the name of the client's NIS [17] domain.
    /// <pre>
    ///  Code   Len      NIS Domain Name
    /// +-----+-----+-----+-----+-----+-----+---
    /// |  40 |  n  |  n1 |  n2 |  n3 |  n4 | ...
    /// +-----+-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpNetworkInformationServiceDomainOption : DhcpStringOption
    {
        /// <summary>
        /// create new DhcpNetworkInformationServiceDomainOption.
        /// </summary>
        /// <param name="nisDomainName">NIS Domain Name</param>
        public DhcpNetworkInformationServiceDomainOption(string nisDomainName) : base(nisDomainName, DhcpOptionCode.NetworkInformationServiceDomain)
        {
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.NetworkInformationServiceDomain)]
        internal static DhcpNetworkInformationServiceDomainOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpNetworkInformationServiceDomainOption>(data, ref offset, p => new DhcpNetworkInformationServiceDomainOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// NIS Domain Name.
        /// </summary>
        public string NisDomainName
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}