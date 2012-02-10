using System.Collections.Generic;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1183.
    /// <pre>
    /// +---------------+
    /// | ISDN-address  |
    /// +---------------+
    /// | sa (optional) |
    /// +---------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Isdn)]
    public sealed class DnsResourceDataIsdn : DnsResourceDataStrings
    {
        private const int MinNumStrings = 1;
        private const int MaxNumStrings = 2;

        public DnsResourceDataIsdn(DataSegment isdnAddress)
            : base(isdnAddress)
        {

        }

        public DnsResourceDataIsdn(DataSegment isdnAddress, DataSegment subaddress)
            : base(isdnAddress, subaddress)
        {
        }

        /// <summary>
        /// Identifies the ISDN number of the owner and DDI (Direct Dial In) if any, as defined by E.164 and E.163, 
        /// the ISDN and PSTN (Public Switched Telephone Network) numbering plan.
        /// E.163 defines the country codes, and E.164 the form of the addresses.
        /// </summary>
        public DataSegment IsdnAddress { get { return Strings[0]; } }

        /// <summary>
        /// Specifies the subaddress (SA).
        /// </summary>
        public DataSegment Subaddress { get { return Strings.Count == MaxNumStrings ? Strings[1] : null; } }

        internal DnsResourceDataIsdn()
            : this(DataSegment.Empty)
        {
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            List<DataSegment> strings = ReadStrings(data, MaxNumStrings);
            if (strings == null)
                return null;
            if (strings.Count == MinNumStrings)
                return new DnsResourceDataIsdn(strings[0]);
            if (strings.Count == MaxNumStrings)
                return new DnsResourceDataIsdn(strings[0], strings[1]);
            return null;
        }
    }
}