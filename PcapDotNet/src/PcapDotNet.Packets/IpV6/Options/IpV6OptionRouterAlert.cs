namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2711.
    /// <pre>
    /// +-----+-------------+--------+
    /// | Bit | 0-7         | 8-15   |
    /// +-----+-------------+--------+
    /// | 0   | Option Type | 2      |
    /// +-----+-------------+--------+
    /// | 16  | Router Alert Type    |
    /// +-----+----------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.RouterAlert)]
    public class IpV6OptionRouterAlert : IpV6OptionComplex
    {
        public const int OptionDataLength = sizeof(ushort);

        public IpV6OptionRouterAlert(IpV6RouterAlertType routerAlertType)
            : base(IpV6OptionType.RouterAlert)
        {
            RouterAlertType = routerAlertType;
        }

        /// <summary>
        /// Type of router alert.
        /// </summary>
        public IpV6RouterAlertType RouterAlertType { get; private set; }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6OptionRouterAlert((IpV6RouterAlertType)data.ReadUShort(0, Endianity.Big));
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, (ushort)RouterAlertType, Endianity.Big);
        }
    }
}