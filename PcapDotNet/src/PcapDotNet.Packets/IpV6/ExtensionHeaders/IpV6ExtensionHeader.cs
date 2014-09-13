using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2460.
    /// </summary>
    public abstract class IpV6ExtensionHeader : IEquatable<IpV6ExtensionHeader>
    {
        /// <summary>
        /// Identifies the type of this extension header.
        /// </summary>
        public abstract IpV4Protocol Protocol { get; }

        /// <summary>
        /// Identifies the type of header immediately following this extension header.
        /// </summary>
        public IpV4Protocol? NextHeader { get; private set; }

        /// <summary>
        /// The number of bytes this extension header takes.
        /// </summary>
        public abstract int Length { get; }

        /// <summary>
        /// List of all extension header protocols.
        /// </summary>
        public static ReadOnlyCollection<IpV4Protocol> ExtensionHeaders
        {
            get { return _extensionHeaders; }
        }

        /// <summary>
        /// True iff the extension header parsing didn't encounter an issue.
        /// </summary>
        public abstract bool IsValid { get; }

        /// <summary>
        /// True iff the given extension header is equal to this extension header.
        /// </summary>
        public sealed override bool Equals(object obj)
        {
            return Equals(obj as IpV6ExtensionHeader);
        }

        /// <summary>
        /// True iff the given extension header is equal to this extension header.
        /// </summary>
        public abstract bool Equals(IpV6ExtensionHeader other);

        /// <summary>
        /// Returns a hash code of the extension header.
        /// </summary>
        public abstract override int GetHashCode();

        internal static bool IsExtensionHeader(IpV4Protocol nextHeader)
        {
            if (IpV6ExtensionHeaderStandard.IsStandard(nextHeader))
                return true;

            switch (nextHeader)
            {
                case IpV4Protocol.EncapsulatingSecurityPayload: // 50
                case IpV4Protocol.AuthenticationHeader: // 51
                    return true;

                default:
                    return false;
            }
        }

        internal IpV6ExtensionHeader(IpV4Protocol? nextHeader)
        {
            NextHeader = nextHeader;
        }

        internal static IpV6ExtensionHeader CreateInstance(IpV4Protocol nextHeader, DataSegment extensionHeaderData, out int numBytesRead)
        {
            if (IpV6ExtensionHeaderStandard.IsStandard(nextHeader))
                return IpV6ExtensionHeaderStandard.CreateInstanceStandard(nextHeader, extensionHeaderData, out numBytesRead);

            switch (nextHeader)
            {
                case IpV4Protocol.EncapsulatingSecurityPayload: // 50
                    return IpV6ExtensionHeaderEncapsulatingSecurityPayload.CreateInstance(extensionHeaderData, out numBytesRead);

                case IpV4Protocol.AuthenticationHeader: // 51
                    return IpV6ExtensionHeaderAuthentication.CreateInstance(extensionHeaderData, out numBytesRead);

                default:
                    throw new InvalidOperationException("Invalid nextHeader value" + nextHeader);
            }
        }

        internal static void GetNextNextHeaderAndLength(IpV4Protocol nextHeader, DataSegment data, out IpV4Protocol? nextNextHeader,
                                                        out int extensionHeaderLength)
        {
            if (IpV6ExtensionHeaderStandard.IsStandard(nextHeader))
            {
                IpV6ExtensionHeaderStandard.GetNextNextHeaderAndLength(data, out nextNextHeader, out extensionHeaderLength);
                return;
            }

            switch (nextHeader)
            {
                case IpV4Protocol.EncapsulatingSecurityPayload: // 50
                    IpV6ExtensionHeaderEncapsulatingSecurityPayload.GetNextNextHeaderAndLength(data, out nextNextHeader, out extensionHeaderLength);
                    break;

                case IpV4Protocol.AuthenticationHeader: // 51
                    IpV6ExtensionHeaderAuthentication.GetNextNextHeaderAndLength(data, out nextNextHeader, out extensionHeaderLength);
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Invalid nextHeader value {0}", nextHeader));
            }
        }

        internal abstract void Write(byte[] buffer, ref int offset);

        private static readonly ReadOnlyCollection<IpV4Protocol> _extensionHeaders =
            IpV6ExtensionHeaderStandard.StandardExtensionHeaders.Concat(
                new[]
                    {
                        IpV4Protocol.EncapsulatingSecurityPayload,
                        IpV4Protocol.AuthenticationHeader
                    }).
                ToArray().AsReadOnly();
    }
}
