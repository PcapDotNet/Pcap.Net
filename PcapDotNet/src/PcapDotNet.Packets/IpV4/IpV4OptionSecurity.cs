using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// This option provides a way for hosts to send security, compartmentation, handling restrictions, and TCC (closed user group) parameters.
    /// 
    /// The format for this option is as follows:
    /// +--------+--------+---//---+---//---+---//---+---//---+
    /// |10000010|00001011|SSS  SSS|CCC  CCC|HHH  HHH|  TCC   |
    /// +--------+--------+---//---+---//---+---//---+---//---+
    /// Type=130 Length=11
    /// 
    /// Must be copied on fragmentation.  
    /// This option appears at most once in a datagram.
    /// </summary>
    public class IpV4OptionSecurity : IpV4OptionComplex, IEquatable<IpV4OptionSecurity>
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 11;

        /// <summary>
        /// The number of bytes this option's value take.
        /// </summary>
        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        /// <summary>
        /// Create the security option from the different security field values.
        /// </summary>
        /// <param name="level">Specifies one of the levels of security.</param>
        /// <param name="compartments">
        /// Compartments (C field):  16 bits
        /// An all zero value is used when the information transmitted is not compartmented.  
        /// Other values for the compartments field may be obtained from the Defense Intelligence Agency.
        /// </param>
        /// <param name="handlingRestrictions">
        /// Handling Restrictions (H field):  16 bits
        /// The values for the control and release markings are alphanumeric digraphs 
        /// and are defined in the Defense Intelligence Agency Manual DIAM 65-19, "Standard Security Markings".
        /// </param>
        /// <param name="transmissionControlCode">
        /// Transmission Control Code (TCC field):  24 bits
        /// Provides a means to segregate traffic and define controlled communities of interest among subscribers. 
        /// The TCC values are trigraphs, and are available from HQ DCA Code 530.
        /// </param>
        public IpV4OptionSecurity(IpV4OptionSecurityLevel level, ushort compartments,
                                  ushort handlingRestrictions, UInt24 transmissionControlCode)
            : base(IpV4OptionType.Security)
        {
            _level = level;
            _compartments = compartments;
            _handlingRestrictions = handlingRestrictions;
            _transmissionControlCode = transmissionControlCode;
        }

        /// <summary>
        /// Specifies one of the levels of security.
        /// </summary>
        public IpV4OptionSecurityLevel Level
        {
            get { return _level; }
        }

        /// <summary>
        /// Compartments (C field):  16 bits
        /// An all zero value is used when the information transmitted is not compartmented.  
        /// Other values for the compartments field may be obtained from the Defense Intelligence Agency.
        /// </summary>
        public ushort Compartments
        {
            get { return _compartments; }
        }
        
        /// <summary>
        /// Handling Restrictions (H field):  16 bits
        /// The values for the control and release markings are alphanumeric digraphs 
        /// and are defined in the Defense Intelligence Agency Manual DIAM 65-19, "Standard Security Markings".
        /// </summary>
        public ushort HandlingRestrictions
        {
            get { return _handlingRestrictions; }
        }

        /// <summary>
        /// Transmission Control Code (TCC field):  24 bits
        /// Provides a means to segregate traffic and define controlled communities of interest among subscribers. 
        /// The TCC values are trigraphs, and are available from HQ DCA Code 530.
        /// </summary>
        public UInt24 TransmissionControlCode
        {
            get { return _transmissionControlCode; }
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
        /// Two security options are equal iff they have the exam same field values.
        /// </summary>
        public bool Equals(IpV4OptionSecurity other)
        {
            if (other == null)
                return false;

            return Level == other.Level &&
                   Compartments == other.Compartments &&
                   HandlingRestrictions == other.HandlingRestrictions &&
                   TransmissionControlCode == other.TransmissionControlCode;
        }

        /// <summary>
        /// Two security options are equal iff they have the exam same field values.
        /// </summary>
        public override bool Equals(IpV4Option other)
        {
            return Equals(other as IpV4OptionSecurity);
        }

        /// <summary>
        /// The hash code is the xor of the following hash code: base class hash code, level and compartments, handling restrictions, transmission control code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   (((ushort)Level << 16) | Compartments).GetHashCode() ^
                   (HandlingRestrictions << 16).GetHashCode() ^
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