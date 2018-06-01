using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option is used by DHCP clients to optionally identify the vendor
    /// type and configuration of a DHCP client. The information is a string
    /// of n octets, interpreted by servers. Vendors may choose to define
    /// specific vendor class identifiers to convey particular configuration
    /// or other identification information about a client. For example, the
    /// identifier may encode the client's hardware configuration. Servers
    /// not equipped to interpret the class-specific information sent by a
    /// client MUST ignore it(although it may be reported). Servers that
    /// respond SHOULD only use option 43 to return the vendor-specific
    /// information to the client.
    /// <pre>
    ///  Code   Len   Vendor class Identifier
    /// +-----+-----+-----+-----+---
    /// |  60 |  n  |  i1 |  i2 | ...
    /// +-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpVendorClassidentifierOption : DhcpDataSegmentOption
    {
        /// <summary>
        /// create new DhcpVendorClassidentifierOption.
        /// </summary>
        /// <param name="vendorClassIdentifier">Vendor class Identifier</param>
        public DhcpVendorClassidentifierOption(DataSegment vendorClassIdentifier) : base(vendorClassIdentifier, DhcpOptionCode.VendorClassidentifier)
        {
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.VendorClassidentifier)]
        internal static DhcpVendorClassidentifierOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpVendorClassidentifierOption>(data, ref offset, p => new DhcpVendorClassidentifierOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Vendor class Identifier.
        /// </summary>
        public DataSegment VendorClassIdentifier
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}