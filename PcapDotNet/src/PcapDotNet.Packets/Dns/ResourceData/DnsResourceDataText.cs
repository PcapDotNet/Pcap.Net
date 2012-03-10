using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// A resource data that contains 0 or more DNS strings.
    /// Each DNS string is a segment of up to 255 bytes.
    /// The format of each DNS string is one byte for the length of the string and then the specified number of bytes.
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Txt)]
    [DnsTypeRegistration(Type = DnsType.Spf)]
    public sealed class DnsResourceDataText : DnsResourceDataStrings
    {
        /// <summary>
        /// Constructs the resource data from the given list of strings, each up to 255 bytes.
        /// </summary>
        public DnsResourceDataText(ReadOnlyCollection<DataSegment> strings)
            : base(strings)
        {
        }

        /// <summary>
        /// The list of strings, each up to 255 bytes.
        /// </summary>
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