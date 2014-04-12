using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
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
}