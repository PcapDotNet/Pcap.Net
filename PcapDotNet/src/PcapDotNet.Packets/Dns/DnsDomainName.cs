using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// A domain name represented as a series of labels, and terminated by a label with zero length.
    /// </summary>
    public sealed class DnsDomainName : IEquatable<DnsDomainName>
    {
        /// <summary>
        /// The number of bytes a root ('.') domain name takes.
        /// </summary>
        public const int RootLength = sizeof(byte);
        private const byte MaxLabelLength = 63;
        private const ushort CompressionMarker = 0xC000;
        internal const ushort OffsetMask = 0x3FFF;
        private static readonly char[] Colon = new[] {'.'};

        /// <summary>
        /// Creates a domain name out of a string.
        /// </summary>
        public DnsDomainName(string domainName)
        {
            if (domainName == null) 
                throw new ArgumentNullException("domainName");

            string[] labels = domainName.Split(Colon, StringSplitOptions.RemoveEmptyEntries);
            _labels = labels.Select(label => new DataSegment(Encoding.UTF8.GetBytes(label))).ToList();
        }

        /// <summary>
        /// The root ('.') domain name.
        /// </summary>
        public static DnsDomainName Root { get { return _root; } }

        /// <summary>
        /// The number of labels the domain name has.
        /// For example, root ('.') has 0 labels.
        /// </summary>
        public int LabelsCount
        {
            get
            {
                return _labels.Count;
            }
        }

        /// <summary>
        /// Returns true iff the domain name is the root ('.') domain name.
        /// </summary>
        public bool IsRoot
        {
            get { return LabelsCount == 0; }
        }

        /// <summary>
        /// The number of bytes the domain name takes assuming it won't be compressed.
        /// </summary>
        public int NonCompressedLength
        {
            get
            {
                return _labels.Sum(label => label.Length + sizeof(byte)) + sizeof(byte);
            }
        }

        /// <summary>
        /// Returns a string representation of the domain name.
        /// </summary>
        public override string ToString()
        {
            if (_utf8 == null)
                _utf8 = _labels.Select(label => label.Decode(Encoding.UTF8)).SequenceToString('.') + ".";
            return _utf8;
        }

        /// <summary>
        /// Two domain names are equal if their labels are equal ignoring any casing.
        /// </summary>
        public bool Equals(DnsDomainName other)
        {
            return other != null &&
                   string.Equals(ToString(), other.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Two domain names are equal if their labels are equal ignoring any casing.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsDomainName);
        }

        /// <summary>
        /// Returns the hash code of the domain name.
        /// </summary>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        internal int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            int length = 0;
            for (int i = 0; i != LabelsCount; ++i)
            {
                ListSegment<DataSegment> labels = new ListSegment<DataSegment>(_labels, i);
                if (compressionData.IsAvailable(labels))
                    return length + sizeof(ushort);
                compressionData.AddCompressionData(labels, offsetInDns + length);
                length += sizeof(byte) + labels[0].Length;
            }
            return length + sizeof(byte);
        }

        internal static bool TryParse(DnsDatagram dns, int offsetInDns, int maximumLength, out DnsDomainName domainName, out int numBytesRead)
        {
            List<DataSegment> labels = new List<DataSegment>();
            if (!TryReadLabels(dns, offsetInDns, out numBytesRead, labels) || numBytesRead > maximumLength)
            {
                domainName = null;
                return false;
            }
            domainName = new DnsDomainName(labels);
            return true;
        }

        internal void WriteUncompressed(byte[] buffer, int offset)
        {
            foreach (DataSegment label in _labels)
            {
                buffer.Write(ref offset, (byte)label.Length);
                label.Write(buffer, ref offset);
            }
        }

        internal int Write(byte[] buffer, int dnsOffset, DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            int length = 0;
            for (int i = 0; i != LabelsCount; ++i)
            {
                ListSegment<DataSegment> labels = new ListSegment<DataSegment>(_labels, i);
                int pointerOffset;
                if (compressionData.TryGetOffset(labels, out pointerOffset))
                {
                    buffer.Write(dnsOffset + offsetInDns + length, (ushort)(CompressionMarker | (ushort)pointerOffset), Endianity.Big);
                    return length + sizeof(ushort);
                }
                DataSegment currentLabel = labels[0];
                compressionData.AddCompressionData(labels, offsetInDns + length);
                buffer.Write(dnsOffset + offsetInDns + length, (byte)currentLabel.Length);
                length += sizeof(byte);
                currentLabel.Write(buffer, dnsOffset + offsetInDns + length);
                length += currentLabel.Length;
            }
            return length + sizeof(byte);
        }

        private DnsDomainName(List<DataSegment> labels)
        {
            _labels = labels;
        }

        private static bool TryReadLabels(DnsDatagram dns, int offsetInDns, out int numBytesRead, List<DataSegment> labels)
        {
            numBytesRead = 0;
            byte labelLength;
            do
            {
                if (offsetInDns >= dns.Length)
                    return false;  // Can't read label's length.
                labelLength = dns[offsetInDns];
                ++numBytesRead;
                if (labelLength > MaxLabelLength)
                {
                    // Compression.
                    if (offsetInDns + 1 >= dns.Length)
                        return false;  // Can't read compression pointer.
                    int newOffsetInDns = dns.ReadUShort(offsetInDns, Endianity.Big) & OffsetMask;
                    if (newOffsetInDns >= offsetInDns)
                        return false;  // Can't handle pointers that are not back pointers.
                    ++numBytesRead;
                    int internalBytesRead;
                    return TryReadLabels(dns, newOffsetInDns, out internalBytesRead, labels);
                }
                
                if (labelLength != 0)
                {
                    ++offsetInDns;
                    if (offsetInDns + labelLength >= dns.Length)
                        return false;  // Can't read label.
                    labels.Add(dns.Subsegment(offsetInDns, labelLength));
                    numBytesRead += labelLength;
                    offsetInDns += labelLength;
                }
            } while (labelLength != 0);

            return true;
        }

        private static readonly DnsDomainName _root = new DnsDomainName("");

        private readonly List<DataSegment> _labels;
        private string _utf8;
    }
}