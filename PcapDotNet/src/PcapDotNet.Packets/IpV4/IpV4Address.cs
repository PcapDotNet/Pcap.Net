using System.Text;

namespace PcapDotNet.Packets
{
    public struct IpV4Address
    {
        public IpV4Address(uint value)
        {
            _value = value;
        }

        public IpV4Address(string value)
        {
            string[] values = value.Split('.');
            _value = (uint)((byte.Parse(values[0]) << 24) +
                            (byte.Parse(values[1]) << 16) +
                            (byte.Parse(values[2]) << 8) +
                            (byte.Parse(values[3])));
        }

        public static IpV4Address Zero
        {
            get { return _zero; }
        }

        public uint ToValue()
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

        public bool Equals(IpV4Address other)
        {
            return ToValue() == other.ToValue();
        }

        public override bool Equals(object obj)
        {
            return (obj is IpV4Address &&
                    Equals((IpV4Address)obj));
        }

        public static bool operator ==(IpV4Address ipV4Address1, IpV4Address ipV4Address2)
        {
            return ipV4Address1.Equals(ipV4Address2);
        }

        public static bool operator !=(IpV4Address ipV4Address1, IpV4Address ipV4Address2)
        {
            return !(ipV4Address1 == ipV4Address2);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
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
        private static readonly IpV4Address _zero = new IpV4Address(0);
    }
}