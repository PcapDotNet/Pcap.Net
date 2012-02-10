using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    internal class DnsDomainNameCompressionData
    {
        public DnsDomainNameCompressionData(DnsDomainNameCompressionMode domainNameCompressionMode)
        {
            DomainNameCompressionMode = domainNameCompressionMode;
        }

        public DnsDomainNameCompressionMode DomainNameCompressionMode { get; private set; }

        public bool IsAvailable(ListSegment<DataSegment> labels)
        {
            int offsetInDns;
            return TryGetOffset(labels, out offsetInDns);
        }

        public bool TryGetOffset(ListSegment<DataSegment> labels, out int offsetInDns)
        {
            switch (DomainNameCompressionMode)
            {
                case DnsDomainNameCompressionMode.All:
                    return _data.TryGetValue(labels, out offsetInDns);
                case DnsDomainNameCompressionMode.Nothing:
                    offsetInDns = 0;
                    return false;
                default:
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Invalid DomainNameCompressionMode {0}",
                                                                      DomainNameCompressionMode));
            }
        }

        public void AddCompressionData(ListSegment<DataSegment> labels, int dnsOffset)
        {
            if (dnsOffset > DnsDomainName.OffsetMask)
                return;

            switch (DomainNameCompressionMode)
            {
                case DnsDomainNameCompressionMode.All:
                    if (!_data.ContainsKey(labels))
                        _data.Add(labels, dnsOffset);
                    return;
                case DnsDomainNameCompressionMode.Nothing:
                    return;
                default:
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Invalid DomainNameCompressionMode {0}",
                                                                      DomainNameCompressionMode));
            }
        }

        private static readonly InlineEqualityComparer<ListSegment<DataSegment>> _labelsComparer =
            new InlineEqualityComparer<ListSegment<DataSegment>>((x, y) => x.SequenceEqual(y), obj => obj.SequenceGetHashCode());

        private readonly Dictionary<ListSegment<DataSegment>, int> _data = new Dictionary<ListSegment<DataSegment>, int>(_labelsComparer);
    }
}