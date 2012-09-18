using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2460.
    /// For IpV6HopByHopOption, IpV6Route and FragmentHeaderForIpV6 we use the following format:
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
    public abstract class IpV6ExtensionHeader
    {
        private static class Offset
        {
            public const int NextHeader = 0;
            public const int HeaderExtensionLength = 1;
            public const int Data = 2;
        }

        public const int MinimumLength = 8;

        public IpV4Protocol? NextHeader { get; private set; }

        protected IpV6ExtensionHeader(IpV4Protocol? nextHeader)
        {
            NextHeader = nextHeader;
        }

        internal static IpV6ExtensionHeader CreateInstance(IpV4Protocol nextHeader, DataSegment extensionHeaderData, out int numBytesRead)
        {
            switch (nextHeader)
            {
                case IpV4Protocol.IpV6HopByHopOption: // 0
                case IpV4Protocol.IpV6Route: // 43
                case IpV4Protocol.FragmentHeaderForIpV6: // 44
                    numBytesRead = 0;
                    if (extensionHeaderData.Length < MinimumLength)
                        return null;
                    IpV4Protocol nextNextHeader = (IpV4Protocol)extensionHeaderData[Offset.NextHeader];
                    int length = (extensionHeaderData[Offset.HeaderExtensionLength] + 1) * 8;
                    if (extensionHeaderData.Length < length)
                        return null;

                    DataSegment data = extensionHeaderData.Subsegment(Offset.Data, length - Offset.Data);
                    numBytesRead = data.Length;

                    return CreateStandardInstance(nextHeader, nextNextHeader, data);

                case IpV4Protocol.EncapsulatingSecurityPayload: // 50
                    return IpV6ExtensionHeaderEncapsulatingSecurityPayload.CreateInstance(extensionHeaderData, out numBytesRead);

                    /*
    case IpV4Protocol.AuthenticationHeader:         // 51
        return IpV6ExtensionHeaderAuthentication.Parse(data);

    case IpV4Protocol.IpV6Opts:                     // 60
        return IpV6ExtensionHeaderDestinationOptions.Parse(data);

    case IpV4Protocol.MobilityHeader:               // 135
        return IpV6MobilityExtensionHeader.Parse(data);
        */
                default:
                    throw new InvalidOperationException("Invalid nextHeader value" + nextHeader);
            }

        }

        private static IpV6ExtensionHeader CreateStandardInstance(IpV4Protocol nextHeader, IpV4Protocol nextNextHeader, DataSegment data)
        {
            switch (nextHeader)
            {
                case IpV4Protocol.IpV6HopByHopOption: // 0
                    return IpV6ExtensionHeaderHopByHopOptions.ParseData(nextNextHeader, data);

                case IpV4Protocol.IpV6Route: // 43
                    return IpV6ExtensionHeaderRouting.ParseData(nextNextHeader, data);

                case IpV4Protocol.FragmentHeaderForIpV6: // 44
                    return IpV6ExtensionHeaderFragmentData.ParseData(nextNextHeader, data);

                default:
                    throw new InvalidOperationException("Invalid nextHeader value" + nextHeader);
            }
        }
    }
}