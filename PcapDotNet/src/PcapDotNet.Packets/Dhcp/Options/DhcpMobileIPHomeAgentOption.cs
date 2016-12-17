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
    /// This option specifies a list of IP addresses indicating mobile IP
    /// home agents available to the client. Agents SHOULD be listed in
    /// order of preference.
    /// <pre>
    ///  Code Len    Home Agent Addresses (zero or more)
    /// +-----+-----+-----+-----+-----+-----+--
    /// | 68  |  n  | a1  | a2  | a3  | a4  | ...
    /// +-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpMobileIPHomeAgentOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpMobileIPHomeAgentOption
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpMobileIPHomeAgentOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.MobileIPHomeAgent)
        {
        }

        internal static DhcpMobileIPHomeAgentOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            return new DhcpMobileIPHomeAgentOption(GetAddresses(data, length, ref offset));
        }

        /// <summary>
        /// true if Addresses-List is allowed to be empty
        /// </summary>
        protected override bool AllowEmptyAddresses
        {
            get
            {
                return true;
            }
        }
    }
}