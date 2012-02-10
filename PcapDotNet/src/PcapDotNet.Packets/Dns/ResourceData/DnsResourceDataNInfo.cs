using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// Reid.
    /// <pre>
    /// +---------------------+
    /// | One ore more strings|
    /// +---------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.NInfo)]
    public sealed class DnsResourceDataNInfo : DnsResourceDataStrings
    {
        private const int MinNumStrings = 1;

        public DnsResourceDataNInfo(ReadOnlyCollection<DataSegment> strings)
            : base(strings)
        {
            if (strings == null)
                throw new ArgumentNullException("strings");

            if (strings.Count < MinNumStrings)
                throw new ArgumentOutOfRangeException("strings", strings.Count, "There must be at least one string.");
        }

        public DnsResourceDataNInfo(params DataSegment[] strings)
            : this(strings.AsReadOnly())
        {
        }

        internal DnsResourceDataNInfo()
            : this(DataSegment.Empty)
        {
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            List<DataSegment> strings = ReadStrings(data, MinNumStrings);
            if (strings == null || strings.Count < 1)
                return null;

            return new DnsResourceDataNInfo(strings.AsReadOnly());
        }
    }
}