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
        public IpV6Options Options { get; private set; }

        public override sealed bool IsValid
        {
            get { return Options.IsValid; }
        }

        internal override sealed int DataLength
        {
            get { return Options.BytesLength; }
        }

        internal override sealed bool EqualsData(IpV6ExtensionHeader other)
        {
            return EqualsData(other as IpV6ExtensionHeaderOptions);
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