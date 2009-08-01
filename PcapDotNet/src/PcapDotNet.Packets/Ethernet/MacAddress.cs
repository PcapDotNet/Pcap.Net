using System;
using System.Globalization;
using PcapDotNet.Base;

namespace PcapDotNet.Packets
{
    public struct MacAddress
    {
        public const int SizeOf = UInt48.SizeOf;

        public MacAddress(UInt48 value)
        {
            _value = value;
        }

        public MacAddress(string address)
        {
            string[] hexes = address.Split(':');
            if (hexes.Length != 6)
                throw new ArgumentException("Failed parsing " + address + " as mac address. Expected 6 hexes and got " + hexes.Length + " hexes", "address");

            _value = (UInt48)(((long)Convert.ToByte(hexes[0], 16) << 40) |
                              ((long)Convert.ToByte(hexes[1], 16) << 32) |
                              ((long)Convert.ToByte(hexes[2], 16) << 24) |
                              ((long)Convert.ToByte(hexes[3], 16) << 16) |
                              ((long)Convert.ToByte(hexes[4], 16) << 8) |
                              Convert.ToByte(hexes[5], 16));
        }

        public UInt48 ToValue()
        {
            return _value;
        }

        public bool Equals(MacAddress other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return (obj is MacAddress &&
                    Equals((MacAddress)obj));
        }

        public static bool operator ==(MacAddress macAddress1, MacAddress macAddress2)
        {
            return macAddress1.Equals(macAddress2);
        }

        public static bool operator !=(MacAddress macAddress1, MacAddress macAddress2)
        {
            return !(macAddress1 == macAddress2);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:X2}:{1:X2}:{2:X2}:{3:X2}:{4:X2}:{5:X2}",
                                 (byte)(_value >> 40),
                                 (byte)(_value >> 32),
                                 (byte)(_value >> 24),
                                 (byte)(_value >> 16),
                                 (byte)(_value >> 8),
                                 (byte)(_value));
        }

        private readonly UInt48 _value;
    }
}