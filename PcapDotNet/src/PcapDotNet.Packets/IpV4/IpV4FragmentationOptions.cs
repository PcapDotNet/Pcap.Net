using System;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Fragmentation information flags for IPv4 datagram.
    /// </summary>
    [Flags]
    public enum IpV4FragmentationOptions : ushort
    {
        /// <summary>
        /// May Fragment, Last Fragment.
        /// </summary>
        None = 0x0 << 13,

        /// <summary>
        /// More Fragments.
        /// </summary>
        MoreFragments = 0x1 << 13,

        /// <summary>
        /// Don't Fragment.
        /// </summary>
        DoNotFragment = 0x2 << 13
    }
}