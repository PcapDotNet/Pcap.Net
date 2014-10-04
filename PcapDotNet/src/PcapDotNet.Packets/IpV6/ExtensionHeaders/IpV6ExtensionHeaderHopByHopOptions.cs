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
    public sealed class IpV6ExtensionHeaderHopByHopOptions : IpV6ExtensionHeaderOptions
    {
        /// <summary>
        /// Creates an instance from next header and options.
        /// </summary>
        /// <param name="nextHeader">Identifies the type of header immediately following this extension header.</param>
        /// <param name="options">Options for the extension header.</param>
        public IpV6ExtensionHeaderHopByHopOptions(IpV4Protocol? nextHeader, IpV6Options options)
            : base(nextHeader, options)
        {
        }

        /// <summary>
        /// Identifies the type of this extension header.
        /// </summary>
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
