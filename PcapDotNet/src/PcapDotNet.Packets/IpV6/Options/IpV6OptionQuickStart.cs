using System;
using PcapDotNet.Base;
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
    public sealed class IpV6OptionQuickStart : IpV6OptionComplex, IIpOptionQuickStart, IIpV6OptionComplexFactory
    {
        /// <summary>
        /// The number of bytes the option data takes.
        /// </summary>
        public const int OptionDataLength = IpOptionQuickStartCommon.DataLength;

        /// <summary>
        /// Creates an instance from the given function, rate, TTL and nonce.
        /// </summary>
        /// <param name="function">Function field.</param>
        /// <param name="rate">
        /// For rate request, this is the Rate Request field.
        /// For Report of Approved Rate, this is the Rate Report field.
        /// </param>
        /// <param name="ttl">
        /// For a Rate Request, contains the Quick-Start TTL (QS TTL) field.
        /// The sender must set the QS TTL field to a random value.
        /// Routers that approve the Quick-Start Request decrement the QS TTL (mod 256) by the same amount that they decrement the IP TTL.
        /// The QS TTL is used by the sender to detect if all the routers along the path understood and approved the Quick-Start option.
        /// The transport sender must calculate and store the TTL Diff, the difference between the IP TTL value, and the QS TTL value in the Quick-Start Request packet, as follows:
        /// TTL Diff = ( IP TTL - QS TTL ) mod 256.
        /// For a Report of Approved Rate, this is not used.
        /// </param>
        /// <param name="nonce">
        /// For a Rate Request and Report of Approved Rate, contain a 30-bit QS Nonce.
        /// The sender should set the QS Nonce to a random value.
        /// </param>
        public IpV6OptionQuickStart(IpV4OptionQuickStartFunction function, byte rate, byte ttl, uint nonce)
            : base(IpV6OptionType.QuickStart)
        {
            IpOptionQuickStartCommon.AssertValidParameters(function, rate, nonce);

            QuickStartFunction = function;
            Rate = rate;
            Ttl = ttl;
            Nonce = nonce;
        }

        /// <summary>
        /// Function field.
        /// </summary>
        public IpV4OptionQuickStartFunction QuickStartFunction { get; private set; }

        /// <summary>
        /// For rate request, this is the Rate Request field.
        /// For Report of Approved Rate, this is the Rate Report field.
        /// </summary>
        public byte Rate { get; private set; }

        /// <summary>
        /// The rate translated to KBPS.
        /// </summary>
        public int RateKbps
        {
            get { return IpOptionQuickStartCommon.CalcRateKbps(Rate); }
        }

        /// <summary>
        /// For a Rate Request, contains the Quick-Start TTL (QS TTL) field.
        /// The sender must set the QS TTL field to a random value.
        /// Routers that approve the Quick-Start Request decrement the QS TTL (mod 256) by the same amount that they decrement the IP TTL.
        /// The QS TTL is used by the sender to detect if all the routers along the path understood and approved the Quick-Start option.
        /// The transport sender must calculate and store the TTL Diff, the difference between the IP TTL value, and the QS TTL value in the Quick-Start Request packet, as follows:
        /// TTL Diff = ( IP TTL - QS TTL ) mod 256.
        /// For a Report of Approved Rate, this is not used.
        /// </summary>
        public byte Ttl { get; private set; }

        /// <summary>
        /// For a Rate Request and Report of Approved Rate, contain a 30-bit QS Nonce.
        /// The sender should set the QS Nonce to a random value.
        /// </summary>
        public uint Nonce { get; private set; }

        /// <summary>
        /// Parses an option from the given data.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The option if parsing was successful, null otherwise.</returns>
        public IpV6Option CreateInstance(DataSegment data)
        {
            if (data == null) 
                throw new ArgumentNullException("data");
            if (data.Length != OptionDataLength)
                return null;

            IpV4OptionQuickStartFunction function;
            byte rate;
            byte ttl;
            uint nonce;
            IpOptionQuickStartCommon.ReadData(data, out function, out rate, out ttl, out nonce);

            return new IpV6OptionQuickStart(function, rate, ttl, nonce);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6Option other)
        {
            return EqualsData(other as IpV6OptionQuickStart);
        }

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge((byte)QuickStartFunction, Rate, Ttl), Nonce);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            IpOptionQuickStartCommon.WriteData(buffer, ref offset, QuickStartFunction, Rate, Ttl, Nonce);
        }

        private IpV6OptionQuickStart()
            : this(IpV4OptionQuickStartFunction.RateRequest, 0, 0, 0)
        {
        }

        private bool EqualsData(IpV6OptionQuickStart other)
        {
            return other != null &&
                   QuickStartFunction == other.QuickStartFunction && Rate == other.Rate && Ttl == other.Ttl && Nonce == other.Nonce;
        }
    }
}