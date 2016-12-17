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
    /// The router option specifies a list of IP addresses for routers on the
    /// client's subnet. Routers SHOULD be listed in order of preference.
    /// <pre>
    ///  Code   Len         Address 1               Address 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// |  3  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpRouterOption : DhcpAddressListOption
    {
        /// <summary>
        /// create new DhcpRouterOption
        /// </summary>
        /// <param name="addresses">Addresses</param>
        public DhcpRouterOption(IList<IpV4Address> addresses) : base(addresses, DhcpOptionCode.Router)
        {
        }

        internal static DhcpRouterOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpRouterOption>(data, ref offset, p => new DhcpRouterOption(p));
        }
    }
}