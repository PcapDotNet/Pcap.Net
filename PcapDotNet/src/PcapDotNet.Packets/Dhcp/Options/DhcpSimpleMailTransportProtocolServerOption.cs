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
    /// The SMTP server option specifies a list of SMTP servers available to
    /// the client. Servers SHOULD be listed in order of preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// | 69  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpSimpleMailTransportProtocolServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpSimpleMailTransportProtocolServerOption.
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpSimpleMailTransportProtocolServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.SimpleMailTransportProtocolServer)
        {
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.SimpleMailTransportProtocolServer)]
        internal static DhcpSimpleMailTransportProtocolServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpSimpleMailTransportProtocolServerOption>(data, ref offset, p => new DhcpSimpleMailTransportProtocolServerOption(p));
        }
    }
}