using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// <pre>
    /// +---------+
    /// | Strings |
    /// +---------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Txt)]
    [DnsTypeRegistration(Type = DnsType.Spf)]
    public sealed class DnsResourceDataText : DnsResourceDataStrings
    {
        public DnsResourceDataText(ReadOnlyCollection<DataSegment> strings)
            : base(strings)
        {
        }

        public ReadOnlyCollection<DataSegment> Text { get { return Strings; } }

        internal DnsResourceDataText()
        {
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            List<DataSegment> strings = ReadStrings(data);
            return new DnsResourceDataText(strings.AsReadOnly());
        }
    }
}