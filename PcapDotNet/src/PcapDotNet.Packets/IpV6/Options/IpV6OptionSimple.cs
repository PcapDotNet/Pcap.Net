using System;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2460.
    /// <pre>
    /// +-----+-------------+
    /// | Bit | 0-7         |
    /// +-----+-------------+
    /// | 0   | Option Type |
    /// +-----+-------------+
    /// </pre>
    /// </summary>
    public abstract class IpV6OptionSimple : IpV6Option
    {
        protected IpV6OptionSimple(IpV6OptionType type) : base(type)
        {
        }

        public sealed override int Length
        {
            get { return sizeof(byte); }
        }

        internal sealed override bool EqualsData(IpV6Option other)
        {
            return true;
        }

        internal sealed override int GetDataHashCode()
        {
            return 0;
        }

        internal sealed override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
        }
    }
}
