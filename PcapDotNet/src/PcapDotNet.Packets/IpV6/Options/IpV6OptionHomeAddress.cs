namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6275.
    /// Home Address Option.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Home Address               |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.HomeAddress)]
    public class IpV6OptionHomeAddress : IpV6OptionComplex, IIpV6OptionComplexFactory
    {
        public const int OptionDataLength = IpV6Address.SizeOf;

        public IpV6OptionHomeAddress(IpV6Address homeAddress)
            : base(IpV6OptionType.HomeAddress)
        {
            HomeAddress = homeAddress;
        }

        /// <summary>
        /// The home address of the mobile node sending the packet.  
        /// This address must be a unicast routable address.
        /// </summary>
        public IpV6Address HomeAddress { get; private set; }

        public IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;
            IpV6Address homeAddress = data.ReadIpV6Address(0, Endianity.Big);
            return new IpV6OptionHomeAddress(homeAddress);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }


        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, HomeAddress, Endianity.Big);
        }

        private IpV6OptionHomeAddress()
            : this(IpV6Address.Zero)
        {
        }
    }
}