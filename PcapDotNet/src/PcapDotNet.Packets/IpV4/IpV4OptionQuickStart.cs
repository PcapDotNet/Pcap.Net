using System;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// The Quick-Start Option for IPv4
    /// 
    /// <para>
    ///   The Quick-Start Request for IPv4 is defined as follows:
    ///   <pre>
    /// +--------+----------+-------+---------+-------+-------+
    /// | 0-7    | 8-15     | 16-19 | 20-23   | 24-29 | 30-31 |
    /// +--------+----------+-------+---------+-------+-------+
    /// | Option | Length=8 | Func. | Rate    | QS TTL        |
    /// |        |          | 0000  | Request |               |
    /// +--------+----------+-------+---------+-------+-------+
    /// | QS Nonce                                    | R     |
    /// +---------------------------------------------+-------+
    ///   </pre>
    /// </para>
    /// </summary>
    [OptionTypeRegistration(typeof(IpV4OptionType), IpV4OptionType.QuickStart)]
    public class IpV4OptionQuickStart : IpV4OptionComplex, IOptionComplexFactory, IEquatable<IpV4OptionQuickStart>
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 8;

        /// <summary>
        /// The number of bytes this option's value take.
        /// </summary>
        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        /// <summary>
        /// The maximum value for the rate field.
        /// </summary>
        public const byte RateMaximumValue = 0x0F;

        /// <summary>
        /// Create a quick start option according to the given field values.
        /// </summary>
        /// <param name="function">The function of this quick start option.</param>
        /// <param name="rate">Either the rate requested or reported.</param>
        /// <param name="ttl">
        /// The Quick-Start TTL (QS TTL) field.  
        /// The sender MUST set the QS TTL field to a random value.
        /// Routers that approve the Quick-Start Request decrement the QS TTL (mod 256) by the same amount that they decrement the IP TTL.  
        /// The QS TTL is used by the sender to detect if all the routers along the path understood and approved the Quick-Start option.
        /// </param>
        /// <param name="nonce">
        /// The QS Nonce gives the Quick-Start sender some protection against receivers lying about the value of the received Rate Request. 
        /// </param>
        public IpV4OptionQuickStart(IpV4OptionQuickStartFunction function, byte rate, byte ttl, uint nonce)
            : base(IpV4OptionType.QuickStart)
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

            _function = function;
            _rate = rate;
            _ttl = ttl;
            _nonce = nonce;
        }

        /// <summary>
        /// Creates a request with 0 fields.
        /// </summary>
        public IpV4OptionQuickStart()
            : this(IpV4OptionQuickStartFunction.RateRequest, 0, 0, 0)
        {
        }

        /// <summary>
        /// The function of this quick start option.
        /// </summary>
        public IpV4OptionQuickStartFunction Function
        {
            get { return _function; }
        }

        /// <summary>
        /// If function is request then this field is the rate request.
        /// If function is report then this field is the rate report.
        /// </summary>
        public byte Rate
        {
            get { return _rate; }
        }

        /// <summary>
        /// The rate translated to KBPS.
        /// </summary>
        public int RateKbps
        {
            get
            {
                if (Rate == 0)
                    return 0;

                return 40 * (1 << Rate);
            }
        }

        /// <summary>
        /// The Quick-Start TTL (QS TTL) field.  
        /// The sender MUST set the QS TTL field to a random value.
        /// Routers that approve the Quick-Start Request decrement the QS TTL (mod 256) by the same amount that they decrement the IP TTL.  
        /// The QS TTL is used by the sender to detect if all the routers along the path understood and approved the Quick-Start option.
        /// 
        /// <para>
        ///   For a Rate Request, the transport sender MUST calculate and store the TTL Diff, 
        ///   the difference between the IP TTL value, and the QS TTL value in the Quick-Start Request packet, as follows:
        ///   TTL Diff = ( IP TTL - QS TTL ) mod 256                 
        /// </para>
        /// </summary>
        public byte Ttl
        {
            get { return _ttl; }
        }

        /// <summary>
        /// The QS Nonce gives the Quick-Start sender some protection against receivers lying about the value of the received Rate Request. 
        /// This is particularly important if the receiver knows the original value of the Rate Request 
        /// (e.g., when the sender always requests the same value, and the receiver has a long history of communication with that sender).  
        /// Without the QS Nonce, there is nothing to prevent the receiver from reporting back to the sender a Rate Request of K, 
        /// when the received Rate Request was, in fact, less than K.
        /// 
        /// <para>
        ///   The format for the 30-bit QS Nonce.
        ///   <list type="table">
        ///     <listheader>
        ///         <term>Bits</term>
        ///         <description>Purpose</description>
        ///     </listheader>
        ///     <item><term>Bits 0-1</term><description>Rate 15 -> Rate 14</description></item>
        ///     <item><term>Bits 2-3</term><description>Rate 14 -> Rate 13</description></item>
        ///     <item><term>Bits 4-5</term><description>Rate 13 -> Rate 12</description></item>
        ///     <item><term>Bits 6-7</term><description>Rate 12 -> Rate 11</description></item>
        ///     <item><term>Bits 8-9</term><description>Rate 11 -> Rate 10</description></item>
        ///     <item><term>Bits 10-11</term><description>Rate 10 -> Rate 9</description></item>
        ///     <item><term>Bits 12-13</term><description>Rate 9 -> Rate 8</description></item>
        ///     <item><term>Bits 14-15</term><description>Rate 8 -> Rate 7</description></item>
        ///     <item><term>Bits 16-17</term><description>Rate 7 -> Rate 6</description></item>
        ///     <item><term>Bits 18-19</term><description>Rate 6 -> Rate 5</description></item>
        ///     <item><term>Bits 20-21</term><description>Rate 5 -> Rate 4</description></item>
        ///     <item><term>Bits 22-23</term><description>Rate 4 -> Rate 3</description></item>
        ///     <item><term>Bits 24-25</term><description>Rate 3 -> Rate 2</description></item>
        ///     <item><term>Bits 26-27</term><description>Rate 2 -> Rate 1</description></item>
        ///     <item><term>Bits 28-29</term><description>Rate 1 -> Rate 0</description></item>
        ///   </list>
        /// </para>
        /// 
        /// <para>
        /// The transport sender MUST initialize the QS Nonce to a random value. 
        /// If the router reduces the Rate Request from rate K to rate K-1, 
        /// then the router MUST set the field in the QS Nonce for "Rate K -> Rate K-1" to a new random value.  
        /// Similarly, if the router reduces the Rate Request by N steps, 
        /// the router MUST set the 2N bits in the relevant fields in the QS Nonce to a new random value.  
        /// The receiver MUST report the QS Nonce back to the sender.
        /// </para>
        /// 
        /// <para>
        /// If the Rate Request was not decremented in the network, then the QS Nonce should have its original value.  
        /// Similarly, if the Rate Request was decremented by N steps in the network, 
        /// and the receiver reports back a Rate Request of K, then the last 2K bits of the QS Nonce should have their original value.
        /// </para>
        /// </summary>
        public uint Nonce
        {
            get { return _nonce; }
        }

        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public override int Length
        {
            get { return OptionLength; }
        }

        /// <summary>
        /// True iff this option may appear at most once in a datagram.
        /// </summary>
        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        /// <summary>
        /// Two quick start options are equal iff they have the exact same field values.
        /// </summary>
        public bool Equals(IpV4OptionQuickStart other)
        {
            if (other == null)
                return false;

            return Function == other.Function &&
                   Rate == other.Rate &&
                   Ttl == other.Ttl &&
                   Nonce == other.Nonce;
        }

        /// <summary>
        /// Two trace route options are equal iff they have the exact same field values.
        /// </summary>
        public override bool Equals(IpV4Option other)
        {
            return Equals(other as IpV4OptionQuickStart);
        }

        /// <summary>
        /// The hash code is the xor of the base class hash code with the following values hash code:
        /// The combination of function, rate and ttl and the nonce.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   ((((byte)Function | Rate) << 8) | Ttl).GetHashCode() ^
                   Nonce.GetHashCode();
        }

        /// <summary>
        /// Tries to read the option from a buffer starting from the option value (after the type and length).
        /// </summary>
        /// <param name="buffer">The buffer to read the option from.</param>
        /// <param name="offset">The offset to the first byte to read the buffer. Will be incremented by the number of bytes read.</param>
        /// <param name="valueLength">The number of bytes the option value should take according to the length field that was already read.</param>
        /// <returns>On success - the complex option read. On failure - null.</returns>
        Option IOptionComplexFactory.CreateInstance(byte[] buffer, ref int offset, byte valueLength)
        {
//            if (buffer == null) 
//                throw new ArgumentNullException("buffer");

            if (valueLength != OptionValueLength)
                return null;

            byte functionAndRate = buffer[offset++];
            IpV4OptionQuickStartFunction function = (IpV4OptionQuickStartFunction)(functionAndRate & 0xF0);
            byte rate = (byte)(functionAndRate & 0x0F);

            byte ttl = buffer[offset++];
            uint nonce = buffer.ReadUInt(ref offset, Endianity.Big);

            return new IpV4OptionQuickStart(function, rate, ttl, nonce);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);

            buffer[offset++] = (byte)((byte)Function | Rate);
            buffer[offset++] = Ttl;
            buffer.Write(ref offset, Nonce, Endianity.Big);
        }

        private readonly IpV4OptionQuickStartFunction _function;
        private readonly byte _rate;
        private readonly byte _ttl;
        private readonly uint _nonce;
    }
}