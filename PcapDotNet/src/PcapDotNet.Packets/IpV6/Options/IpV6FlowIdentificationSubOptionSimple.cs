using System;

namespace PcapDotNet.Packets.IpV6
{
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

        public sealed override int Length
        {
            get { return base.Length; }
        }

        internal sealed override bool EqualsData(IpV6FlowIdentificationSubOption other)
        {
            return true;
        }

        internal override object GetDataHashCode()
        {
            return 0;
        }

        internal sealed override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
        }

        internal sealed override IpV6FlowIdentificationSubOption CreateInstance(DataSegment data)
        {
            throw new InvalidOperationException("Simple options shouldn't be registered.");
        }
    }
}