using System.Text;

namespace Packets
{
    public struct IpV4Address
    {
        public IpV4Address(uint value)
        {
            _value = value;
        }

        public uint ToUInt()
        {
            return _value;
        }

        public static IpV4Address FromReversedEndianity(uint value)
        {
            uint result = value >> 24;
            result += (value >> 16) % 256 << 8;
            result += (value >> 8) % 256 << 16;
            result += value % 256 << 24;
            return new IpV4Address(result);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(15);

            stringBuilder.Append(_value >> 24);
            stringBuilder.Append('.');
            stringBuilder.Append((_value >> 16) % 256);
            stringBuilder.Append('.');
            stringBuilder.Append((_value >> 8) % 256);
            stringBuilder.Append('.');
            stringBuilder.Append(_value % 256);

            return stringBuilder.ToString();
        }

        private readonly uint _value;
    }
}