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
    /// This option specifies whether or not the client should respond to
    /// subnet mask requests using ICMP.
    /// <pre>
    ///  Code   Len  Value
    /// +-----+-----+-----+
    /// |  30 |  1  | 0/1 |
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpMaskSupplierOption : DhcpBooleanOption
    {
        /// <summary>
        /// create new DhcpMaskSupplierOption.
        /// </summary>
        /// <param name="value">Value</param>
        public DhcpMaskSupplierOption(bool value) : base(value, DhcpOptionCode.MaskSupplier)
        {
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.MaskSupplier)]
        internal static DhcpMaskSupplierOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpMaskSupplierOption>(data, ref offset, p => new Options.DhcpMaskSupplierOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// A value of false indicates that the client
        /// should not perform mask discovery. A value of true means that the
        /// client should perform mask discovery.
        /// </summary>
        public bool Value
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}