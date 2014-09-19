using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

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
    /// | 16  | FID ...                    |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.FlowSummary)]
    public sealed class IpV6MobilityOptionFlowSummary : IpV6MobilityOptionComplex
    {
        /// <summary>
        /// The minimum number of bytes this option data takes.
        /// </summary>
        public const int OptionDataMinimumLength = sizeof(ushort);

        /// <summary>
        /// Creates an instance from an array of flow identifiers.
        /// </summary>
        /// <param name="flowIdentifiers">
        /// Indicating a registered FID.
        /// One or more FID fields can be included in this option.
        /// </param>
        public IpV6MobilityOptionFlowSummary(params ushort[] flowIdentifiers)
            : this(flowIdentifiers.AsReadOnly())
        {
        }

        /// <summary>
        /// Creates an instance from an enumerable of flow identifiers.
        /// </summary>
        /// <param name="flowIdentifiers">
        /// Indicating a registered FID.
        /// One or more FID fields can be included in this option.
        /// </param>
        public IpV6MobilityOptionFlowSummary(IEnumerable<ushort> flowIdentifiers)
            : this((IList<ushort>)flowIdentifiers.ToList())
        {
        }

        /// <summary>
        /// Creates an instance from a list of flow identifiers.
        /// </summary>
        /// <param name="flowIdentifiers">
        /// Indicating a registered FID.
        /// One or more FID fields can be included in this option.
        /// </param>
        public IpV6MobilityOptionFlowSummary(IList<ushort> flowIdentifiers)
            : this(flowIdentifiers.AsReadOnly())
        {
        }

        /// <summary>
        /// Creates an instance from a collection of flow identifiers.
        /// </summary>
        /// <param name="flowIdentifiers">
        /// Indicating a registered FID.
        /// One or more FID fields can be included in this option.
        /// </param>
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

        internal override int GetDataHashCode()
        {
            return FlowIdentifiers.SequenceGetHashCode();
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
}