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
    /// This option specifies whether or not the client should negotiate the
    /// use of trailers (RFC 893 [14]) when using the ARP protocol.
    /// <pre>
    ///  Code   Len  Value
    /// +-----+-----+-----+
    /// |  34 |  1  | 0/1 |
    /// +-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpTrailerEncapsulationOption : DhcpBooleanOption
    {
        /// <summary>
        /// create new DhcpTrailerEncapsulationOption.
        /// </summary>
        /// <param name="value">Value</param>
        public DhcpTrailerEncapsulationOption(bool value) : base(value, DhcpOptionCode.TrailerEncapsulation)
        {
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.TrailerEncapsulation)]
        internal static DhcpTrailerEncapsulationOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpTrailerEncapsulationOption>(data, ref offset, p => new DhcpTrailerEncapsulationOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Value.
        /// A value of false indicates that the client should not attempt to use trailers. A
        /// value of true means that the client should attempt to use trailers.
        /// </summary>
        public bool Value
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}