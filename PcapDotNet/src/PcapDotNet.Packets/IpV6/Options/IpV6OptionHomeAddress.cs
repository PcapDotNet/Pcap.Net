using System;

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
        /// <summary>
        /// The number of bytes this option data takes.
        /// </summary>
        public const int OptionDataLength = IpV6Address.SizeOf;

        /// <summary>
        /// Creates an instance from home address.
        /// </summary>
        /// <param name="homeAddress">
        /// The home address of the mobile node sending the packet.  
        /// This address must be a unicast routable address.
        /// </param>
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

        /// <summary>
        /// Parses an option from the given data.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The option if parsing was successful, null otherwise.</returns>
        public IpV6Option CreateInstance(DataSegment data)
        {
            if (data == null) 
                throw new ArgumentNullException("data");
            if (data.Length != OptionDataLength)
                return null;
            IpV6Address homeAddress = data.ReadIpV6Address(0, Endianity.Big);
            return new IpV6OptionHomeAddress(homeAddress);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6Option other)
        {
            return EqualsData(other as IpV6OptionHomeAddress);
        }

        internal override int GetDataHashCode()
        {
            return HomeAddress.GetHashCode();
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, HomeAddress, Endianity.Big);
        }

        private IpV6OptionHomeAddress()
            : this(IpV6Address.Zero)
        {
        }

        private bool EqualsData(IpV6OptionHomeAddress other)
        {
            return other != null &&
                   HomeAddress.Equals(other.HomeAddress);
        }
    }
}