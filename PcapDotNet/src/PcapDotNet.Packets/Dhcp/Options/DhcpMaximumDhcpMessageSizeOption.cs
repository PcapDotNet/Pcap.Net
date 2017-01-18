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
    /// This option specifies the maximum length DHCP message that it is
    /// willing to accept. A client may use the maximum DHCP message size option in
    /// DHCPDISCOVER or DHCPREQUEST messages, but should not use the option
    /// in DHCPDECLINE messages.
    /// <pre>
    ///  Code   Len     Length
    /// +-----+-----+-----+-----+
    /// |  57 |  2  |  l1 |  l2 |
    /// +-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpMaximumDhcpMessageSizeOption : DhcpUShortOption
    {
        /// <summary>
        /// create new DhcpMaximumDhcpMessageSizeOption.
        /// </summary>
        /// <param name="length">Length</param>
        public DhcpMaximumDhcpMessageSizeOption(ushort length) : base(length, DhcpOptionCode.MaximumDhcpMessageSize)
        {
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.MaximumDhcpMessageSize)]
        internal static DhcpMaximumDhcpMessageSizeOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpMaximumDhcpMessageSizeOption>(data, ref offset, p => new DhcpMaximumDhcpMessageSizeOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Length.
        /// </summary>
        public ushort MaxLength
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}