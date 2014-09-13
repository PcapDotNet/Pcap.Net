using System;

namespace PcapDotNet.Packets.IpV6
{
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
        /// <summary>
        /// Creates an instance from type and data.
        /// </summary>
        /// <param name="type">The type of the option.</param>
        /// <param name="data">The data of the option.</param>
        public IpV6FlowIdentificationSubOptionUnknown(IpV6FlowIdentificationSubOptionType type, DataSegment data)
            : base(type)
        {
            Data = data;
        }

        /// <summary>
        /// The data of the option.
        /// </summary>
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

        internal override object GetDataHashCode()
        {
            return Data.GetHashCode();
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
}