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
    /// The WWW server option specifies a list of WWW available to the
    /// client.Servers SHOULD be listed in order of preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// | 72  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpDefaultWorldWideWebServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpDefaultWorldWideWebServerOption.
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpDefaultWorldWideWebServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.DefaultWorldWideWebServer)
        {
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.DefaultWorldWideWebServer)]
        internal static DhcpDefaultWorldWideWebServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpDefaultWorldWideWebServerOption>(data, ref offset, p => new DhcpDefaultWorldWideWebServerOption(p));
        }
    }
}