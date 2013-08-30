using PcapDotNet.Packets.Ip;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 4782.
    /// <pre>
    /// +-----+-------------+------+----------+--------------+-------+-------+
    /// | Bit | 0-7         | 8-15 | 16-19    | 20-23        | 24-29 | 30-31 |
    /// +-----+-------------+------+----------+--------------+-------+-------+
    /// | 0   | Option Type | 6    | Function | Rate Request | QS TTL        |
    /// +-----+-------------+------+----------+--------------+-------+-------+
    /// | 32  | QS Nonce                                             | R     |
    /// +-----+------------------------------------------------------+-------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.QuickStart)]
    public class IpV6OptionQuickStart : IpV6OptionComplex, IIpOptionQuickStart
    {
        public const int OptionDataLength = IpOptionQuickStartCommon.DataLength;

        public IpV6OptionQuickStart(IpV4OptionQuickStartFunction function, byte rate, byte ttl, uint nonce)
            : base(IpV6OptionType.QuickStart)
        {
            IpOptionQuickStartCommon.AssertValidParameters(function, rate, ttl, nonce);

            Function = function;
            Rate = rate;
            Ttl = ttl;
            Nonce = nonce;
        }

        public IpV4OptionQuickStartFunction Function { get; private set; }
        public byte Rate { get; private set; }

        public int RateKbps
        {
            get { return IpOptionQuickStartCommon.CalcRateKbps(Rate); }
        }

        public byte Ttl { get; private set; }
        public uint Nonce { get; private set; }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            IpV4OptionQuickStartFunction function;
            byte rate;
            byte ttl;
            uint nonce;
            IpOptionQuickStartCommon.ReadData(data, out function, out rate, out ttl, out nonce);

            return new IpV6OptionQuickStart(function, rate, ttl, nonce);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            IpOptionQuickStartCommon.WriteData(buffer, ref offset, Function, Rate, Ttl, Nonce);
        }
    }
}