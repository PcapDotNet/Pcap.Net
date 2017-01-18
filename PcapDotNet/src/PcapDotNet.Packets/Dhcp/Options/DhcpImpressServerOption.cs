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
    /// The Impress server option specifies a list of Imagen Impress servers
    /// available to the client.Servers SHOULD be listed in order of
    /// preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// |  10 |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpImpressServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpImpressServerOption.
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpImpressServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.ImpressServer)
        {
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.ImpressServer)]
        internal static DhcpImpressServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpImpressServerOption>(data, ref offset, p => new Options.DhcpImpressServerOption(p));
        }
    }
}