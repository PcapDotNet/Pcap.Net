using System;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2460.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Option Data                |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    public class IpV6OptionUnknown : IpV6OptionComplex
    {
        public IpV6OptionUnknown(IpV6OptionType type, DataSegment data)
            : base(type)
        {
            Data = data;
        }

        public DataSegment Data { get; private set; }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            throw new InvalidOperationException("IpV6OptionUnknown shouldn't be registered.");
        }

        internal override int DataLength
        {
            get { return Data.Length; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Data);
        }
    }

    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Option Data                |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    public class IpV6MobilityOptionUnknown : IpV6MobilityOptionSingleDataSegmentField
    {
        public IpV6MobilityOptionUnknown(IpV6MobilityOptionType type, DataSegment data)
            : base(type, data)
        {
        }

        public DataSegment Data
        {
            get { return Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            throw new InvalidOperationException("IpV6MobilityOptionUnknown shouldn't be registered.");
        }
    }

    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+--------------+
    /// | Bit | 0-7          |
    /// +-----+--------------+
    /// | 0   | Option Type  |
    /// +-----+--------------+
    /// | 8   | Opt Data Len |
    /// +-----+--------------+
    /// | 16  | 0            |
    /// | ... |              |
    /// +-----+--------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.PadN)]
    public class IpV6MobilityOptionPadN : IpV6MobilityOptionComplex
    {
        public IpV6MobilityOptionPadN(int paddingDataLength)
            : base(IpV6MobilityOptionType.PadN)
        {
            PaddingDataLength = paddingDataLength;
        }

        public int PaddingDataLength { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            return new IpV6MobilityOptionPadN(data.Length);
        }

        internal override int DataLength
        {
            get { return PaddingDataLength; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            offset += PaddingDataLength;
        }
    }

    
    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+--------------+--------------+
    /// | Bit | 0-7          | 8-15         |
    /// +-----+--------------+--------------+
    /// | 0   | Option Type  | Opt Data Len |
    /// +-----+--------------+--------------+
    /// | 16  | Refresh Interval            |
    /// +-----+-----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.BindingRefreshAdvice)]
    public class IpV6MobilityOptionBindingRefreshAdvice : IpV6MobilityOptionComplex
    {
        public const int OptionDataLength = sizeof(ushort);

        public IpV6MobilityOptionBindingRefreshAdvice(ushort refreshInterval)
            : base(IpV6MobilityOptionType.BindingRefreshAdvice)
        {
            refreshInterval = refreshInterval;
        }

        /// <summary>
        /// Measured in units of four seconds, and indicates remaining time until the mobile node should send a new home registration to the home agent.
        /// The Refresh Interval must be set to indicate a smaller time interval than the Lifetime value of the Binding Acknowledgement.
        /// </summary>
        public ushort RefreshInterval { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6MobilityOptionBindingRefreshAdvice(data.ReadUShort(0, Endianity.Big));
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, RefreshInterval, Endianity.Big);
        }
    }

    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+--------------+--------------+
    /// | Bit | 0-7          | 8-15         |
    /// +-----+--------------+--------------+
    /// | 0   | Option Type  | Opt Data Len |
    /// +-----+--------------+--------------+
    /// | 16  | Alternate Care-of Address   |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// |     |                             |
    /// +-----+-----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.AlternateCareOfAddress)]
    public class IpV6MobilityOptionAlternateCareOfAddress : IpV6MobilityOptionComplex
    {
        public const int OptionDataLength = IpV6Address.SizeOf;

        public IpV6MobilityOptionAlternateCareOfAddress(IpV6Address alternateCareOfAddress)
            : base(IpV6MobilityOptionType.AlternateCareOfAddress)
        {
            AlternateCareOfAddress = alternateCareOfAddress;
        }

        /// <summary>
        /// Contains an address to use as the care-of address for the binding, rather than using the Source Address of the packet as the care-of address.
        /// </summary>
        public IpV6Address AlternateCareOfAddress { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6MobilityOptionAlternateCareOfAddress(data.ReadIpV6Address(0, Endianity.Big));
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, AlternateCareOfAddress, Endianity.Big);
        }
    }

    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+--------------+--------------+
    /// | Bit | 0-7          | 8-15         |
    /// +-----+--------------+--------------+
    /// | 0   | Option Type  | Opt Data Len |
    /// +-----+--------------+--------------+
    /// | 16  | Home Nonce Index            |
    /// +-----+-----------------------------+
    /// | 32  | Care-of Nonce Index         |
    /// +-----+-----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.NonceIndices)]
    public class IpV6MobilityOptionNonceIndices : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int HomeNonceIndex = 0;
            public const int CareOfNonceIndex = HomeNonceIndex + sizeof(ushort);
        }

        public const int OptionDataLength = Offset.CareOfNonceIndex + sizeof(ushort);

        public IpV6MobilityOptionNonceIndices(ushort homeNonceIndex, ushort careOfNonceIndex)
            : base(IpV6MobilityOptionType.NonceIndices)
        {
            HomeNonceIndex = homeNonceIndex;
            CareOfNonceIndex = careOfNonceIndex;
        }

        /// <summary>
        /// Tells the correspondent node which nonce value to use when producing the home keygen token.
        /// </summary>
        public ushort HomeNonceIndex { get; private set; }

        /// <summary>
        /// Ignored in requests to delete a binding.
        /// Otherwise, it tells the correspondent node which nonce value to use when producing the care-of keygen token.
        /// </summary>
        public ushort CareOfNonceIndex { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            ushort homeNonceIndex = data.ReadUShort(Offset.HomeNonceIndex, Endianity.Big);
            ushort careOfNonceIndex = data.ReadUShort(Offset.CareOfNonceIndex, Endianity.Big);

            return new IpV6MobilityOptionNonceIndices(homeNonceIndex, careOfNonceIndex);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, HomeNonceIndex, Endianity.Big);
            buffer.Write(ref offset, CareOfNonceIndex, Endianity.Big);
        }
    }

    /// <summary>
    /// RFC 6275.
    /// <pre>
    /// +-----+--------------+--------------+
    /// | Bit | 0-7          | 8-15         |
    /// +-----+--------------+--------------+
    /// | 0   | Option Type  | Opt Data Len |
    /// +-----+--------------+--------------+
    /// | 16  | Authenticator               |
    /// | ... |                             |
    /// +-----+-----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.BindingAuthorizationData)]
    public class IpV6MobilityOptionBindingAuthorizationData : IpV6MobilityOptionSingleDataSegmentField
    {
        public IpV6MobilityOptionBindingAuthorizationData(DataSegment authenticator)
            : base(IpV6MobilityOptionType.BindingAuthorizationData, authenticator)
        {
        }

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
        /// The "correspondent" is the IPv6 address of the correspondent node.
        /// Note that, if the message is sent to a destination that is itself mobile, the "correspondent" address may not be the address found in the 
        /// Destination Address field of the IPv6 header; instead, the home address from the type 2 Routing header should be used.
        /// </para>
        /// <para>
        /// "MH Data" is the content of the Mobility Header, excluding the Authenticator field itself.
        /// The Authenticator value is calculated as if the Checksum field in the Mobility Header was zero.  
        /// The Checksum in the transmitted packet is still calculated in the usual manner, 
        /// with the calculated Authenticator being a part of the packet protected by the Checksum.
        /// Kbm is the binding management key, which is typically created using nonces provided by the correspondent node.
        /// Note that while the contents of a potential Home Address destination option are not covered in this formula,
        /// the rules for the calculation of the Kbm do take the home address in account.
        /// This ensures that the MAC will be different for different home addresses.
        /// </para>
        /// <para>
        /// The first 96 bits from the MAC result are used as the Authenticator field.
        /// </para>
        /// </summary>
        public DataSegment Authenticator
        {
            get { return Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            return new IpV6MobilityOptionBindingAuthorizationData(data);
        }
    }

    /// <summary>
    /// RFC 3963.
    /// <pre>
    /// +-----+--------------+---------------+
    /// | Bit | 0-7          | 8-15          |
    /// +-----+--------------+---------------+
    /// | 0   | Option Type  | Opt Data Len  |
    /// +-----+--------------+---------------+
    /// | 16  | Reserved     | Prefix Length |
    /// +-----+--------------+---------------+
    /// | 32  | Mobile Network Prefix        |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// |     |                              |
    /// +-----+------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.MobileNetworkPrefix)]
    public class IpV6MobilityOptionMobileNetworkPrefix : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int PrefixLength = sizeof(ushort);
            public const int MobileNetworkPrefix = PrefixLength + sizeof(ushort);
        }

        public const int OptionDataLength = Offset.MobileNetworkPrefix + IpV6Address.SizeOf;

        public IpV6MobilityOptionMobileNetworkPrefix(byte prefixLength, IpV6Address mobileNetworkPrefix)
            : base(IpV6MobilityOptionType.MobileNetworkPrefix)
        {
            PrefixLength = prefixLength;
            MobileNetworkPrefix = mobileNetworkPrefix;
        }

        /// <summary>
        /// Indicates the prefix length of the IPv6 prefix contained in the option.
        /// </summary>
        public byte PrefixLength { get; private set; }

        /// <summary>
        /// Contains the Mobile Network Prefix.
        /// </summary>
        public IpV6Address MobileNetworkPrefix { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            byte prefixLength = data[Offset.PrefixLength];
            IpV6Address mobileNetworkPrefix = data.ReadIpV6Address(Offset.MobileNetworkPrefix, Endianity.Big);

            return new IpV6MobilityOptionMobileNetworkPrefix(prefixLength, mobileNetworkPrefix);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.PrefixLength, PrefixLength);
            buffer.Write(offset + Offset.MobileNetworkPrefix, MobileNetworkPrefix, Endianity.Big);
            offset += OptionDataLength;
        }
    }

    public enum IpV6MobilityOptionLinkLayerAddressCode : byte
    {
        /// <summary>
        /// Wildcard requesting resolution for all nearby access points.
        /// </summary>
        Wildcard = 0,

        /// <summary>
        /// Link-Layer Address of the New Access Point.
        /// The address contains the link-layer address of the access point for which handover is about to be attempted.
        /// This is used in the Router Solicitation for Proxy Advertisement message.
        /// </summary>
        NewAccessPoint = 1,

        /// <summary>
        /// Link-Layer Address of the MN (Mobility Node).
        /// The address contains the link-layer address of an MN.
        /// It is used in the Handover Initiate message.
        /// </summary>
        MobilityNode = 2,

        /// <summary>
        /// Link-Layer Address of the NAR (New Access Router) (i.e., Proxied Originator).
        /// The address contains the link-layer address of the access router to which the Proxy Router Solicitation message refers.
        /// </summary>
        NewAccessRouter = 3,

        /// <summary>
        /// Link-Layer Address of the source of RtSolPr (Router Solicitation for Proxy Advertisement) or PrRtAdv (Proxy Router Advertisement) message.
        /// </summary>
        SourceRouterSolicitationForProxyAdvertisementOrProxyRouterAdvertisement = 4,

        /// <summary>
        /// The access point identified by the LLA belongs to the current interface of the router.
        /// </summary>
        AccessPoint = 5,

        /// <summary>
        /// No prefix information available for the access point identified by the LLA
        /// </summary>
        NoPrefixForAccessPoint = 6,

        /// <summary>
        /// No fast handover support available for the access point identified by the LLA
        /// </summary>
        NoFastHandoverSupportForAccessPoint = 7,
    }

    /// <summary>
    /// RFC 5568.
    /// <pre>
    /// +-----+--------------+
    /// | Bit | 0-7          |
    /// +-----+--------------+
    /// | 0   | Option Type  |
    /// +-----+--------------+
    /// | 8   | Opt Data Len |
    /// +-----+--------------+
    /// | 16  | Option-Code  |
    /// +-----+--------------+
    /// | 24  | LLA          |
    /// | ... |              |
    /// +-----+--------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.LinkLayerAddress)]
    public class IpV6MobilityOptionLinkLayerAddress : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int OptionCode = 0;
            public const int LinkLayerAddress = OptionCode + sizeof(byte);
        }

        public const int OptionDataMiniumLength = Offset.LinkLayerAddress;

        public IpV6MobilityOptionLinkLayerAddress(IpV6MobilityOptionLinkLayerAddressCode code, DataSegment linkLayerAddress)
            : base(IpV6MobilityOptionType.LinkLayerAddress)
        {
            Code = code;
            LinkLayerAddress = linkLayerAddress;
        }

        public IpV6MobilityOptionLinkLayerAddressCode Code { get; private set; }

        /// <summary>
        /// Variable-length link-layer address.
        /// </summary>
        public DataSegment LinkLayerAddress { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMiniumLength)
                return null;

            IpV6MobilityOptionLinkLayerAddressCode code = (IpV6MobilityOptionLinkLayerAddressCode)data[Offset.OptionCode];
            DataSegment linkLayerAddress = data.Subsegment(Offset.LinkLayerAddress, data.Length - Offset.LinkLayerAddress);

            return new IpV6MobilityOptionLinkLayerAddress(code, linkLayerAddress);
        }

        internal override int DataLength
        {
            get { return OptionDataMiniumLength + LinkLayerAddress.Length; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.OptionCode, (byte)Code);
            LinkLayerAddress.Write(buffer, offset + Offset.LinkLayerAddress);
            offset += DataLength;
        }
    }

    /// <summary>
    /// RFC 4283.
    /// </summary>
    public enum IpV6MobileNodeIdentifierSubtype : byte
    {
        /// <summary>
        /// RFC 4283.
        /// Uses an identifier of the form user@realm (RFC 4282).
        /// </summary>
        NetworkAccessIdentifier = 1,
    }

    /// <summary>
    /// RFC 4283.
    /// <pre>
    /// +-----+--------------+
    /// | Bit | 0-7          |
    /// +-----+--------------+
    /// | 0   | Option Type  |
    /// +-----+--------------+
    /// | 8   | Opt Data Len |
    /// +-----+--------------+
    /// | 16  | Subtype      |
    /// +-----+--------------+
    /// | 24  | Identifier   |
    /// | ... |              |
    /// +-----+--------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.MobileNodeIdentifier)]
    public class IpV6MobilityOptionMobileNodeIdentifier : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int Subtype = 0;
            public const int Identifier = Subtype + sizeof(byte);
        }

        public const int OptionDataMiniumLength = Offset.Identifier;

        public IpV6MobilityOptionMobileNodeIdentifier(IpV6MobileNodeIdentifierSubtype subtype, DataSegment identifier)
            : base(IpV6MobilityOptionType.MobileNodeIdentifier)
        {
            Subtype = subtype;
            Identifier = identifier;
        }

        /// <summary>
        /// Defines the specific type of identifier included in the Identifier field.
        /// </summary>
        public IpV6MobileNodeIdentifierSubtype Subtype { get; private set; }

        /// <summary>
        /// A variable length identifier of type, as specified by the Subtype field of this option.
        /// </summary>
        public DataSegment Identifier { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMiniumLength)
                return null;

            IpV6MobileNodeIdentifierSubtype subtype = (IpV6MobileNodeIdentifierSubtype)data[Offset.Subtype];
            DataSegment identifier = data.Subsegment(Offset.Identifier, data.Length - Offset.Identifier);

            return new IpV6MobilityOptionMobileNodeIdentifier(subtype, identifier);
        }

        internal override int DataLength
        {
            get { return OptionDataMiniumLength + Identifier.Length; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Subtype, (byte)Subtype);
            Identifier.Write(buffer, offset + Offset.Identifier);
            offset += DataLength;
        }
    }

    /// <summary>
    /// RFC 4285
    /// </summary>
    public enum IpV6AuthenticationSubtype : byte
    {
        /// <summary>
        /// Used to authenticate the Binding Update and Binding Acknowledgement messages based on the shared-key-based security association 
        /// between the Mobile Node and the Home Agent.
        /// The shared-key-based mobility security association between Mobile Node and Home Agent used within this specification consists of a mobility SPI,
        /// a key, an authentication algorithm, and the replay protection mechanism in use.  
        /// The mobility SPI is a number in the range [0-4294967296], where the range [0-255] is reserved.  
        /// The key consists of an arbitrary value and is 16 octets in length.  
        /// The authentication algorithm is HMAC_SHA1.  
        /// The replay protection mechanism may use the Sequence number as specified in RFC 3775 or the Timestamp option.
        /// If the Timestamp option is used for replay protection, the mobility security association includes a "close enough" field to account 
        /// for clock drift.
        /// A default value of 7 seconds should be used.
        /// This value should be greater than 3 seconds.
        /// The MN-HA mobility message authentication option must be the last option in a message with a mobility header 
        /// if it is the only mobility message authentication option in the message.
        /// The authentication data is calculated on the message starting from the mobility header up to and including the mobility SPI value of this option.
        /// Authentication Data = First(96, HMAC_SHA1(MN-HA Shared key, Mobility Data))
        /// Mobility Data = care-of address | home address | Mobility Header (MH) Data
        /// MH Data is the content of the Mobility Header up to and including the mobility SPI field of this option.
        /// The Checksum field in the Mobility Header must be set to 0 to calculate the Mobility Data.
        /// The first 96 bits from the Message Authentication Code (MAC) result are used as the Authentication Data field.
        /// </summary>
        HomeAgent = 1,

        /// <summary>
        /// The MN-AAA authentication mobility option is used to authenticate the Binding Update message based on the shared mobility security association 
        /// between the Mobile Node and AAA server in Home network (AAAH).
        /// It is not used in Binding Acknowledgement messages.
        /// The corresponding Binding Acknowledgement messages must be authenticated using the MN-HA mobility message authentication option.
        /// The MN-AAA mobility message authentication option must be the last option in a message with a mobility header.
        /// The corresponding response must include the MN-HA mobility message authentication option,
        /// and must not include the MN-AAA mobility message authentication option.
        /// The Mobile Node may use the Mobile Node Identifier option (RFC 4283) to enable the Home Agent to make use of available AAA infrastructure.
        /// The authentication data is calculated on the message starting from the mobility header up to and including the mobility SPI value of this option.
        /// The authentication data shall be calculated as follows:
        /// Authentication data = hash_fn(MN-AAA Shared key, MAC_Mobility Data)
        /// hash_fn() is decided by the value of mobility SPI field in the MN-AAA mobility message authentication option.
        /// SPI = HMAC_SHA1_SPI:
        /// If mobility SPI has the well-known value HMAC_SHA1_SPI, then hash_fn() is HMAC_SHA1.
        /// When HMAC_SHA1_SPI is used, the BU is authenticated by AAA using HMAC_SHA1 authentication.
        /// In that case, MAC_Mobility Data is calculated as follows:
        /// MAC_Mobility Data = SHA1(care-of address | home address | MH Data)
        /// MH Data is the content of the Mobility Header up to and including the mobility SPI field of this option.
        /// </summary>
        AuthenticationAuthorizationAccountingServer = 2,
    }

    /// <summary>
    /// RFC 4285.
    /// <pre>
    /// +-----+---------------------+
    /// | Bit | 0-7                 |
    /// +-----+---------------------+
    /// | 0   | Option Type         |
    /// +-----+---------------------+
    /// | 8   | Opt Data Len        |
    /// +-----+---------------------+
    /// | 16  | Subtype             |
    /// +-----+---------------------+
    /// | 24  | Mobility SPI        |
    /// |     |                     |
    /// |     |                     |
    /// |     |                     |
    /// +-----+---------------------+
    /// | 56  | Authentication Data |
    /// | ... |                     |
    /// +-----+---------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.Authentication)]
    public class IpV6MobilityOptionAuthentication : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int Subtype = 0;
            public const int MobilitySecurityParameterIndex = Subtype + sizeof(byte);
            public const int AuthenticationData = MobilitySecurityParameterIndex + sizeof(uint);
        }

        public const int OptionDataMiniumLength = Offset.AuthenticationData;

        public IpV6MobilityOptionAuthentication(IpV6AuthenticationSubtype subtype, uint mobilitySecurityParameterIndex, DataSegment authenticationData)
            : base(IpV6MobilityOptionType.Authentication)
        {
            Subtype = subtype;
            MobilitySecurityParameterIndex = mobilitySecurityParameterIndex;
            AuthenticationData = authenticationData;
        }

        /// <summary>
        /// A number assigned to identify the entity and/or mechanism to be used to authenticate the message.
        /// </summary>
        public IpV6AuthenticationSubtype Subtype { get; private set; }

        /// <summary>
        /// A number in the range [0-4294967296] used to index into the shared-key-based mobility security associations.
        /// </summary>
        public uint MobilitySecurityParameterIndex { get; private set; }

        /// <summary>
        /// Has the information to authenticate the relevant mobility entity.
        /// This protects the message beginning at the Mobility Header up to and including the mobility SPI field.
        /// </summary>
        public DataSegment AuthenticationData { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMiniumLength)
                return null;

            IpV6AuthenticationSubtype subtype = (IpV6AuthenticationSubtype)data[Offset.Subtype];
            uint mobilitySecurityParameterIndex = data.ReadUInt(Offset.MobilitySecurityParameterIndex, Endianity.Big);
            DataSegment authenticationData = data.Subsegment(Offset.AuthenticationData, data.Length - Offset.AuthenticationData);

            return new IpV6MobilityOptionAuthentication(subtype, mobilitySecurityParameterIndex, authenticationData);
        }

        internal override int DataLength
        {
            get { return OptionDataMiniumLength + AuthenticationData.Length; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Subtype, (byte)Subtype);
            buffer.Write(offset + Offset.MobilitySecurityParameterIndex, MobilitySecurityParameterIndex, Endianity.Big);
            AuthenticationData.Write(buffer, offset + Offset.AuthenticationData);
            offset += DataLength;
        }
    }

    /// <summary>
    /// RFC 4285.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Timestamp                  |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.ReplayProtection)]
    public class IpV6MobilityOptionReplayProtection : IpV6MobilityOptionComplex
    {
        public const int OptionDataLength = sizeof(ulong);

        public IpV6MobilityOptionReplayProtection(ulong timestamp)
            : base(IpV6MobilityOptionType.ReplayProtection)
        {
            Timestamp = timestamp;
        }

        /// <summary>
        /// 64 bit timestamp.
        /// </summary>
        public ulong Timestamp { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            ulong timestamp = data.ReadULong(0, Endianity.Big);

            return new IpV6MobilityOptionReplayProtection(timestamp);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Timestamp, Endianity.Big);
        }
    }

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

        internal override sealed void WriteData(byte[] buffer, ref int offset)
        {
        }
    }

    /// <summary>
    /// RFC 4866.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.CgaParametersRequest)]
    public class IpV6MobilityOptionCgaParametersRequest : IpV6MobilityOptionEmpty
    {
        public IpV6MobilityOptionCgaParametersRequest()
            : base(IpV6MobilityOptionType.CgaParametersRequest)
        {
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6MobilityOptionCgaParametersRequest();
        }
    }

    public abstract class IpV6MobilityOptionSingleDataSegmentField : IpV6MobilityOptionComplex
    {
        public IpV6MobilityOptionSingleDataSegmentField(IpV6MobilityOptionType type, DataSegment value)
            : base(IpV6MobilityOptionType.CgaParameters)
        {
            Value = value;
        }

        internal DataSegment Value { get; private set; }

        internal override sealed int DataLength
        {
            get { return Value.Length; }
        }

        internal override sealed void WriteData(byte[] buffer, ref int offset)
        {
            Value.Write(buffer, ref offset);
        }

    }

    /// <summary>
    /// RFC 4866.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | CGA Parameters             |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.CgaParameters)]
    public class IpV6MobilityOptionCgaParameters : IpV6MobilityOptionSingleDataSegmentField
    {
        public const int OptionDataMaxLength = 255;

        public IpV6MobilityOptionCgaParameters(DataSegment cgaParameters)
            : base(IpV6MobilityOptionType.CgaParameters, cgaParameters)
        {
        }

        /// <summary>
        /// Contains up to 255 bytes of the CGA Parameters data structure defined in RFC 3972.
        /// The concatenation of all CGA Parameters options in the order they appear in the Binding Update message 
        /// must result in the original CGA Parameters data structure.
        /// All CGA Parameters options in the Binding Update message except the last one must contain exactly 255 bytes in the CGA Parameters field,
        /// and the Option Length field must be set to 255 accordingly.
        /// All CGA Parameters options must appear directly one after another, that is, 
        /// a mobility option of a different type must not be placed in between two CGA Parameters options.
        /// </summary>
        public DataSegment CgaParameters
        {
            get { return Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length > OptionDataMaxLength)
                return null;

            return new IpV6MobilityOptionCgaParameters(data);
        }
    }

    /// <summary>
    /// RFC 4866.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Signature                  |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.Signature)]
    public class IpV6MobilityOptionSignature : IpV6MobilityOptionSingleDataSegmentField
    {
        public IpV6MobilityOptionSignature(DataSegment signature)
            : base(IpV6MobilityOptionType.Signature, signature)
        {
        }

        /// <summary>
        /// Contains the mobile or correspondent node's signature, generated with the mobile or correspondent node's private key.
        /// </summary>
        public DataSegment Signature
        {
            get { return Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            return new IpV6MobilityOptionSignature(data);
        }
    }

    /// <summary>
    /// RFC 4866.
    /// <pre>
    /// +-----+-------------+---------------+
    /// | Bit | 0-7         | 8-15          |
    /// +-----+-------------+---------------+
    /// | 0   | Option Type | Opt Data Len  |
    /// +-----+-------------+---------------+
    /// | 16  | Permanent Home Keygen Token |
    /// | ... |                             |
    /// +-----+-----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.PermanentHomeKeygenToken)]
    public class IpV6MobilityOptionPermanentHomeKeygenToken : IpV6MobilityOptionSingleDataSegmentField
    {
        public IpV6MobilityOptionPermanentHomeKeygenToken(DataSegment permanentHomeKeygenToken)
            : base(IpV6MobilityOptionType.PermanentHomeKeygenToken, permanentHomeKeygenToken)
        {
        }

        /// <summary>
        /// Contains the permanent home keygen token generated by the correspondent node.
        /// The content of this field must be encrypted with the mobile node's public key.
        /// The length of the permanent home keygen token is 8 octets before encryption, though the ciphertext and, hence,
        /// the Permanent Home Keygen Token field may be longer.
        /// </summary>
        public DataSegment PermanentHomeKeygenToken
        {
            get { return Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            return new IpV6MobilityOptionPermanentHomeKeygenToken(data);
        }
    }

    /// <summary>
    /// RFC 4866.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.CareOfTestInit)]
    public class IpV6MobilityOptionCareOfTestInit : IpV6MobilityOptionEmpty
    {
        public IpV6MobilityOptionCareOfTestInit()
            : base(IpV6MobilityOptionType.CareOfTestInit)
        {
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6MobilityOptionCareOfTestInit();
        }
    }

    /// <summary>
    /// RFC 4866.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Care-of Keygen Token       |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.CareOfTest)]
    public class IpV6MobilityOptionCareOfTest : IpV6MobilityOptionComplex
    {
        public const int OptionDataLength = sizeof(ulong);

        public IpV6MobilityOptionCareOfTest(ulong careOfKeygenToken)
            : base(IpV6MobilityOptionType.CareOfTest)
        {
        }

        /// <summary>
        /// Contains the care-of keygen token generated by the correspondent node.
        /// </summary>
        public ulong CareOfKeygenToken { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            ulong careOfKeygenToken = data.ReadULong(0, Endianity.Big);

            return new IpV6MobilityOptionCareOfTest(careOfKeygenToken);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, CareOfKeygenToken, Endianity.Big);
        }
    }

}
