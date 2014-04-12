using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ip;
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

        internal override int DataLength
        {
            get { return Data.Length; }
        }

        internal override bool EqualsData(IpV6Option other)
        {
            return EqualsData(other as IpV6OptionUnknown);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Data);
        }

        private bool EqualsData(IpV6OptionUnknown other)
        {
            return other != null &&
                   Data.Equals(other.Data);
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

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return true;
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            offset += PaddingDataLength;
        }

        internal IpV6MobilityOptionPadN()
            : this(0)
        {
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
            RefreshInterval = refreshInterval;
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

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionBindingRefreshAdvice);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, RefreshInterval, Endianity.Big);
        }

        private IpV6MobilityOptionBindingRefreshAdvice()
            : this(0)
        {
        }

        private bool EqualsData(IpV6MobilityOptionBindingRefreshAdvice other)
        {
            return other != null &&
                   RefreshInterval == other.RefreshInterval;
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

        internal sealed override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionIpV6Address);
        }

        internal override sealed void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Address, Endianity.Big);
        }

        private bool EqualsData(IpV6MobilityOptionIpV6Address other)
        {
            return other != null &&
                   other.Address.Equals(other.Address);
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

        private IpV6MobilityOptionAlternateCareOfAddress() 
            : this(IpV6Address.Zero)
        {
        }

        /// <summary>
        /// Contains an address to use as the care-of address for the binding, rather than using the Source Address of the packet as the care-of address.
        /// </summary>
        public IpV6Address AlternateCareOfAddress
        {
            get { return Address; }
        }

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

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionNonceIndices);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, HomeNonceIndex, Endianity.Big);
            buffer.Write(ref offset, CareOfNonceIndex, Endianity.Big);
        }

        private IpV6MobilityOptionNonceIndices()
            : this(0, 0)
        {
        }

        private bool EqualsData(IpV6MobilityOptionNonceIndices other)
        {
            return other != null &&
                   HomeNonceIndex == other.HomeNonceIndex && CareOfNonceIndex == other.CareOfNonceIndex;
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

        private IpV6MobilityOptionBindingAuthorizationData()
            : this(DataSegment.Empty)
        {
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

        internal sealed override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionNetworkPrefix);
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

        private bool EqualsData(IpV6MobilityOptionNetworkPrefix other)
        {
            return other != null &&
                   PrefixLength == other.PrefixLength && NetworkPrefix.Equals(other.NetworkPrefix);
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

        private IpV6MobilityOptionMobileNetworkPrefix()
            : this(0, IpV6Address.Zero)
        {
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

        private IpV6MobilityOptionHomeNetworkPrefix() : this(0, IpV6Address.Zero)
        {
        }
    }

    public enum IpV6MobilityLinkLayerAddressCode : byte
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

        public IpV6MobilityOptionLinkLayerAddress(IpV6MobilityLinkLayerAddressCode code, DataSegment linkLayerAddress)
            : base(IpV6MobilityOptionType.LinkLayerAddress)
        {
            Code = code;
            LinkLayerAddress = linkLayerAddress;
        }

        public IpV6MobilityLinkLayerAddressCode Code { get; private set; }

        /// <summary>
        /// Variable-length link-layer address.
        /// </summary>
        public DataSegment LinkLayerAddress { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            IpV6MobilityLinkLayerAddressCode code = (IpV6MobilityLinkLayerAddressCode)data[Offset.OptionCode];
            DataSegment linkLayerAddress = data.Subsegment(Offset.LinkLayerAddress, data.Length - Offset.LinkLayerAddress);

            return new IpV6MobilityOptionLinkLayerAddress(code, linkLayerAddress);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + LinkLayerAddress.Length; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionLinkLayerAddress);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.OptionCode, (byte)Code);
            LinkLayerAddress.Write(buffer, offset + Offset.LinkLayerAddress);
            offset += DataLength;
        }

        private IpV6MobilityOptionLinkLayerAddress()
            : this(IpV6MobilityLinkLayerAddressCode.Wildcard, DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6MobilityOptionLinkLayerAddress other)
        {
            return other != null &&
                   Code == other.Code && LinkLayerAddress.Equals(other.LinkLayerAddress);

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

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionMobileNodeIdentifier);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Subtype, (byte)Subtype);
            Identifier.Write(buffer, offset + Offset.Identifier);
            offset += DataLength;
        }

        private IpV6MobilityOptionMobileNodeIdentifier()
            : this(IpV6MobileNodeIdentifierSubtype.NetworkAccessIdentifier, DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6MobilityOptionMobileNodeIdentifier other)
        {
            return other != null &&
                   Subtype == other.Subtype && Identifier.Equals(other.Identifier);
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

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionAuthentication);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Subtype, (byte)Subtype);
            buffer.Write(offset + Offset.MobilitySecurityParameterIndex, MobilitySecurityParameterIndex, Endianity.Big);
            AuthenticationData.Write(buffer, offset + Offset.AuthenticationData);
            offset += DataLength;
        }

        private IpV6MobilityOptionAuthentication()
            : this(IpV6AuthenticationSubtype.HomeAgent, 0, DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6MobilityOptionAuthentication other)
        {
            return other != null &&
                   Subtype == other.Subtype && MobilitySecurityParameterIndex == other.MobilitySecurityParameterIndex &&
                   AuthenticationData.Equals(other.AuthenticationData);
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

        internal override sealed bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionULong);
        }

        internal override sealed void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Value, Endianity.Big);
        }

        private bool EqualsData(IpV6MobilityOptionULong other)
        {
            return other != null &&
                   Value == other.Value;
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
        public ulong Timestamp
        {
            get { return Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            ulong timestamp;
            if (!Read(data, out timestamp))
                return null;

            return new IpV6MobilityOptionReplayProtection(timestamp);
        }

        private IpV6MobilityOptionReplayProtection()
            : this(0)
        {
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

        internal sealed override bool EqualsData(IpV6MobilityOption other)
        {
            return true;
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
            : base(type)
        {
            Value = value;
        }

        internal DataSegment Value { get; private set; }

        internal override sealed int DataLength
        {
            get { return Value.Length; }
        }

        internal override sealed bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionSingleDataSegmentField);
        }

        internal override sealed void WriteData(byte[] buffer, ref int offset)
        {
            Value.Write(buffer, ref offset);
        }

        private bool EqualsData(IpV6MobilityOptionSingleDataSegmentField other)
        {
            return other != null &&
                   Value.Equals(other.Value);
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

        private IpV6MobilityOptionCgaParameters() 
            : this(DataSegment.Empty)
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

        private IpV6MobilityOptionSignature()
            : this(DataSegment.Empty)
        {
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

        private IpV6MobilityOptionPermanentHomeKeygenToken()
            : this(DataSegment.Empty)
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
        public ulong CareOfKeygenToken
        {
            get { return Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            ulong careOfKeygenToken;
            if (!Read(data, out careOfKeygenToken))
                return null;

            return new IpV6MobilityOptionCareOfTest(careOfKeygenToken);
        }

        private IpV6MobilityOptionCareOfTest() 
            : this(0)
        {
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

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionDnsUpdate);
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

        private IpV6MobilityOptionDnsUpdate()
            : this(IpV6DnsUpdateStatus.DnsUpdatePerformed, false, DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6MobilityOptionDnsUpdate other)
        {
            return other != null &&
                   Status == other.Status && Remove == other.Remove && MobileNodeIdentity.Equals(other.MobileNodeIdentity);
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

        private IpV6MobilityOptionExperimental() 
            : this(DataSegment.Empty)
        {
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

        internal sealed override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionVendorSpecific);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.VendorId, VendorId, Endianity.Big);
            buffer.Write(offset + Offset.SubType, SubType);
            buffer.Write(offset + Offset.Data, Data);
            offset += DataLength;
        }

        private IpV6MobilityOptionVendorSpecific()
            : this(0, 0, DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6MobilityOptionVendorSpecific other)
        {
            return other != null &&
                   VendorId == other.VendorId && SubType == other.SubType && Data.Equals(other.Data);
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
            : base(IpV6MobilityOptionType.ServiceSelection, data)
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

        private IpV6MobilityOptionServiceSelection()
            : this(DataSegment.Empty)
        {
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

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionBindingAuthorizationDataForFmIpV6);
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

    /// <summary>
    /// RFC 5213.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved    | Value        |
    /// +-----+-------------+--------------+
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

        internal sealed override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionReservedByteValueByte);
        }

        internal override sealed void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Value, Value);
            offset += DataLength;
        }

        private bool EqualsData(IpV6MobilityOptionReservedByteValueByte other)
        {
            return other != null &&
                   Value == other.Value;
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

        private IpV6MobilityOptionHandoffIndicator() 
            : this(IpV6HandoffIndicator.AttachmentOverNewInterface)
        {
        }

        /// <summary>
        /// Specifies the type of handoff.
        /// </summary>
        public IpV6HandoffIndicator HandoffIndicator
        {
            get { return (IpV6HandoffIndicator)Value; }
        }

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
        public IpV6AccessTechnologyType AccessTechnologyType
        {
            get { return (IpV6AccessTechnologyType)Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            byte value;
            if (!Read(data, out value))
                return null;

            return new IpV6MobilityOptionAccessTechnologyType((IpV6AccessTechnologyType)value);
        }

        private IpV6MobilityOptionAccessTechnologyType()
            : this(IpV6AccessTechnologyType.Reserved)
        {
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
    public sealed class IpV6MobilityOptionMobileNodeLinkLayerIdentifier : IpV6MobilityOptionComplex
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

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionMobileNodeLinkLayerIdentifier);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.LinkLayerIdentifier, LinkLayerIdentifier);
            offset += DataLength;
        }

        private IpV6MobilityOptionMobileNodeLinkLayerIdentifier()
            : this(DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6MobilityOptionMobileNodeLinkLayerIdentifier other)
        {
            return other != null &&
                   LinkLayerIdentifier.Equals(other.LinkLayerIdentifier);
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
        public IpV6Address LinkLocalAddress
        {
            get { return Address; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            IpV6Address linkLocalAddress;
            if (!Read(data, out linkLocalAddress))
                return null;

            return new IpV6MobilityOptionLinkLocalAddress(linkLocalAddress);
        }

        private IpV6MobilityOptionLinkLocalAddress()
            : this(IpV6Address.Zero)
        {
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
        public ulong Timestamp
        {
            get { return Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            ulong timestamp;
            if (!Read(data, out timestamp))
                return null;

            return new IpV6MobilityOptionTimestamp(timestamp);
        }

        private IpV6MobilityOptionTimestamp()
            : this(0)
        {
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
    public sealed class IpV6MobilityOptionRestartCounter : IpV6MobilityOptionComplex
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

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionRestartCounter);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, RestartCounter, Endianity.Big);
        }

        private IpV6MobilityOptionRestartCounter()
            : this(0)
        {
        }

        private bool EqualsData(IpV6MobilityOptionRestartCounter other)
        {
            return other != null &&
                   RestartCounter == other.RestartCounter;
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
    public sealed class IpV6MobilityOptionIpV4AddressAcknowledgement : IpV6MobilityOptionComplex
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

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionIpV4AddressAcknowledgement);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Status, (byte)Status);
            buffer.Write(offset + Offset.PrefixLength, (byte)(PrefixLength << Shift.PrefixLength));
            buffer.Write(offset + Offset.HomeAddress, HomeAddress, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionIpV4AddressAcknowledgement()
            : this(IpV6AddressAcknowledgementStatus.Success, 0, IpV4Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionIpV4AddressAcknowledgement other)
        {
            return other != null &&
                   Status == other.Status && PrefixLength == other.PrefixLength && HomeAddress.Equals(other.HomeAddress);
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
    public sealed class IpV6MobilityOptionIpV4HomeAddress : IpV6MobilityOptionComplex
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

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionIpV4HomeAddress);
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

        private IpV6MobilityOptionIpV4HomeAddress()
            : this(0, false, IpV4Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionIpV4HomeAddress other)
        {
            return other != null &&
                   PrefixLength == other.PrefixLength && RequestPrefix == other.RequestPrefix && HomeAddress.Equals(other.HomeAddress);
        }
    }

    /// <summary>
    /// RFC 5555.
    /// <pre>
    /// +-----+---+---------+--------------+
    /// | Bit | 0 | 1-7     | 8-15         |
    /// +-----+---+---------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+---+---------+--------------+
    /// | 16  | F | Reserved               |
    /// +-----+---+------------------------+
    /// | 32  | Refresh time               |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.NatDetection)]
    public sealed class IpV6MobilityOptionNatDetection : IpV6MobilityOptionComplex
    {
        public const uint RecommendedRefreshTime = 110;

        private static class Offset
        {
            public const int UdpEncapsulationRequired = 0;
            public const int RefreshTime = UdpEncapsulationRequired + sizeof(ushort);
        }

        public const int OptionDataLength = Offset.RefreshTime + sizeof(uint);

        private static class Mask
        {
            public const byte UdpEncapsulationRequired = 0x80;
        }

        public IpV6MobilityOptionNatDetection(bool udpEncapsulationRequired, uint refreshTime)
            : base(IpV6MobilityOptionType.NatDetection)
        {
            UdpEncapsulationRequired = udpEncapsulationRequired;
            RefreshTime = refreshTime;
        }

        /// <summary>
        /// Indicates to the mobile node that UDP encapsulation is required.
        /// When set, this flag indicates that the mobile node must use UDP encapsulation even if a NAT is not located between the mobile node and home agent.
        /// This flag should not be set when the mobile node is assigned an IPv6 care-of address with some exceptions.
        /// </summary>
        public bool UdpEncapsulationRequired { get; private set; }

        /// <summary>
        /// A suggested time (in seconds) for the mobile node to refresh the NAT binding.
        /// If set to zero, it is ignored.
        /// If this field is set to uint.MaxValue, it means that keepalives are not needed, i.e., no NAT was detected.
        /// The home agent must be configured with a default value for the refresh time.
        /// The recommended value is RecommendedRefreshTime.
        /// </summary>
        public uint RefreshTime { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            bool udpEncapsulationRequired = data.ReadBool(Offset.UdpEncapsulationRequired, Mask.UdpEncapsulationRequired);
            uint refreshTime = data.ReadUInt(Offset.RefreshTime, Endianity.Big);
            return new IpV6MobilityOptionNatDetection(udpEncapsulationRequired, refreshTime);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionNatDetection);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            byte udpEncapsulationRequired = 0;
            if (UdpEncapsulationRequired)
                udpEncapsulationRequired |= Mask.UdpEncapsulationRequired;

            buffer.Write(offset + Offset.UdpEncapsulationRequired, udpEncapsulationRequired);
            buffer.Write(offset + Offset.RefreshTime, RefreshTime, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionNatDetection()
            : this(false, 0)
        {
        }

        private bool EqualsData(IpV6MobilityOptionNatDetection other)
        {
            return other != null &&
                   UdpEncapsulationRequired == other.UdpEncapsulationRequired && RefreshTime == other.RefreshTime;
        }
    }

    /// <summary>
    /// RFC 5555, 5844.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved                   |
    /// +-----+----------------------------+
    /// | 32  | IPv4 address               |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6MobilityOptionIpV4Address : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int Address = sizeof(ushort);
        }

        public const int OptionDataLength = Offset.Address + IpV4Address.SizeOf;

        internal IpV6MobilityOptionIpV4Address(IpV6MobilityOptionType type, IpV4Address address) 
            : base(type)
        {
            Address = address;
        }

        internal IpV4Address Address { get; private set; }

        internal static bool Read(DataSegment data, out IpV4Address address)
        {
            if (data.Length != OptionDataLength)
            {
                address = IpV4Address.Zero;
                return false;
            }

            address = data.ReadIpV4Address(Offset.Address, Endianity.Big);
            return true;
        }

        internal override sealed int DataLength
        {
            get { return OptionDataLength; }
        }

        internal sealed override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionIpV4Address);
        }

        internal override sealed void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Address, Address, Endianity.Big);
            offset += OptionDataLength;
        }

        private bool EqualsData(IpV6MobilityOptionIpV4Address other)
        {
            return other != null &&
                   Address.Equals(other.Address);
        }
    }

    /// <summary>
    /// RFC 5555.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved                   |
    /// +-----+----------------------------+
    /// | 32  | IPv4 Care-of address       |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.IpV4CareOfAddress)]
    public sealed class IpV6MobilityOptionIpV4CareOfAddress : IpV6MobilityOptionIpV4Address
    {
        public IpV6MobilityOptionIpV4CareOfAddress(IpV4Address careOfAddress)
            : base(IpV6MobilityOptionType.IpV4CareOfAddress, careOfAddress)
        {
        }

        /// <summary>
        /// Contains the mobile node's IPv4 care-of address.
        /// The IPv4 care-of address is used when the mobile node is located in an IPv4-only network.
        /// </summary>
        public IpV4Address CareOfAddress { get { return Address; } }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            IpV4Address careOfAddress;
            if (!Read(data, out careOfAddress))
                return null;

            return new IpV6MobilityOptionIpV4CareOfAddress(careOfAddress);
        }

        private IpV6MobilityOptionIpV4CareOfAddress()
            : this(IpV4Address.Zero)
        {
        }
}

    /// <summary>
    /// RFC 5845.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved                   |
    /// +-----+----------------------------+
    /// | 32  | GRE Key Identifier         |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.GreKey)]
    public sealed class IpV6MobilityOptionGreKey : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int GreKeyIdentifier = sizeof(ushort);
        }

        public const int OptionDataLength = Offset.GreKeyIdentifier + sizeof(uint);

        public IpV6MobilityOptionGreKey(uint greKeyIdentifier)
            : base(IpV6MobilityOptionType.GreKey)
        {
            GreKeyIdentifier = greKeyIdentifier;
        }

        /// <summary>
        /// Contains the downlink or the uplink GRE key.
        /// This field is present in the GRE Key option only if the GRE keys are being exchanged using the Proxy Binding Update and Proxy Binding
        /// Acknowledgement messages.
        /// </summary>
        public uint GreKeyIdentifier { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            uint greKeyIdentifier = data.ReadUInt(Offset.GreKeyIdentifier, Endianity.Big);
            return new IpV6MobilityOptionGreKey(greKeyIdentifier);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionGreKey);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.GreKeyIdentifier, GreKeyIdentifier, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionGreKey() 
            : this(0)
        {
        }

        private bool EqualsData(IpV6MobilityOptionGreKey other)
        {
            return other != null &&
                   GreKeyIdentifier == other.GreKeyIdentifier;
        }
    }

    /// <summary>
    /// RFC 5845.
    /// </summary>
    public enum IpV6MobilityIpV6AddressPrefixCode : byte
    {
        /// <summary>
        /// Old Care-of Address.
        /// </summary>
        OldCareOfAddress = 1,

        /// <summary>
        /// New Care-of Address.
        /// </summary>
        NewCareOfAddress = 2,

        /// <summary>
        /// NAR's IP address.
        /// </summary>
        NewAccessRouterIpAddress = 3,

        /// <summary>
        /// NAR's Prefix, sent in PrRtAdv.
        /// The Prefix Length field contains the number of valid leading bits in the prefix.
        /// The bits in the prefix after the prefix length are reserved and must be initialized to zero by the sender and ignored by the receiver.
        /// </summary>
        NewAccessRouterPrefix = 4,
    }

    /// <summary>
    /// RFC 5845.
    /// <pre>
    /// +-----+-------------+---------------+
    /// | Bit | 0-7         | 8-15          |
    /// +-----+-------------+---------------+
    /// | 0   | Option Type | Opt Data Len  |
    /// +-----+-------------+---------------+
    /// | 16  | Option-Code | Prefix Length |
    /// +-----+-------------+---------------+
    /// | 32  | IPv6 Address/Prefix         |
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
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.IpV6AddressPrefix)]
    public class IpV6MobilityOptionIpV6AddressPrefix : IpV6MobilityOptionComplex
    {
        public const byte MaxPrefixLength = 128;

        private static class Offset
        {
            public const int Code = 0;
            public const int PrefixLength = Code + sizeof(byte);
            public const int AddressOrPrefix = PrefixLength + sizeof(byte);
        }

        public const int OptionDataLength = Offset.AddressOrPrefix + IpV6Address.SizeOf;

        public IpV6MobilityOptionIpV6AddressPrefix(IpV6MobilityIpV6AddressPrefixCode code, byte prefixLength, IpV6Address addressOrPrefix)
            : base(IpV6MobilityOptionType.IpV6AddressPrefix)
        {
            if (prefixLength > MaxPrefixLength)
                throw new ArgumentOutOfRangeException("prefixLength", prefixLength, string.Format("Max value is {0}", MaxPrefixLength));

            Code = code;
            PrefixLength = prefixLength;
            AddressOrPrefix = addressOrPrefix;
        }

        /// <summary>
        /// Describes the kind of the address or the prefix.
        /// </summary>
        public IpV6MobilityIpV6AddressPrefixCode Code { get; private set; }

        /// <summary>
        /// Indicates the length of the IPv6 Address Prefix.
        /// The value ranges from 0 to 128.
        /// </summary>
        public byte PrefixLength { get; private set; }

        /// <summary>
        /// The IP address/prefix defined by the Option-Code field.
        /// </summary>
        public IpV6Address AddressOrPrefix { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            IpV6MobilityIpV6AddressPrefixCode code = (IpV6MobilityIpV6AddressPrefixCode)data[Offset.Code];
            byte prefixLength = data[Offset.PrefixLength];
            IpV6Address addressOrPrefix = data.ReadIpV6Address(Offset.AddressOrPrefix, Endianity.Big);
            return new IpV6MobilityOptionIpV6AddressPrefix(code, prefixLength, addressOrPrefix);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionIpV6AddressPrefix);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Code, (byte)Code);
            buffer.Write(offset + Offset.PrefixLength, PrefixLength);
            buffer.Write(offset + Offset.AddressOrPrefix, AddressOrPrefix, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionIpV6AddressPrefix()
            : this(IpV6MobilityIpV6AddressPrefixCode.NewCareOfAddress, 0, IpV6Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionIpV6AddressPrefix other)
        {
            return other != null &&
                   Code == other.Code && PrefixLength == other.PrefixLength && AddressOrPrefix.Equals(other.AddressOrPrefix);
        }
    }

    /// <summary>
    /// RFCs 5648, 6089.
    /// <pre>
    /// +-----+-------------+---+-----------+
    /// | Bit | 0-7         | 8 | 9-15      |
    /// +-----+-------------+---+-----------+
    /// | 0   | Option Type | Opt Data Len  |
    /// +-----+-------------+---------------+
    /// | 16  |  Binding ID (BID)           |
    /// +-----+-------------+---+-----------+
    /// | 32  |  Status     | H | BID-PRI   |
    /// +-----+-------------+---+-----------+
    /// | 48  | IPv4 or IPv6                |
    /// | ... | care-of address (CoA)       |
    /// +-----+-----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.BindingIdentifier)]
    public class IpV6MobilityOptionBindingIdentifier : IpV6MobilityOptionComplex
    {
        public const byte MaxPriority = 0x7F;

        private static class Offset
        {
            public const int BindingId = 0;
            public const int Status = BindingId + sizeof(ushort);
            public const int SimultaneousHomeAndForeignBinding = Status + sizeof(byte);
            public const int Priority = SimultaneousHomeAndForeignBinding;
            public const int CareOfAddress = Priority + sizeof(byte);
        }

        public const int OptionDataMinimumLength = Offset.CareOfAddress;

        private static class Mask
        {
            public const byte SimultaneousHomeAndForeignBinding = 0x80;
            public const byte Priority = 0x7F;
        }

        public IpV6MobilityOptionBindingIdentifier(ushort bindingId, IpV6BindingAcknowledgementStatus status, bool simultaneousHomeAndForeignBinding,
                                                   byte priority, IpV4Address careOfAddress)
            : this(bindingId, status, simultaneousHomeAndForeignBinding, priority, careOfAddress, null)
        {
        }

        public IpV6MobilityOptionBindingIdentifier(ushort bindingId, IpV6BindingAcknowledgementStatus status, bool simultaneousHomeAndForeignBinding,
                                                   byte priority, IpV6Address careOfAddress)
            : this(bindingId, status, simultaneousHomeAndForeignBinding, priority, null, careOfAddress)
        {
        }

        public IpV6MobilityOptionBindingIdentifier(ushort bindingId, IpV6BindingAcknowledgementStatus status, bool simultaneousHomeAndForeignBinding,
                                                   byte priority)
            : this(bindingId, status, simultaneousHomeAndForeignBinding, priority, null, null)
        {
        }

        /// <summary>
        /// The BID that is assigned to the binding indicated by the care-of address in the Binding Update or the Binding Identifier mobility option.
        /// The value of zero is reserved and should not be used.
        /// </summary>
        public ushort BindingId { get; private set; }

        /// <summary>
        /// When the Binding Identifier mobility option is included in a Binding Acknowledgement,
        /// this field overwrites the Status field in the Binding Acknowledgement only for this BID.
        /// If this field is set to zero, the receiver ignores this field and uses the registration status stored in the Binding Acknowledgement message.
        /// The receiver must ignore this field if the Binding Identifier mobility option is not carried within either the Binding Acknowledgement
        /// or the Care-of Test messages.
        /// The possible status codes are the same as the status codes of the Binding Acknowledgement.
        /// This Status field is also used to carry error information related to the care-of address test in the Care-of Test message.
        /// </summary>
        public IpV6BindingAcknowledgementStatus Status { get; private set; }

        /// <summary>
        /// Indicates that the mobile node registers multiple bindings to the home agent while it is attached to the home link.
        /// This flag is valid only for a Binding Update sent to the home agent.
        /// </summary>
        public bool SimultaneousHomeAndForeignBinding { get; private set; }

        /// <summary>
        /// Places each BID to a relative priority (PRI) with other registered BIDs.
        /// Value '0' is reserved and must not be used.
        /// A lower number in this field indicates a higher priority, while BIDs with the same BID-PRI value have equal priority meaning that,
        /// the BID used is an implementation issue.
        /// This is consistent with current practice in packet classifiers.
        /// </summary>
        public byte Priority { get; private set; }

        /// <summary>
        /// The IPv4 care-of address for the corresponding BID, or null if no IPv4 care-of address is stored.
        /// </summary>
        public IpV4Address? IpV4CareOfAddress { get; private set; }

        /// <summary>
        /// The IPv6 care-of address for the corresponding BID, or null if no IPv6 care-of address is stored.
        /// </summary>
        public IpV6Address? IpV6CareOfAddress { get; private set; }

        /// <summary>
        /// If a Binding Identifier mobility option is included in a Binding Update for the home registration,
        /// either IPv4 or IPv6 care-of addresses for the corresponding BID can be stored in this field.
        /// For the binding registration to correspondent nodes (i.e., route optimization), only IPv6 care-of addresses can be stored in this field.
        /// If no address is specified in this field, returns null.
        /// If the option is included in any messages other than a Binding Update, returns null.
        /// </summary>
        public object CareOfAddress
        {
            get
            {
                if (IpV4CareOfAddress.HasValue)
                    return IpV4CareOfAddress.Value;
                if (IpV6CareOfAddress.HasValue)
                    return IpV6CareOfAddress.Value;
                return null;
            }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            ushort bindingId = data.ReadUShort(Offset.BindingId, Endianity.Big);
            IpV6BindingAcknowledgementStatus status = (IpV6BindingAcknowledgementStatus)data[Offset.Status];
            bool simultaneousHomeAndForeignBinding = data.ReadBool(Offset.SimultaneousHomeAndForeignBinding, Mask.SimultaneousHomeAndForeignBinding);
            byte priority = (byte)(data[Offset.Priority] & Mask.Priority);
            if (data.Length == OptionDataMinimumLength)
                return new IpV6MobilityOptionBindingIdentifier(bindingId, status, simultaneousHomeAndForeignBinding, priority);
            if (data.Length == OptionDataMinimumLength + IpV4Address.SizeOf)
            {
                IpV4Address careOfAddress = data.ReadIpV4Address(Offset.CareOfAddress, Endianity.Big);
                return new IpV6MobilityOptionBindingIdentifier(bindingId, status, simultaneousHomeAndForeignBinding, priority, careOfAddress);
            }
            if (data.Length == OptionDataMinimumLength + IpV6Address.SizeOf)
            {
                IpV6Address careOfAddress = data.ReadIpV6Address(Offset.CareOfAddress, Endianity.Big);
                return new IpV6MobilityOptionBindingIdentifier(bindingId, status, simultaneousHomeAndForeignBinding, priority, careOfAddress);
            }
            return null;
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + (IpV4CareOfAddress.HasValue ? IpV4Address.SizeOf : (IpV6CareOfAddress.HasValue ? IpV6Address.SizeOf : 0)); }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionBindingIdentifier);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.BindingId, BindingId, Endianity.Big);
            buffer.Write(offset + Offset.Status, (byte)Status);
            byte simultaneousHomeAndForeignBindingAndPriority = (byte)(Priority & Mask.Priority);
            if (SimultaneousHomeAndForeignBinding)
                simultaneousHomeAndForeignBindingAndPriority |= Mask.SimultaneousHomeAndForeignBinding;
            buffer.Write(offset + Offset.SimultaneousHomeAndForeignBinding, simultaneousHomeAndForeignBindingAndPriority);
            if (IpV4CareOfAddress.HasValue)
            {
                buffer.Write(offset + Offset.CareOfAddress, IpV4CareOfAddress.Value, Endianity.Big);
                offset += OptionDataMinimumLength + IpV4Address.SizeOf;
                return;
            }
            if (IpV6CareOfAddress.HasValue)
            {
                buffer.Write(offset + Offset.CareOfAddress, IpV6CareOfAddress.Value, Endianity.Big);
                offset += OptionDataMinimumLength + IpV6Address.SizeOf;
                return;
            }
            offset += OptionDataMinimumLength;
        }

        private IpV6MobilityOptionBindingIdentifier()
            : this(0, IpV6BindingAcknowledgementStatus.BindingUpdateAccepted, false, 0)
        {
        }

        private IpV6MobilityOptionBindingIdentifier(ushort bindingId, IpV6BindingAcknowledgementStatus status, bool simultaneousHomeAndForeignBinding,
                                                    byte priority, IpV4Address? ipV4CareOfAddress, IpV6Address? ipV6CareOfAddress)
            : base(IpV6MobilityOptionType.BindingIdentifier)
        {
            if (priority > MaxPriority)
                throw new ArgumentOutOfRangeException("priority", priority, string.Format("Must not exceed {0}", MaxPriority));
            BindingId = bindingId;
            Status = status;
            SimultaneousHomeAndForeignBinding = simultaneousHomeAndForeignBinding;
            Priority = priority;
            IpV4CareOfAddress = ipV4CareOfAddress;
            IpV6CareOfAddress = ipV6CareOfAddress;
        }

        private bool EqualsData(IpV6MobilityOptionBindingIdentifier other)
        {
            return other != null &&
                   BindingId == other.BindingId && Status == other.Status && SimultaneousHomeAndForeignBinding == other.SimultaneousHomeAndForeignBinding &&
                   Priority == other.Priority && IpV4CareOfAddress.Equals(other.IpV4CareOfAddress) && IpV6CareOfAddress.Equals(other.IpV6CareOfAddress);
        }
    }

    /// <summary>
    /// RFC 5844.
    /// <pre>
    /// +-----+------------+-----+--------------+
    /// | Bit | 0-5        | 6-7 | 8-15         |
    /// +-----+------------+-----+--------------+
    /// | 0   | Option Type      | Opt Data Len |
    /// +-----+------------+-----+--------------+
    /// | 16  | Prefix-len | Reserved           |
    /// +-----+------------+--------------------+
    /// | 32  | IPv4 home address               |
    /// |     |                                 |
    /// +-----+---------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.IpV4HomeAddressRequest)]
    public sealed class IpV6MobilityOptionIpV4HomeAddressRequest : IpV6MobilityOptionComplex
    {
        public const byte MaxPrefixLength = 0x3F;

        private static class Offset
        {
            public const int PrefixLength = 0;
            public const int HomeAddress = PrefixLength + sizeof(ushort);
        }

        public const int OptionDataLength = Offset.HomeAddress + IpV4Address.SizeOf;

        private static class Mask
        {
            public const byte PrefixLength = 0xFC;
        }

        private static class Shift
        {
            public const int PrefixLength = 2;
        }

        public IpV6MobilityOptionIpV4HomeAddressRequest(byte prefixLength, IpV4Address homeAddress)
            : base(IpV6MobilityOptionType.IpV4HomeAddressRequest)
        {
            if (prefixLength > MaxPrefixLength)
                throw new ArgumentOutOfRangeException("prefixLength", prefixLength, string.Format("Max prefix length is {0}", MaxPrefixLength));

            PrefixLength = prefixLength;
            HomeAddress = homeAddress;
        }

        /// <summary>
        /// Indicates the prefix length of the mobile node's IPv4 home network corresponding to the IPv4 home address contained in the option.
        /// </summary>
        public byte PrefixLength { get; private set; }

        /// <summary>
        /// Containing the IPv4 home address that is being requested.
        /// The value of 0.0.0.0 is used to request that the local mobility anchor perform the address allocation.
        /// </summary>
        public IpV4Address HomeAddress { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            byte prefixLength = (byte)((data[Offset.PrefixLength] & Mask.PrefixLength) >> Shift.PrefixLength);
            IpV4Address homeAddress = data.ReadIpV4Address(Offset.HomeAddress, Endianity.Big);
            return new IpV6MobilityOptionIpV4HomeAddressRequest(prefixLength, homeAddress);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionIpV4HomeAddressRequest);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.PrefixLength, (byte)(PrefixLength << Shift.PrefixLength));
            buffer.Write(offset + Offset.HomeAddress, HomeAddress, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionIpV4HomeAddressRequest()
            : this(0, IpV4Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionIpV4HomeAddressRequest other)
        {
            return other != null &&
                   PrefixLength == other.PrefixLength && HomeAddress.Equals(other.HomeAddress);
        }
    }

    /// <summary>
    /// RFC 5844.
    /// </summary>
    public enum IpV6IpV4HomeAddressReplyStatus : byte
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
    }

    /// <summary>
    /// RFC 5844.
    /// <pre>
    /// +-----+-------------+----------+-------+
    /// | Bit | 0-7         | 8-13     | 14-15 |
    /// +-----+-------------+----------+-------+
    /// | 0   | Option Type | Opt Data Len     |
    /// +-----+-------------+----------+-------+
    /// | 16  | Status      | Pref-len | Res   |
    /// +-----+-------------+----------+-------+
    /// | 32  | IPv4 home address              |
    /// |     |                                |
    /// +-----+--------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.IpV4HomeAddressReply)]
    public sealed class IpV6MobilityOptionIpV4HomeAddressReply : IpV6MobilityOptionComplex
    {
        public const byte MaxPrefixLength = 0x3F;

        private static class Offset
        {
            public const int Status = 0;
            public const int PrefixLength = Status + sizeof(byte);
            public const int HomeAddress = PrefixLength + sizeof(byte);
        }

        public const int OptionDataLength = Offset.HomeAddress + IpV4Address.SizeOf;

        private static class Mask
        {
            public const byte PrefixLength = 0xFC;
        }

        private static class Shift
        {
            public const int PrefixLength = 2;
        }

        public IpV6MobilityOptionIpV4HomeAddressReply(IpV6IpV4HomeAddressReplyStatus status, byte prefixLength, IpV4Address homeAddress)
            : base(IpV6MobilityOptionType.IpV4HomeAddressReply)
        {
            if (prefixLength > MaxPrefixLength)
                throw new ArgumentOutOfRangeException("prefixLength", prefixLength, string.Format("Max prefix length is {0}", MaxPrefixLength));

            Status = status;
            PrefixLength = prefixLength;
            HomeAddress = homeAddress;
        }

        /// <summary>
        /// Indicates success or failure for the IPv4 home address assignment.
        /// Values from 0 to 127 indicate success.
        /// Higher values (128 to 255) indicate failure.
        /// </summary>
        public IpV6IpV4HomeAddressReplyStatus Status { get; private set; }

        /// <summary>
        /// Used to carry the prefix length of the mobile node's IPv4 home network corresponding to the IPv4 home address contained in the option.
        /// </summary>
        public byte PrefixLength { get; private set; }

        /// <summary>
        /// Used to carry the IPv4 home address assigned to the mobile node.
        /// </summary>
        public IpV4Address HomeAddress { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            IpV6IpV4HomeAddressReplyStatus status = (IpV6IpV4HomeAddressReplyStatus)data[Offset.Status];
            byte prefixLength = (byte)((data[Offset.PrefixLength] & Mask.PrefixLength) >> Shift.PrefixLength);
            IpV4Address homeAddress = data.ReadIpV4Address(Offset.HomeAddress, Endianity.Big);
            return new IpV6MobilityOptionIpV4HomeAddressReply(status, prefixLength, homeAddress);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionIpV4HomeAddressReply);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Status, (byte)Status);
            buffer.Write(offset + Offset.PrefixLength, (byte)(PrefixLength << Shift.PrefixLength));
            buffer.Write(offset + Offset.HomeAddress, HomeAddress, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionIpV4HomeAddressReply()
            : this(IpV6IpV4HomeAddressReplyStatus.Success, 0, IpV4Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionIpV4HomeAddressReply other)
        {
            return other != null &&
                   Status == other.Status && PrefixLength == other.PrefixLength && HomeAddress.Equals(other.HomeAddress);
        }
    }

    /// <summary>
    /// RFC 5844.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved                   |
    /// +-----+----------------------------+
    /// | 32  | IPv4 home address          |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.IpV4DefaultRouterAddress)]
    public class IpV6MobilityOptionIpV4DefaultRouterAddress : IpV6MobilityOptionIpV4Address
    {
        public IpV6MobilityOptionIpV4DefaultRouterAddress(IpV4Address defaultRouterAddress)
            : base(IpV6MobilityOptionType.IpV4DefaultRouterAddress, defaultRouterAddress)
        {
        }

        /// <summary>
        /// The mobile node's default router address.
        /// </summary>
        public IpV4Address DefaultRouterAddress { get { return Address; } }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            IpV4Address defaultRouterAddress;
            if (!Read(data, out defaultRouterAddress))
                return null;

            return new IpV6MobilityOptionIpV4DefaultRouterAddress(defaultRouterAddress);
        }

        private IpV6MobilityOptionIpV4DefaultRouterAddress()
            : this(IpV4Address.Zero)
        {
        }
    }

    /// <summary>
    /// RFC 5844.
    /// <pre>
    /// +-----+-------------+------+-------+
    /// | Bit | 0-7         | 8-14 | 15    |
    /// +-----+-------------+------+-------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+------+-------+
    /// | 16  | Reserved           | S     |
    /// +-----+--------------------+-------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.IpV4DhcpSupportMode)]
    public sealed class IpV6MobilityOptionIpV4DhcpSupportMode : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int IsServer = sizeof(byte);
        }

        private static class Mask
        {
            public const byte IsServer = 0x01;
        }

        public const int OptionDataLength = Offset.IsServer + sizeof(byte);

        public IpV6MobilityOptionIpV4DhcpSupportMode(bool isServer)
            : base(IpV6MobilityOptionType.IpV4DhcpSupportMode)
        {
            IsServer = isServer;
        }

        /// <summary>
        /// Specifies the DHCP support mode.
        /// This flag indicates whether the mobile access gateway should function as a DHCP Server or a DHCP Relay for the attached mobile node.
        /// If false, the mobile access gateway should act as a DHCP Relay and if true, it should act as a DHCP Server.
        /// </summary>
        public bool IsServer { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            bool isServer = data.ReadBool(Offset.IsServer, Mask.IsServer);
            return new IpV6MobilityOptionIpV4DhcpSupportMode(isServer);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionIpV4DhcpSupportMode);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            if (IsServer)
                buffer.Write(offset + Offset.IsServer, Mask.IsServer);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionIpV4DhcpSupportMode() 
            : this(false)
        {
        }

        private bool EqualsData(IpV6MobilityOptionIpV4DhcpSupportMode other)
        {
            return other != null &&
                   IsServer == other.IsServer;
        }
    }

    /// <summary>
    /// RFC 5949.
    /// <pre>
    /// +-----+----------+------------+
    /// | Bit | 0-7      | 8-15       |
    /// +-----+----------+------------+
    /// | 0   | Req-type | Req-length |
    /// +-----+----------+------------+
    /// | 16  | Req-option            |
    /// | ... |                       |
    /// +-----+-----------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6MobilityOptionContextRequestEntry : IEquatable<IpV6MobilityOptionContextRequestEntry>
    {
        public IpV6MobilityOptionContextRequestEntry(byte requestType, DataSegment option)
        {
            if (option.Length > byte.MaxValue)
                throw new ArgumentOutOfRangeException("option", option, string.Format("Option length must not exceed {0}", byte.MaxValue));
            RequestType = requestType;
            Option = option;
        }

        /// <summary>
        /// The total length of the request in bytes.
        /// </summary>
        public int Length
        {
            get { return sizeof(byte) + sizeof(byte) + OptionLength; }
        }

        /// <summary>
        /// The type value for the requested option.
        /// </summary>
        public byte RequestType { get; private set; }

        /// <summary>
        /// The length of the requested option, excluding the Request Type and Request Length fields.
        /// </summary>
        public byte OptionLength
        {
            get { return (byte)Option.Length; }
        }

        /// <summary>
        /// The optional data to uniquely identify the requested context for the requested option.
        /// </summary>
        public DataSegment Option { get; private set; }

        internal void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, RequestType);
            buffer.Write(ref offset, OptionLength);
            Option.Write(buffer, ref offset);
        }

        public bool Equals(IpV6MobilityOptionContextRequestEntry other)
        {
            return (other != null && RequestType.Equals(other.RequestType) && Option.Equals(other.Option));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IpV6MobilityOptionContextRequestEntry);
        }
    }

    /// <summary>
    /// RFC 5949.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Request 1                  |
    /// | ... | Request 2                  |
    /// |     | ...                        |
    /// |     | Request n                  |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.ContextRequest)]
    public sealed class IpV6MobilityOptionContextRequest : IpV6MobilityOptionComplex
    {
        public IpV6MobilityOptionContextRequest(params IpV6MobilityOptionContextRequestEntry[] requests)
            : this(requests.AsReadOnly())
        {
        }

        public IpV6MobilityOptionContextRequest(IList<IpV6MobilityOptionContextRequestEntry> requests)
            : this(requests.AsReadOnly())
        {
        }

        public IpV6MobilityOptionContextRequest(ReadOnlyCollection<IpV6MobilityOptionContextRequestEntry> requests)
            : base(IpV6MobilityOptionType.ContextRequest)
        {
            Requests = requests;
            _dataLength = Requests.Sum(request => request.Length);
            if (_dataLength > byte.MaxValue)
                throw new ArgumentOutOfRangeException("requests", requests, string.Format("requests length is too large. Takes over {0}>{1} bytes.", _dataLength, byte.MaxValue));
        }

        /// <summary>
        /// The requests types and options.
        /// </summary>
        public ReadOnlyCollection<IpV6MobilityOptionContextRequestEntry> Requests { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            List<IpV6MobilityOptionContextRequestEntry> requests = new List<IpV6MobilityOptionContextRequestEntry>();
            int offset = 0;
            while (data.Length > offset)
            {
                byte requestType = data[offset++];

                if (offset >= data.Length)
                    return null;
                byte requestLength = data[offset++];

                if (offset + requestLength > data.Length)
                    return null;

                DataSegment requestOption = data.Subsegment(offset, requestLength);
                offset += requestLength;

                requests.Add(new IpV6MobilityOptionContextRequestEntry(requestType, requestOption));
            }

            return new IpV6MobilityOptionContextRequest(requests);
        }

        internal override int DataLength
        {
            get { return _dataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionContextRequest);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            foreach (var request in Requests)
                request.Write(buffer, ref offset);
        }

        private IpV6MobilityOptionContextRequest()
            : this(new IpV6MobilityOptionContextRequestEntry[0])
        {
        }

        private bool EqualsData(IpV6MobilityOptionContextRequest other)
        {
            return other != null &&
                   Requests.SequenceEqual(other.Requests);
        }

        private readonly int _dataLength;
    }

    /// <summary>
    /// RFC 5949.
    /// </summary>
    public enum IpV6LocalMobilityAnchorAddressCode : byte
    {
        /// <summary>
        /// IPv6 address of the local mobility anchor (LMAA).
        /// </summary>
        IpV6 = 1,

        /// <summary>
        /// IPv4 address of the local mobility anchor (IPv4-LMAA).
        /// </summary>
        IpV4 = 2,
    }

    /// <summary>
    /// RFC 5949.
    /// <pre>
    /// +-----+-------------+-----------------+
    /// | Bit | 0-7         | 8-15            |
    /// +-----+-------------+-----------------+
    /// | 0   | Option Type | Opt Data Len    |
    /// +-----+-------------+-----------------+
    /// | 16  | Option-Code | Reserved        |
    /// +-----+-------------+-----------------+
    /// | 32  | Local Mobility Anchor Address |
    /// |     |                               |
    /// | ... |                               |
    /// +-----+-------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.LocalMobilityAnchorAddress)]
    public sealed class IpV6MobilityOptionLocalMobilityAnchorAddress : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int Code = 0;
            public const int LocalMobilityAnchorAddress = Code + sizeof(byte) + sizeof(byte);
        }

        public const int OptionDataMinimumLength = Offset.LocalMobilityAnchorAddress;

        public IpV6MobilityOptionLocalMobilityAnchorAddress(IpV6LocalMobilityAnchorAddressCode code, IpV4Address localMobilityAnchorAddress)
            : this(code, localMobilityAnchorAddress, null)
        {
        }

        public IpV6MobilityOptionLocalMobilityAnchorAddress(IpV6LocalMobilityAnchorAddressCode code, IpV6Address localMobilityAnchorAddress)
            : this(code, null, localMobilityAnchorAddress)
        {
        }

        public IpV6MobilityOptionLocalMobilityAnchorAddress(IpV4Address localMobilityAnchorAddress)
            : this(IpV6LocalMobilityAnchorAddressCode.IpV4, localMobilityAnchorAddress)
        {
        }

        public IpV6MobilityOptionLocalMobilityAnchorAddress(IpV6Address localMobilityAnchorAddress)
            : this(IpV6LocalMobilityAnchorAddressCode.IpV6, localMobilityAnchorAddress)
        {
        }

        /// <summary>
        /// Determines the type of the local mobility anchor address.
        /// </summary>
        public IpV6LocalMobilityAnchorAddressCode Code { get; private set; }

        /// <summary>
        /// If the Code IPv6, the LMA IPv6 address (LMAA), otherwise null.
        /// </summary>
        public IpV6Address? LocalMobilityAnchorAddressIpV6 { get; private set; }

        /// <summary>
        /// If the Code is IPv4, the LMA IPv4 address (IPv4-LMA), otherwise null.
        /// </summary>
        public IpV4Address? LocalMobilityAnchorAddressIpV4 { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            IpV6LocalMobilityAnchorAddressCode code = (IpV6LocalMobilityAnchorAddressCode)data[Offset.Code];
            switch (code)
            {
                case IpV6LocalMobilityAnchorAddressCode.IpV6:
                {
                    if (data.Length != Offset.LocalMobilityAnchorAddress + IpV6Address.SizeOf)
                        return null;
                    IpV6Address localMobilityAnchorAddress = data.ReadIpV6Address(Offset.LocalMobilityAnchorAddress, Endianity.Big);
                    return new IpV6MobilityOptionLocalMobilityAnchorAddress(localMobilityAnchorAddress);
                }

                case IpV6LocalMobilityAnchorAddressCode.IpV4:
                {
                    if (data.Length != Offset.LocalMobilityAnchorAddress + IpV4Address.SizeOf)
                        return null;
                    IpV4Address localMobilityAnchorAddress = data.ReadIpV4Address(Offset.LocalMobilityAnchorAddress, Endianity.Big);
                    return new IpV6MobilityOptionLocalMobilityAnchorAddress(localMobilityAnchorAddress);
                }

                default:
                    return null;
            }
        }

        internal override int DataLength
        {
            get
            {
                return OptionDataMinimumLength +
                       (LocalMobilityAnchorAddressIpV4.HasValue ? IpV4Address.SizeOf : 0) +
                       (LocalMobilityAnchorAddressIpV6.HasValue ? IpV6Address.SizeOf : 0);
            }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionLocalMobilityAnchorAddress);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Code, (byte)Code);
            if (LocalMobilityAnchorAddressIpV4.HasValue)
                buffer.Write(offset + Offset.LocalMobilityAnchorAddress, LocalMobilityAnchorAddressIpV4.Value, Endianity.Big);
            else if (LocalMobilityAnchorAddressIpV6.HasValue)
                buffer.Write(offset + Offset.LocalMobilityAnchorAddress, LocalMobilityAnchorAddressIpV6.Value, Endianity.Big);
            offset += DataLength;
        }

        private IpV6MobilityOptionLocalMobilityAnchorAddress(IpV6LocalMobilityAnchorAddressCode code, IpV4Address? localMobilityAnchorAddressIpV4,
                                                             IpV6Address? localMobilityAnchorAddressIpV6)
            : base(IpV6MobilityOptionType.LocalMobilityAnchorAddress)
        {
            Code = code;
            LocalMobilityAnchorAddressIpV6 = localMobilityAnchorAddressIpV6;
            LocalMobilityAnchorAddressIpV4 = localMobilityAnchorAddressIpV4;
        }

        private IpV6MobilityOptionLocalMobilityAnchorAddress()
            : this(IpV6Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionLocalMobilityAnchorAddress other)
        {
            return other != null &&
                   Code == other.Code && LocalMobilityAnchorAddressIpV4.Equals(other.LocalMobilityAnchorAddressIpV4) &&
                   LocalMobilityAnchorAddressIpV6.Equals(other.LocalMobilityAnchorAddressIpV6);
        }
    }

    /// <summary>
    /// RFC 5949.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved                   |
    /// +-----+----------------------------+
    /// | 32  | Interface Identifier       |
    /// |     |                            |
    /// |     |                            |
    /// |     |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.MobileNodeLinkLocalAddressInterfaceIdentifier)]
    public sealed class IpV6MobilityOptionMobileNodeLinkLocalAddressInterfaceIdentifier : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int InterfaceIdentifier = sizeof(ushort);
        }

        public const int OptionDataLength = Offset.InterfaceIdentifier + sizeof(ulong);

        public IpV6MobilityOptionMobileNodeLinkLocalAddressInterfaceIdentifier(ulong interfaceIdentifier)
            : base(IpV6MobilityOptionType.MobileNodeLinkLocalAddressInterfaceIdentifier)
        {
            InterfaceIdentifier = interfaceIdentifier;
        }

        /// <summary>
        /// The Interface Identifier value used for the mobile node's IPv6 Link-local address in the P-AN.
        /// </summary>
        public ulong InterfaceIdentifier { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            ulong interfaceIdentifier = data.ReadULong(Offset.InterfaceIdentifier, Endianity.Big);
            return new IpV6MobilityOptionMobileNodeLinkLocalAddressInterfaceIdentifier(interfaceIdentifier);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionMobileNodeLinkLocalAddressInterfaceIdentifier);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.InterfaceIdentifier, InterfaceIdentifier, Endianity.Big);
            offset += DataLength;
        }

        private IpV6MobilityOptionMobileNodeLinkLocalAddressInterfaceIdentifier()
            : this(0)
        {
        }

        private bool EqualsData(IpV6MobilityOptionMobileNodeLinkLocalAddressInterfaceIdentifier other)
        {
            return other != null &&
                   InterfaceIdentifier == other.InterfaceIdentifier;
        }
    }

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

    /// <summary>
    /// RFC 6089.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | FID ...                    |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.FlowSummary)]
    public sealed class IpV6MobilityOptionFlowSummary : IpV6MobilityOptionComplex
    {
        public const int OptionDataMinimumLength = sizeof(ushort);

        public IpV6MobilityOptionFlowSummary(params ushort[] flowIdentifiers)
            : this(flowIdentifiers.AsReadOnly())
        {
        }

        public IpV6MobilityOptionFlowSummary(IEnumerable<ushort> flowIdentifiers)
            : this(flowIdentifiers.ToList())
        {
        }

        public IpV6MobilityOptionFlowSummary(IList<ushort> flowIdentifiers)
            : this(flowIdentifiers.AsReadOnly())
        {
        }

        public IpV6MobilityOptionFlowSummary(ReadOnlyCollection<ushort> flowIdentifiers)
            : base(IpV6MobilityOptionType.FlowSummary)
        {
            if (!flowIdentifiers.Any())
                throw new ArgumentOutOfRangeException("flowIdentifiers", flowIdentifiers, "Must not be empty.");
            FlowIdentifiers = flowIdentifiers;
        }

        /// <summary>
        /// Indicating a registered FID.
        /// One or more FID fields can be included in this option.
        /// </summary>
        public ReadOnlyCollection<ushort> FlowIdentifiers { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength || data.Length % sizeof(ushort) != 0)
                return null;

            ushort[] flowIdentifiers = new ushort[data.Length / sizeof(ushort)];
            for (int i = 0; i != flowIdentifiers.Length; ++i)
                flowIdentifiers[i] = data.ReadUShort(i * sizeof(ushort), Endianity.Big);
            return new IpV6MobilityOptionFlowSummary(flowIdentifiers);
        }

        internal override int DataLength
        {
            get { return FlowIdentifiers.Count * sizeof(ushort); }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionFlowSummary);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            foreach (ushort flowIdentifier in FlowIdentifiers)
                buffer.Write(ref offset, flowIdentifier, Endianity.Big);
        }

        private IpV6MobilityOptionFlowSummary()
            : this(new ushort[1])
        {
        }

        private bool EqualsData(IpV6MobilityOptionFlowSummary other)
        {
            return other != null &&
                   FlowIdentifiers.SequenceEqual(other.FlowIdentifiers);
        }
    }

    /// <summary>
    /// RFC 6089.
    /// </summary>
    public enum IpV6FlowIdentificationStatus : byte
    {
        /// <summary>
        /// Flow binding successful.
        /// </summary>
        FlowBindingSuccessful = 0,

        /// <summary>
        /// Administratively prohibited
        /// </summary>
        AdministrativelyProhibited = 128,

        /// <summary>
        /// Flow binding rejected, reason unspecified
        /// </summary>
        FlowBindingRejectedReasonUnspecified = 129,

        /// <summary>
        /// Flow identification mobility option malformed.
        /// </summary>
        FlowIdentificationMobilityOptionMalformed = 130,

        /// <summary>
        /// BID not found.
        /// </summary>
        BindingIdNotFound = 131,

        /// <summary>
        /// FID not found.
        /// </summary>
        FlowIdentifierNotFound = 132,

        /// <summary>
        /// Traffic selector format not supported.
        /// </summary>
        TrafficSelectorFormatNotSupported = 133,
    }

    /// <summary>
    /// RFC 6089.
    /// </summary>
    public abstract class IpV6FlowIdentificationSubOption : Option, IEquatable<IpV6FlowIdentificationSubOption>
    {
        /// <summary>
        /// The type of the option.
        /// </summary>
        public IpV6FlowIdentificationSubOptionType OptionType { get; private set; }

        public override int Length
        {
            get { return sizeof(byte); }
        }

        internal abstract IpV6FlowIdentificationSubOption CreateInstance(DataSegment data);

        protected IpV6FlowIdentificationSubOption(IpV6FlowIdentificationSubOptionType type)
        {
            OptionType = type;
        }

        public override sealed bool Equals(Option other)
        {
            return Equals(other as IpV6FlowIdentificationSubOption);
        }

        public bool Equals(IpV6FlowIdentificationSubOption other)
        {
            return other != null &&
                   OptionType == other.OptionType && Length == other.Length && EqualsData(other);
        }

        internal abstract bool EqualsData(IpV6FlowIdentificationSubOption other);

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)OptionType;
        }
    }

    /// <summary>
    /// RFC 6089.
    /// <pre>
    /// +-----+--------------+
    /// | Bit | 0-7          |
    /// +-----+--------------+
    /// | 0   | Sub-Opt Type |
    /// +-----+--------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6FlowIdentificationSubOptionSimple : IpV6FlowIdentificationSubOption
    {
        protected IpV6FlowIdentificationSubOptionSimple(IpV6FlowIdentificationSubOptionType type)
            : base(type)
        {
        }

        public override sealed int Length
        {
            get { return base.Length; }
        }

        internal override sealed bool EqualsData(IpV6FlowIdentificationSubOption other)
        {
            return true;
        }

        internal override sealed void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
        }

        internal override sealed IpV6FlowIdentificationSubOption CreateInstance(DataSegment data)
        {
            throw new InvalidOperationException("Simple options shouldn't be registered.");
        }
    }

    /// <summary>
    /// RFC 6089.
    /// <pre>
    /// +-----+--------------+-------------+
    /// | Bit | 0-7          | 8-15        |
    /// +-----+--------------+-------------+
    /// | 0   | Sub-Opt Type | Sub-Opt Len |
    /// +-----+--------------+-------------+
    /// | 16  | Option Data                |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6FlowIdentificationSubOptionComplex : IpV6FlowIdentificationSubOption
    {
        protected IpV6FlowIdentificationSubOptionComplex(IpV6FlowIdentificationSubOptionType type)
            : base(type)
        {
        }

        public override sealed int Length
        {
            get { return base.Length + sizeof(byte) + DataLength; }
        }

        internal abstract int DataLength { get; }

        internal override sealed void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer[offset++] = (byte)DataLength;
            WriteData(buffer, ref offset);
        }

        internal abstract void WriteData(byte[] buffer, ref int offset);
    }


    /// <summary>
    /// RFC 6089.
    /// </summary>
    public enum IpV6FlowIdentificationSubOptionType : byte
    {
        /// <summary>
        /// RFC 6089.
        /// </summary>
        Pad1 = 0,

        /// <summary>
        /// RFC 6089.
        /// </summary>
        PadN = 1,

        /// <summary>
        /// RFC 6089.
        /// </summary>
        BindingReference = 2,

        /// <summary>
        /// RFC 6089.
        /// </summary>
        TrafficSelector = 3,
    }

    /// <summary>
    /// RFC 6089.
    /// </summary>
    public sealed class IpV6FlowIdentificationSubOptions : V6Options<IpV6FlowIdentificationSubOption>
    {
        /// <summary>
        /// Creates options from a list of options.
        /// </summary>
        /// <param name="options">The list of options.</param>
        public IpV6FlowIdentificationSubOptions(IList<IpV6FlowIdentificationSubOption> options)
            : base(options, true)
        {
        }

        /// <summary>
        /// Creates options from a list of options.
        /// </summary>
        /// <param name="options">The list of options.</param>
        public IpV6FlowIdentificationSubOptions(params IpV6FlowIdentificationSubOption[] options)
            : this((IList<IpV6FlowIdentificationSubOption>)options)
        {
        }

        internal IpV6FlowIdentificationSubOptions(DataSegment data)
            : this(Read(data))
        {
        }

        private IpV6FlowIdentificationSubOptions(Tuple<IList<IpV6FlowIdentificationSubOption>, bool> optionsAndIsValid)
            : base(optionsAndIsValid.Item1, optionsAndIsValid.Item2)
        {
        }

        /// <summary>
        /// No options instance.
        /// </summary>
        public static IpV6FlowIdentificationSubOptions None
        {
            get { return _none; }
        }

        internal static Tuple<IList<IpV6FlowIdentificationSubOption>, bool> Read(DataSegment data)
        {
            int offset = 0;
            List<IpV6FlowIdentificationSubOption> options = new List<IpV6FlowIdentificationSubOption>();
            bool isValid = true;
            while (offset < data.Length)
            {
                IpV6FlowIdentificationSubOptionType optionType = (IpV6FlowIdentificationSubOptionType)data[offset++];
                if (optionType == IpV6FlowIdentificationSubOptionType.Pad1)
                {
                    options.Add(new IpV6FlowIdentificationSubOptionPad1());
                    continue;
                }

                if (offset >= data.Length)
                {
                    isValid = false;
                    break;
                }

                byte optionDataLength = data[offset++];
                if (offset + optionDataLength > data.Length)
                {
                    isValid = false;
                    break;
                }

                IpV6FlowIdentificationSubOption option = CreateOption(optionType, data.Subsegment(ref offset, optionDataLength));
                if (option == null)
                {
                    isValid = false;
                    break;
                }

                options.Add(option);
            }

            return new Tuple<IList<IpV6FlowIdentificationSubOption>, bool>(options, isValid);
        }

        private static IpV6FlowIdentificationSubOption CreateOption(IpV6FlowIdentificationSubOptionType optionType, DataSegment data)
        {
            IpV6FlowIdentificationSubOption prototype;
            if (!_prototypes.TryGetValue(optionType, out prototype))
                return new IpV6FlowIdentificationSubOptionUnknown(optionType, data);
            return prototype.CreateInstance(data);
        }

        private static readonly Dictionary<IpV6FlowIdentificationSubOptionType, IpV6FlowIdentificationSubOption> _prototypes = InitializePrototypes();

        private static Dictionary<IpV6FlowIdentificationSubOptionType, IpV6FlowIdentificationSubOption> InitializePrototypes()
        {
            var prototypes =
                from type in Assembly.GetExecutingAssembly().GetTypes()
                where typeof (IpV6FlowIdentificationSubOption).IsAssignableFrom(type) &&
                      GetRegistrationAttribute(type) != null
                select new
                           {
                               GetRegistrationAttribute(type).OptionType,
                               Option = (IpV6FlowIdentificationSubOption)Activator.CreateInstance(type, true)
                           };

            return prototypes.ToDictionary(option => option.OptionType, option => option.Option);
        }

        private static IpV6FlowIdentificationSubOptionTypeRegistrationAttribute GetRegistrationAttribute(Type type)
        {
            var registraionAttributes = type.GetCustomAttributes<IpV6FlowIdentificationSubOptionTypeRegistrationAttribute>(false);
            if (!registraionAttributes.Any())
                return null;

            return registraionAttributes.First();
        }

        private static readonly IpV6FlowIdentificationSubOptions _none = new IpV6FlowIdentificationSubOptions();
    }

    internal sealed class IpV6FlowIdentificationSubOptionTypeRegistrationAttribute : Attribute
    {
        public IpV6FlowIdentificationSubOptionTypeRegistrationAttribute(IpV6FlowIdentificationSubOptionType optionType)
        {
            OptionType = optionType;
        }

        public IpV6FlowIdentificationSubOptionType OptionType { get; private set; }
    }

    public sealed class IpV6FlowIdentificationSubOptionPad1 : IpV6FlowIdentificationSubOptionSimple
    {
        public const int OptionLength = sizeof(byte);

        public IpV6FlowIdentificationSubOptionPad1()
            : base(IpV6FlowIdentificationSubOptionType.Pad1)
        {
        }
    }

    /// <summary>
    /// RFC 6089.
    /// <pre>
    /// +-----+--------------+
    /// | Bit | 0-7          |
    /// +-----+--------------+
    /// | 0   | Sub-Opt Type |
    /// +-----+--------------+
    /// | 8   | N            |
    /// +-----+--------------+
    /// | 16  | 0            |
    /// | ... |              |
    /// +-----+--------------+
    /// </pre>
    /// </summary>
    [IpV6FlowIdentificationSubOptionTypeRegistration(IpV6FlowIdentificationSubOptionType.PadN)]
    public sealed class IpV6FlowIdentificationSubOptionPadN : IpV6FlowIdentificationSubOptionComplex
    {
        public IpV6FlowIdentificationSubOptionPadN(int paddingDataLength)
            : base(IpV6FlowIdentificationSubOptionType.PadN)
        {
            PaddingDataLength = paddingDataLength;
        }

        public int PaddingDataLength { get; private set; }

        internal override IpV6FlowIdentificationSubOption CreateInstance(DataSegment data)
        {
            return new IpV6FlowIdentificationSubOptionPadN(data.Length);
        }

        internal override int DataLength
        {
            get { return PaddingDataLength; }
        }

        internal override bool EqualsData(IpV6FlowIdentificationSubOption other)
        {
            return true;
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            offset += PaddingDataLength;
        }

        private IpV6FlowIdentificationSubOptionPadN()
            : this(0)
        {
        }
    }
    
    /// <summary>
    /// RFC 6089.
    /// <pre>
    /// +-----+--------------+-------------+
    /// | Bit | 0-7          | 8-15        |
    /// +-----+--------------+-------------+
    /// | 0   | Sub-Opt Type | Sub-Opt Len |
    /// +-----+--------------+-------------+
    /// | 16  | Option Data                |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6FlowIdentificationSubOptionUnknown : IpV6FlowIdentificationSubOptionComplex
    {
        public IpV6FlowIdentificationSubOptionUnknown(IpV6FlowIdentificationSubOptionType type, DataSegment data)
            : base(type)
        {
            Data = data;
        }

        public DataSegment Data { get; private set; }

        internal override IpV6FlowIdentificationSubOption CreateInstance(DataSegment data)
        {
            throw new InvalidOperationException("IpV6FlowIdentificationSubOptionUnknown shouldn't be registered.");
        }

        internal override int DataLength
        {
            get { return Data.Length; }
        }

        internal override bool EqualsData(IpV6FlowIdentificationSubOption other)
        {
            return EqualsData(other as IpV6FlowIdentificationSubOptionUnknown);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Data);
        }

        private bool EqualsData(IpV6FlowIdentificationSubOptionUnknown other)
        {
            return other != null &&
                   Data.Equals(other.Data);
        }
    }
    
    /// <summary>
    /// RFC 6089.
    /// <pre>
    /// +-----+--------------+-------------+
    /// | Bit | 0-7          | 8-15        |
    /// +-----+--------------+-------------+
    /// | 0   | Sub-Opt Type | Sub-Opt Len | 
    /// +-----+--------------+-------------+
    /// | 16  | BIDs                       |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6FlowIdentificationSubOptionTypeRegistration(IpV6FlowIdentificationSubOptionType.BindingReference)]
    public sealed class IpV6FlowIdentificationSubOptionBindingReference : IpV6FlowIdentificationSubOptionComplex
    {
        public IpV6FlowIdentificationSubOptionBindingReference(IList<ushort> bindingIds)
            : this(bindingIds.AsReadOnly())
        {
        }

        public IpV6FlowIdentificationSubOptionBindingReference(params ushort[] bindingIds)
            : this(bindingIds.AsReadOnly())
        {
        }

        public IpV6FlowIdentificationSubOptionBindingReference(IEnumerable<ushort> bindingIds)
            : this(bindingIds.ToList())
        {
        }

        public IpV6FlowIdentificationSubOptionBindingReference(ReadOnlyCollection<ushort> bindingIds)
            : base(IpV6FlowIdentificationSubOptionType.BindingReference)
        {
            BindingIds = bindingIds;
        }

        /// <summary>
        /// Indicates the BIDs that the mobile node wants to associate with the flow identification option.
        /// One or more BID fields can be included in this sub-option.
        /// </summary>
        public ReadOnlyCollection<ushort> BindingIds { get; private set; }

        internal override IpV6FlowIdentificationSubOption CreateInstance(DataSegment data)
        {
            if (data.Length % sizeof(ushort) != 0)
                return null;

            ushort[] bindingIds = new ushort[data.Length / sizeof(ushort)];
            for (int i = 0; i != bindingIds.Length; ++i)
                bindingIds[i] = data.ReadUShort(i * sizeof(ushort), Endianity.Big);
            return new IpV6FlowIdentificationSubOptionBindingReference(bindingIds);
        }

        internal override int DataLength
        {
            get { return BindingIds.Count * sizeof(ushort); }
        }

        internal override bool EqualsData(IpV6FlowIdentificationSubOption other)
        {
            return EqualsData(other as IpV6FlowIdentificationSubOptionBindingReference);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            foreach (ushort bindingId in BindingIds)
                buffer.Write(ref offset, bindingId, Endianity.Big);
        }

        private IpV6FlowIdentificationSubOptionBindingReference()
            : this(new ushort[0])
        {
        }

        private bool EqualsData(IpV6FlowIdentificationSubOptionBindingReference other)
        {
            return other != null &&
                   BindingIds.SequenceEqual(other.BindingIds);
        }
    }

    /// <summary>
    /// RFCs 6088, 6089.
    /// </summary>
    public enum IpV6FlowIdentificationTrafficSelectorFormat : byte
    {
        /// <summary>
        /// IPv4 binary traffic selector.
        /// </summary>
        IpV4Binary = 1,

        /// <summary>
        /// IPv6 binary traffic selector.
        /// </summary>
        IpV6Binary = 2,
    }

    /// <summary>
    /// RFC 6089.
    /// <pre>
    /// +-----+--------------+-------------+
    /// | Bit | 0-7          | 8-15        |
    /// +-----+--------------+-------------+
    /// | 0   | Sub-Opt Type | Sub-Opt Len | 
    /// +-----+--------------+-------------+
    /// | 16  | TS Format    | Reserved    |
    /// +-----+--------------+-------------+
    /// | 32  | Traffic Selector           |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6FlowIdentificationSubOptionTypeRegistration(IpV6FlowIdentificationSubOptionType.TrafficSelector)]
    public sealed class IpV6FlowIdentificationSubOptionTrafficSelector : IpV6FlowIdentificationSubOptionComplex
    {
        private static class Offset
        {
            public const int TrafficSelectorFormat = 0;
            public const int TrafficSelector = TrafficSelectorFormat + sizeof(byte) + sizeof(byte);
        }

        public const int OptionDataMinimumLength = Offset.TrafficSelector;

        public IpV6FlowIdentificationSubOptionTrafficSelector(IpV6FlowIdentificationTrafficSelectorFormat trafficSelectorFormat, DataSegment trafficSelector)
            : base(IpV6FlowIdentificationSubOptionType.TrafficSelector)
        {
            TrafficSelectorFormat = trafficSelectorFormat;
            TrafficSelector = trafficSelector;
        }

        /// <summary>
        /// Indicates the Traffic Selector Format.
        /// </summary>
        public IpV6FlowIdentificationTrafficSelectorFormat TrafficSelectorFormat { get; private set; }

        /// <summary>
        /// The traffic selector formatted according to TrafficSelectorFormat.
        /// </summary>
        public DataSegment TrafficSelector { get; private set; }

        internal override IpV6FlowIdentificationSubOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            IpV6FlowIdentificationTrafficSelectorFormat trafficSelectorFormat = (IpV6FlowIdentificationTrafficSelectorFormat)data[Offset.TrafficSelectorFormat];
            DataSegment trafficSelector = data.Subsegment(Offset.TrafficSelector, data.Length - Offset.TrafficSelector);
            return new IpV6FlowIdentificationSubOptionTrafficSelector(trafficSelectorFormat, trafficSelector);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + TrafficSelector.Length; }
        }

        internal override bool EqualsData(IpV6FlowIdentificationSubOption other)
        {
            return EqualsData(other as IpV6FlowIdentificationSubOptionTrafficSelector);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.TrafficSelectorFormat, (byte)TrafficSelectorFormat);
            buffer.Write(offset + Offset.TrafficSelector, TrafficSelector);
            offset += DataLength;
        }

        private IpV6FlowIdentificationSubOptionTrafficSelector()
            : this(IpV6FlowIdentificationTrafficSelectorFormat.IpV4Binary, DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6FlowIdentificationSubOptionTrafficSelector other)
        {
            return other != null &&
                   TrafficSelectorFormat == other.TrafficSelectorFormat && TrafficSelector.Equals(other.TrafficSelector);
        }
    }

    /// <summary>
    /// RFC 6089.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | FID                        |
    /// +-----+----------------------------+
    /// | 32  | FID-PRI                    |
    /// +-----+-------------+--------------+
    /// | 48  | Reserved    | Status       |
    /// +-----+-------------+--------------+
    /// | 64  | Sub-options (optional)     |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.FlowIdentification)]
    public sealed class IpV6MobilityOptionFlowIdentification : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int FlowIdentifier = 0;
            public const int Priority = FlowIdentifier + sizeof(ushort);
            public const int Status = Priority + sizeof(ushort) + sizeof(byte);
            public const int SubOptions = Status + sizeof(byte);
        }

        public const int OptionDataMinimumLength = Offset.SubOptions;

        public IpV6MobilityOptionFlowIdentification(ushort flowIdentifier, ushort priority, IpV6FlowIdentificationStatus status,
                                                    IpV6FlowIdentificationSubOptions subOptions)
            : base(IpV6MobilityOptionType.FlowIdentification)
        {
            if (Offset.SubOptions + subOptions.BytesLength > byte.MaxValue)
            {
                throw new ArgumentOutOfRangeException("subOptions", subOptions,
                                                      string.Format("Sub Options take {0} bytes, which is more than the maximum length of {1} bytes",
                                                                    subOptions.BytesLength, (byte.MaxValue - Offset.SubOptions)));
            }
            FlowIdentifier = flowIdentifier;
            Priority = priority;
            Status = status;
            SubOptions = subOptions;
        }

        /// <summary>
        /// Includes the unique identifier for the flow binding.
        /// This field is used to refer to an existing flow binding or to create a new flow binding.
        /// The value of this field is set by the mobile node.
        /// FID = 0 is reserved and must not be used.
        /// </summary>
        public ushort FlowIdentifier { get; private set; }

        /// <summary>
        /// Indicates the priority of a particular option.
        /// This field is needed in cases where two different flow descriptions in two different options overlap.
        /// The priority field decides which policy should be executed in those cases.
        /// A lower number in this field indicates a higher priority.
        /// Value '0' is reserved and must not be used.
        /// Must be unique to each of the flows pertaining to a given MN.
        /// In other words, two FIDs must not be associated with the same priority value.
        /// </summary>
        public ushort Priority { get; private set; }

        /// <summary>
        /// indicates the success or failure of the flow binding operation for the particular flow in the option.
        /// This field is not relevant to the binding update message as a whole or to other flow identification options.
        /// This field is only relevant when included in the Binding Acknowledgement message and must be ignored in the binding update message.
        /// </summary>
        public IpV6FlowIdentificationStatus Status { get; private set; }

        /// <summary>
        /// Zero or more sub-options.
        /// </summary>
        public IpV6FlowIdentificationSubOptions SubOptions { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            ushort flowIdentifier = data.ReadUShort(Offset.FlowIdentifier, Endianity.Big);
            ushort priority = data.ReadUShort(Offset.Priority, Endianity.Big);
            IpV6FlowIdentificationStatus status = (IpV6FlowIdentificationStatus)data[Offset.Status];
            IpV6FlowIdentificationSubOptions subOptions =
                new IpV6FlowIdentificationSubOptions(data.Subsegment(Offset.SubOptions, data.Length - Offset.SubOptions));

            return new IpV6MobilityOptionFlowIdentification(flowIdentifier, priority, status, subOptions);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + SubOptions.BytesLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionFlowIdentification);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.FlowIdentifier, FlowIdentifier, Endianity.Big);
            buffer.Write(offset + Offset.Priority, Priority, Endianity.Big);
            buffer.Write(offset + Offset.Status, (byte)Status);
            SubOptions.Write(buffer, offset + Offset.SubOptions);
            offset += DataLength;
        }

        private IpV6MobilityOptionFlowIdentification()
            : this(0, 0, IpV6FlowIdentificationStatus.FlowBindingSuccessful, IpV6FlowIdentificationSubOptions.None)
        {
        }

        private bool EqualsData(IpV6MobilityOptionFlowIdentification other)
        {
            return other != null &&
                   FlowIdentifier == other.FlowIdentifier && Priority == other.Priority && Status == other.Status && SubOptions.Equals(other.SubOptions);
        }
    }

    /// <summary>
    /// RFC 6463.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Reserved                   |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.RedirectCapability)]
    public sealed class IpV6MobilityOptionRedirectCapability : IpV6MobilityOptionComplex
    {
        public const int OptionDataLength = sizeof(ushort);

        public IpV6MobilityOptionRedirectCapability()
            : base(IpV6MobilityOptionType.RedirectCapability)
        {
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6MobilityOptionRedirectCapability();
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return true;
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            offset += DataLength;
        }
    }

    /// <summary>
    /// RFC 6463.
    /// <pre>
    /// +-----+---+---+-----+--------------+
    /// | Bit | 0 | 1 | 2-7 | 8-15         |
    /// +-----+---+---+-----+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+---+---+-----+--------------+
    /// | 16  | K | N | Reserved           |
    /// +-----+---+---+--------------------+
    /// | 32  | r2LMA Address              |
    /// |     |                            |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.Redirect)]
    public sealed class IpV6MobilityOptionRedirect : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int IsIpV6 = 0;
            public const int IsIpV4 = IsIpV6;
            public const int LocalMobilityAddress = IsIpV4 + sizeof(byte) + sizeof(byte);
        }

        private static class Mask
        {
            public const byte IsIpV6 = 0x80;
            public const byte IsIpV4 = 0x40;
        }

        public const int OptionDataMinimumLength = Offset.LocalMobilityAddress;

        public IpV6MobilityOptionRedirect(IpV4Address localMobilityAddress)
            : this(localMobilityAddress, null)
        {
        }

        public IpV6MobilityOptionRedirect(IpV6Address localMobilityAddress)
            : this(null, localMobilityAddress)
        {
        }

        /// <summary>
        /// The IPv4 address of the r2LMA.
        /// This value is present when the corresponding PBU was sourced from an IPv4 address.
        /// </summary>
        public IpV4Address? LocalMobilityAddressIpV4 { get; private set; }

        /// <summary>
        /// The unicast IPv6 address of the r2LMA.
        /// This value is present when the corresponding PBU was sourced from an IPv6 address.
        /// </summary>
        public IpV6Address? LocalMobilityAddressIpV6 { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            bool isIpV4 = data.ReadBool(Offset.IsIpV4, Mask.IsIpV4);
            bool isIpV6 = data.ReadBool(Offset.IsIpV6, Mask.IsIpV6);

            if (isIpV4 && !isIpV6)
            {
                if (data.Length != OptionDataMinimumLength + IpV4Address.SizeOf)
                    return null;
                IpV4Address localMobilityAddress = data.ReadIpV4Address(Offset.LocalMobilityAddress, Endianity.Big);
                return new IpV6MobilityOptionRedirect(localMobilityAddress);
            }
            if (isIpV6 && !isIpV4)
            {
                if (data.Length != OptionDataMinimumLength + IpV6Address.SizeOf)
                    return null;
                IpV6Address localMobilityAddress = data.ReadIpV6Address(Offset.LocalMobilityAddress, Endianity.Big);
                return new IpV6MobilityOptionRedirect(localMobilityAddress);
            }

            return null;
        }

        internal override int DataLength
        {
            get
            {
                return OptionDataMinimumLength +
                       (LocalMobilityAddressIpV4.HasValue ? IpV4Address.SizeOf : 0) +
                       (LocalMobilityAddressIpV6.HasValue ? IpV6Address.SizeOf : 0);
            }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionRedirect);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            if (LocalMobilityAddressIpV4.HasValue)
            {
                buffer.Write(offset + Offset.IsIpV4, Mask.IsIpV4);
                buffer.Write(offset + Offset.LocalMobilityAddress, LocalMobilityAddressIpV4.Value, Endianity.Big);
                offset += OptionDataMinimumLength + IpV4Address.SizeOf;
                return;
            }
            buffer.Write(offset + Offset.IsIpV6, Mask.IsIpV6);
            buffer.Write(offset + Offset.LocalMobilityAddress, LocalMobilityAddressIpV6.Value, Endianity.Big);
            offset += OptionDataMinimumLength + IpV6Address.SizeOf;
        }

        private IpV6MobilityOptionRedirect(IpV4Address? localMobilityAddressIpV4, IpV6Address? localMobilityAddressIpV6)
            : base(IpV6MobilityOptionType.Redirect)
        {
            LocalMobilityAddressIpV4 = localMobilityAddressIpV4;
            LocalMobilityAddressIpV6 = localMobilityAddressIpV6;
        }

        private IpV6MobilityOptionRedirect()
            : this(IpV6Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionRedirect other)
        {
            return other != null &&
                   LocalMobilityAddressIpV4.Equals(other.LocalMobilityAddressIpV4) && LocalMobilityAddressIpV6.Equals(other.LocalMobilityAddressIpV6);
        }
    }

    /// <summary>
    /// RFC 6463.
    /// <pre>
    /// +-----+-------------+--------------+----------+
    /// | Bit | 0-7         | 8-15         | 16-31    |
    /// +-----+-------------+--------------+----------+
    /// | 0   | Option Type | Opt Data Len | Priority |
    /// +-----+-------------+--------------+----------+
    /// | 32  | Sessions in Use                       |
    /// +-----+---------------------------------------+
    /// | 64  | Maximum Sessions                      |
    /// +-----+---------------------------------------+
    /// | 96  | Used Capacity                         |
    /// +-----+---------------------------------------+
    /// | 128 | Maximum Capacity                      |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.LoadInformation)]
    public sealed class IpV6MobilityOptionLoadInformation : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int Priority = 0;
            public const int SessionsInUse = Priority + sizeof(ushort);
            public const int MaximumSessions = SessionsInUse + sizeof(uint);
            public const int UsedCapacity = MaximumSessions + sizeof(uint);
            public const int MaximumCapacity = UsedCapacity + sizeof(uint);
        }

        public const int OptionDataLength = Offset.MaximumCapacity + sizeof(uint);

        public IpV6MobilityOptionLoadInformation(ushort priority, uint sessionsInUse, uint maximumSessions, uint usedCapacity, uint maximumCapacity)
            : base(IpV6MobilityOptionType.LoadInformation)
        {
            Priority = priority;
            SessionsInUse = sessionsInUse;
            MaximumSessions = maximumSessions;
            UsedCapacity = usedCapacity;
            MaximumCapacity = maximumCapacity;
        }

        /// <summary>
        /// Represents the priority of an LMA.
        /// The lower value, the higher the priority.
        /// The priority only has meaning among a group of LMAs under the same administration, for example, determined by a common LMA FQDN, a domain name,
        /// or a realm.
        /// </summary>
        public ushort Priority { get; private set; }

        /// <summary>
        /// Represents the number of parallel mobility sessions the LMA has in use.
        /// </summary>
        public uint SessionsInUse { get; private set; }

        /// <summary>
        /// Represents the maximum number of parallel mobility sessions the LMA is willing to accept.
        /// </summary>
        public uint MaximumSessions { get; private set; }

        /// <summary>
        /// Represents the used bandwidth/throughput capacity of the LMA in kilobytes per second.
        /// </summary>
        public uint UsedCapacity { get; private set; }

        /// <summary>
        /// Represents the maximum bandwidth/throughput capacity in kilobytes per second the LMA is willing to accept.
        /// </summary>
        public uint MaximumCapacity { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            ushort priority = data.ReadUShort(Offset.Priority, Endianity.Big);
            uint sessionsInUse = data.ReadUInt(Offset.SessionsInUse, Endianity.Big);
            uint maximumSessions = data.ReadUInt(Offset.MaximumSessions, Endianity.Big);
            uint usedCapacity = data.ReadUInt(Offset.UsedCapacity, Endianity.Big);
            uint maximumCapacity = data.ReadUInt(Offset.MaximumCapacity, Endianity.Big);

            return new IpV6MobilityOptionLoadInformation(priority, sessionsInUse, maximumSessions, usedCapacity, maximumCapacity);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionLoadInformation);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.Priority, Priority, Endianity.Big);
            buffer.Write(offset + Offset.SessionsInUse, SessionsInUse, Endianity.Big);
            buffer.Write(offset + Offset.MaximumSessions, MaximumSessions, Endianity.Big);
            buffer.Write(offset + Offset.UsedCapacity, UsedCapacity, Endianity.Big);
            buffer.Write(offset + Offset.MaximumCapacity, MaximumCapacity, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionLoadInformation()
            : this(0, 0, 0, 0, 0)
        {
        }

        private bool EqualsData(IpV6MobilityOptionLoadInformation other)
        {
            return other != null &&
                   Priority == other.Priority && SessionsInUse == other.SessionsInUse && MaximumSessions == other.MaximumSessions &&
                   UsedCapacity == other.UsedCapacity && MaximumCapacity == other.MaximumCapacity;
        }
    }

    /// <summary>
    /// RFC 6463.
    /// <pre>
    /// +-----+-------------+------------------+
    /// | Bit | 0-7         | 8-15             |
    /// +-----+-------------+------------------+
    /// | 0   | Option Type | Opt Data Len     |
    /// +-----+-------------+------------------+
    /// | 16  | Alternate IPv4 Care-of Address |
    /// |     |                                |
    /// +-----+--------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.AlternateIpV4CareOfAddress)]
    public sealed class IpV6MobilityOptionAlternateIpV4CareOfAddress : IpV6MobilityOptionComplex
    {
        public const int OptionDataLength = IpV4Address.SizeOf;

        public IpV6MobilityOptionAlternateIpV4CareOfAddress(IpV4Address alternateCareOfAddress)
            : base(IpV6MobilityOptionType.AlternateIpV4CareOfAddress)
        {
            AlternateCareOfAddress = alternateCareOfAddress;
        }

        /// <summary>
        /// An IPv4 equivalent of the RFC 6275 Alternate Care-of Address option for IPv6.
        /// In the context of PMIPv6, its semantic is equivalent to the Alternate Care-of Address option for IPv6.
        /// </summary>
        public IpV4Address AlternateCareOfAddress { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            return new IpV6MobilityOptionAlternateIpV4CareOfAddress(data.ReadIpV4Address(0, Endianity.Big));
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionAlternateIpV4CareOfAddress);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, AlternateCareOfAddress, Endianity.Big);
        }

        private IpV6MobilityOptionAlternateIpV4CareOfAddress()
            : this(IpV4Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionAlternateIpV4CareOfAddress other)
        {
            return other != null &&
                   AlternateCareOfAddress.Equals(other.AlternateCareOfAddress);
        }
    }

    /// <summary>
    /// RFC 6602.
    /// </summary>
    public enum IpV6MobileNodeGroupIdentifierSubType : byte
    {
        /// <summary>
        /// RFC 6602.
        /// </summary>
        BulkBindingUpdateGroup = 1,
    }

    /// <summary>
    /// RFC 6602.
    /// <pre>
    /// +-----+-------------+----------------+
    /// | Bit | 0-7         | 8-15           |
    /// +-----+-------------+----------------+
    /// | 0   | Option Type | Opt Data Len   |
    /// +-----+-------------+----------------+
    /// | 16  | Sub-type    | Reserved       |
    /// +-----+-------------+----------------+
    /// | 32  | Mobile Node Group Identifier |
    /// |     |                              |
    /// +-----+------------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.MobileNodeGroupIdentifier)]
    public sealed class IpV6MobilityOptionMobileNodeGroupIdentifier : IpV6MobilityOptionComplex
    {
        private static class Offset
        {
            public const int SubType = 0;
            public const int MobileNodeGroupIdentifier = SubType + sizeof(byte) + sizeof(byte);
        }

        public const int OptionDataLength = Offset.MobileNodeGroupIdentifier + sizeof(uint);

        public IpV6MobilityOptionMobileNodeGroupIdentifier(IpV6MobileNodeGroupIdentifierSubType subType, uint mobileNodeGroupIdentifier)
            : base(IpV6MobilityOptionType.MobileNodeGroupIdentifier)
        {
            SubType = subType;
            MobileNodeGroupIdentifier = mobileNodeGroupIdentifier;
        }

        /// <summary>
        /// Identifies the specific mobile node's group type.
        /// </summary>
        public IpV6MobileNodeGroupIdentifierSubType SubType { get; private set; }

        /// <summary>
        /// Contains the mobile node's group identifier.
        /// The value of (0) is reserved and should not be used.
        /// The value of (1) ALL-SESSIONS is the default group of all mobility sessions established between a given local mobility anchor and a mobile access
        /// gateway.
        /// </summary>
        public uint MobileNodeGroupIdentifier { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            IpV6MobileNodeGroupIdentifierSubType subType = (IpV6MobileNodeGroupIdentifierSubType)data[Offset.SubType];
            uint mobileNodeGroupIdentifier = data.ReadUInt(Offset.MobileNodeGroupIdentifier, Endianity.Big);

            return new IpV6MobilityOptionMobileNodeGroupIdentifier(subType, mobileNodeGroupIdentifier);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionMobileNodeGroupIdentifier);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.SubType, (byte)SubType);
            buffer.Write(offset + Offset.MobileNodeGroupIdentifier, MobileNodeGroupIdentifier, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionMobileNodeGroupIdentifier()
            : this(IpV6MobileNodeGroupIdentifierSubType.BulkBindingUpdateGroup, 0)
        {
        }

        private bool EqualsData(IpV6MobilityOptionMobileNodeGroupIdentifier other)
        {
            return other != null &&
                   SubType == other.SubType && MobileNodeGroupIdentifier == other.MobileNodeGroupIdentifier;
        }
    }

    /// <summary>
    /// RFC 6705.
    /// <pre>
    /// +-----+-------------+----------------+
    /// | Bit | 0-7         | 8-15           |
    /// +-----+-------------+----------------+
    /// | 0   | Option Type | Opt Data Len   |
    /// +-----+-------------+----------------+
    /// | 16  | Reserved    | Address Length |
    /// +-----+-------------+----------------+
    /// | 32  | MAG IPv6 Address             |
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
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.MobileAccessGatewayIpV6Address)]
    public sealed class IpV6MobilityOptionMobileAccessGatewayIpV6Address : IpV6MobilityOptionComplex
    {
        public const byte AddressLength = 128;

        private static class Offset
        {
            public const int AddressLength = sizeof(byte);
            public const int Address = AddressLength + sizeof(byte);
        }

        public const int OptionDataLength = Offset.Address + IpV6Address.SizeOf;

        public IpV6MobilityOptionMobileAccessGatewayIpV6Address(IpV6Address address)
            : base(IpV6MobilityOptionType.MobileAccessGatewayIpV6Address)
        {
            Address = address;
        }

        /// <summary>
        /// Contains the MAG's IPv6 address.
        /// </summary>
        public IpV6Address Address { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            if (data[Offset.AddressLength] != AddressLength)
                return null;

            return new IpV6MobilityOptionMobileAccessGatewayIpV6Address(data.ReadIpV6Address(Offset.Address, Endianity.Big));
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionMobileAccessGatewayIpV6Address);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.AddressLength, AddressLength);
            buffer.Write(offset + Offset.Address, Address, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6MobilityOptionMobileAccessGatewayIpV6Address()
            : this(IpV6Address.Zero)
        {
        }

        private bool EqualsData(IpV6MobilityOptionMobileAccessGatewayIpV6Address other)
        {
            return other != null &&
                   Address.Equals(other.Address);
        }
    }

    /// <summary>
    /// RFC 6757.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | ANI Sub-option(s)          |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.AccessNetworkIdentifier)]
    public sealed class IpV6MobilityOptionAccessNetworkIdentifier : IpV6MobilityOptionComplex
    {
        public IpV6MobilityOptionAccessNetworkIdentifier(IpV6AccessNetworkIdentifierSubOptions subOptions)
            : base(IpV6MobilityOptionType.AccessNetworkIdentifier)
        {
            if (subOptions.BytesLength > MaxDataLength)
                throw new ArgumentOutOfRangeException("subOptions", subOptions, string.Format("SubOptions take more than {0} bytes", MaxDataLength));
            SubOptions = subOptions;
        }

        /// <summary>
        /// Sub options.
        /// </summary>
        public IpV6AccessNetworkIdentifierSubOptions SubOptions { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            return new IpV6MobilityOptionAccessNetworkIdentifier(new IpV6AccessNetworkIdentifierSubOptions(data));
        }

        internal override int DataLength
        {
            get { return SubOptions.BytesLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionAccessNetworkIdentifier);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            SubOptions.Write(buffer, offset);
            offset += DataLength;
        }

        private IpV6MobilityOptionAccessNetworkIdentifier()
            : this(IpV6AccessNetworkIdentifierSubOptions.None)
        {
        }

        private bool EqualsData(IpV6MobilityOptionAccessNetworkIdentifier other)
        {
            return other != null &&
                   SubOptions.Equals(other.SubOptions);
        }
    }

    /// <summary>
    /// RFC 6757.
    /// </summary>
    public class IpV6AccessNetworkIdentifierSubOptions : V6Options<IpV6AccessNetworkIdentifierSubOption>
    {
        /// <summary>
        /// Creates options from a list of options.
        /// </summary>
        /// <param name="options">The list of options.</param>
        public IpV6AccessNetworkIdentifierSubOptions(IList<IpV6AccessNetworkIdentifierSubOption> options)
            : base(options, true)
        {
        }

        /// <summary>
        /// Creates options from a list of options.
        /// </summary>
        /// <param name="options">The list of options.</param>
        public IpV6AccessNetworkIdentifierSubOptions(params IpV6AccessNetworkIdentifierSubOption[] options)
            : this((IList<IpV6AccessNetworkIdentifierSubOption>)options)
        {
        }

        internal IpV6AccessNetworkIdentifierSubOptions(DataSegment data)
            : this(Read(data))
        {
        }

        private IpV6AccessNetworkIdentifierSubOptions(Tuple<IList<IpV6AccessNetworkIdentifierSubOption>, bool> optionsAndIsValid)
            : base(optionsAndIsValid.Item1, optionsAndIsValid.Item2)
        {
        }

        /// <summary>
        /// No options instance.
        /// </summary>
        public static IpV6AccessNetworkIdentifierSubOptions None
        {
            get { return _none; }
        }

        internal static Tuple<IList<IpV6AccessNetworkIdentifierSubOption>, bool> Read(DataSegment data)
        {
            int offset = 0;
            List<IpV6AccessNetworkIdentifierSubOption> options = new List<IpV6AccessNetworkIdentifierSubOption>();
            bool isValid = true;
            while (offset < data.Length)
            {
                IpV6AccessNetworkIdentifierSubOptionType optionType = (IpV6AccessNetworkIdentifierSubOptionType)data[offset++];
                if (offset >= data.Length)
                {
                    isValid = false;
                    break;
                }

                byte optionDataLength = data[offset++];
                if (offset + optionDataLength > data.Length)
                {
                    isValid = false;
                    break;
                }

                IpV6AccessNetworkIdentifierSubOption option = CreateOption(optionType, data.Subsegment(ref offset, optionDataLength));
                if (option == null)
                {
                    isValid = false;
                    break;
                }

                options.Add(option);
            }

            return new Tuple<IList<IpV6AccessNetworkIdentifierSubOption>, bool>(options, isValid);
        }

        private static IpV6AccessNetworkIdentifierSubOption CreateOption(IpV6AccessNetworkIdentifierSubOptionType optionType, DataSegment data)
        {
            IpV6AccessNetworkIdentifierSubOption prototype;
            if (!_prototypes.TryGetValue(optionType, out prototype))
                return new IpV6AccessNetworkIdentifierSubOptionUnknown(optionType, data);
            return prototype.CreateInstance(data);
        }

        private static readonly Dictionary<IpV6AccessNetworkIdentifierSubOptionType, IpV6AccessNetworkIdentifierSubOption> _prototypes = InitializePrototypes();

        private static Dictionary<IpV6AccessNetworkIdentifierSubOptionType, IpV6AccessNetworkIdentifierSubOption> InitializePrototypes()
        {
            var prototypes =
                from type in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(IpV6AccessNetworkIdentifierSubOption).IsAssignableFrom(type) &&
                      GetRegistrationAttribute(type) != null
                select new
                {
                    GetRegistrationAttribute(type).OptionType,
                    Option = (IpV6AccessNetworkIdentifierSubOption)Activator.CreateInstance(type, true)
                };

            return prototypes.ToDictionary(option => option.OptionType, option => option.Option);
        }

        private static IpV6AccessNetworkIdentifierSubOptionTypeRegistrationAttribute GetRegistrationAttribute(Type type)
        {
            var registraionAttributes = type.GetCustomAttributes<IpV6AccessNetworkIdentifierSubOptionTypeRegistrationAttribute>(false);
            if (!registraionAttributes.Any())
                return null;

            return registraionAttributes.First();
        }

        private static readonly IpV6AccessNetworkIdentifierSubOptions _none = new IpV6AccessNetworkIdentifierSubOptions();
    }

    /// <summary>
    /// RFC 6757.
    /// </summary>
    internal sealed class IpV6AccessNetworkIdentifierSubOptionTypeRegistrationAttribute : Attribute
    {
        public IpV6AccessNetworkIdentifierSubOptionTypeRegistrationAttribute(IpV6AccessNetworkIdentifierSubOptionType optionType)
        {
            OptionType = optionType;
        }

        public IpV6AccessNetworkIdentifierSubOptionType OptionType { get; private set; }
    }

    /// <summary>
    /// RFC 6757.
    /// </summary>
    public enum IpV6AccessNetworkIdentifierSubOptionType : byte
    {
        /// <summary>
        /// Network-Identifier sub-option.
        /// </summary>
        NetworkIdentifier = 1,

        /// <summary>
        /// Geo-Location sub-option.
        /// </summary>
        GeoLocation = 2,

        /// <summary>
        /// Operator-Identifier sub-option.
        /// </summary>
        OperatorIdentifier = 3,
    }

    /// <summary>
    /// RFC 6757.
    /// </summary>
    public abstract class IpV6AccessNetworkIdentifierSubOption : Option, IEquatable<IpV6AccessNetworkIdentifierSubOption>
    {
        /// <summary>
        /// The type of the option.
        /// </summary>
        public IpV6AccessNetworkIdentifierSubOptionType OptionType { get; private set; }

        internal abstract IpV6AccessNetworkIdentifierSubOption CreateInstance(DataSegment data);

        protected IpV6AccessNetworkIdentifierSubOption(IpV6AccessNetworkIdentifierSubOptionType type)
        {
            OptionType = type;
        }

        public override sealed int Length
        {
            get { return sizeof(byte) + sizeof(byte) + DataLength; }
        }

        public bool Equals(IpV6AccessNetworkIdentifierSubOption other)
        {
            return other != null &&
                   OptionType == other.OptionType && Length == other.Length && EqualsData(other);
        }

        public override sealed bool Equals(Option other)
        {
            return Equals(other as IpV6AccessNetworkIdentifierSubOption);
        }

        internal abstract int DataLength { get; }

        internal abstract bool EqualsData(IpV6AccessNetworkIdentifierSubOption other);

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)OptionType;
            // TODO: Remove this check.
            if (DataLength > byte.MaxValue)
                throw new InvalidOperationException("Option length is too long.");
            buffer[offset++] = (byte)DataLength;
            WriteData(buffer, ref offset);
        }

        internal abstract void WriteData(byte[] buffer, ref int offset);
    }

    /// <summary>
    /// RFC 6757.
    /// <pre>
    /// +-----+---+---------------------------------+
    /// | Bit | 0 | 6-7                             |
    /// +-----+---+---------------------------------+
    /// | 0   | ANI Type                            |
    /// +-----+-------------------------------------+
    /// | 8   | ANI Length                          |
    /// +-----+---+---------------------------------+
    /// | 16  | E | Reserved                        | 
    /// +-----+---+---------------------------------+
    /// | 24  | Net-Name Len                        |
    /// +-----+-------------------------------------+
    /// | 32  | Network Name (e.g., SSID or PLMNID) |
    /// | ... |                                     |
    /// +-----+-------------------------------------+
    /// |     | AP-Name Len                         |       
    /// +-----+-------------------------------------+
    /// |     | Access-Point Name                   |
    /// | ... |                                     |
    /// +-----+-------------------------------------+
    /// </pre>
    /// </summary>
    [IpV6AccessNetworkIdentifierSubOptionTypeRegistration(IpV6AccessNetworkIdentifierSubOptionType.NetworkIdentifier)]
    public sealed class IpV6AccessNetworkIdentifierSubOptionNetworkIdentifier : IpV6AccessNetworkIdentifierSubOption
    {
        private static class Offset
        {
            public const int IsNetworkNameUtf8 = 0;
            public const int NetworkNameLength = IsNetworkNameUtf8 + sizeof(byte);
            public const int NetworkName = NetworkNameLength + sizeof(byte);
        }

        private static class Mask
        {
            public const byte IsNetworkNameUtf8 = 0x80;
        }

        public const int OptionDataMinimumLength = Offset.NetworkName + sizeof(byte);

        public IpV6AccessNetworkIdentifierSubOptionNetworkIdentifier(bool isNetworkNameUtf8, DataSegment networkName, DataSegment accessPointName)
            : base(IpV6AccessNetworkIdentifierSubOptionType.NetworkIdentifier)
        {
            if (networkName.Length > byte.MaxValue)
                throw new ArgumentOutOfRangeException("networkName", networkName, string.Format("Network Name cannot be longer than {0} bytes.", byte.MaxValue));
            if (accessPointName.Length > byte.MaxValue)
                throw new ArgumentOutOfRangeException("accessPointName", accessPointName, string.Format("Access Point Name cannot be longer than {0} bytes.", byte.MaxValue));

            IsNetworkNameUtf8 = isNetworkNameUtf8;
            NetworkName = networkName;
            AccessPointName = accessPointName;
        }

        /// <summary>
        /// Indicates whether the Network Name is encoded in UTF-8.
        /// If true, then the Network Name is encoded using UTF-8.
        /// If false, this indicates that the encoding is undefined and is determined by out-of-band mechanisms.
        /// </summary>
        public bool IsNetworkNameUtf8 { get; private set; }

        /// <summary>
        /// The name of the access network to which the mobile node is attached.
        /// The type of the Network Name is dependent on the access technology to which the mobile node is attached.
        /// If it is 802.11 access, the Network Name must be the SSID of the network.
        /// If the access network is 3GPP access, the Network Name is the PLMN Identifier of the network.
        /// If the access network is 3GPP2 access, the Network Name is the Access Network Identifier.
        /// 
        /// When encoding the PLMN Identifier, both the Mobile Network Code (MNC) and Mobile Country Code (MCC) must be 3 digits.
        /// If the MNC in use only has 2 digits, then it must be preceded with a '0'.
        /// Encoding must be UTF-8.
        /// </summary>
        public DataSegment NetworkName { get; private set; }

        /// <summary>
        /// The name of the access point (physical device name) to which the mobile node is attached.
        /// This is the identifier that uniquely identifies the access point.
        /// While Network Name (e.g., SSID) identifies the operator's access network,
        /// Access-Point Name identifies a specific network device in the network to which the mobile node is attached.
        /// In some deployments, the Access-Point Name can be set to the Media Access Control (MAC) address of the device or some unique identifier
        /// that can be used by the policy systems in the operator network to unambiguously identify the device.
        /// The string is carried in UTF-8 representation.
        /// </summary>
        public DataSegment AccessPointName { get; private set; }

        internal override IpV6AccessNetworkIdentifierSubOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            bool isNetworkNameUtf8 = data.ReadBool(Offset.IsNetworkNameUtf8, Mask.IsNetworkNameUtf8);
            
            byte networkNameLength = data[Offset.NetworkNameLength];
            if (data.Length < OptionDataMinimumLength + networkNameLength)
                return null;
            DataSegment networkName = data.Subsegment(Offset.NetworkName, networkNameLength);

            int accessPointNameLengthOffset = Offset.NetworkName + networkNameLength;
            byte accessPointNameLength = data[accessPointNameLengthOffset];
            if (data.Length != OptionDataMinimumLength + networkNameLength + accessPointNameLength)
                return null;
            int accessPointNameOffset = accessPointNameLengthOffset + sizeof(byte);
            DataSegment accessPointName = data.Subsegment(accessPointNameOffset, accessPointNameLength);

            return new IpV6AccessNetworkIdentifierSubOptionNetworkIdentifier(isNetworkNameUtf8, networkName, accessPointName);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + NetworkName.Length + AccessPointName.Length; }
        }

        internal override bool EqualsData(IpV6AccessNetworkIdentifierSubOption other)
        {
            return EqualsData(other as IpV6AccessNetworkIdentifierSubOptionNetworkIdentifier);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            if (IsNetworkNameUtf8)
                buffer.Write(offset + Offset.IsNetworkNameUtf8, Mask.IsNetworkNameUtf8);
            buffer.Write(offset + Offset.NetworkNameLength, (byte)NetworkName.Length);
            NetworkName.Write(buffer, offset + Offset.NetworkName);
            buffer.Write(offset + AccessPointNameLengthOffset, (byte)AccessPointName.Length);
            AccessPointName.Write(buffer, offset + AccessPointNameOffset);
            offset += DataLength;
        }

        private int AccessPointNameLengthOffset { get { return Offset.NetworkName + NetworkName.Length; } }
        private int AccessPointNameOffset { get { return AccessPointNameLengthOffset + sizeof(byte); } }

        private IpV6AccessNetworkIdentifierSubOptionNetworkIdentifier()
            : this(false, DataSegment.Empty, DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6AccessNetworkIdentifierSubOptionNetworkIdentifier other)
        {
            return other != null &&
                   IsNetworkNameUtf8 == other.IsNetworkNameUtf8 && NetworkName.Equals(other.NetworkName) && AccessPointName.Equals(other.AccessPointName);
        }
    }

    /// <summary>
    /// RFC 6757.
    /// <pre>
    /// +-----+----------+------------+
    /// | Bit | 0-7      | 8-15       |
    /// +-----+----------+------------+
    /// | 0   | ANI Type | ANI Length |
    /// +-----+----------+------------+
    /// | 16  | Option Data           |
    /// | ... |                       |
    /// +-----+-----------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6AccessNetworkIdentifierSubOptionUnknown : IpV6AccessNetworkIdentifierSubOption
    {
        public IpV6AccessNetworkIdentifierSubOptionUnknown(IpV6AccessNetworkIdentifierSubOptionType type, DataSegment data)
            : base(type)
        {
            Data = data;
        }

        public DataSegment Data { get; private set; }

        internal override IpV6AccessNetworkIdentifierSubOption CreateInstance(DataSegment data)
        {
            throw new InvalidOperationException("IpV6AccessNetworkIdentifierSubOptionUnknown shouldn't be registered.");
        }

        internal override int DataLength
        {
            get { return Data.Length; }
        }

        internal override bool EqualsData(IpV6AccessNetworkIdentifierSubOption other)
        {
            return EqualsData(other as IpV6AccessNetworkIdentifierSubOptionUnknown);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Data);
        }

        private bool EqualsData(IpV6AccessNetworkIdentifierSubOptionUnknown other)
        {
            return other != null &&
                   Data.Equals(other.Data);
        }
    }

    /// <summary>
    /// RFC 6757.
    /// <pre>
    /// +-----+-------------------+
    /// | Bit | 0-7               |
    /// +-----+-------------------+
    /// | 0   | ANI Type          |
    /// +-----+-------------------+
    /// | 8   | ANI Length        |
    /// +-----+-------------------+
    /// | 16  | Latitude Degrees  | 
    /// |     |                   | 
    /// |     |                   | 
    /// +-----+-------------------+
    /// | 40  | Longitude Degrees |
    /// |     |                   | 
    /// |     |                   | 
    /// +-----+-------------------+
    /// </pre>
    /// </summary>
    [IpV6AccessNetworkIdentifierSubOptionTypeRegistration(IpV6AccessNetworkIdentifierSubOptionType.GeoLocation)]
    public sealed class IpV6AccessNetworkIdentifierSubOptionGeoLocation : IpV6AccessNetworkIdentifierSubOption
    {
        private static class Offset
        {
            public const int LatitudeDegrees = 0;
            public const int LongitudeDegrees = LatitudeDegrees + UInt24.SizeOf;
        }

        public const int OptionDataLength = Offset.LongitudeDegrees + UInt24.SizeOf;

        public IpV6AccessNetworkIdentifierSubOptionGeoLocation(UInt24 latitudeDegrees, UInt24 longitudeDegrees)
            : base(IpV6AccessNetworkIdentifierSubOptionType.GeoLocation)
        {
            LatitudeDegrees = latitudeDegrees;
            LongitudeDegrees = longitudeDegrees;
        }

        /// <summary>
        /// A 24-bit latitude degree value encoded as a two's complement, fixed point number with 9 whole bits.
        /// Positive degrees correspond to the Northern Hemisphere and negative degrees correspond to the Southern Hemisphere.
        /// The value ranges from -90 to +90 degrees.
        /// </summary>
        public UInt24 LatitudeDegrees { get; private set; }

        /// <summary>
        /// Positive degrees correspond to the Northern Hemisphere and negative degrees correspond to the Southern Hemisphere.
        /// The value ranges from -90 to +90 degrees.
        /// </summary>
        public double LatitudeDegreesReal
        {
            get { return ToReal(LatitudeDegrees); }
        }

        /// <summary>
        /// A 24-bit longitude degree value encoded as a two's complement, fixed point number with 9 whole bits.
        /// The value ranges from -180 to +180 degrees.
        /// </summary>
        public UInt24 LongitudeDegrees { get; private set; }

        /// <summary>
        /// The value ranges from -180 to +180 degrees.
        /// </summary>
        public double LongitudeDegreesReal
        {
            get { return ToReal(LatitudeDegrees); }
        }

        internal override IpV6AccessNetworkIdentifierSubOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            UInt24 latitudeDegrees = data.ReadUInt24(Offset.LatitudeDegrees, Endianity.Big);
            UInt24 longitudeDegrees = data.ReadUInt24(Offset.LongitudeDegrees, Endianity.Big);

            return new IpV6AccessNetworkIdentifierSubOptionGeoLocation(latitudeDegrees, longitudeDegrees);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6AccessNetworkIdentifierSubOption other)
        {
            return EqualsData(other as IpV6AccessNetworkIdentifierSubOptionGeoLocation);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.LatitudeDegrees, LatitudeDegrees, Endianity.Big);
            buffer.Write(offset + Offset.LongitudeDegrees, LongitudeDegrees, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6AccessNetworkIdentifierSubOptionGeoLocation()
            : this(0, 0)
        {
        }

        private static double ToReal(UInt24 twosComplementFixedPointWith9WholeBits)
        {
            bool isPositive = twosComplementFixedPointWith9WholeBits >> 23 == 1;
            int integerPart = (twosComplementFixedPointWith9WholeBits & 0x7F8000) >> 15;
            int fractionPart = twosComplementFixedPointWith9WholeBits & 0x007FFF;
            return (isPositive ? 1 : -1) *
                   (integerPart + (((double)fractionPart) / (1 << 15)));
        }

        private bool EqualsData(IpV6AccessNetworkIdentifierSubOptionGeoLocation other)
        {
            return other != null &&
                   LatitudeDegrees == other.LatitudeDegrees && LongitudeDegrees == other.LongitudeDegrees;
        }
    }

    /// <summary>
    /// RFC 6757.
    /// </summary>
    public enum IpV6AccessNetworkIdentifierOperatorIdentifierType : byte
    {
        /// <summary>
        /// Operator-Identifier as a variable-length Private Enterprise Number (PEN) encoded in a network-byte order.
        /// The maximum PEN value depends on the ANI Length and is calculated using the formula: maximum PEN = 2^((ANI_length-1)*8)-1.
        /// For example, the ANI Length of 4 allows for encoding PENs from 0 to 2^24-1, i.e., from 0 to 16777215,
        /// and uses 3 octets of Operator-Identifier space.
        /// </summary>
        PrivateEnterpriseNumber = 1,

        /// <summary>
        /// Realm of the operator.
        /// Realm names are required to be unique and are piggybacked on the administration of the DNS namespace.
        /// Realms meet the syntactic requirements of the "Preferred Name Syntax".
        /// They are encoded as US-ASCII.
        /// 3GPP specifications also define realm names that can be used to convey PLMN Identifiers.
        /// </summary>
        RealmOfTheOperator = 2,
    }

    /// <summary>
    /// RFC 6757.
    /// <pre>
    /// +-----+---------------------+
    /// | Bit | 0-7                 |
    /// +-----+---------------------+
    /// | 0   | ANI Type            |
    /// +-----+---------------------+
    /// | 8   | ANI Length          |
    /// +-----+---------------------+
    /// | 16  | Op-ID Type          | 
    /// +-----+---------------------+
    /// | 24  | Operator-Identifier |
    /// | ... |                     | 
    /// +-----+---------------------+
    /// </pre>
    /// </summary>
    [IpV6AccessNetworkIdentifierSubOptionTypeRegistration(IpV6AccessNetworkIdentifierSubOptionType.OperatorIdentifier)]
    public sealed class IpV6AccessNetworkIdentifierSubOptionOperatorIdentifier : IpV6AccessNetworkIdentifierSubOption
    {
        private static class Offset
        {
            public const int IdentifierType = 0;
            public const int Identifier = IdentifierType + sizeof(byte);
        }

        public const int OptionDataMinimumLength = Offset.Identifier;

        public IpV6AccessNetworkIdentifierSubOptionOperatorIdentifier(IpV6AccessNetworkIdentifierOperatorIdentifierType identifierType, DataSegment identifier)
            : base(IpV6AccessNetworkIdentifierSubOptionType.OperatorIdentifier)
        {
            IdentifierType = identifierType;
            Identifier = identifier;
        }

        /// <summary>
        /// Indicates the type of the Operator-Identifier.
        /// </summary>
        public IpV6AccessNetworkIdentifierOperatorIdentifierType IdentifierType { get; private set; }

        /// <summary>
        /// Up to 253 octets of the Operator-Identifier.
        /// The encoding of the identifier depends on the used Operator-Identifier Type.
        /// </summary>
        public DataSegment Identifier { get; private set; }

        internal override IpV6AccessNetworkIdentifierSubOption CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            IpV6AccessNetworkIdentifierOperatorIdentifierType identifierType = (IpV6AccessNetworkIdentifierOperatorIdentifierType)data[Offset.IdentifierType];
            DataSegment identifier = data.Subsegment(Offset.Identifier, data.Length - Offset.Identifier);

            return new IpV6AccessNetworkIdentifierSubOptionOperatorIdentifier(identifierType, identifier);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + Identifier.Length; }
        }

        internal override bool EqualsData(IpV6AccessNetworkIdentifierSubOption other)
        {
            return EqualsData(other as IpV6AccessNetworkIdentifierSubOptionOperatorIdentifier);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.IdentifierType, (byte)IdentifierType);
            Identifier.Write(buffer, offset + Offset.Identifier);
            offset += DataLength;
        }

        private IpV6AccessNetworkIdentifierSubOptionOperatorIdentifier()
            : this(IpV6AccessNetworkIdentifierOperatorIdentifierType.PrivateEnterpriseNumber, DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6AccessNetworkIdentifierSubOptionOperatorIdentifier other)
        {
            return other != null &&
                   IdentifierType == other.IdentifierType && Identifier.Equals(other.Identifier);
        }
    }
}
