using System;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5570.
    /// <pre>
    /// +-----+-------------+-----------------+
    /// | Bit | 0-7         | 8-15            |
    /// +-----+-------------+-----------------+
    /// | 0   | Option Type | Opt Data Len    |
    /// +-----+-------------+-----------------+
    /// | 16  | Domain of Interpretation      |
    /// |     |                               |
    /// +-----+-------------+-----------------+
    /// | 48  | Cmpt Lengt  | Sens Level      |
    /// +-----+-------------+-----------------+
    /// | 64  | Checksum (CRC-16)             |
    /// +-----+-------------------------------+
    /// | 80  | Compartment Bitmap (Optional) |
    /// | ... |                               |
    /// +-----+-------------------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.Calipso)]
    public class IpV6OptionCalipso : IpV6OptionComplex, IIpV6OptionComplexFactory
    {
        private static class Offset
        {
            public const int DomainOfInterpretation = 0;
            public const int CompartmentLength = DomainOfInterpretation + sizeof(uint);
            public const int SensitivityLevel = CompartmentLength + sizeof(byte);
            public const int Checksum = SensitivityLevel + sizeof(byte);
            public const int CompartmentBitmap = Checksum + sizeof(ushort);
        }

        public const int OptionDataMinimumLength = Offset.CompartmentBitmap;

        public IpV6OptionCalipso(IpV6CalipsoDomainOfInterpretation domainOfInterpretation, byte sensitivityLevel, ushort checksum, DataSegment compartmentBitmap)
            : base(IpV6OptionType.Calipso)
        {
            if (compartmentBitmap.Length % sizeof(int) != 0)
                throw new ArgumentException(string.Format("Compartment Bitmap length must divide by {0}.", sizeof(int)), "compartmentBitmap");
            if (compartmentBitmap.Length / sizeof(int) > byte.MaxValue)
            {
                throw new ArgumentOutOfRangeException(string.Format("Compartment Bitmap length must not be bigger than {0}.", byte.MaxValue * sizeof(int)),
                                                      "compartmentBitmap");
            }

            DomainOfInterpretation = domainOfInterpretation;
            SensitivityLevel = sensitivityLevel;
            Checksum = checksum;
            CompartmentBitmap = compartmentBitmap;
        }

        /// <summary>
        /// The DOI identifies the rules under which this datagram must be handled and protected.
        /// </summary>
        public IpV6CalipsoDomainOfInterpretation DomainOfInterpretation { get; private set; }

        /// <summary>
        /// Specifies the size of the Compartment Bitmap field in 32-bit words.
        /// The minimum value is zero, which is used only when the information in this packet is not in any compartment.
        /// (In that situation, the CALIPSO Sensitivity Label has no need for a Compartment Bitmap).
        /// </summary>
        public byte CompartmentLength { get { return (byte)(CompartmentLengthInBytes / sizeof(int)); } }

        /// <summary>
        /// Specifies the size of the Compartment Bitmap field in bytes.
        /// The minimum value is zero, which is used only when the information in this packet is not in any compartment.
        /// (In that situation, the CALIPSO Sensitivity Label has no need for a Compartment Bitmap).
        /// </summary>
        public int CompartmentLengthInBytes
        {
            get { return CompartmentBitmap.Length; }
        }

        /// <summary>
        /// Contains an opaque octet whose value indicates the relative sensitivity of the data contained in this datagram in the context of the indicated DOI.
        /// The values of this field must be ordered, with 00000000 being the lowest Sensitivity Level and 11111111 being the highest Sensitivity Level.
        /// However, in a typical deployment, not all 256 Sensitivity Levels will be in use.
        /// So the set of valid Sensitivity Level values depends upon the CALIPSO DOI in use.
        /// This sensitivity ordering rule is necessary so that Intermediate Systems (e.g., routers or MLS guards) will be able to apply MAC policy
        /// with minimal per-packet computation and minimal configuration.
        /// </summary>
        public byte SensitivityLevel { get; private set; }

        /// <summary>
        /// Contains the a CRC-16 checksum as defined in RFC 1662. 
        /// The checksum is calculated over the entire CALIPSO option in this packet, including option header, zeroed-out checksum field, option contents,
        /// and any required padding zero bits.
        /// The checksum must always be computed on transmission and must always be verified on reception.
        /// This checksum only provides protection against accidental corruption of the CALIPSO option in cases where neither the underlying medium 
        /// nor other mechanisms, such as the IP Authentication Header (AH), are available to protect the integrity of this option.
        /// Note that the checksum field is always required, even when other integrity protection mechanisms (e.g., AH) are used.
        /// </summary>
        public ushort Checksum { get; private set; }

        public bool IsChecksumCorrect
        {
            get
            {
                if (_isChecksumCorrect == null)
                {
                    byte[] domainOfInterpretationBytes = new byte[sizeof(uint)];
                    domainOfInterpretationBytes.Write(0, (uint)DomainOfInterpretation, Endianity.Big);
                    ushort expectedValue =
                        PppFrameCheckSequenceCalculator.CalculateFcs16(
                            new byte[0].Concat((byte)OptionType, (byte)DataLength).Concat(domainOfInterpretationBytes)
                                .Concat<byte>(CompartmentLength, SensitivityLevel, 0, 0).Concat(CompartmentBitmap));
                    _isChecksumCorrect = (Checksum == expectedValue);
                }

                return _isChecksumCorrect.Value;
            }
        }

        /// <summary>
        /// Each bit represents one compartment within the DOI.
        /// Each "1" bit within an octet in the Compartment Bitmap field represents a separate compartment under whose rules the data in this packet
        /// must be protected.
        /// Hence, each "0" bit indicates that the compartment corresponding with that bit is not applicable to the data in this packet.
        /// The assignment of identity to individual bits within a Compartment Bitmap for a given DOI is left to the owner of that DOI.
        /// This specification represents a Releasability on the wire as if it were an inverted Compartment.
        /// So the Compartment Bitmap holds the sum of both logical Releasabilities and also logical Compartments for a given DOI value.
        /// The encoding of the Releasabilities in this field is described elsewhere in this document.
        /// The Releasability encoding is designed to permit the Compartment Bitmap evaluation to occur without the evaluator necessarily knowing
        /// the human semantic associated with each bit in the Compartment Bitmap.
        /// In turn, this facilitates the implementation and configuration of Mandatory Access Controls based on the Compartment Bitmap 
        /// within IPv6 routers or guard devices.
        /// </summary>
        public DataSegment CompartmentBitmap { get; private set; }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + CompartmentLengthInBytes; }
        }

        public IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            IpV6CalipsoDomainOfInterpretation domainOfInterpretation = (IpV6CalipsoDomainOfInterpretation)data.ReadUInt(Offset.DomainOfInterpretation,
                                                                                                                        Endianity.Big);
            byte compartmentLength = data[Offset.CompartmentLength];
            int compartmentLengthInBytes = compartmentLength * sizeof(int);
            if (OptionDataMinimumLength + compartmentLengthInBytes > data.Length)
                return null;
            byte sensitivityLevel = data[Offset.SensitivityLevel];
            ushort checksum = data.ReadUShort(Offset.Checksum, Endianity.Big);
            DataSegment compartmentBitmap = data.Subsegment(Offset.CompartmentBitmap, compartmentLengthInBytes);

            return new IpV6OptionCalipso(domainOfInterpretation, sensitivityLevel, checksum, compartmentBitmap);
        }

        internal override bool EqualsData(IpV6Option other)
        {
            return EqualsData(other as IpV6OptionCalipso);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, (uint)DomainOfInterpretation, Endianity.Big);
            buffer.Write(ref offset, CompartmentLength);
            buffer.Write(ref offset, SensitivityLevel);
            buffer.Write(ref offset, Checksum, Endianity.Big);
            buffer.Write(ref offset, CompartmentBitmap);
        }

        private IpV6OptionCalipso()
            : this(IpV6CalipsoDomainOfInterpretation.Null, 0, 0, DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6OptionCalipso other)
        {
            return other != null &&
                   DomainOfInterpretation == other.DomainOfInterpretation && CompartmentLength == other.CompartmentLength &&
                   SensitivityLevel == other.SensitivityLevel && Checksum == other.Checksum && CompartmentBitmap.Equals(CompartmentBitmap);
        }

        private bool? _isChecksumCorrect;
    }
}