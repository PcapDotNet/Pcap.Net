using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Ip
{
    public static class IpOptionQuickStartCommon
    {
        internal const int DataLength = 6;

        /// <summary>
        /// The maximum value for the rate field.
        /// </summary>
        public const byte RateMaximumValue = 0x0F;

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

        internal static void AssertValidParameters(IpV4OptionQuickStartFunction function, byte rate, byte ttl, uint nonce)
        {
            if (function != IpV4OptionQuickStartFunction.RateRequest &&
                function != IpV4OptionQuickStartFunction.RateReport)
            {
                throw new ArgumentException("Illegal function " + function, "function");
            }

            if (rate > RateMaximumValue)
                throw new ArgumentOutOfRangeException("rate", rate, "Rate maximum value is " + RateMaximumValue);

            if ((nonce & 0x00000003) != 0)
                throw new ArgumentException("nonce last two bits are reserved and must be zero", "nonce");
        }

        internal static int CalcRateKbps(byte rate)
        {
            if (rate == 0)
                return 0;

            return 40*(1 << rate);
        }

        internal static void ReadData(DataSegment data, out IpV4OptionQuickStartFunction function, out byte rate, out byte ttl, out uint nonce)
        {
            function = (IpV4OptionQuickStartFunction)(data[Offset.Function] & Mask.Function);
            rate = (byte)(data[Offset.Rate] & Mask.Rate);
            ttl = data[Offset.Ttl];
            nonce = data.ReadUInt(Offset.Nonce, Endianity.Big);
        }

        internal static void WriteData(byte[] buffer, ref int offset, IpV4OptionQuickStartFunction function, byte rate, byte ttl, uint nonce)
        {
            buffer[offset + Offset.Function] = (byte)(((byte)function & Mask.Function) | (rate & Mask.Rate));
            buffer[offset + Offset.Ttl] = ttl;
            buffer.Write(offset + Offset.Nonce, nonce, Endianity.Big);
            offset += DataLength;
        }
    }
}