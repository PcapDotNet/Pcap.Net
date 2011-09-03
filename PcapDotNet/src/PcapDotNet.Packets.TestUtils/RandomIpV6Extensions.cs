using System;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.TestUtils
{
    public static class RandomIpV6Extensions
    {
        public static IpV6Address NextIpV6Address(this Random random)
        {
            return new IpV6Address(random.NextUInt128());
        }
    }
}