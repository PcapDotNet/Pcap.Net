using System;

namespace PcapDotNet.Packets
{
    [Flags]
    public enum IpV4FragmentationOptions : ushort
    {
        None =          0x0 << 13,
//        Reserved =      0x4 << 13,
        DoNotFragment = 0x2 << 13,
        MoreFragments = 0x1 << 13
    }
}