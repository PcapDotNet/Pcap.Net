using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6089.
    /// <pre>
    /// +-----+--------------+-------------+
    /// | Bit | 0-7          | 8-15        |
    /// +-----+--------------+-------------+
    /// | 0   | Sub-Opt Type | Sub-Opt Len | 
    /// +-----+--------------+-------------+
    /// | 16  | TS Format    | Reserved    |
    /// +-----+--------------+-------------+
    /// | 32  | Traffic Selector           |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6FlowIdentificationSubOptionTypeRegistration(IpV6FlowIdentificationSubOptionType.TrafficSelector)]
    public sealed class IpV6FlowIdentificationSubOptionTrafficSelector : IpV6FlowIdentificationSubOptionComplex
    {
        private static class Offset
        {
            public const int TrafficSelectorFormat = 0;
            public const int TrafficSelector = TrafficSelectorFormat + sizeof(byte) + sizeof(byte);
        }

        public const int OptionDataMinimumLength = Offset.TrafficSelector;

        public IpV6FlowIdentificationSubOptionTrafficSelector(IpV6FlowIdentificationTrafficSelectorFormat trafficSelectorFormat, DataSegment trafficSelector)
            : base(IpV6FlowIdentificationSubOptionType.TrafficSelector)
        {
            TrafficSelectorFormat = trafficSelectorFormat;
            TrafficSelector = trafficSelector;
        }

        /// <summary>
        /// Indicates the Traffic Selector Format.
        /// </summary>
        public IpV6FlowIdentificationTrafficSelectorFormat TrafficSelectorFormat { get; private set; }

        /// <summary>
        /// The traffic selector formatted according to TrafficSelectorFormat.
        /// </summary>
        public DataSegment TrafficSelector { get; private set; }

        internal override IpV6FlowIdentificationSubOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            IpV6FlowIdentificationTrafficSelectorFormat trafficSelectorFormat = (IpV6FlowIdentificationTrafficSelectorFormat)data[Offset.TrafficSelectorFormat];
            DataSegment trafficSelector = data.Subsegment(Offset.TrafficSelector, data.Length - Offset.TrafficSelector);
            return new IpV6FlowIdentificationSubOptionTrafficSelector(trafficSelectorFormat, trafficSelector);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + TrafficSelector.Length; }
        }

        internal override bool EqualsData(IpV6FlowIdentificationSubOption other)
        {
            return EqualsData(other as IpV6FlowIdentificationSubOptionTrafficSelector);
        }

        internal override object GetDataHashCode()
        {
            return Sequence.GetHashCode(TrafficSelectorFormat, TrafficSelector);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.TrafficSelectorFormat, (byte)TrafficSelectorFormat);
            buffer.Write(offset + Offset.TrafficSelector, TrafficSelector);
            offset += DataLength;
        }

        private IpV6FlowIdentificationSubOptionTrafficSelector()
            : this(IpV6FlowIdentificationTrafficSelectorFormat.IpV4Binary, DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6FlowIdentificationSubOptionTrafficSelector other)
        {
            return other != null &&
                   TrafficSelectorFormat == other.TrafficSelectorFormat && TrafficSelector.Equals(other.TrafficSelector);
        }
    }
}