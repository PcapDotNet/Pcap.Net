using System;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// <pre>
    /// +-----+---------+
    /// | bit | 0-7     |
    /// +-----+---------+
    /// | 0   | FORMAT  |
    /// +-----+---------+
    /// | 8   | ADDRESS |
    /// | ... |         |
    /// +-----+---------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.AtmA)]
    public sealed class DnsResourceDataAtmAddress : DnsResourceDataSimple, IEquatable<DnsResourceDataAtmAddress>
    {
        private static class Offset
        {
            public const int Format = 0;
            public const int Address = Format + sizeof(byte);
        }

        public const int ConstantPartLength = Offset.Address;

        public DnsResourceDataAtmAddress(DnsAtmAddressFormat format, DataSegment address)
        {
            Format = format;
            Address = address;
        }

        /// <summary>
        /// The format of Address.
        /// </summary>
        public DnsAtmAddressFormat Format { get; private set; }

        /// <summary>
        /// Variable length string of octets containing the ATM address of the node to which this RR pertains.
        /// When the format is AESA, the address is coded as described in ISO 8348/AD 2 using the preferred binary encoding of the ISO NSAP format.
        /// When the format value is E.164, the Address/Number Digits appear in the order in which they would be entered on a numeric keypad.
        /// Digits are coded in IA5 characters with the leftmost bit of each digit set to 0.
        /// This ATM address appears in ATM End System Address Octets field (AESA format) or the Address/Number Digits field (E.164 format) of the Called party number information element [ATMUNI3.1].
        /// Subaddress information is intentionally not included because E.164 subaddress information is used for routing.
        /// </summary>
        public DataSegment Address { get; private set; }

        public bool Equals(DnsResourceDataAtmAddress other)
        {
            return other != null &&
                   Format.Equals(other.Format) &&
                   Address.Equals(other.Address);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataAtmAddress);
        }

        internal DnsResourceDataAtmAddress()
            : this(DnsAtmAddressFormat.AtmEndSystemAddress, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + Address.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Format, (byte)Format);
            Address.Write(buffer, offset + Offset.Address);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            DnsAtmAddressFormat format = (DnsAtmAddressFormat)data[Offset.Format];
            DataSegment address = data.SubSegment(Offset.Address, data.Length - ConstantPartLength);

            return new DnsResourceDataAtmAddress(format, address);
        }
    }
}