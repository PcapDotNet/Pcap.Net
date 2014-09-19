using System;
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
    public abstract class IpV6ExtensionHeaderOptions : IpV6ExtensionHeaderStandard
    {
        /// <summary>
        /// Extension header options.
        /// </summary>
        public IpV6Options Options { get; private set; }

        /// <summary>
        /// True iff the extension header parsing didn't encounter an issue.
        /// </summary>
        public sealed override bool IsValid
        {
            get { return Options.IsValid; }
        }

        internal sealed override int DataLength
        {
            get { return Options.BytesLength; }
        }

        internal sealed override bool EqualsData(IpV6ExtensionHeader other)
        {
            return EqualsData(other as IpV6ExtensionHeaderOptions);
        }

        internal sealed override int GetDataHashCode()
        {
            return Options.GetHashCode();
        }

        internal override void WriteData(byte[] buffer, int offset)
        {
            Options.Write(buffer, offset);
        }

        internal IpV6ExtensionHeaderOptions(IpV4Protocol nextHeader, IpV6Options options)
            : base(nextHeader)
        {
            if (options.BytesLength % 8 != 6)
                options = options.Pad((14 - options.BytesLength % 8) % 8);
            Options = options;
        }

        private bool EqualsData(IpV6ExtensionHeaderOptions other)
        {
            return other != null &&
                   Options.Equals(other.Options);
        }
    }
}