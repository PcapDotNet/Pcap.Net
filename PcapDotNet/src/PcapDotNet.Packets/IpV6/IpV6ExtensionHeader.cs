using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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

        public const int MinimumLength = 8;

        public override sealed bool Equals(IpV6ExtensionHeader other)
        {
            return other != null &&
                   Protocol == other.Protocol && NextHeader == other.NextHeader && EqualsData(other);
        }

        internal abstract bool EqualsData(IpV6ExtensionHeader other);

        internal IpV6ExtensionHeaderStandard(IpV4Protocol nextHeader) 
            : base(nextHeader)
        {
        }

        internal override sealed void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.NextHeader, (byte)NextHeader);
            int length = Length;
            buffer.Write(offset + Offset.HeaderExtensionLength, (byte)((length / 8) - 1));
            WriteData(buffer, offset + Offset.Data);
            offset += length;
        }

        internal abstract void WriteData(byte[] buffer, int offset);

        public override sealed int Length { get { return Offset.Data + DataLength; } }
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
            extensionHeaderLength = (extensionHeader[Offset.HeaderExtensionLength] + 1) * 8;
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

    /// <summary>
    /// RFC 2460.
    /// </summary>
    public abstract class IpV6ExtensionHeader : IEquatable<IpV6ExtensionHeader>
    {
        public abstract IpV4Protocol Protocol { get; }

        public IpV4Protocol? NextHeader { get; private set; }

        public abstract int Length { get; }

        public static ReadOnlyCollection<IpV4Protocol> ExtensionHeaders
        {
            get { return _extensionHeaders; }
        }

        public abstract bool IsValid { get; }

        public override sealed bool Equals(object obj)
        {
            return Equals(obj as IpV6ExtensionHeader);
        }

        public abstract bool Equals(IpV6ExtensionHeader other);

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

    public class IpV6ExtensionHeaders : IEnumerable<IpV6ExtensionHeader>
    {
        public IpV6ExtensionHeaders(ReadOnlyCollection<IpV6ExtensionHeader> extensionHeaders)
        {
            Headers = extensionHeaders;
            IsValid = true;
        }

        public IpV6ExtensionHeaders(params IpV6ExtensionHeader[] extensionHeaders)
            : this(extensionHeaders.AsReadOnly())
        {
        }

        public IpV6ExtensionHeaders(IList<IpV6ExtensionHeader> extensionHeaders)
            : this(extensionHeaders.AsReadOnly())
        {
        }

        public IpV6ExtensionHeaders(IEnumerable<IpV6ExtensionHeader> extensionHeaders)
            : this(extensionHeaders.ToArray())
        {
        }

        public IEnumerator<IpV6ExtensionHeader> GetEnumerator()
        {
            return Headers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IpV6ExtensionHeader this[int index]
        {
            get { return Headers[index]; }
        }

        public ReadOnlyCollection<IpV6ExtensionHeader> Headers { get; private set; }
        public bool IsValid { get; private set; }

        public IpV4Protocol? FirstHeader
        {
            get
            {
                if (!Headers.Any()) 
                    return null;
                return Headers[0].Protocol;
            }
        }

        public IpV4Protocol? NextHeader
        {
            get
            {
                if (!Headers.Any())
                    return null;
                return Headers[Headers.Count - 1].Protocol;
            }
        }

        public static IpV6ExtensionHeaders Empty
        {
            get { return _empty; }
        }

        internal IpV6ExtensionHeaders(DataSegment data, IpV4Protocol firstHeader)
        {
            IpV4Protocol? nextHeader = firstHeader;
            List<IpV6ExtensionHeader> headers = new List<IpV6ExtensionHeader>();
            while (data.Length >= 8 && nextHeader.HasValue && IpV6ExtensionHeader.IsExtensionHeader(nextHeader.Value))
            {
                int numBytesRead;
                IpV6ExtensionHeader extensionHeader = IpV6ExtensionHeader.CreateInstance(nextHeader.Value, data, out numBytesRead);
                if (extensionHeader == null)
                    break;
                headers.Add(extensionHeader);
                nextHeader = extensionHeader.NextHeader;
                data = data.Subsegment(numBytesRead, data.Length - numBytesRead);
            }
            Headers = headers.AsReadOnly();
            IsValid = (!nextHeader.HasValue || !IpV6ExtensionHeader.IsExtensionHeader(nextHeader.Value)) && headers.All(header => header.IsValid);
        }

        private static readonly IpV6ExtensionHeaders _empty = new IpV6ExtensionHeaders();

        public void Write(byte[] buffer, int offset)
        {
            foreach (IpV6ExtensionHeader extensionHeader in this)
                extensionHeader.Write(buffer, ref offset);
        }
    }
}
