using System;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// An option that can hold any data.
    /// </summary>
    public sealed class DnsOptionAnything : DnsOption
    {
        /// <summary>
        /// Constructs the option from a code and data.
        /// </summary>
        /// <param name="code">The option code.</param>
        /// <param name="data">The option data.</param>
        public DnsOptionAnything(DnsOptionCode code, DataSegment data)
            : base(code)
        {
            Data = data;
        }

        /// <summary>
        /// The option data.
        /// </summary>
        public DataSegment Data { get; private set; }

        /// <summary>
        /// The number of bytes the option data takes.
        /// </summary>
        public override int DataLength
        {
            get { return Data.Length; }
        }

        internal override bool EqualsData(DnsOption other)
        {
            return Data.Equals(((DnsOptionAnything)other).Data);
        }

        internal override int DataGetHashCode()
        {
            return Data.GetHashCode();
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            Data.Write(buffer, ref offset);
        }
    }
}