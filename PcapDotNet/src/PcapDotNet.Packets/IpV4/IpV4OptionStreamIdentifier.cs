using System;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Stream Identifier
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
        public const int OptionLength = 4;
        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        public IpV4OptionStreamIdentifier(ushort identifier)
            : base(IpV4OptionType.StreamIdentifier)
        {
            _identifier = identifier;
        }

        public ushort Identifier
        {
            get { return _identifier; }
        }

        public override int Length
        {
            get { return OptionLength; }
        }

        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        public bool Equals(IpV4OptionStreamIdentifier other)
        {
            if (other == null)
                return false;
            return Identifier == other.Identifier;
        }

        public override bool Equals(IpV4Option other)
        {
            return Equals(other as IpV4OptionStreamIdentifier);
        }

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
            buffer[offset++] = (byte)Length;
            buffer.Write(ref offset, Identifier, Endianity.Big);
        }

        private readonly ushort _identifier;
    }
}