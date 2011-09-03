﻿using System;
using PcapDotNet.Packets.Transport;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.TestUtils
{
    public static class RandomUdpExtensions
    {
        public static UdpLayer NextUdpLayer(this Random random)
        {
            return new UdpLayer
                   {
                       Checksum = random.NextUShort(),
                       SourcePort = random.NextUShort(),
                       DestinationPort = random.NextUShort(),
                       CalculateChecksumValue = random.NextBool()
                   };
        }
    }
}