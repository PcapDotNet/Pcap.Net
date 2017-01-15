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
    /// This option is used in DHCPOFFER and DHCPREQUEST messages, and may
    /// optionally be included in the DHCPACK and DHCPNAK messages.DHCP
    /// servers include this option in the DHCPOFFER in order to allow the
    /// client to distinguish between lease offers.  DHCP clients use the
    /// contents of the 'server identifier' field as the destination address
    /// for any DHCP messages unicast to the DHCP server.  DHCP clients also
    /// indicate which of several lease offers is being accepted by including
    /// this option in a DHCPREQUEST message.
    /// The identifier is the IP address of the selected server.
    /// <pre>
    ///  Code   Len             Address
    /// +-----+-----+-----+-----+-----+-----+
    /// |  54 |  4  |  a1 |  a2 |  a3 |  a4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpServerIdentifierOption : DhcpSingleAddressOption
    {
        /// <summary>
        /// create new DhcpServerIdentifierOption.
        /// </summary>
        /// <param name="address">Address</param>
        public DhcpServerIdentifierOption(IpV4Address address) : base(address, DhcpOptionCode.ServerIdentifier)
        {
        }

        internal static DhcpServerIdentifierOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpServerIdentifierOption>(data, ref offset, p => new DhcpServerIdentifierOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Address.
        /// </summary>
        public IpV4Address Address
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}