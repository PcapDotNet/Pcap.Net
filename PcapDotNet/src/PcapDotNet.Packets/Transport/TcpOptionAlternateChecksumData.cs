using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// TCP Alternate Checksum Data Option (RFC 1146).
    /// 
    /// <para>
    /// The format of the TCP Alternate Checksum Data Option is:
    /// <pre>
    /// +---------+---------+---------+     +---------+
    /// | Kind=15 |Length=N |  data   | ... |  data   |
    /// +---------+---------+---------+     +---------+
    /// </pre>
    /// </para>
    /// 
    /// <para>
    /// This field is used only when the alternate checksum that is negotiated is longer than 16 bits. 
    /// These checksums will not fit in the checksum field of the TCP header and thus at least part of them must be put in an option.  
    /// Whether the checksum is split between the checksum field in the TCP header and the option or the entire checksum is placed in the option 
    /// is determined on a checksum by checksum basis.
    /// </para>
    /// 
    /// <para>
    /// The length of this option will depend on the choice of alternate checksum algorithm for this connection.
    /// </para>
    /// 
    /// <para>
    /// While computing the alternate checksum, the TCP checksum field and the data portion TCP Alternate Checksum Data Option are replaced with zeros.
    /// </para>
    /// </summary>
    [OptionTypeRegistration(typeof(TcpOptionType), TcpOptionType.AlternateChecksumData)]
    public class TcpOptionAlternateChecksumData : TcpOptionComplex, IOptionComplexFactory, IEquatable<TcpOptionAlternateChecksumData>
    {
        /// <summary>
        /// The minimum number of bytes this option take.
        /// </summary>
        public const int OptionMinimumLength = OptionHeaderLength;

        /// <summary>
        /// The minimum number of bytes this option's value take.
        /// </summary>
        public const int OptionValueMinimumLength = OptionMinimumLength - OptionHeaderLength;

        /// <summary>
        /// Creates the option using the given data.
        /// </summary>
        public TcpOptionAlternateChecksumData(IList<byte> data)
            : base(TcpOptionType.AlternateChecksumData)
        {
            Data = new ReadOnlyCollection<byte>(data);
        }

        /// <summary>
        /// the default option data is no data.
        /// </summary>
        public TcpOptionAlternateChecksumData()
            : this(new byte[0])
        {
        }

        /// <summary>
        /// The alternate checksum data.
        /// </summary>
        public ReadOnlyCollection<byte> Data { get; private set; }

        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public override int Length
        {
            get { return OptionMinimumLength + Data.Count; }
        }

        /// <summary>
        /// True iff this option may appear at most once in a datagram.
        /// </summary>
        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        /// <summary>
        /// Two alternate checksum data options are equal if they have the same data.
        /// </summary>
        public bool Equals(TcpOptionAlternateChecksumData other)
        {
            if (other == null)
                return false;
            return Data.SequenceEqual(other.Data);
        }

        /// <summary>
        /// Two alternate checksum data options are equal if they have the same data.
        /// </summary>
        public override bool Equals(TcpOption other)
        {
            return Equals(other as TcpOptionAlternateChecksumData);
        }

        /// <summary>
        /// The hash code of this option is the hash code of the option type xored with the hash code of the data.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   Data.BytesSequenceGetHashCode();
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
            if (valueLength < OptionValueMinimumLength)
                return null;

            byte[] data = buffer.ReadBytes(ref offset, valueLength);
            return new TcpOptionAlternateChecksumData(data);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Data);
        }
    }
}