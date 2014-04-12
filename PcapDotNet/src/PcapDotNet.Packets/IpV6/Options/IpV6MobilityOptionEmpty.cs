namespace PcapDotNet.Packets.IpV6
{
    public abstract class IpV6MobilityOptionEmpty : IpV6MobilityOptionComplex
    {
        public const int OptionDataLength = 0;

        public IpV6MobilityOptionEmpty(IpV6MobilityOptionType type)
            : base(type)
        {
        }

        internal override sealed int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override sealed bool EqualsData(IpV6MobilityOption other)
        {
            return true;
        }

        internal override sealed void WriteData(byte[] buffer, ref int offset)
        {
        }
    }
}