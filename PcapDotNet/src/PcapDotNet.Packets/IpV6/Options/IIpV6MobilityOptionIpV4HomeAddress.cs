using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5844.
    /// </summary>
    public interface IIpV6MobilityOptionIpV4HomeAddress
    {
        /// <summary>
        /// The prefix length of the address.
        /// </summary>
        byte PrefixLength { get; }

        /// <summary>
        /// Contains the IPv4 home address.
        /// </summary>
        IpV4Address HomeAddress { get; }
    }
}