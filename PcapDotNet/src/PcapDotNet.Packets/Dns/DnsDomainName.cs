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
    public class DnsDomainName
    {
        private const byte MaxLabelLength = 63;
        private const ushort OffsetMask = 0xC000;

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
                _ascii = _labels.Select(label => label.ToString(Encoding.ASCII)).Concat(string.Empty).SequenceToString('.');
            return _ascii;
        }

        internal int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            int length = 0;
            for (int i = 0; i != NumLabels; ++i)
            {
                ListSegment<DataSegment> labels = new ListSegment<DataSegment>(_labels, i);
                if (compressionData.IsAvailable(labels))
                    return length + sizeof(ushort);
                length += sizeof(byte) + _labels[i].Length;
                compressionData.AddCompressionData(labels, offsetInDns + length);
            }
            return length + sizeof(byte);
        }

        internal static DnsDomainName Parse(DnsDatagram dns, int offsetInDns, out int numBytesRead)
        {
            List<DataSegment> labels = new List<DataSegment>();
            ReadLabels(dns, offsetInDns, out numBytesRead, labels);
            return new DnsDomainName(labels);
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
                }
            } while (labelLength != 0);
        }

        private readonly List<DataSegment> _labels;
        private string _ascii;
    }
}