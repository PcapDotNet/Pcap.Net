using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
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

        /// <summary>
        /// The minimum number of bytes this option data takes.
        /// </summary>
        public const int OptionDataMinimumLength = Offset.MobileNodeIdentity;

        /// <summary>
        /// Creates an instance from status, remove and mobile node identity.
        /// </summary>
        /// <param name="status">
        /// Indicating the result of the dynamic DNS update procedure.
        /// This field must be set to 0 and ignored by the receiver when the DNS Update mobility option is included in a Binding Update message.
        /// When the DNS Update mobility option is included in the Binding Acknowledgement message, 
        /// values of the Status field less than 128 indicate that the dynamic DNS update was performed successfully by the Home Agent.
        /// Values greater than or equal to 128 indicate that the dynamic DNS update was not completed by the HA.
        /// </param>
        /// <param name="remove">
        /// Whether the Mobile Node is requesting the HA to remove the DNS entry identified by the FQDN specified in this option and the HoA of the Mobile Node.
        /// If false, the Mobile Node is requesting the HA to create or update a DNS entry with its HoA and the FQDN specified in the option.
        /// </param>
        /// <param name="mobileNodeIdentity">
        /// The identity of the Mobile Node in FQDN format to be used by the Home Agent to send a Dynamic DNS update.
        /// </param>
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

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge((byte)Status, Remove.ToByte()), MobileNodeIdentity);
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
}