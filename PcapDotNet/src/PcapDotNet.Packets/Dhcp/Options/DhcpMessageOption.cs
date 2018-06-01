using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option is used by a DHCP server to provide an error message to a
    /// DHCP client in a DHCPNAK message in the event of a failure. A client
    /// may use this option in a DHCPDECLINE message to indicate the why the
    /// client declined the offered parameters.
    /// <pre>
    ///  Code   Len     Text
    /// +-----+-----+-----+-----+---
    /// |  56 |  n  |  c1 |  c2 | ...
    /// +-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpMessageOption : DhcpStringOption
    {
        /// <summary>
        /// create new DhcpMessageOption.
        /// </summary>
        /// <param name="text">Text</param>
        public DhcpMessageOption(string text) : base(text, DhcpOptionCode.Message)
        {
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.Message)]
        internal static DhcpMessageOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpMessageOption>(data, ref offset, p => new DhcpMessageOption(p));
        }

        ///<summary>
        /// RFC 2132.
        /// Text.
        /// </summary>
        public string Text
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}