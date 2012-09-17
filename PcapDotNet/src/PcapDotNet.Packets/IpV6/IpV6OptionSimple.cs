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

        public override sealed int Length
        {
            get { return base.Length; }
        }

        internal override sealed void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
        }
    }
}