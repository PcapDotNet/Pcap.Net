using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2460.
    /// +-----+-------------+-------------------------+
    /// | Bit | 0-7         | 8-15                    |
    /// +-----+-------------+-------------------------+
    /// | 0   | Next Header | Header Extension Length |
    /// +-----+-------------+-------------------------+
    /// | 16  | Options                               |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </summary>
    public class IpV6ExtensionHeaderHopByHopOptions : IpV6ExtensionHeader
    {
        private static class Offset
        {
            public const int NextHeader = 0;
            public const int HeaderExtensionLength = 1;
            public const int Options = 2;
        }

        public const int MinimumLength = 8;

        private IpV6ExtensionHeaderHopByHopOptions(IpV4Protocol nextHeader, IpV6Options options)
            : base(nextHeader)
        {
            Options = options;
        }

        public IpV6Options Options { get; private set; }

        internal static IpV6ExtensionHeaderHopByHopOptions Parse(DataSegment data, out int numBytesRead)
        {
            numBytesRead = 0;
            if (data.Length < MinimumLength)
                return null;
            IpV4Protocol nextHeader = (IpV4Protocol)data[Offset.NextHeader];
            int length = (data[Offset.HeaderExtensionLength] + 1) * 8;
            if (data.Length < length)
                return null;

            IpV6Options options = new IpV6Options(data.Subsegment(Offset.Options, length - Offset.Options));
            numBytesRead = length;
            return new IpV6ExtensionHeaderHopByHopOptions(nextHeader, options);
        }
    }
}