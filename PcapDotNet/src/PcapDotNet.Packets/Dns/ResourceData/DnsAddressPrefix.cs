using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 3123.
    /// <pre>
    /// +-----+--------+---+-----------+
    /// | bit | 0-7    | 8 | 9-15      |
    /// +-----+--------+---+-----------+
    /// | 0   | ADDRESSFAMILY          |
    /// +-----+--------+---+-----------+
    /// | 16  | PREFIX | N | AFDLENGTH |
    /// +-----+--------+---+-----------+
    /// | 32  | AFDPART                |
    /// | ... |                        |
    /// +-----+------------------------+
    /// </pre>
    /// </summary>
    public sealed class DnsAddressPrefix : IEquatable<DnsAddressPrefix>
    {
        private static class Offset
        {
            public const int AddressFamily = 0;
            public const int PrefixLength = AddressFamily + sizeof(ushort);
            public const int Negation = PrefixLength + sizeof(byte);
            public const int AddressFamilyDependentPartLength = Negation;
            public const int AddressFamilyDependentPart = AddressFamilyDependentPartLength + sizeof(byte);
        }

        private const int MinimumLength = Offset.AddressFamilyDependentPart;

        private static class Mask
        {
            public const byte Negation = 0x80;
            public const byte AddressFamilyDependentPartLength = 0x7F;
        }

        private const int AddressFamilyDependentPartMaxLength = (1 << 7) - 1;

        /// <summary>
        /// Constructs an instance out of the address family, prefix length, negation and address family dependent part fields.
        /// </summary>
        /// <param name="addressFamily">The address family value.</param>
        /// <param name="prefixLength">
        /// Prefix length.
        /// Upper and lower bounds and interpretation of this value are address family specific.
        /// 
        /// For IPv4, specifies the number of bits of the IPv4 address starting at the most significant bit.
        /// Legal values range from 0 to 32.
        /// 
        /// For IPv6, specifies the number of bits of the IPv6 address starting at the most significant bit.
        /// Legal values range from 0 to 128.
        /// </param>
        /// <param name="negation">Negation flag, indicates the presence of the "!" character in the textual format.</param>
        /// <param name="addressFamilyDependentPart">
        /// For IPv4, the encoding follows the encoding specified for the A RR by RFC 1035.
        /// Trailing zero octets do not bear any information (e.g., there is no semantic difference between 10.0.0.0/16 and 10/16) in an address prefix,
        /// so the shortest possible AddressFamilyDependentPart can be used to encode it.
        /// However, for DNSSEC (RFC 2535) a single wire encoding must be used by all.
        /// Therefore the sender must not include trailing zero octets in the AddressFamilyDependentPart regardless of the value of PrefixLength.
        /// This includes cases in which AddressFamilyDependentPart length times 8 results in a value less than PrefixLength.
        /// The AddressFamilyDependentPart is padded with zero bits to match a full octet boundary.
        /// An IPv4 AddressFamilyDependentPart has a variable length of 0 to 4 octets.
        /// 
        /// For IPv6, the 128 bit IPv6 address is encoded in network byte order (high-order byte first).
        /// The sender must not include trailing zero octets in the AddressFamilyDependentPart regardless of the value of PrefixLength.
        /// This includes cases in which AddressFamilyDependentPart length times 8 results in a value less than PrefixLength.
        /// The AddressFamilyDependentPart is padded with zero bits to match a full octet boundary.
        /// An IPv6 AddressFamilyDependentPart has a variable length of 0 to 16 octets.
        /// </param>
        public DnsAddressPrefix(AddressFamily addressFamily, byte prefixLength, bool negation, DataSegment addressFamilyDependentPart)
        {
            if (addressFamilyDependentPart == null) 
                throw new ArgumentNullException("addressFamilyDependentPart");

            if (addressFamilyDependentPart.Length > AddressFamilyDependentPartMaxLength)
                throw new ArgumentOutOfRangeException("addressFamilyDependentPart", addressFamilyDependentPart, "Cannot be longer than " + AddressFamilyDependentPartMaxLength);

            AddressFamily = addressFamily;
            PrefixLength = prefixLength;
            Negation = negation;
            AddressFamilyDependentPart = addressFamilyDependentPart;
        }

        /// <summary>
        /// The address family value.
        /// </summary>
        public AddressFamily AddressFamily { get; private set; }

        /// <summary>
        /// Prefix length.
        /// Upper and lower bounds and interpretation of this value are address family specific.
        /// 
        /// For IPv4, specifies the number of bits of the IPv4 address starting at the most significant bit.
        /// Legal values range from 0 to 32.
        /// 
        /// For IPv6, specifies the number of bits of the IPv6 address starting at the most significant bit.
        /// Legal values range from 0 to 128.
        /// </summary>
        public byte PrefixLength { get; private set; }

        /// <summary>
        /// Negation flag, indicates the presence of the "!" character in the textual format.
        /// </summary>
        public bool Negation { get; private set; }

        /// <summary>
        /// For IPv4, the encoding follows the encoding specified for the A RR by RFC 1035.
        /// Trailing zero octets do not bear any information (e.g., there is no semantic difference between 10.0.0.0/16 and 10/16) in an address prefix,
        /// so the shortest possible AddressFamilyDependentPart can be used to encode it.
        /// However, for DNSSEC (RFC 2535) a single wire encoding must be used by all.
        /// Therefore the sender must not include trailing zero octets in the AddressFamilyDependentPart regardless of the value of PrefixLength.
        /// This includes cases in which AddressFamilyDependentPart length times 8 results in a value less than PrefixLength.
        /// The AddressFamilyDependentPart is padded with zero bits to match a full octet boundary.
        /// An IPv4 AddressFamilyDependentPart has a variable length of 0 to 4 octets.
        /// 
        /// For IPv6, the 128 bit IPv6 address is encoded in network byte order (high-order byte first).
        /// The sender must not include trailing zero octets in the AddressFamilyDependentPart regardless of the value of PrefixLength.
        /// This includes cases in which AddressFamilyDependentPart length times 8 results in a value less than PrefixLength.
        /// The AddressFamilyDependentPart is padded with zero bits to match a full octet boundary.
        /// An IPv6 AddressFamilyDependentPart has a variable length of 0 to 16 octets.
        /// </summary>
        public DataSegment AddressFamilyDependentPart { get; private set; }

        /// <summary>
        /// The number of bytes this resource data takes.
        /// </summary>
        public int Length
        {
            get { return MinimumLength + AddressFamilyDependentPart.Length; }
        }

        /// <summary>
        /// Two DnsAddressPrefix are equal iff their address family, prefix length, negation and address family dependent part are equal.
        /// </summary>
        public bool Equals(DnsAddressPrefix other)
        {
            return other != null &&
                   AddressFamily.Equals(other.AddressFamily) &&
                   PrefixLength.Equals(other.PrefixLength) &&
                   Negation.Equals(other.Negation) &&
                   AddressFamilyDependentPart.Equals(other.AddressFamilyDependentPart);
        }

        /// <summary>
        /// Two DnsAddressPrefix are equal iff their address family, prefix length, negation and address family dependent part fields are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsAddressPrefix);
        }

        /// <summary>
        /// A hash code based on the address family, prefix length, negation and address family dependent part fields.
        /// </summary>
        public override int GetHashCode()
        {
            return BitSequence.Merge((ushort)AddressFamily, PrefixLength, (byte)(((Negation ? 1 : 0) << 7) | AddressFamilyDependentPart.Length)).GetHashCode() ^
                   AddressFamilyDependentPart.BytesSequenceGetHashCode();
        }

        internal static DnsAddressPrefix Read(DataSegment data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Length < MinimumLength)
                return null;
            AddressFamily addressFamily = (AddressFamily)data.ReadUShort(Offset.AddressFamily, Endianity.Big);
            byte prefixLength = data[Offset.PrefixLength];
            bool negation = data.ReadBool(Offset.Negation, Mask.Negation);
            byte addressFamilyDependentPartLength = (byte)(data[Offset.AddressFamilyDependentPartLength] & Mask.AddressFamilyDependentPartLength);
            
            if (data.Length < MinimumLength + addressFamilyDependentPartLength)
                return null;
            DataSegment addressFamilyDependentPart = data.Subsegment(Offset.AddressFamilyDependentPart, addressFamilyDependentPartLength);

            return new DnsAddressPrefix(addressFamily, prefixLength, negation, addressFamilyDependentPart);
        }

        internal void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.AddressFamily, (ushort)AddressFamily, Endianity.Big);
            buffer.Write(offset + Offset.PrefixLength, PrefixLength);
            buffer.Write(offset + Offset.Negation, (byte)((Negation ? Mask.Negation : 0) | AddressFamilyDependentPart.Length));
            AddressFamilyDependentPart.Write(buffer, offset + Offset.AddressFamilyDependentPart);

            offset += MinimumLength + AddressFamilyDependentPart.Length;
        }
    }
}