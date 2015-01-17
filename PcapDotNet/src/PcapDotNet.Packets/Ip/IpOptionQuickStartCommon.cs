using System;
using System.Globalization;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Ip
{
    /// <summary>
    /// Contains common QuickStart option parameters.
    /// </summary>
    public static class IpOptionQuickStartCommon
    {
        /// <summary>
        /// The maximum value for the rate field.
        /// </summary>
        public const byte RateMaximumValue = 0x0F;

        /// <summary>
        /// The maximum value for the nonce field.
        /// </summary>
        public const uint NonceMaximumValue = 0x3FFFFFFF;

        internal const int DataLength = 6;

        private static class Offset
        {
            public const int Function = 0;
            public const int Rate = Function;
            public const int Ttl = Rate + sizeof(byte);
            public const int Nonce = Ttl + sizeof(byte);
        }

        private static class Mask
        {
            public const byte Function = 0xF0;
            public const byte Rate = 0x0F;
        }

        private static class Shift
        {
            public const int Function = 4;
            public const int Nonce = 2;
        }

        internal static void AssertValidParameters(IpV4OptionQuickStartFunction function, byte rate, uint nonce)
        {
            if (function != IpV4OptionQuickStartFunction.RateRequest &&
                function != IpV4OptionQuickStartFunction.RateReport)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Illegal function {0}", function), "function");
            }

            if (rate > RateMaximumValue)
                throw new ArgumentOutOfRangeException("rate", rate, string.Format(CultureInfo.InvariantCulture, "Rate maximum value is {0}", RateMaximumValue));

            if (nonce > NonceMaximumValue)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "nonce cannot be bigger than {0}", NonceMaximumValue), "nonce");
        }

        internal static int CalcRateKbps(byte rate)
        {
            if (rate == 0)
                return 0;

            return 40 * (1 << rate);
        }

        internal static void ReadData(DataSegment data, out IpV4OptionQuickStartFunction function, out byte rate, out byte ttl, out uint nonce)
        {
            function = (IpV4OptionQuickStartFunction)((data[Offset.Function] & Mask.Function) >> Shift.Function);
            rate = (byte)(data[Offset.Rate] & Mask.Rate);
            ttl = data[Offset.Ttl];
            nonce = data.ReadUInt(Offset.Nonce, Endianity.Big) >> Shift.Nonce;
        }

        internal static void WriteData(byte[] buffer, ref int offset, IpV4OptionQuickStartFunction function, byte rate, byte ttl, uint nonce)
        {
            buffer[offset + Offset.Function] = (byte)((((byte)function << Shift.Function) & Mask.Function) | (rate & Mask.Rate));
            buffer[offset + Offset.Ttl] = ttl;
            buffer.Write(offset + Offset.Nonce, nonce << Shift.Nonce, Endianity.Big);
            offset += DataLength;
        }
    }
}