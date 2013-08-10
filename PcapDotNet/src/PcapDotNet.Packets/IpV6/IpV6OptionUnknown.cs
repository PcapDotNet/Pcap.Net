using System;
using PcapDotNet.Packets.IpV4;

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
    public sealed class IpV6OptionUnknown : IpV6OptionComplex
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
    public sealed class IpV6MobilityOptionUnknown : IpV6MobilityOptionSingleDataSegmentField
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
    public sealed class IpV6MobilityOptionPadN : IpV6MobilityOptionComplex
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
    public sealed class IpV6MobilityOptionBindingRefreshAdvice : IpV6MobilityOptionComplex
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
    /// <pre>
    /// +-----+--------------+--------------+
    /// | Bit | 0-7          | 8-15         |
    /// +-----+--------------+--------------+
    /// | 0   | Option Type  | Opt Data Len |
    /// +-----+--------------+--------------+
    /// | 16  | Address                     |
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
    public abstract class IpV6MobilityOptionIpV6Address : IpV6MobilityOptionComplex
    {
        public const int OptionDataLength = IpV6Address.SizeOf;

        public IpV6MobilityOptionIpV6Address(IpV6MobilityOptionType type, IpV6Address address)
            : base(type)
        {
            Address = address;
        }

        internal IpV6Address Address { get; private set; }

        internal static bool Read(DataSegment data, out IpV6Address address)
        {
            if (data.Length != OptionDataLength)
            {
                address = IpV6Address.Zero;
                return false;
            }

            address = data.ReadIpV6Address(0, Endianity.Big);
            return true;
        }

        internal override sealed int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override sealed void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Address, Endianity.Big);
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
    public sealed class IpV6MobilityOptionAlternateCareOfAddress : IpV6MobilityOptionIpV6Address
    {
        public IpV6MobilityOptionAlternateCareOfAddress(IpV6Address alternateCareOfAddress)
            : base(IpV6MobilityOptionType.AlternateCareOfAddress, alternateCareOfAddress)
        {
        }

        /// <summary>
        /// Contains an address to use as the care-of address for the binding, rather than using the Source Address of the packet as the care-of address.
        /// </summary>
        public IpV6Address AlternateCareOfAddress { get { return Address; } }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            IpV6Address alternateCareOfAddress;
            if (!Read(data, out alternateCareOfAddress))
                return null;

            return new IpV6MobilityOptionAlternateCareOfAddress(alternateCareOfAddress);
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
    public sealed class IpV6MobilityOptionNonceIndices : IpV6MobilityOptionComplex
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
    public sealed class IpV6MobilityOptionBindingAuthorizationData : IpV6MobilityOptionSingleDataSegmentField
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
    /// RFC 3963, 5213.
    /// <pre>
    /// +-----+--------------+---------------+
    /// | Bit | 0-7          | 8-15          |
    /// +-----+--------------+---------------+
    /// | 0   | Option Type  | Opt Data Len  |
    /// +-----+--------------+---------------+
    /// | 16  | Reserved     | Prefix Length |
    /// +-----+--------------+---------------+
    /// | 32  | Network Prefix               |
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
    public abstract class IpV6MobilityOptionNetworkPrefix : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int PrefixLength = sizeof(ushort);
            public const int NetworkPrefix = PrefixLength + sizeof(ushort);
        }

        public const int OptionDataLength = Offset.NetworkPrefix + IpV6Address.SizeOf;

        public IpV6MobilityOptionNetworkPrefix(IpV6MobilityOptionType type, byte prefixLength, IpV6Address networkPrefix)
            : base(type)
        {
            PrefixLength = prefixLength;
            NetworkPrefix = networkPrefix;
        }

        /// <summary>
        /// Indicates the prefix length of the IPv6 prefix contained in the option.
        /// </summary>
        public byte PrefixLength { get; private set; }

        /// <summary>
        /// Contains the Network Prefix.
        /// </summary>
        public IpV6Address NetworkPrefix { get; private set; }

        internal override sealed int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override sealed void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.PrefixLength, PrefixLength);
            buffer.Write(offset + Offset.NetworkPrefix, NetworkPrefix, Endianity.Big);
            offset += OptionDataLength;
        }

        internal static bool Read(DataSegment data, out byte prefixLength, out IpV6Address networkPrefix)
        {
            if (data.Length != OptionDataLength)
            {
                prefixLength = 0;
                networkPrefix = IpV6Address.Zero;
                return false;
            }

            prefixLength = data[Offset.PrefixLength];
            networkPrefix = data.ReadIpV6Address(Offset.NetworkPrefix, Endianity.Big);
            return true;
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
    public sealed class IpV6MobilityOptionMobileNetworkPrefix : IpV6MobilityOptionNetworkPrefix
    {
        public IpV6MobilityOptionMobileNetworkPrefix(byte prefixLength, IpV6Address mobileNetworkPrefix)
            : base(IpV6MobilityOptionType.MobileNetworkPrefix, prefixLength, mobileNetworkPrefix)
        {
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            byte prefixLength;
            IpV6Address mobileNetworkPrefix;
            if (!Read(data, out prefixLength, out mobileNetworkPrefix))
                return null;

            return new IpV6MobilityOptionMobileNetworkPrefix(prefixLength, mobileNetworkPrefix);
        }
    }

    /// <summary>
    /// RFC 5213.
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
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.HomeNetworkPrefix)]
    public sealed class IpV6MobilityOptionHomeNetworkPrefix : IpV6MobilityOptionNetworkPrefix
    {
        public IpV6MobilityOptionHomeNetworkPrefix(byte prefixLength, IpV6Address homeNetworkPrefix)
            : base(IpV6MobilityOptionType.HomeNetworkPrefix, prefixLength, homeNetworkPrefix)
        {
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            byte prefixLength;
            IpV6Address mobileNetworkPrefix;
            if (!Read(data, out prefixLength, out mobileNetworkPrefix))
                return null;

            return new IpV6MobilityOptionHomeNetworkPrefix(prefixLength, mobileNetworkPrefix);
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
    public sealed class IpV6MobilityOptionLinkLayerAddress : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int OptionCode = 0;
            public const int LinkLayerAddress = OptionCode + sizeof(byte);
        }

        public const int OptionDataMinimumLength = Offset.LinkLayerAddress;

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
            if (data.Length < OptionDataMinimumLength)
                return null;

            IpV6MobilityOptionLinkLayerAddressCode code = (IpV6MobilityOptionLinkLayerAddressCode)data[Offset.OptionCode];
            DataSegment linkLayerAddress = data.Subsegment(Offset.LinkLayerAddress, data.Length - Offset.LinkLayerAddress);

            return new IpV6MobilityOptionLinkLayerAddress(code, linkLayerAddress);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + LinkLayerAddress.Length; }
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
    public sealed class IpV6MobilityOptionMobileNodeIdentifier : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int Subtype = 0;
            public const int Identifier = Subtype + sizeof(byte);
        }

        public const int OptionDataMinimumLength = Offset.Identifier;

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
            if (data.Length < OptionDataMinimumLength)
                return null;

            IpV6MobileNodeIdentifierSubtype subtype = (IpV6MobileNodeIdentifierSubtype)data[Offset.Subtype];
            DataSegment identifier = data.Subsegment(Offset.Identifier, data.Length - Offset.Identifier);

            return new IpV6MobilityOptionMobileNodeIdentifier(subtype, identifier);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + Identifier.Length; }
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
    public sealed class IpV6MobilityOptionAuthentication : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int Subtype = 0;
            public const int MobilitySecurityParameterIndex = Subtype + sizeof(byte);
            public const int AuthenticationData = MobilitySecurityParameterIndex + sizeof(uint);
        }

        public const int OptionDataMinimumLength = Offset.AuthenticationData;

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
            if (data.Length < OptionDataMinimumLength)
                return null;

            IpV6AuthenticationSubtype subtype = (IpV6AuthenticationSubtype)data[Offset.Subtype];
            uint mobilitySecurityParameterIndex = data.ReadUInt(Offset.MobilitySecurityParameterIndex, Endianity.Big);
            DataSegment authenticationData = data.Subsegment(Offset.AuthenticationData, data.Length - Offset.AuthenticationData);

            return new IpV6MobilityOptionAuthentication(subtype, mobilitySecurityParameterIndex, authenticationData);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + AuthenticationData.Length; }
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
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Value                      |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6MobilityOptionULong : IpV6MobilityOptionComplex
    {
        public const int OptionDataLength = sizeof(ulong);

        public IpV6MobilityOptionULong(IpV6MobilityOptionType type, ulong value)
            : base(type)
        {
            Value = value;
        }

        internal ulong Value { get; private set; }

        internal static bool Read(DataSegment data, out ulong value)
        {
            if (data.Length != OptionDataLength)
            {
                value = 0;
                return false;
            }

            value = data.ReadULong(0, Endianity.Big);
            return true;
        }

        internal override sealed int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override sealed void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Value, Endianity.Big);
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
    public sealed class IpV6MobilityOptionReplayProtection : IpV6MobilityOptionULong
    {
        public IpV6MobilityOptionReplayProtection(ulong timestamp)
            : base(IpV6MobilityOptionType.ReplayProtection, timestamp)
        {
        }

        /// <summary>
        /// 64 bit timestamp.
        /// </summary>
        public ulong Timestamp { get { return Value; } }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            ulong timestamp;
            if (!Read(data, out timestamp))
                return null;

            return new IpV6MobilityOptionReplayProtection(timestamp);
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
    public sealed class IpV6MobilityOptionCgaParametersRequest : IpV6MobilityOptionEmpty
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

    /// <summary>
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Value                      |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
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
    public sealed class IpV6MobilityOptionCgaParameters : IpV6MobilityOptionSingleDataSegmentField
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
    public sealed class IpV6MobilityOptionSignature : IpV6MobilityOptionSingleDataSegmentField
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
    public sealed class IpV6MobilityOptionPermanentHomeKeygenToken : IpV6MobilityOptionSingleDataSegmentField
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
    public sealed class IpV6MobilityOptionCareOfTestInit : IpV6MobilityOptionEmpty
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
    public sealed class IpV6MobilityOptionCareOfTest : IpV6MobilityOptionULong
    {
        public IpV6MobilityOptionCareOfTest(ulong careOfKeygenToken)
            : base(IpV6MobilityOptionType.CareOfTest, careOfKeygenToken)
        {
        }

        /// <summary>
        /// Contains the care-of keygen token generated by the correspondent node.
        /// </summary>
        public ulong CareOfKeygenToken { get { return Value; } }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            ulong careOfKeygenToken;
            if (!Read(data, out careOfKeygenToken))
                return null;

            return new IpV6MobilityOptionCareOfTest(careOfKeygenToken);
        }
    }

    /// <summary>
    /// RFC 5026.
    /// </summary>
    public enum IpV6DnsUpdateStatus : byte
    {
        /// <summary>
        /// DNS update performed.
        /// </summary>
        DnsUpdatePerformed = 0,
        
        /// <summary>
        /// Reason unspecified.
        /// </summary>
        ReasonUnspecified = 128,
        
        /// <summary>
        /// Administratively prohibited.
        /// </summary>
        AdministrativelyProhibited = 129,

        /// <summary>
        /// DNS update failed.
        /// </summary>
        DnsUpdateFailed = 130,
    }

    /// <summary>
    /// RFC 5026.
    /// <pre>
    /// +-----+-------------+---+----------+
    /// | Bit | 0-7         | 8 | 9-15     |
    /// +-----+-------------+---+----------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+---+----------+
    /// | 16  | Status      | R | Reserved |
    /// +-----+-------------+---+----------+
    /// | 32  | MN identity (FQDN)         |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.DnsUpdate)]
    public sealed class IpV6MobilityOptionDnsUpdate : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int Status = 0;
            public const int Remove = Status + sizeof(byte);
            public const int MobileNodeIdentity = Remove + sizeof(byte);
        }

        private static class Mask
        {
            public const byte Remove = 0x80;
        }

        public const int OptionDataMinimumLength = Offset.MobileNodeIdentity;

        public IpV6MobilityOptionDnsUpdate(IpV6DnsUpdateStatus status, bool remove, DataSegment mobileNodeIdentity)
            : base(IpV6MobilityOptionType.DnsUpdate)
        {
            Status = status;
            Remove = remove;
            MobileNodeIdentity = mobileNodeIdentity;
        }

        /// <summary>
        /// Indicating the result of the dynamic DNS update procedure.
        /// This field must be set to 0 and ignored by the receiver when the DNS Update mobility option is included in a Binding Update message.
        /// When the DNS Update mobility option is included in the Binding Acknowledgement message, 
        /// values of the Status field less than 128 indicate that the dynamic DNS update was performed successfully by the Home Agent.
        /// Values greater than or equal to 128 indicate that the dynamic DNS update was not completed by the HA.
        /// </summary>
        public IpV6DnsUpdateStatus Status { get; private set; }

        /// <summary>
        /// Whether the Mobile Node is requesting the HA to remove the DNS entry identified by the FQDN specified in this option and the HoA of the Mobile Node.
        /// If false, the Mobile Node is requesting the HA to create or update a DNS entry with its HoA and the FQDN specified in the option.
        /// </summary>
        public bool Remove { get; private set; }

        /// <summary>
        /// The identity of the Mobile Node in FQDN format to be used by the Home Agent to send a Dynamic DNS update.
        /// </summary>
        public DataSegment MobileNodeIdentity { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            IpV6DnsUpdateStatus status = (IpV6DnsUpdateStatus)data[Offset.Status];
            bool remove = data.ReadBool(Offset.Remove, Mask.Remove);
            DataSegment mobileNodeIdentity = data.Subsegment(Offset.MobileNodeIdentity, data.Length - Offset.MobileNodeIdentity);

            return new IpV6MobilityOptionDnsUpdate(status, remove, mobileNodeIdentity);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + MobileNodeIdentity.Length; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Status, (byte)Status);
            byte flags = 0;
            if (Remove)
                flags |= Mask.Remove;
            buffer.Write(offset + Offset.Remove, flags);
            buffer.Write(offset + Offset.MobileNodeIdentity, MobileNodeIdentity);
            offset += DataLength;
        }
    }

    /// <summary>
    /// RFC 5096.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Data                       |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.Experimental)]
    public sealed class IpV6MobilityOptionExperimental : IpV6MobilityOptionSingleDataSegmentField
    {
        public IpV6MobilityOptionExperimental(DataSegment data)
            : base(IpV6MobilityOptionType.Experimental, data)
        {
        }

        /// <summary>
        /// Data related to the experimental protocol extension.
        /// </summary>
        public DataSegment Data
        {
            get { return Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            return new IpV6MobilityOptionExperimental(data);
        }
    }

    /// <summary>
    /// RFC 5094.
    /// <pre>
    /// +-----+--------------+
    /// | Bit | 0-7          |
    /// +-----+--------------+
    /// | 0   | Option Type  |
    /// +-----+--------------+
    /// | 8   | Opt Data Len |
    /// +-----+--------------+
    /// | 16  | Vendor ID    |
    /// |     |              |
    /// |     |              |
    /// |     |              |
    /// +-----+--------------+
    /// | 48  | Sub-Type     |
    /// +-----+--------------+
    /// | 56  | Data         |
    /// | ... |              |
    /// +-----+--------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.VendorSpecific)]
    public sealed class IpV6MobilityOptionVendorSpecific : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int VendorId = 0;
            public const int SubType = VendorId + sizeof(uint);
            public const int Data = SubType + sizeof(byte);
        }

        public const int OptionDataMinimumLength = Offset.Data;

        public IpV6MobilityOptionVendorSpecific(uint vendorId, byte subType, DataSegment data)
            : base(IpV6MobilityOptionType.VendorSpecific)
        {
            VendorId = vendorId;
            SubType = subType;
            Data = data;
        }

        /// <summary>
        /// The SMI Network Management Private Enterprise Code of the IANA- maintained Private Enterprise Numbers registry.
        /// See http://www.iana.org/assignments/enterprise-numbers/enterprise-numbers
        /// </summary>
        public uint VendorId { get; private set; }

        /// <summary>
        /// Indicating the type of vendor-specific information carried in the option.
        /// The administration of the Sub-type is done by the Vendor.
        /// </summary>
        public byte SubType { get; private set; }

        /// <summary>
        /// Vendor-specific data that is carried in this message.
        /// </summary>
        public DataSegment Data { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            uint vendorId = data.ReadUInt(Offset.VendorId, Endianity.Big);
            byte subType = data[Offset.SubType];
            DataSegment vendorSpecificData = data.Subsegment(Offset.Data, data.Length - Offset.Data);
            return new IpV6MobilityOptionVendorSpecific(vendorId, subType, vendorSpecificData);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + Data.Length; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.VendorId, VendorId, Endianity.Big);
            buffer.Write(offset + Offset.SubType, SubType);
            buffer.Write(offset + Offset.Data, Data);
            offset += DataLength;
        }
    }

    /// <summary>
    /// RFC 5149.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Identifier                 |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.ServiceSelection)]
    public sealed class IpV6MobilityOptionServiceSelection : IpV6MobilityOptionSingleDataSegmentField
    {
        public IpV6MobilityOptionServiceSelection(DataSegment data)
            : base(IpV6MobilityOptionType.Experimental, data)
        {
        }

        /// <summary>
        /// Encoded service identifier string used to identify the requested service.
        /// The identifier string length is between 1 and 255 octets.
        /// This specification allows international identifier strings that are based on the use of Unicode characters, encoded as UTF-8,
        /// and formatted using Normalization Form KC (NFKC).
        /// 
        /// 'ims', 'voip', and 'voip.companyxyz.example.com' are valid examples of Service Selection option Identifiers.
        /// At minimum, the Identifier must be unique among the home agents to which the mobile node is authorized to register.
        /// </summary>
        public DataSegment Identifier
        {
            get { return Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            return new IpV6MobilityOptionServiceSelection(data);
        }
    }

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

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.SecurityParameterIndex, SecurityParameterIndex, Endianity.Big);
            buffer.Write(offset + Offset.Authenticator, Authenticator);
            offset += DataLength;
        }
    }

    /// <summary>
    /// RFC 5213.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved    | Value        |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6MobilityOptionReservedByteValueByte : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int Value = sizeof(byte);
        }

        public const int OptionDataLength = Offset.Value + sizeof(byte);

        public IpV6MobilityOptionReservedByteValueByte(IpV6MobilityOptionType type, byte value)
            : base(type)
        {
            Value = value;
        }

        internal byte Value { get; private set; }

        internal static bool Read(DataSegment data, out byte value)
        {
            if (data.Length != OptionDataLength)
            {
                value = 0;
                return false;
            }

            value = data[Offset.Value];
            return true;
        }

        internal override sealed int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override sealed void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Value, Value);
            offset += DataLength;
        }
    }

    /// <summary>
    /// RFC 5213.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved    | HI           |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.HandoffIndicator)]
    public sealed class IpV6MobilityOptionHandoffIndicator : IpV6MobilityOptionReservedByteValueByte
    {
        public IpV6MobilityOptionHandoffIndicator(IpV6HandoffIndicator handoffIndicator)
            : base(IpV6MobilityOptionType.HandoffIndicator, (byte)handoffIndicator)
        {
        }

        /// <summary>
        /// Specifies the type of handoff.
        /// </summary>
        public IpV6HandoffIndicator HandoffIndicator { get { return (IpV6HandoffIndicator)Value; } }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            byte value;
            if (!Read(data, out value))
                return null;

            return new IpV6MobilityOptionHandoffIndicator((IpV6HandoffIndicator)value);
        }
    }

    /// <summary>
    /// RFC 5213.
    /// </summary>
    public enum IpV6HandoffIndicator : byte
    {
        /// <summary>
        /// Attachment over a new interface.
        /// </summary>
        AttachmentOverNewInterface = 1,

        /// <summary>
        /// Handoff between two different interfaces of the mobile node.
        /// </summary>
        HandoffBetweenTwoDifferentInterfacesOfTheMobileNode = 2,

        /// <summary>
        /// Handoff between mobile access gateways for the same interface.
        /// </summary>
        HandoffBetweenMobileAccessGatewaysForTheSameInterface = 3,

        /// <summary>
        /// Handoff state unknown.
        /// </summary>
        HandoffStateUnknown = 4,

        /// <summary>
        /// Handoff state not changed (Re-registration).
        /// </summary>
        HandoffStateNotChanged = 5,
    }

    /// <summary>
    /// RFC 5213.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved    | ATT          |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.AccessTechnologyType)]
    public sealed class IpV6MobilityOptionAccessTechnologyType : IpV6MobilityOptionReservedByteValueByte
    {
        public IpV6MobilityOptionAccessTechnologyType(IpV6AccessTechnologyType accessTechnologyType)
            : base(IpV6MobilityOptionType.AccessTechnologyType, (byte)accessTechnologyType)
        {
        }

        /// <summary>
        /// Specifies the access technology through which the mobile node is connected to the access link on the mobile access gateway.
        /// </summary>
        public IpV6AccessTechnologyType AccessTechnologyType { get { return (IpV6AccessTechnologyType)Value; } }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            byte value;
            if (!Read(data, out value))
                return null;

            return new IpV6MobilityOptionAccessTechnologyType((IpV6AccessTechnologyType)value);
        }
    }

    /// <summary>
    /// RFC 5213.
    /// </summary>
    public enum IpV6AccessTechnologyType : byte
    {
        /// <summary>
        /// Reserved.
        /// </summary>
        Reserved = 0,

        /// <summary>
        /// Virtual.
        /// Logical Network Interface.
        /// </summary>
        LogicalNetworkInterface = 1,

        /// <summary>
        /// Point-to-Point Protocol.
        /// </summary>
        PointToPointProtocol = 2,

        /// <summary>
        /// IEEE 802.3.
        /// Ethernet.
        /// </summary>
        Ethernet = 3,

        /// <summary>
        /// IEEE 802.11a/b/g.
        /// Wireless LAN.
        /// </summary>
        WirelessLan = 4,

        /// <summary>
        /// IEEE 802.16e.
        /// WIMAX.
        /// </summary>
        WiMax = 5,
    }

    /// <summary>
    /// RFC 5213.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved                   |
    /// +-----+----------------------------+
    /// | 32  | Link-layer Identifier      |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.MobileNodeLinkLayerIdentifier)]
    public class IpV6MobilityOptionMobileNodeLinkLayerIdentifier : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int LinkLayerIdentifier = sizeof(ushort);
        }

        public const int OptionDataMinimumLength = Offset.LinkLayerIdentifier;

        public IpV6MobilityOptionMobileNodeLinkLayerIdentifier(DataSegment linkLayerIdentifier)
            : base(IpV6MobilityOptionType.MobileNodeLinkLayerIdentifier)
        {
            LinkLayerIdentifier = linkLayerIdentifier;
        }

        /// <summary>
        /// Contains the mobile node's link-layer identifier.
        /// </summary>
        public DataSegment LinkLayerIdentifier { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            DataSegment linkLayerIdentifier = data.Subsegment(Offset.LinkLayerIdentifier, data.Length - Offset.LinkLayerIdentifier);
            return new IpV6MobilityOptionMobileNodeLinkLayerIdentifier(linkLayerIdentifier);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + LinkLayerIdentifier.Length; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.LinkLayerIdentifier, LinkLayerIdentifier);
            offset += DataLength;
        }
    }

    /// <summary>
    /// RFC 5213.
    /// <pre>
    /// +-----+--------------+--------------+
    /// | Bit | 0-7          | 8-15         |
    /// +-----+--------------+--------------+
    /// | 0   | Option Type  | Opt Data Len |
    /// +-----+--------------+--------------+
    /// | 16  | Link-local Address          |
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
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.LinkLocalAddress)]
    public sealed class IpV6MobilityOptionLinkLocalAddress : IpV6MobilityOptionIpV6Address
    {
        public IpV6MobilityOptionLinkLocalAddress(IpV6Address linkLocalAddress)
            : base(IpV6MobilityOptionType.LinkLocalAddress, linkLocalAddress)
        {
        }

        /// <summary>
        /// Contains the link-local address.
        /// </summary>
        public IpV6Address LinkLocalAddress { get { return Address; } }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            IpV6Address linkLocalAddress;
            if (!Read(data, out linkLocalAddress))
                return null;

            return new IpV6MobilityOptionLinkLocalAddress(linkLocalAddress);
        }
    }

    /// <summary>
    /// RFC 5213.
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
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.Timestamp)]
    public sealed class IpV6MobilityOptionTimestamp : IpV6MobilityOptionULong
    {
        public IpV6MobilityOptionTimestamp(ulong timestamp)
            : base(IpV6MobilityOptionType.Timestamp, timestamp)
        {
        }

        /// <summary>
        /// Timestamp.  
        /// The value indicates the number of seconds since January 1, 1970, 00:00 UTC, by using a fixed point format.
        /// In this format, the integer number of seconds is contained in the first 48 bits of the field, and the remaining 16 bits indicate the number of 1/65536 fractions of a second.
        /// </summary>
        public ulong Timestamp { get { return Value; } }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            ulong timestamp;
            if (!Read(data, out timestamp))
                return null;

            return new IpV6MobilityOptionTimestamp(timestamp);
        }
    }

    /// <summary>
    /// RFC 5847.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Restart Counter            |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.RestartCounter)]
    public class IpV6MobilityOptionRestartCounter : IpV6MobilityOptionComplex
    {
        public const int OptionDataLength = sizeof(uint);

        public IpV6MobilityOptionRestartCounter(uint restartCounter)
            : base(IpV6MobilityOptionType.RestartCounter)
        {
            RestartCounter = restartCounter;
        }

        /// <summary>
        /// Indicates the current Restart Counter value.
        /// </summary>
        public uint RestartCounter { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            uint restartCounter = data.ReadUInt(0, Endianity.Big);
            return new IpV6MobilityOptionRestartCounter(restartCounter);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, RestartCounter, Endianity.Big);
        }
    }

    /// <summary>
    /// RFC 5555.
    /// <pre>
    /// +-----+-------------+--------------+----------+
    /// | Bit | 0-7         | 8-13         | 14-15    |
    /// +-----+-------------+--------------+----------+
    /// | 0   | Option Type | Opt Data Len            |
    /// +-----+-------------+--------------+----------+
    /// | 16  | Status      | Prefix-len   | Reserved |
    /// +-----+-------------+--------------+----------+
    /// | 32  | IPv4 home address                     |
    /// |     |                                       |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.IpV4AddressAcknowledgement)]
    public class IpV6MobilityOptionIpV4AddressAcknowledgement : IpV6MobilityOptionComplex
    {
        public const byte MaxPrefixLength = 0x3F;

        private static class Offset
        {
            public const int Status = 0;
            public const int PrefixLength = Status + sizeof(byte);
            public const int HomeAddress = PrefixLength + sizeof(byte);
        }

        private static class Mask
        {
            public const byte PrefixLength = 0xFC;
        }

        private static class Shift
        {
            public const int PrefixLength = 2;
        }

        public const int OptionDataLength = Offset.HomeAddress + IpV4Address.SizeOf;

        public IpV6MobilityOptionIpV4AddressAcknowledgement(IpV6AddressAcknowledgementStatus status, byte prefixLength, IpV4Address homeAddress)
            : base(IpV6MobilityOptionType.IpV4AddressAcknowledgement)
        {
            if (prefixLength > MaxPrefixLength)
                throw new ArgumentOutOfRangeException("prefixLength", prefixLength, string.Format("Exceeded maximum value {0}", MaxPrefixLength));

            Status = status;
            PrefixLength = prefixLength;
            HomeAddress = homeAddress;
        }

        /// <summary>
        /// Indicates success or failure for the IPv4 home address binding.
        /// Values from 0 to 127 indicate success.
        /// Higher values indicate failure.
        /// </summary>
        public IpV6AddressAcknowledgementStatus Status { get; private set; }

        /// <summary>
        /// The prefix length of the address allocated.
        /// This field is only valid in case of success and must be set to zero and ignored in case of failure.
        /// This field overrides what the mobile node requested (if not equal to the requested length).
        /// </summary>
        public byte PrefixLength { get; private set; }

        /// <summary>
        /// The IPv4 home address that the home agent will use in the binding cache entry.
        /// This could be a public or private address.
        /// This field must contain the mobile node's IPv4 home address.
        /// If the address were dynamically allocated, the home agent will add the address to inform the mobile node.
        /// Otherwise, if the address is statically allocated to the mobile node, the home agent will copy it from the binding update message.
        /// </summary>
        public IpV4Address HomeAddress { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            IpV6AddressAcknowledgementStatus status = (IpV6AddressAcknowledgementStatus)data[Offset.Status];
            byte prefixLength = (byte)((data[Offset.PrefixLength] & Mask.PrefixLength) >> Shift.PrefixLength);
            IpV4Address homeAddress = data.ReadIpV4Address(Offset.HomeAddress, Endianity.Big);
            return new IpV6MobilityOptionIpV4AddressAcknowledgement(status, prefixLength, homeAddress);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Status, (byte)Status);
            buffer.Write(offset + Offset.PrefixLength, (byte)(PrefixLength << Shift.PrefixLength));
            buffer.Write(offset + Offset.HomeAddress, HomeAddress, Endianity.Big);
            offset += OptionDataLength;
        }
    }

    /// <summary>
    /// RFC 5555.
    /// </summary>
    public enum IpV6AddressAcknowledgementStatus : byte
    {
        /// <summary>
        /// Success.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Failure, reason unspecified.
        /// </summary>
        FailureReasonUnspecified = 128,

        /// <summary>
        /// Administratively prohibited.
        /// </summary>
        AdministrativelyProhibited = 129,

        /// <summary>
        /// Incorrect IPv4 home address.
        /// </summary>
        IncorrectIpV4HomeAddress = 130,

        /// <summary>
        /// Invalid IPv4 address.
        /// </summary>
        InvalidIpV4Address = 131,

        /// <summary>
        /// Dynamic IPv4 home address assignment not available.
        /// </summary>
        DynamicIpV4HomeAddressAssignmentNotAvailable = 132,

        /// <summary>
        /// Prefix allocation unauthorized.
        /// </summary>
        PrefixAllocationUnauthorized = 133,
    }

    /// <summary>
    /// RFC 5555.
    /// <pre>
    /// +-----+------------+---+---+--------------+
    /// | Bit | 0-5        | 6 | 7 | 8-15         |
    /// +-----+------------+---+---+--------------+
    /// | 0   | Option Type        | Opt Data Len |
    /// +-----+------------+---+---+--------------+
    /// | 16  | Prefix-len | P | Reserved         |
    /// +-----+------------+---+------------------+
    /// | 32  | IPv4 home address                 |
    /// |     |                                   |
    /// +-----+-----------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.IpV4HomeAddress)]
    public class IpV6MobilityOptionIpV4HomeAddress : IpV6MobilityOptionComplex
    {
        public const byte MaxPrefixLength = 0x3F;

        private static class Offset
        {
            public const int PrefixLength = 0;
            public const int RequestPrefix = PrefixLength;
            public const int HomeAddress = RequestPrefix + sizeof(byte) + sizeof(byte);
        }

        private static class Mask
        {
            public const byte PrefixLength = 0xFC;
            public const byte RequestPrefix = 0x02;
        }

        private static class Shift
        {
            public const int PrefixLength = 2;
        }

        public const int OptionDataLength = Offset.HomeAddress + IpV4Address.SizeOf;

        public IpV6MobilityOptionIpV4HomeAddress(byte prefixLength, bool requestPrefix, IpV4Address homeAddress)
            : base(IpV6MobilityOptionType.IpV4HomeAddress)
        {
            if (prefixLength > MaxPrefixLength)
                throw new ArgumentOutOfRangeException("prefixLength", prefixLength, string.Format("Exceeded maximum value {0}", MaxPrefixLength));

            PrefixLength = prefixLength;
            RequestPrefix = requestPrefix;
            HomeAddress = homeAddress;
        }

        /// <summary>
        /// The length of the prefix allocated to the mobile node.
        /// If only a single address is allocated, this field must be set to 32.
        /// In the first binding update requesting a prefix, the field contains the prefix length requested.
        /// However, in the following binding updates, this field must contain the length of the prefix allocated.
        /// A value of zero is invalid and must be considered an error.
        /// </summary>
        public byte PrefixLength { get; private set; }

        /// <summary>
        /// When true, indicates that the mobile node requests a mobile network prefix.
        /// This flag is only relevant for new requests, and must be ignored for binding refreshes.
        /// </summary>
        public bool RequestPrefix { get; private set; }

        /// <summary>
        /// The mobile node's IPv4 home address that should be defended by the home agent.
        /// This field could contain any unicast IPv4 address (public or private) that was assigned to the mobile node.
        /// The value 0.0.0.0 is used to request an IPv4 home address from the home agent.
        /// A mobile node may choose to use this option to request a prefix by setting the address to All Zeroes and setting the RequestPrefix flag.
        /// The mobile node could then form an IPv4 home address based on the allocated prefix.
        /// Alternatively, the mobile node may use two different options, one for requesting an address (static or dynamic) and another for requesting a 
        /// prefix.
        /// </summary>
        public IpV4Address HomeAddress { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            byte prefixLength = (byte)((data[Offset.PrefixLength] & Mask.PrefixLength) >> Shift.PrefixLength);
            bool requestPrefix = data.ReadBool(Offset.RequestPrefix, Mask.RequestPrefix);
            IpV4Address homeAddress = data.ReadIpV4Address(Offset.HomeAddress, Endianity.Big);
            return new IpV6MobilityOptionIpV4HomeAddress(prefixLength, requestPrefix, homeAddress);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            byte prefixLengthAndRequestPrefix = (byte)(PrefixLength << Shift.PrefixLength);
            if (RequestPrefix)
                prefixLengthAndRequestPrefix |= Mask.RequestPrefix;

            buffer.Write(offset + Offset.PrefixLength, prefixLengthAndRequestPrefix);
            buffer.Write(offset + Offset.HomeAddress, HomeAddress, Endianity.Big);
            offset += OptionDataLength;
        }
    }
}
