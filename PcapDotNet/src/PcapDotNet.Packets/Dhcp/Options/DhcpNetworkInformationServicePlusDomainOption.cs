using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option specifies the name of the client's NIS+ [17] domain.
    /// <pre>
    ///  Code   Len      NIS Client Domain Name
    /// +-----+-----+-----+-----+-----+-----+---
    /// |  64 |  n  |  n1 |  n2 |  n3 |  n4 | ...
    /// +-----+-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpNetworkInformationServicePlusDomainOption : DhcpStringOption
    {
        /// <summary>
        /// create new DhcpNetworkInformationServicePlusDomainOption
        /// </summary>
        /// <param name="nisClientDomainName">NIS Client Domain Name</param>
        public DhcpNetworkInformationServicePlusDomainOption(string nisClientDomainName) : base(nisClientDomainName, DhcpOptionCode.NetworkInformationServicePlusDomain)
        {
        }

        internal static DhcpNetworkInformationServicePlusDomainOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpNetworkInformationServicePlusDomainOption>(data, ref offset, p => new Options.DhcpNetworkInformationServicePlusDomainOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// NIS Client Domain Name
        /// </summary>
        public string NisClientDomainName

        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}