using System;
using System.Globalization;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
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

        /// <summary>
        /// The minimum number of bytes this option data takes.
        /// </summary>
        public const int OptionDataMinimumLength = Offset.NetworkName + sizeof(byte);

        /// <summary>
        /// Creates an instance from is network name UTF8, network name and access point name.
        /// </summary>
        /// <param name="isNetworkNameUtf8">
        /// Indicates whether the Network Name is encoded in UTF-8.
        /// If true, then the Network Name is encoded using UTF-8.
        /// If false, this indicates that the encoding is undefined and is determined by out-of-band mechanisms.
        /// </param>
        /// <param name="networkName">
        /// The name of the access network to which the mobile node is attached.
        /// The type of the Network Name is dependent on the access technology to which the mobile node is attached.
        /// If it is 802.11 access, the Network Name must be the SSID of the network.
        /// If the access network is 3GPP access, the Network Name is the PLMN Identifier of the network.
        /// If the access network is 3GPP2 access, the Network Name is the Access Network Identifier.
        /// 
        /// When encoding the PLMN Identifier, both the Mobile Network Code (MNC) and Mobile Country Code (MCC) must be 3 digits.
        /// If the MNC in use only has 2 digits, then it must be preceded with a '0'.
        /// Encoding must be UTF-8.
        /// </param>
        /// <param name="accessPointName">
        /// The name of the access point (physical device name) to which the mobile node is attached.
        /// This is the identifier that uniquely identifies the access point.
        /// While Network Name (e.g., SSID) identifies the operator's access network,
        /// Access-Point Name identifies a specific network device in the network to which the mobile node is attached.
        /// In some deployments, the Access-Point Name can be set to the Media Access Control (MAC) address of the device or some unique identifier
        /// that can be used by the policy systems in the operator network to unambiguously identify the device.
        /// The string is carried in UTF-8 representation.
        /// </param>
        public IpV6AccessNetworkIdentifierSubOptionNetworkIdentifier(bool isNetworkNameUtf8, DataSegment networkName, DataSegment accessPointName)
            : base(IpV6AccessNetworkIdentifierSubOptionType.NetworkIdentifier)
        {
            if (networkName.Length > byte.MaxValue)
                throw new ArgumentOutOfRangeException("networkName", networkName,
                                                      string.Format(CultureInfo.InvariantCulture, "Network Name cannot be longer than {0} bytes.", byte.MaxValue));
            if (accessPointName.Length > byte.MaxValue)
                throw new ArgumentOutOfRangeException("accessPointName", accessPointName,
                                                      string.Format(CultureInfo.InvariantCulture, "Access Point Name cannot be longer than {0} bytes.",
                                                                    byte.MaxValue));

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

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(IsNetworkNameUtf8, NetworkName, AccessPointName);
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
}