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
    public class DnsDomainName : IEquatable<DnsDomainName>
    {
        private const byte MaxLabelLength = 63;
        private const ushort CompressionMarker = 0xC000;
        private const ushort OffsetMask = 0x3FFF;
        private static readonly char[] Colon = new[] {'.'};

        public DnsDomainName(string domainName)
        {
            string[] labels = domainName.ToLowerInvariant().Split(Colon, StringSplitOptions.RemoveEmptyEntries);
            _labels = labels.Select(label => new DataSegment(Encoding.UTF8.GetBytes(label))).ToList();
        }

        public int NumLabels
        {
            get
            {
                return _labels.Count;
            }
        }

        public override string ToString()
        {
            if (_ascii == null)
                _ascii = _labels.Select(label => label.ToString(Encoding.UTF8)).SequenceToString('.') + ".";
            return _ascii;
        }

        public bool Equals(DnsDomainName other)
        {
            return _labels.SequenceEqual(other._labels);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DnsDomainName);
        }

        internal int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            int length = 0;
            for (int i = 0; i != NumLabels; ++i)
            {
                ListSegment<DataSegment> labels = new ListSegment<DataSegment>(_labels, i);
                if (compressionData.IsAvailable(labels))
                    return length + sizeof(ushort);
                compressionData.AddCompressionData(labels, offsetInDns + length);
                length += sizeof(byte) + labels[0].Length;
            }
            return length + sizeof(byte);
        }

        internal static DnsDomainName Parse(DnsDatagram dns, int offsetInDns, out int numBytesRead)
        {
            List<DataSegment> labels = new List<DataSegment>();
            ReadLabels(dns, offsetInDns, out numBytesRead, labels);
            return new DnsDomainName(labels);
        }

        internal int Write(byte[] buffer, int dnsOffset, DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            int length = 0;
            for (int i = 0; i != NumLabels; ++i)
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

        private static void ReadLabels(DnsDatagram dns, int offsetInDns, out int numBytesRead, List<DataSegment> labels)
        {
            numBytesRead = 0;
            byte labelLength;
            do
            {
                if (offsetInDns >= dns.Length)
                    return;
                labelLength = dns[offsetInDns];
                ++numBytesRead;
                if (labelLength > MaxLabelLength)
                {
                    // Compression.
                    if (offsetInDns + 1 >= dns.Length)
                        return;
                    offsetInDns = dns.ReadUShort(offsetInDns, Endianity.Big) & OffsetMask;
                    ++numBytesRead;
                    int internalBytesRead;
                    ReadLabels(dns, offsetInDns, out internalBytesRead, labels);
                    return;
                }
                
                if (labelLength != 0)
                {
                    ++offsetInDns;
                    if (offsetInDns + labelLength >= dns.Length)
                        return;
                    labels.Add(dns.SubSegment(offsetInDns, labelLength));
                    numBytesRead += labelLength;
                    offsetInDns += labelLength;
                }
            } while (labelLength != 0);
        }

        private readonly List<DataSegment> _labels;
        private string _ascii;
    }
}