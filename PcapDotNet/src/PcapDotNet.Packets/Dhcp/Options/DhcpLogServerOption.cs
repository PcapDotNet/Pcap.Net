﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// The log server option specifies a list of MIT-LCS UDP log servers
    /// available to the client. Servers SHOULD be listed in order of
    /// preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// |  7  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpLogServerOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpLogServerOption
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpLogServerOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.LogServer)
        {
        }

        internal static DhcpLogServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpLogServerOption>(data, ref offset, p => new DhcpLogServerOption(p));
        }
    }
}