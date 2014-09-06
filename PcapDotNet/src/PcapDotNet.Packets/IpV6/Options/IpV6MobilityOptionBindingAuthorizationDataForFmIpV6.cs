using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5568.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | SPI                        |
    /// |     |                            |
    /// +-----+----------------------------+
    /// | 48  | Authenticator              |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.BindingAuthorizationDataForFmIpV6)]
    public sealed class IpV6MobilityOptionBindingAuthorizationDataForFmIpV6 : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int SecurityParameterIndex = 0;
            public const int Authenticator = SecurityParameterIndex + sizeof(uint);
        }

        public const int OptionDataMinimumLength = Offset.Authenticator;

        public IpV6MobilityOptionBindingAuthorizationDataForFmIpV6(uint securityParameterIndex, DataSegment authenticator)
            : base(IpV6MobilityOptionType.BindingAuthorizationDataForFmIpV6)
        {
            SecurityParameterIndex = securityParameterIndex;
            Authenticator = authenticator;
        }

        /// <summary>
        /// SPI = 0 is reserved for the Authenticator computed using SEND-based handover keys.
        /// </summary>
        public uint SecurityParameterIndex { get; private set; }

        /// <summary>
        /// <para>
        /// Contains a cryptographic value that can be used to determine that the message in question comes from the right authority.  
        /// Rules for calculating this value depends on the used authorization procedure.
        /// </para>
        /// <para>
        /// For the return routability procedure, this option can appear in the Binding Update and Binding Acknowledgements.
        /// Rules for calculating the Authenticator value are the following:
        /// </para>
        /// <para>
        ///   Mobility Data = care-of address | correspondent | MH Data
        ///   Authenticator = First (96, HMAC_SHA1 (Kbm, Mobility Data))
        /// </para>
        /// <para>
        /// Where | denotes concatenation.
        /// "Care-of address" is the care-of address that will be registered for the mobile node if the Binding Update succeeds,
        /// or the home address of the mobile node if this option is used in de-registration.
        /// Note also that this address might be different from the source address of the Binding Update message,
        /// if the Alternative Care-of Address mobility option is used, or when the lifetime of the binding is set to zero.
        /// </para>
        /// <para>
        /// The "correspondent" is the IPv6 address of the Previous Access Router (PAR).
        /// Note that, if the message is sent to a destination that is itself mobile, the "correspondent" address may not be the address found in the 
        /// Destination Address field of the IPv6 header; instead, the home address from the type 2 Routing header should be used.
        /// </para>
        /// <para>
        /// "MH Data" is the content of the Mobility Header, excluding the Authenticator field itself.
        /// The Authenticator value is calculated as if the Checksum field in the Mobility Header was zero.  
        /// The Checksum in the transmitted packet is still calculated in the usual manner, 
        /// with the calculated Authenticator being a part of the packet protected by the Checksum.
        /// Kbm is a shared key between the MN and the PAR.
        /// Note that while the contents of a potential Home Address destination option are not covered in this formula,
        /// the rules for the calculation of the Kbm do take the home address in account.
        /// This ensures that the MAC will be different for different home addresses.
        /// </para>
        /// <para>
        /// The first 96 bits from the MAC result are used as the Authenticator field.
        /// </para>
        /// </summary>
        public DataSegment Authenticator { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            uint securityParameterIndex = data.ReadUInt(Offset.SecurityParameterIndex, Endianity.Big);
            DataSegment authenticator = data.Subsegment(Offset.Authenticator, data.Length - Offset.Authenticator);
            return new IpV6MobilityOptionBindingAuthorizationDataForFmIpV6(securityParameterIndex, authenticator);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + Authenticator.Length; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionBindingAuthorizationDataForFmIpV6);
        }

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(SecurityParameterIndex, Authenticator);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.SecurityParameterIndex, SecurityParameterIndex, Endianity.Big);
            buffer.Write(offset + Offset.Authenticator, Authenticator);
            offset += DataLength;
        }

        private IpV6MobilityOptionBindingAuthorizationDataForFmIpV6()
            : this(0, DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6MobilityOptionBindingAuthorizationDataForFmIpV6 other)
        {
            return other != null &&
                   SecurityParameterIndex == other.SecurityParameterIndex && Authenticator.Equals(other.Authenticator);
        }
    }
}