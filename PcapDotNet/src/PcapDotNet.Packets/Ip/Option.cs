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

        public sealed override bool Equals(object obj)
        {
            return Equals(obj as Option);
        }

        public abstract bool Equals(Option other);

        internal abstract void Write(byte[] buffer, ref int offset);
    }
}