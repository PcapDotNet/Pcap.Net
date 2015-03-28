using System;
using PcapDotNet.Base;

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
    [DnsTypeRegistration(Type = DnsType.AsynchronousTransferModeAddress)]
    public sealed class DnsResourceDataAsynchronousTransferModeAddress : DnsResourceDataSimple, IEquatable<DnsResourceDataAsynchronousTransferModeAddress>
    {
        private static class Offset
        {
            public const int Format = 0;
            public const int Address = Format + sizeof(byte);
        }

        private const int ConstantPartLength = Offset.Address;

        /// <summary>
        /// Constructs an instance from the format and address fields.
        /// </summary>
        /// <param name="format">The format of Address.</param>
        /// <param name="address">
        /// Variable length string of octets containing the ATM address of the node to which this RR pertains.
        /// When the format is AESA, the address is coded as described in ISO 8348/AD 2 using the preferred binary encoding of the ISO NSAP format.
        /// When the format value is E.164, the Address/Number Digits appear in the order in which they would be entered on a numeric keypad.
        /// Digits are coded in IA5 characters with the leftmost bit of each digit set to 0.
        /// This ATM address appears in ATM End System Address Octets field (AESA format) or the Address/Number Digits field (E.164 format) of the Called party number information element [ATMUNI3.1].
        /// Subaddress information is intentionally not included because E.164 subaddress information is used for routing.
        /// </param>
        public DnsResourceDataAsynchronousTransferModeAddress(DnsAsynchronousTransferModeAddressFormat format, DataSegment address)
        {
            Format = format;
            Address = address;
        }

        /// <summary>
        /// The format of Address.
        /// </summary>
        public DnsAsynchronousTransferModeAddressFormat Format { get; private set; }

        /// <summary>
        /// Variable length string of octets containing the ATM address of the node to which this RR pertains.
        /// When the format is AESA, the address is coded as described in ISO 8348/AD 2 using the preferred binary encoding of the ISO NSAP format.
        /// When the format value is E.164, the Address/Number Digits appear in the order in which they would be entered on a numeric keypad.
        /// Digits are coded in IA5 characters with the leftmost bit of each digit set to 0.
        /// This ATM address appears in ATM End System Address Octets field (AESA format) or the Address/Number Digits field (E.164 format) of the Called party number information element [ATMUNI3.1].
        /// Subaddress information is intentionally not included because E.164 subaddress information is used for routing.
        /// </summary>
        public DataSegment Address { get; private set; }

        /// <summary>
        /// Two DnsResourceDataAtmAddress are equal iff their format and address fields are equal.
        /// </summary>
        public bool Equals(DnsResourceDataAsynchronousTransferModeAddress other)
        {
            return other != null &&
                   Format.Equals(other.Format) &&
                   Address.Equals(other.Address);
        }

        /// <summary>
        /// Two DnsResourceDataAtmAddress are equal iff their format and address fields are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataAsynchronousTransferModeAddress);
        }

        /// <summary>
        /// A hash code based on the format and address fields.
        /// </summary>
        public override int GetHashCode()
        {
            return Sequence.GetHashCode(Format, Address);
        }

        internal DnsResourceDataAsynchronousTransferModeAddress()
            : this(DnsAsynchronousTransferModeAddressFormat.AsynchronousTransferModeEndSystemAddress, DataSegment.Empty)
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

            DnsAsynchronousTransferModeAddressFormat format = (DnsAsynchronousTransferModeAddressFormat)data[Offset.Format];
            DataSegment address = data.Subsegment(Offset.Address, data.Length - ConstantPartLength);

            return new DnsResourceDataAsynchronousTransferModeAddress(format, address);
        }
    }
}