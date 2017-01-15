using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option is used by clients and servers to exchange vendor-
    /// specific information. The information is an opaque object of n
    /// octets, presumably interpreted by vendor-specific code on the clients
    /// and servers. The definition of this information is vendor specific.
    /// The vendor is indicated in the vendor class identifier option.
    /// Servers not equipped to interpret the vendor-specific information
    /// sent by a client MUST ignore it(although it may be reported).
    /// Clients which do not receive desired vendor-specific information
    /// SHOULD make an attempt to operate without it, although they may do so
    /// (and announce they are doing so) in a degraded mode.
    /// <pre>
    /// Code   Len   Vendor-specific information
    /// +-----+-----+-----+-----+---
    /// |  43 |  n  |  i1 |  i2 | ...
    /// +-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpVendorSpecificInformationOption : DhcpDataSegmentOption
    {
        /// <summary>
        /// create new DhcpVendorSpecificInformationOption.
        /// </summary>
        /// <param name="vendorSpecificInformation">Vendor-specific information</param>
        public DhcpVendorSpecificInformationOption(DataSegment vendorSpecificInformation) : base(vendorSpecificInformation, DhcpOptionCode.VendorSpecificInformation)
        {
        }

        internal static DhcpVendorSpecificInformationOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpVendorSpecificInformationOption>(data, ref offset, p => new DhcpVendorSpecificInformationOption(p));
        }

        /// <summary>
        /// RFC 2132
        /// Vendor-specific information.
        /// </summary>
        public DataSegment VendorSpecificInformation
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}