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
    /// This option specifies whether or not the client should solicit
    /// routers using the Router Discovery mechanism defined in RFC 1256.
    /// <pre>
    ///  Code   Len  Value
    /// +-----+-----+-----+
    /// |  31 |  1  | 0/1 |
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpPerformRouterDiscoveryOption : DhcpBooleanOption
    {
        /// <summary>
        /// create new DhcpPerformRouterDiscoveryOption.
        /// </summary>
        /// <param name="value">Value</param>
        public DhcpPerformRouterDiscoveryOption(bool value) : base(value, DhcpOptionCode.PerformRouterDiscovery)
        {
        }

        internal static DhcpPerformRouterDiscoveryOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpPerformRouterDiscoveryOption>(data, ref offset, p => new DhcpPerformRouterDiscoveryOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// A value of false indicates that the client should not perform
        /// router discovery. A value of true means that the client should perform
        /// router discovery.
        /// </summary>
        public bool Value
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}