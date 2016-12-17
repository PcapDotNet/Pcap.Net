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
    /// This option specifies whether or not the client should use Ethernet
    /// Version 2 (RFC 894 [15]) or IEEE 802.3 (RFC 1042 [16]) encapsulation
    /// if the interface is an Ethernet.
    /// <pre>
    ///  Code   Len  Value
    /// +-----+-----+-----+
    /// |  36 |  1  | 0/1 |
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpEthernetEncapsulationOption : DhcpBooleanOption
    {
        /// <summary>
        /// create new DhcpEthernetEncapsulationOption
        /// </summary>
        /// <param name="value">Value</param>
        public DhcpEthernetEncapsulationOption(bool value) : base(value, DhcpOptionCode.EthernetEncapsulation)
        {
        }

        internal static DhcpEthernetEncapsulationOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpEthernetEncapsulationOption>(data, ref offset, p => new DhcpEthernetEncapsulationOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// A value of false indicates that the
        /// client should use RFC 894 encapsulation. A value of true  means that the
        /// client should use RFC 1042 encapsulation.
        /// </summary>
        public bool Value
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}