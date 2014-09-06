namespace PcapDotNet.Packets.Ip
{
    /// <summary>
    /// Either an IPv4 option or a TCP option.
    /// </summary>
    public abstract class V4Option : Option
    {
        /// <summary>
        /// True iff this option may appear at most once in a datagram.
        /// </summary>
        public abstract bool IsAppearsAtMostOnce { get; }

        /// <summary>
        /// Checks whether two options have equivalent type.
        /// Useful to check if an option that must appear at most once appears in the list.
        /// </summary>
        public abstract bool Equivalent(Option other);

        internal abstract V4Option Read(byte[] buffer, ref int offset, int length);
    }
}