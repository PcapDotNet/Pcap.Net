using System;

namespace PcapDotNet.Packets.Ip
{
    /// <summary>
    /// A generic option (for IPv4, IPv6 and TCP).
    /// The option is read from buffer and can be of different length.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Option")]
    public abstract class Option : IEquatable<Option>
    {
        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public abstract int Length { get; }

        /// <summary>
        /// Returns true iff the given object is an equivalent option.
        /// </summary>
        public sealed override bool Equals(object obj)
        {
            return Equals(obj as Option);
        }

        /// <summary>
        /// Returns true iff the given option is an equivalent option.
        /// </summary>
        public abstract bool Equals(Option other);

        /// <summary>
        /// Returns a hash code for the option.
        /// </summary>
        public override abstract int GetHashCode();

        internal abstract void Write(byte[] buffer, ref int offset);
    }
}