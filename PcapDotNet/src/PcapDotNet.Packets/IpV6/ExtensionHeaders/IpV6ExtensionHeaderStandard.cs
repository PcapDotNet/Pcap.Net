using System;
using System.Collections.ObjectModel;
using PcapDotNet.Base;
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
    /// | 16  | Data                                  |
    /// | ... |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6ExtensionHeaderStandard : IpV6ExtensionHeader
    {
        private static class Offset
        {
            public const int NextHeader = 0;
            public const int HeaderExtensionLength = NextHeader + sizeof(byte);
            public const int Data = HeaderExtensionLength + sizeof(byte);
        }

        /// <summary>
        /// The minimum number of bytes the extension header takes.
        /// </summary>
        public const int MinimumLength = 8;

        /// <summary>
        /// True iff the given extension header is equal to this extension header.
        /// </summary>
        public sealed override bool Equals(IpV6ExtensionHeader other)
        {
            return other != null &&
                   Protocol == other.Protocol && NextHeader == other.NextHeader && EqualsData(other);
        }

        /// <summary>
        /// Returns a hash code of the extension header.
        /// </summary>
        public sealed override int GetHashCode()
        {
            return Sequence.GetHashCode(Protocol, NextHeader, GetDataHashCode());
        }

        /// <summary>
        /// The number of bytes this extension header takes.
        /// </summary>
        public sealed override int Length { get { return Offset.Data + DataLength; } }

        internal abstract bool EqualsData(IpV6ExtensionHeader other);

        internal abstract int GetDataHashCode();

        internal IpV6ExtensionHeaderStandard(IpV4Protocol nextHeader) 
            : base(nextHeader)
        {
        }

        internal sealed override void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.NextHeader, (byte)NextHeader);
            int length = Length;
            buffer.Write(offset + Offset.HeaderExtensionLength, (byte)((length / 8) - 1));
            WriteData(buffer, offset + Offset.Data);
            offset += length;
        }

        internal abstract void WriteData(byte[] buffer, int offset);

        internal abstract int DataLength { get; }

        internal static bool IsStandard(IpV4Protocol nextHeader)
        {
            switch (nextHeader)
            {
                case IpV4Protocol.IpV6HopByHopOption: // 0
                case IpV4Protocol.IpV6Route: // 43
                case IpV4Protocol.FragmentHeaderForIpV6: // 44
                case IpV4Protocol.IpV6Opts: // 60
                case IpV4Protocol.MobilityHeader: // 135
                    return true;

                default:
                    return false;
            }
        }

        internal static IpV6ExtensionHeader CreateInstanceStandard(IpV4Protocol nextHeader, DataSegment extensionHeaderData, out int numBytesRead)
        {
            numBytesRead = 0;
            if (extensionHeaderData.Length < MinimumLength)
                return null;
            IpV4Protocol nextNextHeader = (IpV4Protocol)extensionHeaderData[Offset.NextHeader];
            int length = (extensionHeaderData[Offset.HeaderExtensionLength] + 1) * 8;
            if (extensionHeaderData.Length < length)
                return null;

            DataSegment data = extensionHeaderData.Subsegment(Offset.Data, length - Offset.Data);
            numBytesRead = length;

            // TODO: Implement Shim6.
            switch (nextHeader)
            {
                case IpV4Protocol.IpV6HopByHopOption: // 0
                    return IpV6ExtensionHeaderHopByHopOptions.ParseData(nextNextHeader, data);

                case IpV4Protocol.IpV6Route: // 43
                    return IpV6ExtensionHeaderRouting.ParseData(nextNextHeader, data);

                case IpV4Protocol.FragmentHeaderForIpV6: // 44
                    return IpV6ExtensionHeaderFragmentData.ParseData(nextNextHeader, data);

                case IpV4Protocol.IpV6Opts:                     // 60
                    return IpV6ExtensionHeaderDestinationOptions.ParseData(nextNextHeader, data);

                case IpV4Protocol.MobilityHeader:        // 135
                    return IpV6ExtensionHeaderMobility.ParseData(nextNextHeader, data);

                default:
                    throw new InvalidOperationException("Invalid nextHeader value" + nextHeader);
            }
        }

        internal static void GetNextNextHeaderAndLength(DataSegment extensionHeader, out IpV4Protocol? nextNextHeader, out int extensionHeaderLength)
        {
            if (extensionHeader.Length < MinimumLength)
            {
                nextNextHeader = null;
                extensionHeaderLength = extensionHeader.Length;
                return;
            }
            nextNextHeader = (IpV4Protocol)extensionHeader[Offset.NextHeader];
            extensionHeaderLength = Math.Min(extensionHeader.Length / 8 * 8, (extensionHeader[Offset.HeaderExtensionLength] + 1) * 8);
        }

        internal static ReadOnlyCollection<IpV4Protocol> StandardExtensionHeaders
        {
            get { return _standardExtensionHeaders; }
        }

        private static readonly ReadOnlyCollection<IpV4Protocol> _standardExtensionHeaders =
            new[]
                {
                    IpV4Protocol.IpV6HopByHopOption,
                    IpV4Protocol.IpV6Route,
                    IpV4Protocol.FragmentHeaderForIpV6,
                    IpV4Protocol.IpV6Opts,
                    IpV4Protocol.MobilityHeader
                }.AsReadOnly();
    }
}