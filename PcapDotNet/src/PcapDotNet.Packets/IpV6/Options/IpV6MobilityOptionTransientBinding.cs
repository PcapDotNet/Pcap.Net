namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6058.
    /// <pre>
    /// +-----+----------+---+--------------+
    /// | Bit | 0-6      | 7 | 8-15         |
    /// +-----+----------+---+--------------+
    /// | 0   | Option Type  | Opt Data Len |
    /// +-----+----------+---+--------------+
    /// | 16  | Reserved | L | Lifetime     |
    /// +-----+----------+---+--------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.TransientBinding)]
    public sealed class IpV6MobilityOptionTransientBinding : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int LatePathSwitch = 0;
            public const int Lifetime = LatePathSwitch + sizeof(byte);
        }

        private static class Mask
        {
            public const int LatePathSwitch = 0x01;
        }

        public const int OptionDataLength = Offset.Lifetime + sizeof(byte);

        public IpV6MobilityOptionTransientBinding(bool latePathSwitch, byte lifetime)
            : base(IpV6MobilityOptionType.TransientBinding)
        {
            LatePathSwitch = latePathSwitch;
            Lifetime = lifetime;
        }

        /// <summary>
        /// Indicates that the Local Mobility Anchor (LMA) applies late path switch according to the transient BCE state.
        /// If true, the LMA continues to forward downlink packets towards the pMAG.
        /// Different setting of this flag may be for future use.
        /// </summary>
        public bool LatePathSwitch { get; private set; }

        /// <summary>
        /// Maximum lifetime of a Transient-L state in multiple of 100 ms.
        /// </summary>
        public byte Lifetime { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            bool latePathSwitch = data.ReadBool(Offset.LatePathSwitch, Mask.LatePathSwitch);
            byte lifetime = data[Offset.Lifetime];
            return new IpV6MobilityOptionTransientBinding(latePathSwitch, lifetime);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionTransientBinding);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            if (LatePathSwitch)
                buffer.Write(offset + Offset.LatePathSwitch, Mask.LatePathSwitch);
            buffer.Write(offset + Offset.Lifetime, Lifetime);
            offset += DataLength;
        }

        private IpV6MobilityOptionTransientBinding()
            : this(false, 0)
        {
        }

        private bool EqualsData(IpV6MobilityOptionTransientBinding other)
        {
            return other != null &&
                   LatePathSwitch == other.LatePathSwitch && Lifetime == other.Lifetime;
        }
    }
}