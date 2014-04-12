using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2460.
    /// <pre>
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | Options                               |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public class IpV6ExtensionHeaderHopByHopOptions : IpV6ExtensionHeaderOptions
    {
        public IpV6ExtensionHeaderHopByHopOptions(IpV4Protocol nextHeader, IpV6Options options)
            : base(nextHeader, options)
        {
        }

        public override IpV4Protocol Protocol
        {
            get { return IpV4Protocol.IpV6HopByHopOption; }
        }

        internal static IpV6ExtensionHeaderHopByHopOptions ParseData(IpV4Protocol nextHeader, DataSegment data)
        {
            IpV6Options options = new IpV6Options(data);
            return new IpV6ExtensionHeaderHopByHopOptions(nextHeader, options);
        }
    }
}
