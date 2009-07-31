using System;

namespace PcapDotNet.Packets
{
    public struct IpV4Fragmentation : IEquatable<IpV4Fragmentation>
    {
        public static IpV4Fragmentation None
        {
            get { return _none; }
        }

        public IpV4Fragmentation(IpV4FragmentationFlags flags, ushort offset)
            : this((ushort)((ushort)flags | (offset / 8)))
        {
        }

        public IpV4FragmentationFlags Flags
        {
            get { return (IpV4FragmentationFlags)(_value & 0xE000); }
        }

        public ushort Offset
        {
            get { return (ushort)((_value & 0x1FFF) * 8); }
        }

        public bool Equals(IpV4Fragmentation other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return (obj is IpV4Fragmentation &&
                    Equals((IpV4Fragmentation)obj));
        }

        internal IpV4Fragmentation(ushort value)
        {
            _value = value;
        }

        internal void Write(byte[] buffer, int offset)
        {
            buffer.Write(offset, _value, Endianity.Big);
        }

        private static readonly IpV4Fragmentation _none = new IpV4Fragmentation(IpV4FragmentationFlags.None, 0);
        private readonly ushort _value;
    }
}