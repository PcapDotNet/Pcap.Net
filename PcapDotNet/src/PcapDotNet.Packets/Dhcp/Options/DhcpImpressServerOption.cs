﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    public class DhcpImpressServerOption : DhcpAddressListOption
    {
        public DhcpImpressServerOption(IList<IpV4Address> addresses) : base(DhcpOptionCode.ImpressServer, addresses)
        {
        }

        internal static DhcpImpressServerOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            return new DhcpImpressServerOption(GetAddresses(data, length, ref offset));
        }
    }
}