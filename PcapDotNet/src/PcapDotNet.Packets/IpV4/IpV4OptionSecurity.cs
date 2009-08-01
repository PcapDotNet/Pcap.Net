using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets
{
    public class IpV4OptionSecurity : IpV4OptionComplex, IEquatable<IpV4OptionSecurity>
    {
        public const int OptionLength = 11;
        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        public IpV4OptionSecurity(IpV4OptionSecurityLevel level, ushort compartments,
                                  ushort handlingRestrictions, UInt24 transmissionControlCode)
            : base(IpV4OptionType.Security)
        {
            _level = level;
            _compartments = compartments;
            _handlingRestrictions = handlingRestrictions;
            _transmissionControlCode = transmissionControlCode;
        }

        public IpV4OptionSecurityLevel Level
        {
            get { return _level; }
        }

        public ushort Compartments
        {
            get { return _compartments; }
        }
        
        public ushort HandlingRestrictions
        {
            get { return _handlingRestrictions; }
        }

        public UInt24 TransmissionControlCode
        {
            get { return _transmissionControlCode; }
        }

        public override int Length
        {
            get { return OptionLength; }
        }

        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        public bool Equals(IpV4OptionSecurity other)
        {
            if (other == null)
                return false;

            return Level == other.Level &&
                   Compartments == other.Compartments &&
                   HandlingRestrictions == other.HandlingRestrictions &&
                   TransmissionControlCode == other.TransmissionControlCode;
        }

        public override bool Equals(IpV4Option other)
        {
            return Equals(other as IpV4OptionSecurity);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   (((ushort)Level << 16) | Compartments) ^
                   (HandlingRestrictions << 16) ^
                   TransmissionControlCode.GetHashCode();
        }

        internal static IpV4OptionSecurity ReadOptionSecurity(byte[] buffer, ref int offset, byte valueLength)
        {
            if (valueLength != OptionValueLength)
                return null;

            IpV4OptionSecurityLevel level = (IpV4OptionSecurityLevel)buffer.ReadUShort(ref offset, Endianity.Big);
            ushort compartments = buffer.ReadUShort(ref offset, Endianity.Big);
            ushort handlingRestrictions = buffer.ReadUShort(ref offset, Endianity.Big);
            UInt24 transmissionControlCode = buffer.ReadUInt24(ref offset, Endianity.Big);

            return new IpV4OptionSecurity(level, compartments, handlingRestrictions, transmissionControlCode);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer[offset++] = (byte)Length;
            buffer.Write(ref offset, (ushort)Level, Endianity.Big);
            buffer.Write(ref offset, Compartments, Endianity.Big);
            buffer.Write(ref offset, HandlingRestrictions, Endianity.Big);
            buffer.Write(ref offset, TransmissionControlCode, Endianity.Big);
        }

        private readonly IpV4OptionSecurityLevel _level;
        private readonly ushort _compartments;
        private readonly ushort _handlingRestrictions;
        private readonly UInt24 _transmissionControlCode;
    }
}