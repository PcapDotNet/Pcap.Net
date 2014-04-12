using System;

namespace PcapDotNet.Packets.IpV6
{
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
}