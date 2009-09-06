namespace PcapDotNet.Packets
{
    /// <summary>
    /// A generic option (for IPv4 and TCP).
    /// The option is read from buffer and can be of different length.
    /// </summary>
    public abstract class Option
    {
        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public abstract int Length { get; }

        /// <summary>
        /// True iff this option may appear at most once in a datagram.
        /// </summary>
        public abstract bool IsAppearsAtMostOnce { get; }

        /// <summary>
        /// Checks whether two options have equivalent type.
        /// Useful to check if an option that must appear at most once appears in the list.
        /// </summary>
        public abstract bool Equivalent(Option other);

        internal abstract Option Read(byte[] buffer, ref int offset, int length);

        internal abstract void Write(byte[] buffer, ref int offset);
    }
}