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
    /// This option specifies whether or not the client should perform subnet
    /// mask discovery using ICMP.
    /// <pre>
    ///  Code   Len  Value
    /// +-----+-----+-----+
    /// |  29 |  1  | 0/1 |
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpPerformMaskDiscoveryOption : DhcpBooleanOption
    {
        /// <summary>
        /// create new DhcpPerformMaskDiscoveryOption.
        /// </summary>
        /// <param name="value">Value</param>
        public DhcpPerformMaskDiscoveryOption(bool value) : base(value, DhcpOptionCode.PerformMaskDiscovery)
        {
        }

        internal static DhcpPerformMaskDiscoveryOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpPerformMaskDiscoveryOption>(data, ref offset, p => new DhcpPerformMaskDiscoveryOption(p));
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