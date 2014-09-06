using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
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

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionVendorSpecific);
        }

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(VendorId, SubType, Data);
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
}