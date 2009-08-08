namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Represents a complex IPv4 option.
    /// Complex option means that it contains data and not just the type.
    /// </summary>
    public abstract class IpV4OptionComplex : IpV4Option
    {
        /// <summary>
        /// The header length in bytes for the option (type and size).
        /// </summary>
        public const int OptionHeaderLength = 2;

        internal static IpV4OptionComplex ReadOptionComplex(IpV4OptionType optionType, byte[] buffer, ref int offset, int length)
        {
            if (length < 1)
                return null;
            byte optionLength = buffer[offset++];
            if (length + 1 < optionLength)
                return null;

            byte optionValueLength = (byte)(optionLength - OptionHeaderLength);

            switch (optionType)
            {
                case IpV4OptionType.BasicSecurity:
                    return IpV4OptionBasicSecurity.ReadOptionSecurity(buffer, ref offset, optionValueLength);

                case IpV4OptionType.LooseSourceRouting:
                    return IpV4OptionLooseSourceRouting.ReadOptionLooseSourceRouting(buffer, ref offset, optionValueLength);

                case IpV4OptionType.StrictSourceRouting:
                    return IpV4OptionStrictSourceRouting.ReadOptionStrictSourceRouting(buffer, ref offset, optionValueLength);

                case IpV4OptionType.RecordRoute:
                    return IpV4OptionRecordRoute.ReadOptionRecordRoute(buffer, ref offset, optionValueLength);

                case IpV4OptionType.StreamIdentifier:
                    return IpV4OptionStreamIdentifier.ReadOptionStreamIdentifier(buffer, ref offset, optionValueLength);

                case IpV4OptionType.InternetTimestamp:
                    return IpV4OptionTimestamp.ReadOptionTimestamp(buffer, ref offset, optionValueLength);

                default:
                    return null;

            }
        }

        /// <summary>
        /// Constructs the option by type.
        /// </summary>
        protected IpV4OptionComplex(IpV4OptionType type)
            : base(type)
        {
        }
    }
}