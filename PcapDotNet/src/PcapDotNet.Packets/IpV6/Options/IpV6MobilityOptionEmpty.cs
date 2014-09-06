namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// An IPv6 Mobility option with an empty data.
    /// </summary>
    public abstract class IpV6MobilityOptionEmpty : IpV6MobilityOptionComplex
    {
        /// <summary>
        /// The number of bytes this option data takes.
        /// </summary>
        public const int OptionDataLength = 0;

        internal IpV6MobilityOptionEmpty(IpV6MobilityOptionType type)
            : base(type)
        {
        }

        internal sealed override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal sealed override bool EqualsData(IpV6MobilityOption other)
        {
            return true;
        }

        internal override int GetDataHashCode()
        {
            return 0;
        }

        internal sealed override void WriteData(byte[] buffer, ref int offset)
        {
        }
    }
}