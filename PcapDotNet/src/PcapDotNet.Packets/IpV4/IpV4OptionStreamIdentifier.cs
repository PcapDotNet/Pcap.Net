using System;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Stream Identifier option.
    /// +--------+--------+--------+--------+
    /// |10001000|00000010|    Stream ID    |
    /// +--------+--------+--------+--------+
    ///  Type=136 Length=4
    /// 
    /// This option provides a way for the 16-bit SATNET stream identifier to be carried through networks that do not support the stream concept.
    /// 
    /// Must be copied on fragmentation.  
    /// Appears at most once in a datagram.
    /// </summary>
    public class IpV4OptionStreamIdentifier : IpV4OptionComplex, IEquatable<IpV4OptionStreamIdentifier>
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 4;

        /// <summary>
        /// The number of bytes this option's value take.
        /// </summary>
        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        /// <summary>
        /// Create the option according to the given identifier.
        /// </summary>
        public IpV4OptionStreamIdentifier(ushort identifier)
            : base(IpV4OptionType.StreamIdentifier)
        {
            _identifier = identifier;
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
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   Identifier.GetHashCode();
        }

        internal static IpV4OptionStreamIdentifier ReadOptionStreamIdentifier(byte[] buffer, ref int offset, byte valueLength)
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