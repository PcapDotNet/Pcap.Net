using System;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Stream Identifier option.
    /// <pre>
    /// +--------+--------+--------+--------+
    /// |10001000|00000010|    Stream ID    |
    /// +--------+--------+--------+--------+
    ///  Type=136 Length=4
    /// </pre>
    /// 
    /// This option provides a way for the 16-bit SATNET stream identifier to be carried through networks that do not support the stream concept.
    /// 
    /// Must be copied on fragmentation.  
    /// Appears at most once in a datagram.
    /// </summary>
    [OptionTypeRegistration(typeof(IpV4OptionType), IpV4OptionType.StreamIdentifier)]
    public sealed class IpV4OptionStreamIdentifier : IpV4OptionComplex, IOptionComplexFactory, IEquatable<IpV4OptionStreamIdentifier>
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 4;

        /// <summary>
        /// Create the option according to the given identifier.
        /// </summary>
        public IpV4OptionStreamIdentifier(ushort identifier)
            : base(IpV4OptionType.StreamIdentifier)
        {
            _identifier = identifier;
        }

        /// <summary>
        /// Creates a 0 stream identifier
        /// </summary>
        public IpV4OptionStreamIdentifier()
            : this(0)
        {
        }

        /// <summary>
        /// The identifier of the stream.
        /// </summary>
        public ushort Identifier
        {
            get { return _identifier; }
        }

        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public override int Length
        {
            get { return OptionLength; }
        }

        /// <summary>
        /// True iff this option may appear at most once in a datagram.
        /// </summary>
        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        /// <summary>
        /// Two stream identifier options are equal if they have the same identifier.
        /// </summary>
        public bool Equals(IpV4OptionStreamIdentifier other)
        {
            if (other == null)
                return false;
            return Identifier == other.Identifier;
        }

        /// <summary>
        /// Two stream identifier options are equal if they have the same identifier.
        /// </summary>
        public override bool Equals(IpV4Option other)
        {
            return Equals(other as IpV4OptionStreamIdentifier);
        }

        /// <summary>
        /// The hash code value is the xor of the base class hash code and the identifier hash code.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   Identifier.GetHashCode();
        }

        /// <summary>
        /// Tries to read the option from a buffer starting from the option value (after the type and length).
        /// </summary>
        /// <param name="buffer">The buffer to read the option from.</param>
        /// <param name="offset">The offset to the first byte to read the buffer. Will be incremented by the number of bytes read.</param>
        /// <param name="valueLength">The number of bytes the option value should take according to the length field that was already read.</param>
        /// <returns>On success - the complex option read. On failure - null.</returns>
        Option IOptionComplexFactory.CreateInstance(byte[] buffer, ref int offset, byte valueLength)
        {
            if (valueLength != OptionHeaderLength)
                return null;

            ushort identifier = buffer.ReadUShort(ref offset, Endianity.Big);
            return new IpV4OptionStreamIdentifier(identifier);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Identifier, Endianity.Big);
        }

        private readonly ushort _identifier;
    }
}