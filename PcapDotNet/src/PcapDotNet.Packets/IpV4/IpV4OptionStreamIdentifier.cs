using System;

namespace PcapDotNet.Packets
{
    public class IpV4OptionStreamIdentifier : IpV4Option, IEquatable<IpV4OptionStreamIdentifier>
    {
        public const int OptionLength = 4;

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

        internal static IpV4OptionStreamIdentifier ReadOptionStreamIdentifier(byte[] buffer, ref int offset, int length)
        {
            if (length < OptionLength - 1)
                return null;

            byte optionLength = buffer[offset++];
            if (optionLength != OptionLength)
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