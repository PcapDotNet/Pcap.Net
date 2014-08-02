using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    public class IpV6ExtensionHeaders : IEnumerable<IpV6ExtensionHeader>, IEquatable<IpV6ExtensionHeaders>
    {
        public IpV6ExtensionHeaders(ReadOnlyCollection<IpV6ExtensionHeader> extensionHeaders)
        {
            for (int i = 0; i < extensionHeaders.Count; ++i)
            {
                if (extensionHeaders[i].Protocol == IpV4Protocol.EncapsulatingSecurityPayload && i != extensionHeaders.Count - 1)
                {
                    throw new ArgumentException(
                        string.Format("EncapsulatingSecurityPayload can only be the last extension header. However it is the {0} out of {1}.", (i + 1),
                                      extensionHeaders.Count), "extensionHeaders");
                }
            }
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
            : this((IpV6ExtensionHeader[])extensionHeaders.ToArray())
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

        public IpV4Protocol? LastHeader
        {
            get
            {
                if (!Headers.Any())
                    return null;
                return Headers[Headers.Count - 1].Protocol;
            }
        }

        public IpV4Protocol? NextHeader
        {
            get
            {
                if (!Headers.Any())
                    return null;
                return Headers[Headers.Count - 1].NextHeader;
            }
        }

        public static IpV6ExtensionHeaders Empty
        {
            get { return _empty; }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IpV6ExtensionHeaders);
        }

        public bool Equals(IpV6ExtensionHeaders other)
        {
            return other != null && this.SequenceEqual(other);
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

        internal void Write(byte[] buffer, int offset)
        {
            foreach (IpV6ExtensionHeader extensionHeader in this)
                extensionHeader.Write(buffer, ref offset);
        }

        private static readonly IpV6ExtensionHeaders _empty = new IpV6ExtensionHeaders();
    }
}